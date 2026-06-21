<%@ WebHandler Language="C#" Class="CitizenPortal" %>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;

public class CitizenPortal : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        // Add CORS Headers for local development and testing
        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

        if (context.Request.HttpMethod == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }

        string action = context.Request.QueryString["action"];

        if (context.Request.HttpMethod == "POST")
        {
            context.Response.ContentType = "application/json";
            
            if (action == "send_otp")
            {
                HandleSendOtp(context);
            }
            else if (action == "verify_otp")
            {
                HandleVerifyOtp(context);
            }
            else if (action == "tax_login")
            {
                HandleTaxLogin(context);
            }
            else
            {
                SendJsonResponse(context, 400, false, "Invalid POST action parameter", null);
            }
            return;
        }

        // Default: GET request - serve the HTML Portal
        context.Response.ContentType = "text/html";
        ServeHtmlPortal(context);
    }

    private void HandleTaxLogin(HttpContext context)
    {
        try
        {
            string body;
            using (var rdr = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                body = rdr.ReadToEnd();
            }
            var serializer = new JavaScriptSerializer();
            var payload = serializer.Deserialize<Dictionary<string, string>>(body);

            if (payload == null || !payload.ContainsKey("username") || !payload.ContainsKey("password"))
            {
                SendJsonResponse(context, 400, false, "Username and password are required", null);
                return;
            }

            string username = payload["username"].Trim();
            string password = payload["password"];

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT StaffId, FullName, PasswordHash, Role, IsActive FROM tbl_QR_StaffUsers WHERE LoginId = @LoginId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LoginId", username);
                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            // Verify plain password
                            string storedPassword = sdr["PasswordHash"].ToString(); // using same column for plain text
                            System.Diagnostics.Debug.WriteLine($"DEBUG TAX LOGIN: username={username}, password={password}, storedPassword={storedPassword}, isActive={isActive}");
                            if (storedPassword.Equals(password) && isActive)
                            {
                                context.Session["Staff_Id"] = sdr["StaffId"].ToString();
                                var data = new {
                                    name = sdr["FullName"].ToString(),
                                    role = sdr["Role"].ToString()
                                };
                                SendJsonResponse(context, 200, true, "Login successful", data);
                            }
                            else
                            {
                                SendJsonResponse(context, 401, false, "Invalid credentials or inactive account", null);
                            }
                        }
                        else
                        {
                            SendJsonResponse(context, 401, false, "Invalid credentials", null);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SendJsonResponse(context, 500, false, "Server error: " + ex.Message, null);
        }
    }

    private void HandleSendOtp(HttpContext context)
    {
        try
        {
            string body;
            using (var rdr = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                body = rdr.ReadToEnd();
            }

            var serializer = new JavaScriptSerializer();
            var payload = serializer.Deserialize<Dictionary<string, string>>(body);

            if (payload == null || !payload.ContainsKey("token") || !payload.ContainsKey("mobile"))
            {
                SendJsonResponse(context, 400, false, "Token and mobile parameters are required", null);
                return;
            }

            string token = payload["token"].Trim();
            string mobile = payload["mobile"].Trim();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(mobile))
            {
                SendJsonResponse(context, 400, false, "Token and mobile cannot be empty", null);
                return;
            }

            // 1. Database Lookup
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            string propertyId = "";
            string registeredMobile = "";
            string cleanToken = ExtractToken(token);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string cardQuery = @"
                    SELECT Status, PropertyId 
                    FROM QRMaster 
                    WHERE QRToken = @Token OR QRId = @Token OR QRUrl = @Token";

                using (SqlCommand cmd = new SqlCommand(cardQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", cleanToken);
                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            string status = sdr["Status"].ToString().ToUpper();
                            if (status != "ACTIVATED")
                            {
                                SendJsonResponse(context, 400, false, "This QR card is not activated yet. Status: " + status, null);
                                return;
                            }
                            propertyId = sdr["PropertyId"].ToString();
                        }
                        else
                        {
                            SendJsonResponse(context, 404, false, "QR Card token not found in the system.", null);
                            return;
                        }
                    }
                }

                // Get registered mobile from All_Demand
                string propQuery = "SELECT Mobile_No FROM All_Demand WHERE PID = @PID";
                using (SqlCommand cmd = new SqlCommand(propQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", propertyId);
                    object mobileVal = cmd.ExecuteScalar();
                    if (mobileVal != null && mobileVal != DBNull.Value)
                    {
                        registeredMobile = mobileVal.ToString().Trim();
                    }
                }
            }

            // 2. Validate mobile matches (ignoring country code/leading zeros for robustness)
            string cleanInputMobile = CleanMobile(mobile);
            string cleanDbMobile = CleanMobile(registeredMobile);

            if (string.IsNullOrEmpty(cleanDbMobile))
            {
                // If there's no registered mobile in DB, we allow them to log in for usability
                // (Usually, we would enforce update, but for testing and deployment, we allow)
                registeredMobile = mobile;
                cleanDbMobile = cleanInputMobile;
            }

            if (cleanInputMobile != cleanDbMobile && cleanInputMobile != "9431881414") // Master override for testing
            {
                SendJsonResponse(context, 401, false, "The mobile number entered does not match our records for this property. Registered mobile ends in: " + GetMaskedSuffix(registeredMobile), null);
                return;
            }

            // 3. Generate OTP
            Random rand = new Random();
            string otp = rand.Next(100000, 999999).ToString();
            
            // Store OTP and details in IIS session
            context.Session["OTP_" + cleanToken] = otp;
            context.Session["Mobile_" + cleanToken] = mobile;
            context.Session["PropertyID_" + cleanToken] = propertyId;

            // Log OTP generated to console or trace (In real environment, connect to SMS gateway)
            // For demo, we return success and let client know OTP is sent. 
            // In demo mode we show it, in production it would only send via SMS.
            var responseData = new Dictionary<string, object> {
                { "maskedMobile", GetMaskedMobile(registeredMobile) },
                { "demoOtp", otp } // Return OTP for demo/testing convenience
            };

            SendJsonResponse(context, 200, true, "Verification code sent to registered mobile: " + GetMaskedMobile(registeredMobile), responseData);
        }
        catch (Exception ex)
        {
            SendJsonResponse(context, 500, false, "Server error: " + ex.Message, null);
        }
    }

    private void HandleVerifyOtp(HttpContext context)
    {
        try
        {
            string body;
            using (var rdr = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                body = rdr.ReadToEnd();
            }

            var serializer = new JavaScriptSerializer();
            var payload = serializer.Deserialize<Dictionary<string, string>>(body);

            if (payload == null || !payload.ContainsKey("token") || !payload.ContainsKey("otp"))
            {
                SendJsonResponse(context, 400, false, "Token and OTP parameters are required", null);
                return;
            }

            string token = payload["token"].Trim();
            string otp = payload["otp"].Trim();
            string cleanToken = ExtractToken(token);

            // Fetch stored OTP from session
            string storedOtp = context.Session["OTP_" + cleanToken] as string;
            string propertyId = context.Session["PropertyID_" + cleanToken] as string;

            // Check if OTP matches
            if (string.IsNullOrEmpty(storedOtp) && otp != "123456") // Allow default override for testing
            {
                SendJsonResponse(context, 401, false, "Session expired or OTP not requested. Please request a new OTP.", null);
                return;
            }

            if (otp != storedOtp && otp != "123456")
            {
                SendJsonResponse(context, 401, false, "Incorrect verification code. Please try again.", null);
                return;
            }

            // If session was cleared/bypassed, fetch propertyId from DB again for safety
            if (string.IsNullOrEmpty(propertyId))
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string q = "SELECT PropertyId FROM QRMaster WHERE QRToken = @Token OR QRId = @Token OR QRUrl = @Token";
                    using (SqlCommand cmd = new SqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@Token", cleanToken);
                        conn.Open();
                        object val = cmd.ExecuteScalar();
                        if (val != null) propertyId = val.ToString();
                    }
                }
            }

            if (string.IsNullOrEmpty(propertyId))
            {
                SendJsonResponse(context, 404, false, "Associated property ID not found.", null);
                return;
            }

            // Fetch Property Details
            Dictionary<string, object> propDetails = null;
            string connStr2 = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            
            using (SqlConnection conn = new SqlConnection(connStr2))
            {
                string query = @"
                    SELECT PID, Owner_Name, Guardian_Name, Mobile_No, Address, Total_Dues, Payment_Status, Circle, Ward_No
                    FROM All_Demand 
                    WHERE PID = @PID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", propertyId);
                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            propDetails = new Dictionary<string, object>
                            {
                                { "pid", sdr["PID"].ToString() },
                                { "ownerName", sdr["Owner_Name"].ToString() },
                                { "guardianName", sdr["Guardian_Name"] == DBNull.Value ? "" : sdr["Guardian_Name"].ToString() },
                                { "mobileNo", sdr["Mobile_No"] == DBNull.Value ? "" : sdr["Mobile_No"].ToString() },
                                { "address", sdr["Address"] == DBNull.Value ? "" : sdr["Address"].ToString() },
                                { "totalDues", sdr["Total_Dues"] == DBNull.Value ? "0" : sdr["Total_Dues"].ToString() },
                                { "paymentStatus", sdr["Payment_Status"] == DBNull.Value ? "Pending" : sdr["Payment_Status"].ToString() },
                                { "circle", sdr["Circle"] == DBNull.Value ? "" : sdr["Circle"].ToString() },
                                { "wardNo", sdr["Ward_No"] == DBNull.Value ? "" : sdr["Ward_No"].ToString() }
                            };
                        }
                    }
                }

                if (propDetails != null)
                {
                    // Update stats
                    using (SqlCommand cmd = new SqlCommand("UPDATE All_Demand SET Last_Scanned = GETDATE(), Scan_Count = ISNULL(Scan_Count, 0) + 1 WHERE PID = @PID", conn))
                    {
                        cmd.Parameters.AddWithValue("@PID", propertyId);
                        cmd.ExecuteNonQuery();
                    }

                    // Log this scan in scan log
                    string logQuery = @"
                        INSERT INTO tbl_QR_ScanLog 
                            (PID, Staff_Id, Scan_Type, Scan_Timestamp, IP_Address, Is_Suspicious, Remarks)
                        VALUES 
                            (TRY_CAST(@PID AS BIGINT), 'CITIZEN', 'CITIZEN_PORTAL_SCAN', GETDATE(), @IP, 0, 'Citizen scanned and logged in via OTP')";
                    
                    using (SqlCommand cmd = new SqlCommand(logQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PID", propertyId);
                        cmd.Parameters.AddWithValue("@IP", context.Request.UserHostAddress);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            if (propDetails == null)
            {
                SendJsonResponse(context, 404, false, "Property details not found in database records.", null);
                return;
            }

            // Mark session as fully authenticated
            context.Session["Citizen_Authenticated_" + cleanToken] = true;

            SendJsonResponse(context, 200, true, "Authentication successful. Property retrieved.", propDetails);
        }
        catch (Exception ex)
        {
            SendJsonResponse(context, 500, false, "Server error: " + ex.Message, null);
        }
    }

    private string ExtractToken(string token)
    {
        string t = token.Trim();
        if (t.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                Uri uri = new Uri(t);
                t = uri.Segments[uri.Segments.Length - 1].Trim('/');
            }
            catch {}
        }
        return t;
    }

    private string CleanMobile(string m)
    {
        if (string.IsNullOrEmpty(m)) return "";
        string clean = "";
        foreach (char c in m)
        {
            if (char.IsDigit(c)) clean += c;
        }
        // Take last 10 digits to ignore country code e.g. +91
        if (clean.Length > 10)
        {
            clean = clean.Substring(clean.Length - 10);
        }
        return clean;
    }

    private string GetMaskedMobile(string mobile)
    {
        string clean = CleanMobile(mobile);
        if (clean.Length < 10) return "******";
        return "******" + clean.Substring(6);
    }

    private string GetMaskedSuffix(string mobile)
    {
        string clean = CleanMobile(mobile);
        if (clean.Length < 4) return "****";
        return clean.Substring(clean.Length - 4);
    }

    private void SendJsonResponse(HttpContext context, int statusCode, bool success, string message, object data)
    {
        context.Response.StatusCode = statusCode;
        var ser = new JavaScriptSerializer();
        var res = new Dictionary<string, object>
        {
            { "success", success },
            { "message", message },
            { "data", data }
        };
        context.Response.Write(ser.Serialize(res));
    }

    private void ServeHtmlPortal(HttpContext context)
    {
        // Load the template HTML. Since we want it self-contained, we can write it directly.
        // It uses CSS variables,Outfit google fonts, elegant overlays, dynamic javascript simulation, and premium CSS micro-animations.
        string token = context.Request.QueryString["token"] ?? "";
        
        string html = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>PMC Smart QR Citizen Portal</title>
    <meta name='description' content='Verify and view your Patna Nagar Nigam smart property QR card details, tax status, and dues.'>
    <link href='https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700;800&display=swap' rel='stylesheet'>
    <script src='https://checkout.razorpay.com/v1/checkout.js'></script>
    <style>
        :root {
            --primary: #0D3E73;
            --primary-light: #1E3E62;
            --secondary: #E67E22;
            --success: #2ECC71;
            --danger: #E74C3C;
            --background: #0B192C;
            --surface: rgba(30, 62, 98, 0.45);
            --text-light: #BDC3C7;
            --text-dark: #2C3E50;
            --border: rgba(255, 255, 255, 0.12);
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
            font-family: 'Outfit', sans-serif;
            transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
        }

        body {
            background: radial-gradient(circle at 50% 50%, #15304f 0%, #0B192C 100%);
            min-height: 100vh;
            color: #ffffff;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            overflow-x: hidden;
            padding: 20px;
        }

        /* Container & Glassmorphism Card */
        .portal-container {
            width: 100%;
            max-width: 480px;
            perspective: 1000px;
        }

        .glass-card {
            background: rgba(13, 62, 115, 0.25);
            backdrop-filter: blur(20px);
            -webkit-backdrop-filter: blur(20px);
            border: 1px solid var(--border);
            border-radius: 28px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.4), 0 5px 15px rgba(0, 0, 0, 0.2);
            padding: 30px;
            position: relative;
            overflow: hidden;
        }

        .glass-card::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: radial-gradient(circle, rgba(255,255,255,0.03) 0%, transparent 70%);
            pointer-events: none;
        }

        /* Headings & Branding */
        .header {
            text-align: center;
            margin-bottom: 24px;
        }

        .logos-container {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 12px;
            margin-bottom: 16px;
        }

        .logo-badge {
            background: white;
            padding: 6px;
            border-radius: 12px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.15);
            display: flex;
            align-items: center;
            justify-content: center;
            width: 45px;
            height: 45px;
            border: 1px solid rgba(13, 62, 115, 0.1);
        }

        .logo-badge img {
            width: 100%;
            height: 100%;
            object-fit: contain;
        }

        .plus-sign {
            font-size: 18px;
            font-weight: 700;
            color: var(--secondary);
        }

        .app-subtitle {
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: 2.5px;
            color: var(--secondary);
            font-weight: 700;
            margin-bottom: 4px;
        }

        h1 {
            font-size: 22px;
            font-weight: 800;
            color: #ffffff;
            letter-spacing: 0.5px;
        }

        .sub-desc {
            font-size: 13.5px;
            color: var(--text-light);
            text-align: center;
            margin-top: 6px;
            line-height: 1.4;
        }

        /* Forms & Inputs */
        .form-group {
            margin-bottom: 20px;
            position: relative;
        }

        .form-label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: var(--secondary);
            margin-bottom: 8px;
            letter-spacing: 1px;
            text-transform: uppercase;
        }

        .input-wrapper {
            position: relative;
        }

        .input-wrapper input {
            width: 100%;
            background: rgba(255, 255, 255, 0.06);
            border: 1.5px solid var(--border);
            border-radius: 16px;
            padding: 15px 15px 15px 45px;
            color: white;
            font-size: 15px;
            font-weight: 500;
            outline: none;
        }

        .input-wrapper input:focus {
            background: rgba(255, 255, 255, 0.1);
            border-color: var(--secondary);
            box-shadow: 0 0 12px rgba(230, 126, 34, 0.25);
        }

        .input-wrapper .icon {
            position: absolute;
            left: 16px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--text-light);
            pointer-events: none;
            width: 18px;
            height: 18px;
        }

        /* Buttons */
        .btn {
            width: 100%;
            background: linear-gradient(135deg, var(--secondary) 0%, #d35400 100%);
            border: none;
            border-radius: 16px;
            color: white;
            padding: 16px;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 4px 15px rgba(230, 126, 34, 0.3);
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            letter-spacing: 0.5px;
        }

        .btn:hover:not(:disabled) {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(230, 126, 34, 0.45);
        }

        .btn:active:not(:disabled) {
            transform: translateY(1px);
        }

        .btn:disabled {
            background: #7f8c8d;
            box-shadow: none;
            cursor: not-allowed;
            opacity: 0.6;
        }

        .btn-outline {
            background: transparent;
            border: 1.5px solid var(--border);
            color: #ffffff;
            box-shadow: none;
            margin-top: 12px;
        }

        .btn-outline:hover:not(:disabled) {
            background: rgba(255,255,255,0.05);
            border-color: rgba(255,255,255,0.25);
            box-shadow: none;
        }

        /* Custom Alert Boxes */
        .alert {
            background: rgba(231, 76, 60, 0.12);
            border: 1px solid rgba(231, 76, 60, 0.3);
            border-radius: 14px;
            padding: 12px 16px;
            color: #ff7675;
            font-size: 13px;
            display: flex;
            align-items: flex-start;
            gap: 10px;
            margin-bottom: 20px;
            line-height: 1.4;
        }

        .alert-success {
            background: rgba(46, 204, 113, 0.12);
            border: 1px solid rgba(46, 204, 113, 0.3);
            color: #2ecc71;
        }

        .alert-warning {
            background: rgba(243, 156, 18, 0.12);
            border: 1px solid rgba(243, 156, 18, 0.3);
            color: #f1c40f;
        }

        /* Hidden class for step toggling */
        .hidden {
            display: none !important;
        }

        /* OTP Input Boxes */
        .otp-inputs {
            display: flex;
            justify-content: space-between;
            gap: 10px;
            margin-bottom: 24px;
        }

        .otp-box {
            width: 50px;
            height: 52px;
            background: rgba(255, 255, 255, 0.06);
            border: 1.5px solid var(--border);
            border-radius: 12px;
            text-align: center;
            color: white;
            font-size: 20px;
            font-weight: 700;
            outline: none;
        }

        .otp-box:focus {
            border-color: var(--secondary);
            box-shadow: 0 0 10px rgba(230, 126, 34, 0.2);
            background: rgba(255, 255, 255, 0.1);
        }

        /* Dashboard Details */
        .dashboard-header {
            display: flex;
            align-items: center;
            gap: 12px;
            border-bottom: 1.5px solid var(--border);
            padding-bottom: 14px;
            margin-bottom: 18px;
        }

        .detail-card {
            background: rgba(255, 255, 255, 0.04);
            border: 1px solid var(--border);
            border-radius: 18px;
            padding: 16px;
            margin-bottom: 14px;
        }

        .detail-row {
            display: flex;
            flex-direction: column;
            margin-bottom: 10px;
        }

        .detail-row:last-child {
            margin-bottom: 0;
        }

        .detail-label {
            font-size: 11px;
            color: var(--text-light);
            text-transform: uppercase;
            letter-spacing: 0.8px;
            margin-bottom: 2px;
        }

        .detail-value {
            font-size: 14.5px;
            font-weight: 600;
            color: #ffffff;
        }

        .highlight-value {
            color: #54a0ff;
            font-size: 16px;
        }

        /* Outstanding Dues Badge and Card */
        .dues-card {
            background: rgba(230, 126, 34, 0.08);
            border: 1.5px solid rgba(230, 126, 34, 0.25);
            border-radius: 18px;
            padding: 16px;
            margin-bottom: 16px;
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .dues-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .dues-title {
            font-size: 13px;
            font-weight: 600;
            color: var(--text-light);
        }

        .dues-amount {
            font-size: 20px;
            font-weight: 800;
            color: var(--secondary);
        }

        .status-badge {
            padding: 4px 10px;
            border-radius: 6px;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .status-pending {
            background: rgba(231, 76, 60, 0.15);
            color: var(--danger);
            border: 1px solid rgba(231, 76, 60, 0.25);
        }

        .status-paid {
            background: rgba(46, 204, 113, 0.15);
            color: var(--success);
            border: 1px solid rgba(46, 204, 113, 0.25);
        }

        /* Simulated Digital PVC card representation */
        .pvc-preview-container {
            margin-top: 18px;
            perspective: 800px;
            cursor: pointer;
        }

        .pvc-card {
            width: 100%;
            height: 190px;
            background: linear-gradient(135deg, #092e56 0%, #1e4b7a 50%, #0d3e73 100%);
            border: 1.5px solid rgba(255, 255, 255, 0.12);
            border-radius: 18px;
            padding: 16px;
            position: relative;
            box-shadow: 0 8px 20px rgba(0,0,0,0.3);
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            overflow: hidden;
        }

        .pvc-card::after {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: linear-gradient(45deg, rgba(255,255,255,0) 30%, rgba(255,255,255,0.06) 50%, rgba(255,255,255,0) 70%);
            background-size: 200% 200%;
            animation: shine 4s infinite linear;
        }

        @keyframes shine {
            0% { background-position: -200% 0; }
            100% { background-position: 200% 0; }
        }

        .pvc-card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .pvc-card-title {
            font-size: 11px;
            font-weight: 700;
            color: #ffffff;
            letter-spacing: 1.5px;
        }

        .pvc-card-body {
            display: flex;
            justify-content: space-between;
            align-items: flex-end;
            margin-top: 12px;
        }

        .pvc-details {
            display: flex;
            flex-direction: column;
            gap: 2px;
        }

        .pvc-qr-sim {
            background: white;
            padding: 6px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 70px;
            height: 70px;
        }

        .pvc-qr-sim svg {
            width: 100%;
            height: 100%;
        }

        .pvc-tag-line {
            font-size: 7.5px;
            color: rgba(255,255,255,0.4);
            text-transform: uppercase;
            letter-spacing: 1.5px;
            margin-top: 4px;
            text-align: center;
        }

        /* Demo Mode Settings panel */
        .demo-panel {
            background: rgba(241, 196, 15, 0.07);
            border: 1px dashed rgba(241, 196, 15, 0.3);
            border-radius: 16px;
            padding: 12px 16px;
            margin-top: 24px;
            font-size: 11.5px;
            color: #f1c40f;
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .demo-toggle-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .switch {
            position: relative;
            display: inline-block;
            width: 38px;
            height: 20px;
        }

        .switch input { 
            opacity: 0;
            width: 0;
            height: 0;
        }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0; left: 0; right: 0; bottom: 0;
            background-color: #555;
            transition: .4s;
            border-radius: 34px;
        }

        .slider:before {
            position: absolute;
            content: '';
            height: 14px;
            width: 14px;
            left: 3px;
            bottom: 3px;
            background-color: white;
            transition: .4s;
            border-radius: 50%;
        }

        input:checked + .slider {
            background-color: var(--secondary);
        }

        input:checked + .slider:before {
            transform: translateX(18px);
        }

        /* Canvas Overlay for Confetti Particles */
        #confetti-canvas {
            position: fixed;
            top: 0; left: 0; width: 100%; height: 100%;
            pointer-events: none;
            z-index: 999;
        }

        /* Quick-fill button styles */
        .quick-fill {
            background: rgba(241, 196, 15, 0.15);
            border: 1px solid rgba(241, 196, 15, 0.4);
            color: #f1c40f;
            padding: 2px 6px;
            border-radius: 4px;
            font-size: 10px;
            cursor: pointer;
            font-weight: bold;
            display: inline-block;
            margin-left: 6px;
        }
    </style>
</head>
<body>
    <canvas id='confetti-canvas'></canvas>

    <div class='portal-container'>
        <div class='glass-card'>
            <!-- Header Branding -->
            <div class='header'>
                <div class='logos-container'>
                    <div class='logo-badge'>
                        <!-- Inline PMC Logo fallback svg if missing -->
                        <svg viewBox='0 0 100 100' style='width: 100%; height: 100%; fill: #0D3E73;'>
                            <circle cx='50' cy='50' r='45' fill='none' stroke='#0D3E73' stroke-width='6'/>
                            <path d='M30,30 L50,15 L70,30 L70,75 L30,75 Z' fill='#E67E22'/>
                            <path d='M40,40 L60,40 L60,65 L40,65 Z' fill='white'/>
                            <text x='50' y='88' font-size='10' font-weight='bold' text-anchor='middle' fill='#0D3E73'>PMC</text>
                        </svg>
                    </div>
                    <span class='plus-sign'>+</span>
                    <div class='logo-badge'>
                        <!-- Inline Indian Bank Logo fallback svg if missing -->
                        <svg viewBox='0 0 100 100' style='width: 100%; height: 100%;'>
                            <circle cx='50' cy='50' r='45' fill='#0054A6'/>
                            <polygon points='50,15 80,65 20,65' fill='#FFFFCC'/>
                            <circle cx='50' cy='48' r='18' fill='#0054A6'/>
                        </svg>
                    </div>
                </div>
                <div class='app-subtitle'>Patna Nagar Nigam</div>
                <h1>Resident Smart QR Portal</h1>
                <p id='portal-header-desc' class='sub-desc'>Scan your home's PVC QR plate to verify property records, view tax balances, and pay online.</p>
            </div>

            <!-- Error Banner -->
            <div id='error-banner' class='alert hidden'>
                <svg viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'>
                    <circle cx='12' cy='12' r='10'></circle>
                    <line x1='12' y1='8' x2='12' y2='12'></line>
                    <line x1='12' y1='16' x2='12.01' y2='16'></line>
                </svg>
                <span id='error-message'></span>
            </div>

            <!-- Warning Banner (For unassigned cards) -->
            <div id='warning-banner' class='alert alert-warning hidden'>
                <svg viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'>
                    <path d='M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z'></path>
                    <line x1='12' y1='9' x2='12' y2='13'></line>
                    <line x1='12' y1='17' x2='12.01' y2='17'></line>
                </svg>
                <span id='warning-message'></span>
            </div>

            <!-- Step 1: Scan / Enter Token Manually -->
            <div id='step-scan' class='hidden'>
                <div class='form-group'>
                    <label class='form-label' for='manual-token'>QR Token / Scan Value</label>
                    <div class='input-wrapper'>
                        <svg class='icon' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'>
                            <rect x='3' y='3' width='18' height='18' rx='2' ry='2'></rect>
                            <rect x='7' y='7' width='3' height='3'></rect>
                            <rect x='14' y='7' width='3' height='3'></rect>
                            <rect x='7' y='14' width='3' height='3'></rect>
                            <rect x='14' y='14' width='3' height='3'></rect>
                        </svg>
                        <input type='text' id='manual-token' placeholder='Enter QR ID (e.g. PMC00004567)' />
                    </div>
                    <div class='sub-desc' style='text-align: left; font-size: 11px; margin-top: 8px;'>
                        If you scanned a QR card, the token should load automatically. Otherwise, enter your card QR ID or Token.
                    </div>
                </div>
                <button id='btn-check-token' class='btn'>
                    <span>CHECK CARD STATUS</span>
                    <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'>
                        <line x1='5' y1='12' x2='19' y2='12'></line>
                        <polyline points='12 5 19 12 12 19'></polyline>
                    </svg>
                </button>
            </div>

            <!-- Step 2: Mobile & OTP Verification -->
            <div id='step-login'>
                <!-- Masked Mobile Info -->
                <div id='masked-mobile-alert' class='alert alert-warning hidden'>
                    <svg viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='2'>
                        <path d='M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z'></path>
                    </svg>
                    <div>
                        <strong>Verification Required:</strong>
                        <div id='masked-mobile-text'>This QR card is linked to a registered mobile ending in ****.</div>
                    </div>
                </div>

                <div id='div-mobile-input'>
                    <div class='form-group'>
                        <label class='form-label' for='citizen-mobile'>Registered Mobile Number</label>
                        <div class='input-wrapper'>
                            <svg class='icon' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'>
                                <rect x='5' y='2' width='14' height='20' rx='2' ry='2'></rect>
                                <line x1='12' y1='18' x2='12.01' y2='18'></line>
                            </svg>
                            <input type='tel' id='citizen-mobile' placeholder='Enter 10-digit mobile number' maxlength='10' />
                        </div>
                    </div>
                    <button id='btn-send-otp' class='btn'>
                        <span>GET VERIFICATION CODE</span>
                        <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2'>
                            <path d='M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z'></path>
                            <polyline points='22,6 12,13 2,6'></polyline>
                        </svg>
                    </button>
                </div>

                <div id='div-otp-input' class='hidden'>
                    <div class='form-group'>
                        <label class='form-label'>Enter 6-Digit OTP</label>
                        <div class='otp-inputs'>
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this, 1)' onkeydown='moveBack(this, 1)' id='otp-1' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this, 2)' onkeydown='moveBack(this, 2)' id='otp-2' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this, 3)' onkeydown='moveBack(this, 3)' id='otp-3' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this, 4)' onkeydown='moveBack(this, 4)' id='otp-4' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this, 5)' onkeydown='moveBack(this, 5)' id='otp-5' />
                            <input type='text' class='otp-box' maxlength='1' oninput='verifyOtpAuto(this)' onkeydown='moveBack(this, 6)' id='otp-6' />
                        </div>
                        <div class='sub-desc' style='margin-bottom: 15px;'>
                            We sent a code to your mobile number. Enter it to unlock your holding details.
                        </div>
                    </div>
                    <button id='btn-verify-otp' class='btn'>
                        <span>VERIFY & ACCESS records</span>
                        <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2'>
                            <rect x='3' y='11' width='18' height='11' rx='2' ry='2'></rect>
                            <path d='M7 11V7a5 5 0 0 1 10 0v4'></path>
                        </svg>
                    </button>
                    <button id='btn-resend-otp' class='btn btn-outline' style='margin-top: 12px;'>
                        RESEND OTP
                    </button>
                </div>
            </div>

            <!-- Step 3: Citizen Dashboard -->
            <div id='step-dashboard' class='hidden'>
                <div class='dashboard-header'>
                    <svg viewBox='0 0 24 24' width='26' height='26' fill='none' stroke='var(--success)' stroke-width='2.5'>
                        <path d='M22 11.08V12a10 10 0 1 1-5.93-9.14'></path>
                        <polyline points='22 4 12 14.01 9 11.01'></polyline>
                    </svg>
                    <div>
                        <h2 style='font-size: 16px; font-weight: 800; color: #ffffff;'>Property Verified</h2>
                        <div style='font-size: 11px; color: var(--text-light); text-transform: uppercase;'>PMC sparrow holding record</div>
                    </div>
                </div>

                <!-- Dues alert card -->
                <div id='dashboard-dues-box' class='dues-card'>
                    <div class='dues-row'>
                        <span class='dues-title'>Current Municipal Dues</span>
                        <span id='property-payment-status' class='status-badge'>PENDING</span>
                    </div>
                    <div class='dues-row'>
                        <span id='property-dues-amount' class='dues-amount'>₹0.00</span>
                        <button id='btn-pay-dues' class='btn' style='width: auto; padding: 8px 16px; font-size: 12px; border-radius: 8px;'>
                            PAY TAX ONLINE
                        </button>
                    </div>
                </div>

                <!-- Property details log -->
                <div class='detail-card'>
                    <div class='detail-row'>
                        <span class='detail-label'>Property ID (PID)</span>
                        <span id='detail-pid' class='detail-value highlight-value'></span>
                    </div>
                    <div class='detail-row'>
                        <span class='detail-label'>Registered Owner</span>
                        <span id='detail-owner' class='detail-value' style='color: var(--secondary); font-size:15px;'></span>
                    </div>
                    <div class='detail-row'>
                        <span class='detail-label'>Guardian/Father</span>
                        <span id='detail-guardian' class='detail-value'></span>
                    </div>
                    <div class='detail-row'>
                        <span class='detail-label'>Site Address</span>
                        <span id='detail-address' class='detail-value' style='font-size:13px; font-weight:500;'></span>
                    </div>
                    <div style='display: flex; gap: 15px; margin-top: 10px;'>
                        <div class='detail-row' style='flex: 1;'>
                            <span class='detail-label'>Ward Number</span>
                            <span id='detail-ward' class='detail-value'></span>
                        </div>
                        <div class='detail-row' style='flex: 1;'>
                            <span class='detail-label'>Revenue Circle</span>
                            <span id='detail-circle' class='detail-value'></span>
                        </div>
                    </div>
                </div>

                <!-- PVC Smart card digital representation -->
                <div class='pvc-preview-container' onclick='flipPvcCard()'>
                    <div id='digital-pvc-card' class='pvc-card'>
                        <div class='pvc-card-header'>
                            <span class='pvc-card-title'>PATNA NAGAR NIGAM</span>
                            <span style='font-size: 9px; color: var(--secondary); font-weight:800; border: 1.5px solid var(--secondary); padding: 1px 6px; border-radius: 4px;'>SMART QR</span>
                        </div>
                        <div class='pvc-card-body'>
                            <div class='pvc-details'>
                                <span style='font-size: 9px; color: rgba(255,255,255,0.6);'>PROPERTY IDENTITY CARD</span>
                                <span id='pvc-card-owner' style='font-size: 13.5px; font-weight:700; color:#fff;'></span>
                                <span id='pvc-card-pid' style='font-size: 11px; font-weight:bold; color: var(--secondary); margin-top: 4px;'>PID: </span>
                                <span id='pvc-card-qrid' style='font-size: 9px; color: rgba(255,255,255,0.5);'>QR ID: </span>
                            </div>
                            <div class='pvc-qr-sim'>
                                <!-- Inline barcode/QR representation -->
                                <svg viewBox='0 0 100 100'>
                                    <!-- Corners -->
                                    <path d='M0 0 H25 V10 H10 V25 H0 Z' fill='#000'/>
                                    <path d='M75 0 H100 V25 H90 V10 H75 Z' fill='#000'/>
                                    <path d='M0 75 H10 V90 H25 V100 H0 Z' fill='#000'/>
                                    <path d='M90 75 H100 V100 H75 V90 H90 Z' fill='#000'/>
                                    <!-- Boxes -->
                                    <rect x='15' y='15' width='25' height='25' fill='#000'/>
                                    <rect x='15' y='60' width='25' height='25' fill='#000'/>
                                    <rect x='60' y='15' width='25' height='25' fill='#000'/>
                                    <!-- Scattered random QR blocks for preview -->
                                    <rect x='48' y='20' width='6' height='10' fill='#000'/>
                                    <rect x='52' y='36' width='12' height='6' fill='#000'/>
                                    <rect x='20' y='46' width='8' height='8' fill='#000'/>
                                    <rect x='48' y='48' width='14' height='14' fill='#000'/>
                                    <rect x='68' y='48' width='8' height='16' fill='#000'/>
                                    <rect x='72' y='68' width='14' height='8' fill='#000'/>
                                    <rect x='52' y='76' width='12' height='12' fill='#000'/>
                                </svg>
                            </div>
                        </div>
                        <div class='pvc-tag-line'>Digital Address & Tax Identity Identity</div>
                    </div>
                </div>

                <button id='btn-logout' class='btn btn-outline' style='margin-top: 20px;'>
                    <span>LOGOUT & EXIT</span>
                    <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2'>
                        <path d='M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4'></path>
                        <polyline points='16 17 21 12 16 7'></polyline>
                        <line x1='21' y1='12' x2='9' y2='12'></line>
                    </svg>
                </button>
            </div>

            <!-- Demo Mode Configuration Toggle Panel -->
            <div class='demo-panel'>
                <div class='demo-toggle-container'>
                    <span>Demo Mode (Client Simulation)</span>
                    <label class='switch'>
                        <input type='checkbox' id='demo-mode-checkbox' checked onchange='toggleDemoMode()'>
                        <span class='slider'></span>
                    </label>
                </div>
                <div id='demo-info-text'>
                    Mock SQL Database seed active.<br>
                    Token: <strong id='demo-token-lbl'>5A9F9999-BB99-99AA-AA99-AA9999999999</strong><br>
                    Mock Mobile: <strong>9431881414</strong> <span class='quick-fill' onclick='quickFill()'>Auto Fill</span><br>
                    OTP Override: <strong>123456</strong>
                </div>
            </div>

        </div>
    </div>

    <script>
        // Seeded Mock Data for complete client-side simulation
        const mockQrs = {
            '5A9F9999-BB99-99AA-AA99-AA9999999999': {
                qrId: 'PMC/IND/DQR/00004567',
                status: 'ACTIVATED',
                propertyId: '1409113',
            },
            'CEF733F3-7416-4FFA-87F4-2D4361DA8791': {
                qrId: 'PMC/IND/DQR/00457417',
                status: 'UNASSIGNED',
                propertyId: '',
            }
        };

        const mockProperties = {
            '1409113': {
                pid: '1409113',
                ownerName: 'SRI KUMUD VISHAL',
                guardianName: 'SRI BALMIKI PRASAD SINGH',
                mobileNo: '9431881414',
                address: 'SHANTI VIHAR COLONY DIGHA PATNA',
                totalDues: '21906',
                paymentStatus: 'Pending',
                circle: 'Patliputra Circle',
                wardNo: '1'
            }
        };

        // Current page state
        let scanToken = '";
        html += token;
        html += @"'; 
        let isDemoMode = true;
        let generatedOtp = '123456';
        let currentStep = 'login'; // 'scan', 'login', 'dashboard'

        window.onload = function() {
            // Read query string token if present
            const urlParams = new URLSearchParams(window.location.search);
            const tokenParam = urlParams.get('token') || urlParams.get('value');
            if (tokenParam) {
                scanToken = tokenParam;
                document.getElementById('manual-token').value = tokenParam;
            }

            // Sync toggle
            document.getElementById('demo-mode-checkbox').checked = isDemoMode;
            
            initFlow();
        };

        function initFlow() {
            hideAllAlerts();
            
            if (!scanToken) {
                // Step 1: Manual entry
                showStep('scan');
                document.getElementById('portal-header-desc').innerText = 'Please scan a valid PMC smart QR plate fixed at your residence or enter your QR Token below.';
            } else {
                // Step 2: Validate token & load login
                validateCardToken();
            }
        }

        function showStep(stepName) {
            document.getElementById('step-scan').classList.add('hidden');
            document.getElementById('step-login').classList.add('hidden');
            document.getElementById('step-dashboard').classList.add('hidden');
            
            document.getElementById('step-' + stepName).classList.remove('hidden');
            currentStep = stepName;
        }

        function hideAllAlerts() {
            document.getElementById('error-banner').classList.add('hidden');
            document.getElementById('warning-banner').classList.add('hidden');
        }

        function showError(msg) {
            document.getElementById('error-message').innerText = msg;
            document.getElementById('error-banner').classList.remove('hidden');
            window.scrollTo({top: 0, behavior: 'smooth'});
        }

        function showWarning(msg) {
            document.getElementById('warning-message').innerText = msg;
            document.getElementById('warning-banner').classList.remove('hidden');
            window.scrollTo({top: 0, behavior: 'smooth'});
        }

        function toggleDemoMode() {
            isDemoMode = document.getElementById('demo-mode-checkbox').checked;
            document.getElementById('demo-info-text').classList.toggle('hidden', !isDemoMode);
            initFlow();
        }

        function quickFill() {
            document.getElementById('citizen-mobile').value = '9431881414';
        }

        function extractCleanToken(val) {
            let clean = val.trim();
            if (clean.includes('/qr/')) {
                clean = clean.split('/qr/').pop();
            } else if (clean.includes('token=')) {
                clean = clean.split('token=').pop().split('&')[0];
            }
            return clean;
        }

        // Action: Validate QR Card Token
        function validateCardToken() {
            hideAllAlerts();
            const cleanToken = extractCleanToken(scanToken);
            document.getElementById('demo-token-lbl').innerText = cleanToken;

            if (isDemoMode) {
                // Client Mock Lookup
                const card = mockQrs[cleanToken] || mockQrs['5A9F9999-BB99-99AA-AA99-AA9999999999']; // default fallback for test
                
                if (!card) {
                    showError('Scanned QR card token not found in mock database.');
                    showStep('scan');
                    return;
                }

                if (card.status !== 'ACTIVATED') {
                    showWarning('This QR code (' + card.qrId + ') is UNASSIGNED. PMC staff has not linked it to any Property ID (PID) yet.');
                    showStep('scan');
                    return;
                }

                const prop = mockProperties[card.propertyId];
                const last4 = prop && prop.mobileNo ? prop.mobileNo.substring(6) : '1414';
                
                document.getElementById('masked-mobile-text').innerText = 'This QR card is linked to a registered mobile ending in: ****' + last4;
                document.getElementById('masked-mobile-alert').classList.remove('hidden');
                document.getElementById('div-mobile-input').classList.remove('hidden');
                document.getElementById('div-otp-input').classList.add('hidden');
                
                showStep('login');
            } else {
                // Real server-side lookup via handler
                // We make an AJAX lookup check. For simplicity, we trigger send_otp which does validation on server.
                // But first we fetch card status
                fetch('CardLookupHandler.ashx?value=' + encodeURIComponent(cleanToken))
                    .then(response => response.json())
                    .then(res => {
                        if (res.success) {
                            const card = res.data;
                            if (card.status !== 'ACTIVATED') {
                                showWarning('This QR code (' + card.qrId + ') is ' + card.status + '. It has not been linked to a Property ID (PID) yet.');
                                showStep('scan');
                                return;
                            }
                            
                            // Get masked display. We'll verify mobile on OTP send
                            // Let's ask them to input registered mobile
                            document.getElementById('masked-mobile-text').innerText = 'This QR card is successfully registered. Please enter your mobile number linked with PMC tax holding.';
                            document.getElementById('masked-mobile-alert').classList.remove('hidden');
                            document.getElementById('div-mobile-input').classList.remove('hidden');
                            document.getElementById('div-otp-input').classList.add('hidden');
                            showStep('login');
                        } else {
                            showError(res.message || 'QR Card check failed.');
                            showStep('scan');
                        }
                    })
                    .catch(err => {
                        showError('Database connection error: ' + err.message);
                        showStep('scan');
                    });
            }
        }

        // Action: Request OTP SMS
        document.getElementById('btn-send-otp').onclick = function() {
            hideAllAlerts();
            const mobileInput = document.getElementById('citizen-mobile').value.trim();
            
            if (mobileInput.length !== 10 || isNaN(mobileInput)) {
                showError('Please enter a valid 10-digit registered mobile number.');
                return;
            }

            const cleanToken = extractCleanToken(scanToken);

            if (isDemoMode) {
                const card = mockQrs[cleanToken] || mockQrs['5A9F9999-BB99-99AA-AA99-AA9999999999'];
                const prop = mockProperties[card.propertyId];
                
                if (mobileInput !== prop.mobileNo && mobileInput !== '9431881414') {
                    showError('Incorrect mobile number. In Simulation Mode, enter the registered number: ' + prop.mobileNo);
                    return;
                }

                // Simulate OTP send
                generatedOtp = Math.floor(100000 + Math.random() * 900000).toString();
                
                // Show floating OTP for simulator
                alert('DEMO SMS SENT! Verification OTP is: ' + generatedOtp);
                
                document.getElementById('div-mobile-input').classList.add('hidden');
                document.getElementById('div-otp-input').classList.remove('hidden');
            } else {
                // Server API call
                document.getElementById('btn-send-otp').disabled = true;
                document.getElementById('btn-send-otp').innerText = 'SENDING CODE...';

                fetch('CitizenPortal.ashx?action=send_otp', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ token: cleanToken, mobile: mobileInput })
                })
                .then(r => r.json())
                .then(res => {
                    document.getElementById('btn-send-otp').disabled = false;
                    document.getElementById('btn-send-otp').innerText = 'GET VERIFICATION CODE';

                    if (res.success) {
                        generatedOtp = res.data.demoOtp; // For testing
                        if (generatedOtp) {
                            alert('DEMO OTP (Database server generated): ' + generatedOtp);
                        }
                        document.getElementById('div-mobile-input').classList.add('hidden');
                        document.getElementById('div-otp-input').classList.remove('hidden');
                    } else {
                        showError(res.message || 'OTP delivery failed.');
                    }
                })
                .catch(err => {
                    document.getElementById('btn-send-otp').disabled = false;
                    document.getElementById('btn-send-otp').innerText = 'GET VERIFICATION CODE';
                    showError('Server communication failure: ' + err.message);
                });
            }
        };

        // OTP navigation helper
        window.moveNext = function(el, index) {
            if (el.value.length === 1 && index < 6) {
                document.getElementById('otp-' + (index + 1)).focus();
            }
        };

        window.moveBack = function(el, index) {
            if (event.key === 'Backspace' && el.value.length === 0 && index > 1) {
                document.getElementById('otp-' + (index - 1)).focus();
            }
        };

        window.verifyOtpAuto = function(el) {
            if (el.value.length === 1) {
                // Auto trigger verification when 6th box is filled
                setTimeout(() => {
                    document.getElementById('btn-verify-otp').click();
                }, 100);
            }
        };

        // Action: Verify OTP and fetch property
        document.getElementById('btn-verify-otp').onclick = function() {
            hideAllAlerts();
            
            // Reconstruct OTP
            let otpCode = '';
            for (let i = 1; i <= 6; i++) {
                otpCode += document.getElementById('otp-' + i).value.trim();
            }

            if (otpCode.length !== 6 || isNaN(otpCode)) {
                showError('Please enter a 6-digit OTP code.');
                return;
            }

            const cleanToken = extractCleanToken(scanToken);

            if (isDemoMode) {
                if (otpCode !== generatedOtp && otpCode !== '123456') {
                    showError('Invalid code. Enter the OTP shown in the SMS alert or 123456.');
                    return;
                }

                // Successful verification
                const card = mockQrs[cleanToken] || mockQrs['5A9F9999-BB99-99AA-AA99-AA9999999999'];
                const prop = mockProperties[card.propertyId];
                
                loadDashboard(prop, card.qrId);
            } else {
                document.getElementById('btn-verify-otp').disabled = true;
                document.getElementById('btn-verify-otp').innerText = 'VERIFYING...';

                fetch('CitizenPortal.ashx?action=verify_otp', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ token: cleanToken, otp: otpCode })
                })
                .then(r => r.json())
                .then(res => {
                    document.getElementById('btn-verify-otp').disabled = false;
                    document.getElementById('btn-verify-otp').innerText = 'VERIFY & ACCESS RECORDS';

                    if (res.success) {
                        // Success -> get property details
                        const prop = res.data;
                        // Fetch QR ID for visual digital card
                        fetch('CardLookupHandler.ashx?value=' + encodeURIComponent(cleanToken))
                            .then(r2 => r2.json())
                            .then(res2 => {
                                const qrid = res2.success ? res2.data.qrId : 'PMC00004567';
                                loadDashboard(prop, qrid);
                            })
                            .catch(() => {
                                loadDashboard(prop, 'PMC00004567');
                            });
                    } else {
                        showError(res.message || 'Incorrect OTP code.');
                    }
                })
                .catch(err => {
                    document.getElementById('btn-verify-otp').disabled = false;
                    document.getElementById('btn-verify-otp').innerText = 'VERIFY & ACCESS RECORDS';
                    showError('OTP validation error: ' + err.message);
                });
            }
        };

        function loadDashboard(prop, qrId) {
            hideAllAlerts();
            
            // Fill values
            document.getElementById('detail-pid').innerText = prop.pid;
            document.getElementById('detail-owner').innerText = prop.ownerName;
            document.getElementById('detail-guardian').innerText = prop.guardianName || 'N/A';
            document.getElementById('detail-address').innerText = prop.address;
            document.getElementById('detail-ward').innerText = prop.wardNo || '1';
            document.getElementById('detail-circle').innerText = prop.circle || 'Patliputra Circle';

            // Dues
            const dues = parseFloat(prop.totalDues) || 0.0;
            document.getElementById('property-dues-amount').innerText = '₹' + dues.toLocaleString('en-IN', { minimumFractionDigits: 2 });
            
            const statusBadge = document.getElementById('property-payment-status');
            statusBadge.innerText = prop.paymentStatus.toUpperCase();
            
            if (prop.paymentStatus.toLowerCase() === 'paid' || dues === 0) {
                statusBadge.className = 'status-badge status-paid';
                document.getElementById('dashboard-dues-box').style.background = 'rgba(46, 204, 113, 0.08)';
                document.getElementById('dashboard-dues-box').style.borderColor = 'rgba(46, 204, 113, 0.25)';
                document.getElementById('btn-pay-dues').style.display = 'none';
            } else {
                statusBadge.className = 'status-badge status-pending';
                document.getElementById('dashboard-dues-box').style.background = 'rgba(230, 126, 34, 0.08)';
                document.getElementById('dashboard-dues-box').style.borderColor = 'rgba(230, 126, 34, 0.25)';
                document.getElementById('btn-pay-dues').style.display = 'block';
            }

            // Digital card
            document.getElementById('pvc-card-owner').innerText = prop.ownerName;
            document.getElementById('pvc-card-pid').innerText = 'PID: ' + prop.pid;
            document.getElementById('pvc-card-qrid').innerText = 'QR ID: ' + qrId;

            // Show dashboard step
            showStep('dashboard');
            
            // Adjust title
            document.getElementById('portal-header-desc').innerText = 'Digital Address Card & Municipal Tax Summary';

            // Trigger beautiful celebratory confetti particles
            triggerConfetti();
        }

        // Actions: Pay dues online
        document.getElementById('btn-pay-dues').onclick = function() {
            const dues = parseFloat(document.getElementById('detail-pid').innerText === '1409113' ? '21906' : '1902');
            
            // Check if Razorpay is loaded, otherwise use simulator fallback
            if (typeof Razorpay !== 'undefined') {
                var options = {
                    "key": "rzp_test_dummykey12345", // dummy test key
                    "amount": dues * 100, // in paise
                    "currency": "INR",
                    "name": "Patna Nagar Nigam",
                    "description": "Municipal Tax Payment for PID: " + document.getElementById('detail-pid').innerText,
                    "prefill": {
                        "name": document.getElementById('detail-owner').innerText,
                        "email": "citizen@patnanagarnigam.in",
                        "contact": document.getElementById('citizen-mobile').value || "9431881414"
                    },
                    "theme": {
                        "color": "#0D3E73"
                    },
                    "handler": function (response) {
                        alert("Payment Successful! Payment ID: " + response.razorpay_payment_id);
                        
                        // Mark as Paid on UI
                        document.getElementById('property-payment-status').innerText = 'PAID';
                        document.getElementById('property-payment-status').className = 'status-badge status-paid';
                        document.getElementById('property-dues-amount').innerText = '₹0.00';
                        document.getElementById('dashboard-dues-box').style.background = 'rgba(46, 204, 113, 0.08)';
                        document.getElementById('dashboard-dues-box').style.borderColor = 'rgba(46, 204, 113, 0.25)';
                        document.getElementById('btn-pay-dues').style.display = 'none';
                        triggerConfetti();
                    }
                };
                var rzp = new Razorpay(options);
                rzp.open();
            } else {
                // Fallback simulated payment if offline
                alert("Razorpay payment gateway script not loaded. Simulating payment success...");
                document.getElementById('property-payment-status').innerText = 'PAID';
                document.getElementById('property-payment-status').className = 'status-badge status-paid';
                document.getElementById('property-dues-amount').innerText = '₹0.00';
                document.getElementById('dashboard-dues-box').style.background = 'rgba(46, 204, 113, 0.08)';
                document.getElementById('dashboard-dues-box').style.borderColor = 'rgba(46, 204, 113, 0.25)';
                document.getElementById('btn-pay-dues').style.display = 'none';
                triggerConfetti();
            }
        };

        // Actions: Back check card status button click
        document.getElementById('btn-check-token').onclick = function() {
            const val = document.getElementById('manual-token').value.trim();
            if (!val) {
                showError('Please enter a QR Token or ID.');
                return;
            }
            scanToken = val;
            validateCardToken();
        };

        // Actions: Resend OTP
        document.getElementById('btn-resend-otp').onclick = function() {
            // Clear inputs
            for (let i = 1; i <= 6; i++) {
                document.getElementById('otp-' + i).value = '';
            }
            document.getElementById('otp-1').focus();
            document.getElementById('btn-send-otp').click();
        };

        // Actions: Logout
        document.getElementById('btn-logout').onclick = function() {
            // Clear sessions
            for (let i = 1; i <= 6; i++) {
                document.getElementById('otp-' + i).value = '';
            }
            document.getElementById('citizen-mobile').value = '';
            
            // Restart
            initFlow();
        };

        // Digital PVC Card Click Tilt/Flip Animation
        function flipPvcCard() {
            const card = document.getElementById('digital-pvc-card');
            card.style.transform = card.style.transform === 'rotateY(180deg)' ? 'rotateY(0deg)' : 'rotateY(180deg)';
        }

        // Particle Canvas Confetti celebration
        function triggerConfetti() {
            const canvas = document.getElementById('confetti-canvas');
            const ctx = canvas.getContext('2d');
            
            canvas.width = window.innerWidth;
            canvas.height = window.innerHeight;
            
            const particles = [];
            const colors = ['#E67E22', '#0D3E73', '#2ECC71', '#3498DB', '#9B59B6', '#E74C3C'];
            
            for (let i = 0; i < 120; i++) {
                particles.push({
                    x: Math.random() * canvas.width,
                    y: Math.random() * canvas.height - canvas.height,
                    r: Math.random() * 6 + 4,
                    d: Math.random() * canvas.height,
                    color: colors[Math.floor(Math.random() * colors.length)],
                    tilt: Math.random() * 10 - 5,
                    tiltAngleIncremental: Math.random() * 0.07 + 0.02,
                    tiltAngle: 0
                });
            }
            
            let animationFrameId;
            let counter = 0;
            
            function draw() {
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                
                particles.forEach((p, idx) => {
                    p.tiltAngle += p.tiltAngleIncremental;
                    p.y += (Math.cos(p.d) + 3 + p.r / 2) / 2;
                    p.x += Math.sin(p.tiltAngle);
                    p.tilt = Math.sin(p.tiltAngle - idx/3) * 15;
                    
                    ctx.beginPath();
                    ctx.lineWidth = p.r;
                    ctx.strokeStyle = p.color;
                    ctx.moveTo(p.x + p.tilt + p.r/2, p.y);
                    ctx.lineTo(p.x + p.tilt, p.y + p.tilt + p.r/2);
                    ctx.stroke();
                });
                
                update();
            }
            
            function update() {
                let remaining = 0;
                particles.forEach(p => {
                    if (p.y < canvas.height + 20) {
                        remaining++;
                    }
                });
                
                if (remaining > 0 && counter < 180) {
                    counter++;
                    animationFrameId = requestAnimationFrame(draw);
                } else {
                    ctx.clearRect(0, 0, canvas.width, canvas.height);
                    cancelAnimationFrame(animationFrameId);
                }
            }
            
            draw();
        }

        window.addEventListener('resize', function() {
            const canvas = document.getElementById('confetti-canvas');
            canvas.width = window.innerWidth;
            canvas.height = window.innerHeight;
        });
    </script>
</body>
</html>";
        context.Response.Write(html);
    }

    public bool IsReusable { get { return false; } }
}
