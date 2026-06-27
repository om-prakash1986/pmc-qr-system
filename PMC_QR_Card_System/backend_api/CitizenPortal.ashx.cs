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

        // Default: GET request — serve the HTML Portal
        context.Response.ContentType = "text/html";
        ServeHtmlPortal(context);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST handlers
    // ─────────────────────────────────────────────────────────────────────────

    private void HandleTaxLogin(HttpContext context)
    {
        try
        {
            string body;
            using (var rdr = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                body = rdr.ReadToEnd();

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
                            string storedPassword = sdr["PasswordHash"].ToString();
                            bool isActive = Convert.ToBoolean(sdr["IsActive"]);
                            if (storedPassword.Equals(password) && isActive)
                            {
                                context.Session["Staff_Id"] = sdr["StaffId"].ToString();
                                var data = new
                                {
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
                body = rdr.ReadToEnd();

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

            string cleanToken = ExtractToken(token);
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            string propertyId = "";
            string registeredMobile = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // 1. Validate QR card
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
                                SendJsonResponse(context, 400, false, "This QR card is not activated. Status: " + status, null);
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

                // 2. Get registered mobile
                string propQuery = @"
                    SELECT TOP 1 own.mobile_no
                    FROM tbl_property_detail pd
                    LEFT JOIN tbl_owner_detail own ON pd.id = own.property_id
                    WHERE pd.pid = @PID AND pd.status IN (1,2,3,4)";

                using (SqlCommand cmd = new SqlCommand(propQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", propertyId);
                    object mobileVal = cmd.ExecuteScalar();
                    if (mobileVal != null && mobileVal != DBNull.Value)
                        registeredMobile = mobileVal.ToString().Trim();
                }
            }

            // 3. Validate mobile
            string cleanInputMobile = CleanMobile(mobile);
            string cleanDbMobile    = CleanMobile(registeredMobile);

            if (string.IsNullOrEmpty(cleanDbMobile))
            {
                // No mobile on file — allow and proceed
                registeredMobile = mobile;
                cleanDbMobile    = cleanInputMobile;
            }

            if (cleanInputMobile != cleanDbMobile)
            {
                SendJsonResponse(context, 401, false,
                    "Mobile number does not match our records. Registered mobile ends in: " + GetMaskedSuffix(registeredMobile), null);
                return;
            }

            // 4. Generate OTP and store in DB using pre-existing tbl_QR_OTPLogin table
            Random rand = new Random();
            string otp = rand.Next(100000, 999999).ToString();

            using (SqlConnection connOtp = new SqlConnection(connStr))
            {
                connOtp.Open();

                // Delete any existing OTP for this PID to prevent conflicts
                string deleteOld = "DELETE FROM tbl_QR_OTPLogin WHERE PID = @PID";
                using (SqlCommand cmd = new SqlCommand(deleteOld, connOtp))
                {
                    cmd.Parameters.AddWithValue("@PID", propertyId);
                    cmd.ExecuteNonQuery();
                }

                string insertOtp = @"
                    INSERT INTO tbl_QR_OTPLogin (Mobile, PID, OTP_Hash, Expires_At, Is_Used, Attempts, Created_At)
                    VALUES (@Mobile, @PID, @OTP, DATEADD(MINUTE, 15, GETDATE()), 0, 0, GETDATE())";
                using (SqlCommand cmd = new SqlCommand(insertOtp, connOtp))
                {
                    cmd.Parameters.AddWithValue("@Mobile", mobile);
                    cmd.Parameters.AddWithValue("@PID",    propertyId);
                    cmd.Parameters.AddWithValue("@OTP",    otp);
                    cmd.ExecuteNonQuery();
                }
            }

            // 5. Send OTP via PMC SMS gateway
            try
            {
                // Using "New Assessment" to match the original DLT template perfectly. 
                // The ASMX proxy handles the appending of " - Bihar Government." and the template ID internally.
                string message = "Your OTP for New Assessment is " + otp + " for new assessment form of Property Tax - Patna Municipal Corporation";
                
                // URL encoding the message to ensure spaces and special characters are passed correctly in the GET request
                string strUrl = "https://pmc.bihar.gov.in/service/pmcsendsms.asmx/SendSms?mobileNo=" + mobile + "&message=" + HttpUtility.UrlEncode(message) + "&key=7110EDA4D09E062AA5E4A390B0A572AC0D2C0220";
                
                // Create the request object  
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strUrl);
                request.Timeout = 10000; // 10 second timeout to prevent hanging the Flutter UI
                
                // Get the response back  
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                using (Stream s = response.GetResponseStream())
                using (StreamReader readStream = new StreamReader(s))
                {
                    string dataString = readStream.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine("[HandleSendOtp] Proxy SMS response for " + mobile + ": " + dataString);
                }
            }
            catch (Exception smsEx)
            {
                // SMS failure is non-fatal: OTP is still stored in DB and response is sent to client.
                System.Diagnostics.Debug.WriteLine("[HandleSendOtp] SMS send failed: " + smsEx.Message);
            }

            SendJsonResponse(context, 200, true,
                "Verification code sent to mobile: " + GetMaskedMobile(registeredMobile),
                new { maskedMobile = GetMaskedMobile(registeredMobile) });
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
                body = rdr.ReadToEnd();

            var serializer = new JavaScriptSerializer();
            var payload = serializer.Deserialize<Dictionary<string, string>>(body);

            if (payload == null || !payload.ContainsKey("token") || !payload.ContainsKey("otp"))
            {
                SendJsonResponse(context, 400, false, "Token and OTP parameters are required", null);
                return;
            }

            string token      = payload["token"].Trim();
            string otp        = payload["otp"].Trim();
            string cleanToken = ExtractToken(token);

            // Fetch stored OTP from DB (session-free approach)
            string storedOtp  = null;
            string propertyId = null;
            string connStr2   = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection connOtp = new SqlConnection(connStr2))
            {
                connOtp.Open();

                // 1. Get Property ID from QRMaster using the token
                string q = "SELECT PropertyId FROM QRMaster WHERE QRToken = @Token OR QRId = @Token OR QRUrl = @Token";
                using (SqlCommand cmd = new SqlCommand(q, connOtp))
                {
                    cmd.Parameters.AddWithValue("@Token", cleanToken);
                    object val = cmd.ExecuteScalar();
                    if (val != null) propertyId = val.ToString().Trim();
                }

                if (!string.IsNullOrEmpty(propertyId))
                {
                    // 2. Fetch active OTP from tbl_QR_OTPLogin
                    string fetchOtp = @"
                        SELECT OTP_Hash FROM tbl_QR_OTPLogin
                        WHERE PID = @PID AND Expires_At > GETDATE() AND Is_Used = 0";
                    using (SqlCommand cmd = new SqlCommand(fetchOtp, connOtp))
                    {
                        cmd.Parameters.AddWithValue("@PID", propertyId);
                        object val = cmd.ExecuteScalar();
                        if (val != null) storedOtp = val.ToString().Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(storedOtp))
            {
                SendJsonResponse(context, 401, false, "OTP expired or not requested. Please request a new OTP.", null);
                return;
            }

            if (otp.Trim() != storedOtp.Trim())
            {
                SendJsonResponse(context, 401, false, "Incorrect verification code (" + otp.Trim() + "). Please try again.", null);
                return;
            }

            // If propertyId not in session (edge case), re-fetch from DB
            if (string.IsNullOrEmpty(propertyId))
            {
                string connStr0 = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
                using (SqlConnection conn0 = new SqlConnection(connStr0))
                {
                    string q = "SELECT PropertyId FROM QRMaster WHERE QRToken = @Token OR QRId = @Token OR QRUrl = @Token";
                    using (SqlCommand cmd = new SqlCommand(q, conn0))
                    {
                        cmd.Parameters.AddWithValue("@Token", cleanToken);
                        conn0.Open();
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

            // Fetch property details
            Dictionary<string, object> propDetails = null;
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT TOP 1
                        pd.pid            AS PID,
                        own.owner_name    AS Owner_Name,
                        own.guardian_name AS Guardian_Name,
                        pd.mobile_no      AS Mobile_No,
                        pd.address        AS Address,
                        wm.ward_no        AS Ward_No,
                        cm.circle_name    AS Circle,
                        pd.plot_area      AS Plot_Area,
                        pd.constructed_area AS Constructed_Area
                    FROM tbl_property_detail pd
                    LEFT JOIN tbl_owner_detail own ON pd.id = own.property_id
                    LEFT JOIN tbl_ward_master  wm  ON pd.ward_id = wm.id
                    LEFT JOIN tbl_circle_master cm  ON wm.circle_id = cm.id
                    WHERE pd.pid = @PID AND pd.status IN (1,2,3,4)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", propertyId);
                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            // Calculate outstanding dues
                            decimal dynamicDues = 0;
                            try
                            {
                                PMC.demandbysms dbs = new PMC.demandbysms();
                                dynamicDues = dbs.getDemandForMutation(propertyId);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("[HandleVerifyOtp] Dues calc error: " + ex);
                            }

                            decimal roundedDues = Math.Round(dynamicDues, 0);
                            string paymentStatus = (roundedDues > 0) ? "Pending" : "Paid";

                            propDetails = new Dictionary<string, object>
                            {
                                { "pid",             sdr["PID"].ToString() },
                                { "ownerName",       sdr["Owner_Name"]       == DBNull.Value ? "" : sdr["Owner_Name"].ToString() },
                                { "guardianName",    sdr["Guardian_Name"]    == DBNull.Value ? "" : sdr["Guardian_Name"].ToString() },
                                { "mobileNo",        sdr["Mobile_No"]        == DBNull.Value ? "" : sdr["Mobile_No"].ToString() },
                                { "address",         sdr["Address"]          == DBNull.Value ? "" : sdr["Address"].ToString() },
                                { "totalDues",       roundedDues.ToString() },
                                { "paymentStatus",   paymentStatus },
                                { "circle",          sdr["Circle"]           == DBNull.Value ? "" : sdr["Circle"].ToString() },
                                { "wardNo",          sdr["Ward_No"]          == DBNull.Value ? "" : sdr["Ward_No"].ToString() },
                                { "plotArea",        sdr["Plot_Area"]        == DBNull.Value ? "" : sdr["Plot_Area"].ToString() },
                                { "constructedArea", sdr["Constructed_Area"] == DBNull.Value ? "" : sdr["Constructed_Area"].ToString() }
                            };
                        }
                    }
                }

                if (propDetails != null)
                {
                    // Log citizen scan
                    string logQuery = @"
                        INSERT INTO tbl_QR_ScanLog
                            (PID, Staff_Id, Scan_Type, Scan_Timestamp, IP_Address, Is_Suspicious, Remarks)
                        VALUES
                            (TRY_CAST(@PID AS BIGINT), 'CITIZEN', 'CITIZEN_PORTAL_SCAN', GETDATE(), @IP, 0, 'Citizen verified via OTP')";

                    using (SqlCommand cmd = new SqlCommand(logQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PID", propertyId);
                        cmd.Parameters.AddWithValue("@IP", context.Request.UserHostAddress ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            if (propDetails == null)
            {
                SendJsonResponse(context, 404, false, "Property details not found.", null);
                return;
            }

            // Invalidate OTP after use (mark as used in DB)
            using (SqlConnection connDel = new SqlConnection(connStr))
            {
                connDel.Open();
                string delOtp = "UPDATE tbl_QR_OTPLogin SET Is_Used = 1 WHERE PID = @PID";
                using (SqlCommand cmd = new SqlCommand(delOtp, connDel))
                {
                    cmd.Parameters.AddWithValue("@PID", propertyId);
                    cmd.ExecuteNonQuery();
                }
            }

            SendJsonResponse(context, 200, true, "Authentication successful.", propDetails);
        }
        catch (Exception ex)
        {
            SendJsonResponse(context, 500, false, "Server error: " + ex.Message, null);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

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
            catch { }
        }
        return t;
    }

    private string CleanMobile(string m)
    {
        if (string.IsNullOrEmpty(m)) return "";
        string clean = "";
        foreach (char c in m)
            if (char.IsDigit(c)) clean += c;
        if (clean.Length > 10)
            clean = clean.Substring(clean.Length - 10);
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
            { "data",    data    }
        };
        context.Response.Write(ser.Serialize(res));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HTML Portal (real API calls only — no demo mode)
    // ─────────────────────────────────────────────────────────────────────────

    private void ServeHtmlPortal(HttpContext context)
    {
        string token = context.Request.QueryString["token"] ?? "";

        string html = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>PMC Smart QR Citizen Portal</title>
    <meta name='description' content='Verify property records, view tax dues and pay online — Patna Nagar Nigam Smart QR Card.'>
    <link href='https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700;800&display=swap' rel='stylesheet'>
    <script src='https://checkout.razorpay.com/v1/checkout.js'></script>
    <style>
        :root {
            --primary:      #0D3E73;
            --secondary:    #E67E22;
            --success:      #2ECC71;
            --danger:       #E74C3C;
            --background:   #0B192C;
            --border:       rgba(255,255,255,0.12);
            --text-light:   #BDC3C7;
        }
        * { box-sizing:border-box; margin:0; padding:0; font-family:'Outfit',sans-serif; transition:all 0.3s cubic-bezier(.25,.8,.25,1); }
        body {
            background: radial-gradient(circle at 50% 50%, #15304f 0%, #0B192C 100%);
            min-height:100vh; color:#fff; display:flex; flex-direction:column;
            align-items:center; justify-content:center; overflow-x:hidden; padding:20px;
        }
        .portal-container { width:100%; max-width:480px; }
        .glass-card {
            background:rgba(13,62,115,0.25); backdrop-filter:blur(20px);
            -webkit-backdrop-filter:blur(20px); border:1px solid var(--border);
            border-radius:28px; box-shadow:0 15px 35px rgba(0,0,0,.4);
            padding:30px; position:relative; overflow:hidden;
        }
        .glass-card::before {
            content:''; position:absolute; top:-50%; left:-50%; width:200%; height:200%;
            background:radial-gradient(circle,rgba(255,255,255,.03) 0%,transparent 70%);
            pointer-events:none;
        }
        .header { text-align:center; margin-bottom:24px; }
        .logos-container { display:flex; align-items:center; justify-content:center; gap:12px; margin-bottom:16px; }
        .logo-badge {
            background:white; padding:6px; border-radius:12px;
            box-shadow:0 4px 10px rgba(0,0,0,.15);
            display:flex; align-items:center; justify-content:center;
            width:45px; height:45px; border:1px solid rgba(13,62,115,.1);
        }
        .logo-badge img { width:100%; height:100%; object-fit:contain; }
        .plus-sign { font-size:18px; font-weight:700; color:var(--secondary); }
        .app-subtitle { font-size:11px; text-transform:uppercase; letter-spacing:2.5px; color:var(--secondary); font-weight:700; margin-bottom:4px; }
        h1 { font-size:22px; font-weight:800; color:#fff; letter-spacing:.5px; }
        .sub-desc { font-size:13.5px; color:var(--text-light); text-align:center; margin-top:6px; line-height:1.4; }
        .form-group { margin-bottom:20px; position:relative; }
        .form-label { display:block; font-size:12px; font-weight:600; color:var(--secondary); margin-bottom:8px; letter-spacing:1px; text-transform:uppercase; }
        .input-wrapper { position:relative; }
        .input-wrapper input {
            width:100%; background:rgba(255,255,255,.06); border:1.5px solid var(--border);
            border-radius:16px; padding:15px 15px 15px 45px; color:white; font-size:15px; font-weight:500; outline:none;
        }
        .input-wrapper input:focus { background:rgba(255,255,255,.1); border-color:var(--secondary); box-shadow:0 0 12px rgba(230,126,34,.25); }
        .input-wrapper .icon { position:absolute; left:16px; top:50%; transform:translateY(-50%); color:var(--text-light); pointer-events:none; width:18px; height:18px; }
        .btn {
            width:100%; background:linear-gradient(135deg,var(--secondary) 0%,#d35400 100%);
            border:none; border-radius:16px; color:white; padding:16px; font-size:15px;
            font-weight:700; cursor:pointer; box-shadow:0 4px 15px rgba(230,126,34,.3);
            display:flex; align-items:center; justify-content:center; gap:8px; letter-spacing:.5px;
        }
        .btn:hover:not(:disabled) { transform:translateY(-2px); box-shadow:0 6px 20px rgba(230,126,34,.45); }
        .btn:active:not(:disabled) { transform:translateY(1px); }
        .btn:disabled { background:#7f8c8d; box-shadow:none; cursor:not-allowed; opacity:.6; }
        .btn-outline { background:transparent; border:1.5px solid var(--border); color:#fff; box-shadow:none; margin-top:12px; }
        .btn-outline:hover:not(:disabled) { background:rgba(255,255,255,.05); border-color:rgba(255,255,255,.25); box-shadow:none; }
        .alert {
            background:rgba(231,76,60,.12); border:1px solid rgba(231,76,60,.3);
            border-radius:14px; padding:12px 16px; color:#ff7675; font-size:13px;
            display:flex; align-items:flex-start; gap:10px; margin-bottom:20px; line-height:1.4;
        }
        .alert-success { background:rgba(46,204,113,.12); border-color:rgba(46,204,113,.3); color:#2ecc71; }
        .alert-warning { background:rgba(243,156,18,.12); border-color:rgba(243,156,18,.3); color:#f1c40f; }
        .hidden { display:none !important; }
        .otp-inputs { display:flex; justify-content:space-between; gap:10px; margin-bottom:24px; }
        .otp-box {
            width:50px; height:52px; background:rgba(255,255,255,.06);
            border:1.5px solid var(--border); border-radius:12px;
            text-align:center; color:white; font-size:20px; font-weight:700; outline:none;
        }
        .otp-box:focus { border-color:var(--secondary); box-shadow:0 0 10px rgba(230,126,34,.2); background:rgba(255,255,255,.1); }
        .dashboard-header { display:flex; align-items:center; gap:12px; border-bottom:1.5px solid var(--border); padding-bottom:14px; margin-bottom:18px; }
        .detail-card { background:rgba(255,255,255,.04); border:1px solid var(--border); border-radius:18px; padding:16px; margin-bottom:14px; }
        .detail-row { display:flex; flex-direction:column; margin-bottom:10px; }
        .detail-row:last-child { margin-bottom:0; }
        .detail-label { font-size:11px; color:var(--text-light); text-transform:uppercase; letter-spacing:.8px; margin-bottom:2px; }
        .detail-value { font-size:14.5px; font-weight:600; color:#fff; }
        .highlight-value { color:#54a0ff; font-size:16px; }
        .dues-card {
            background:rgba(230,126,34,.08); border:1.5px solid rgba(230,126,34,.25);
            border-radius:18px; padding:16px; margin-bottom:16px; display:flex; flex-direction:column; gap:6px;
        }
        .dues-row { display:flex; justify-content:space-between; align-items:center; }
        .dues-title { font-size:13px; font-weight:600; color:var(--text-light); }
        .dues-amount { font-size:20px; font-weight:800; color:var(--secondary); }
        .status-badge { padding:4px 10px; border-radius:6px; font-size:11px; font-weight:700; text-transform:uppercase; letter-spacing:.5px; }
        .status-pending { background:rgba(231,76,60,.15); color:var(--danger); border:1px solid rgba(231,76,60,.25); }
        .status-paid    { background:rgba(46,204,113,.15); color:var(--success); border:1px solid rgba(46,204,113,.25); }
        .pvc-preview-container { margin-top:18px; perspective:800px; cursor:pointer; }
        .pvc-card {
            width:100%; height:190px;
            background:linear-gradient(135deg,#092e56 0%,#1e4b7a 50%,#0d3e73 100%);
            border:1.5px solid rgba(255,255,255,.12); border-radius:18px; padding:16px;
            position:relative; box-shadow:0 8px 20px rgba(0,0,0,.3);
            display:flex; flex-direction:column; justify-content:space-between; overflow:hidden;
        }
        .pvc-card::after {
            content:''; position:absolute; top:0; left:0; width:100%; height:100%;
            background:linear-gradient(45deg,rgba(255,255,255,0) 30%,rgba(255,255,255,.06) 50%,rgba(255,255,255,0) 70%);
            background-size:200% 200%; animation:shine 4s infinite linear;
        }
        @keyframes shine { 0%{background-position:-200% 0} 100%{background-position:200% 0} }
        .pvc-card-header { display:flex; justify-content:space-between; align-items:center; }
        .pvc-card-title { font-size:11px; font-weight:700; color:#fff; letter-spacing:1.5px; }
        .pvc-card-body { display:flex; justify-content:space-between; align-items:flex-end; margin-top:12px; }
        .pvc-details { display:flex; flex-direction:column; gap:2px; }
        .pvc-qr-sim { background:white; padding:6px; border-radius:8px; display:flex; align-items:center; justify-content:center; width:70px; height:70px; }
        .pvc-qr-sim svg { width:100%; height:100%; }
        .pvc-tag-line { font-size:7.5px; color:rgba(255,255,255,.4); text-transform:uppercase; letter-spacing:1.5px; margin-top:4px; text-align:center; }
        /* Receipt */
        #receipt-container {
            display:none; max-width:800px; margin:40px auto; background:#fff; padding:40px;
            border-radius:8px; box-shadow:0 10px 30px rgba(0,0,0,.1); position:relative;
            color:#333; font-family:'Outfit',sans-serif; overflow:hidden;
        }
        #receipt-container::before {
            content:''; position:absolute; top:50%; left:50%;
            transform:translate(-50%,-50%); width:400px; height:400px;
            background:url('https://upload.wikimedia.org/wikipedia/commons/thumb/e/e4/Patna_Municipal_Corporation_Logo.png/600px-Patna_Municipal_Corporation_Logo.png') no-repeat center center;
            background-size:contain; opacity:.05; z-index:0; pointer-events:none;
        }
        .receipt-header { display:flex; align-items:center; justify-content:space-between; border-bottom:2px solid #0D3E73; padding-bottom:20px; margin-bottom:30px; position:relative; z-index:1; }
        .receipt-header img { height:80px; }
        .receipt-title { text-align:center; flex-grow:1; }
        .receipt-title h2 { margin:0; color:#0D3E73; font-size:24px; font-weight:800; letter-spacing:1px; }
        .receipt-title p { margin:5px 0 0; font-size:14px; color:#666; font-weight:500; }
        .receipt-info-grid { display:grid; grid-template-columns:1fr 1fr; gap:20px; margin-bottom:30px; position:relative; z-index:1; }
        .info-box { background:#f8fafc; padding:15px; border-radius:6px; border:1px solid #e2e8f0; }
        .info-box label { display:block; font-size:11px; text-transform:uppercase; color:#64748b; margin-bottom:5px; font-weight:600; }
        .info-box .val { font-size:15px; color:#0f172a; font-weight:600; }
        .receipt-table { width:100%; border-collapse:collapse; margin-bottom:30px; position:relative; z-index:1; }
        .receipt-table th, .receipt-table td { border:1px solid #cbd5e1; padding:12px 15px; text-align:left; }
        .receipt-table th { background:#0D3E73; color:white; font-weight:600; font-size:13px; text-transform:uppercase; }
        .receipt-table td { font-size:15px; font-weight:500; }
        .receipt-table .amount-col { text-align:right; }
        .receipt-total { background:#f1f5f9; font-weight:700!important; font-size:18px!important; color:#0D3E73; }
        .receipt-footer { text-align:center; font-size:12px; color:#64748b; margin-top:40px; border-top:1px dashed #cbd5e1; padding-top:20px; position:relative; z-index:1; }
        .receipt-actions { text-align:center; margin-top:30px; }
        .receipt-actions button { background:#10B981; color:white; border:none; padding:12px 30px; font-size:16px; font-weight:600; border-radius:8px; cursor:pointer; margin:0 10px; }
        .receipt-actions button:hover { background:#059669; transform:translateY(-2px); }
        #confetti-canvas { position:fixed; top:0; left:0; width:100%; height:100%; pointer-events:none; z-index:999; }
        @media print {
            body * { visibility:hidden; }
            #receipt-container, #receipt-container * { visibility:visible; }
            #receipt-container { position:absolute; left:0; top:0; width:100%; margin:0; padding:20px; box-shadow:none; border:none; }
            .receipt-actions { display:none!important; }
        }
    </style>
</head>
<body>
    <div id='receipt-container'>
        <div class='receipt-header'>
            <img src='https://upload.wikimedia.org/wikipedia/commons/thumb/e/e4/Patna_Municipal_Corporation_Logo.png/600px-Patna_Municipal_Corporation_Logo.png' alt='PMC Logo'>
            <div class='receipt-title'>
                <h2>PATNA NAGAR NIGAM</h2>
                <p>Smart QR Card System &mdash; Municipal Tax Receipt</p>
            </div>
            <div style='width:80px;'></div>
        </div>
        <div class='receipt-info-grid'>
            <div class='info-box'><label>Transaction ID</label><div class='val' id='receipt-tx-id'>-</div></div>
            <div class='info-box'><label>Date &amp; Time</label><div class='val' id='receipt-date'>-</div></div>
            <div class='info-box'><label>Property ID (PID)</label><div class='val' id='receipt-pid'>-</div></div>
            <div class='info-box'><label>Owner Name</label><div class='val' id='receipt-owner'>-</div></div>
            <div class='info-box' style='grid-column:span 2;'><label>Property Address</label><div class='val' id='receipt-address'>-</div></div>
        </div>
        <table class='receipt-table'>
            <thead><tr><th>Description</th><th>Payment Mode</th><th class='amount-col'>Amount (INR)</th></tr></thead>
            <tbody>
                <tr><td>Municipal Property Tax Payment</td><td>Online Payment Gateway</td><td class='amount-col' id='receipt-amount'>-</td></tr>
                <tr class='receipt-total'><td colspan='2' style='text-align:right;'>Total Amount Paid</td><td class='amount-col' id='receipt-total'>-</td></tr>
            </tbody>
        </table>
        <div class='receipt-footer'>This is a computer-generated receipt. No physical signature required.<br>Powered by Patna Nagar Nigam Smart QR Card System.</div>
        <div class='receipt-actions'>
            <button onclick='window.print()'>Print Receipt</button>
            <button onclick='window.location.reload()'>Back to Dashboard</button>
        </div>
    </div>

    <canvas id='confetti-canvas'></canvas>

    <div class='portal-container'>
        <div class='glass-card'>
            <!-- Branding -->
            <div class='header'>
                <div class='logos-container'>
                    <div class='logo-badge'>
                        <svg viewBox='0 0 100 100' style='width:100%;height:100%;fill:#0D3E73;'>
                            <circle cx='50' cy='50' r='45' fill='none' stroke='#0D3E73' stroke-width='6'/>
                            <path d='M30,30 L50,15 L70,30 L70,75 L30,75 Z' fill='#E67E22'/>
                            <path d='M40,40 L60,40 L60,65 L40,65 Z' fill='white'/>
                            <text x='50' y='88' font-size='10' font-weight='bold' text-anchor='middle' fill='#0D3E73'>PMC</text>
                        </svg>
                    </div>
                    <span class='plus-sign'>+</span>
                    <div class='logo-badge'>
                        <svg viewBox='0 0 100 100' style='width:100%;height:100%;'>
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

            <!-- Alerts -->
            <div id='error-banner' class='alert hidden'>
                <svg viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='2'><circle cx='12' cy='12' r='10'/><line x1='12' y1='8' x2='12' y2='12'/><line x1='12' y1='16' x2='12.01' y2='16'/></svg>
                <span id='error-message'></span>
            </div>
            <div id='warning-banner' class='alert alert-warning hidden'>
                <svg viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='2'><path d='M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z'/><line x1='12' y1='9' x2='12' y2='13'/><line x1='12' y1='17' x2='12.01' y2='17'/></svg>
                <span id='warning-message'></span>
            </div>

            <!-- Step 1: Manual token entry -->
            <div id='step-scan' class='hidden'>
                <div class='form-group'>
                    <label class='form-label' for='manual-token'>QR Token / Card ID</label>
                    <div class='input-wrapper'>
                        <svg class='icon' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><rect x='3' y='3' width='18' height='18' rx='2'/><rect x='7' y='7' width='3' height='3'/><rect x='14' y='7' width='3' height='3'/><rect x='7' y='14' width='3' height='3'/><rect x='14' y='14' width='3' height='3'/></svg>
                        <input type='text' id='manual-token' placeholder='Enter QR ID or Token' />
                    </div>
                    <div class='sub-desc' style='text-align:left;font-size:11px;margin-top:8px;'>If you scanned a QR card the token loads automatically. Otherwise, enter your card QR ID.</div>
                </div>
                <button id='btn-check-token' class='btn'>
                    <span>CHECK CARD STATUS</span>
                    <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2'><line x1='5' y1='12' x2='19' y2='12'/><polyline points='12 5 19 12 12 19'/></svg>
                </button>
            </div>

            <!-- Step 2: Mobile & OTP -->
            <div id='step-login'>
                <div id='masked-mobile-alert' class='alert alert-warning hidden'>
                    <svg viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='2'><path d='M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z'/></svg>
                    <div><strong>Verification Required:</strong><div id='masked-mobile-text'>This QR card is linked to a registered mobile.</div></div>
                </div>
                <div id='div-mobile-input'>
                    <div class='form-group'>
                        <label class='form-label' for='citizen-mobile'>Registered Mobile Number</label>
                        <div class='input-wrapper'>
                            <svg class='icon' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><rect x='5' y='2' width='14' height='20' rx='2'/><line x1='12' y1='18' x2='12.01' y2='18'/></svg>
                            <input type='tel' id='citizen-mobile' placeholder='Enter 10-digit mobile number' maxlength='10' />
                        </div>
                    </div>
                    <button id='btn-send-otp' class='btn'>
                        <span>GET VERIFICATION CODE</span>
                        <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2'><path d='M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z'/><polyline points='22,6 12,13 2,6'/></svg>
                    </button>
                </div>
                <div id='div-otp-input' class='hidden'>
                    <div class='form-group'>
                        <label class='form-label'>Enter 6-Digit OTP</label>
                        <div class='otp-inputs'>
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this,1)' onkeydown='moveBack(this,1)' id='otp-1' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this,2)' onkeydown='moveBack(this,2)' id='otp-2' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this,3)' onkeydown='moveBack(this,3)' id='otp-3' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this,4)' onkeydown='moveBack(this,4)' id='otp-4' />
                            <input type='text' class='otp-box' maxlength='1' oninput='moveNext(this,5)' onkeydown='moveBack(this,5)' id='otp-5' />
                            <input type='text' class='otp-box' maxlength='1' oninput='verifyOtpAuto(this)' onkeydown='moveBack(this,6)' id='otp-6' />
                        </div>
                        <div class='sub-desc' style='margin-bottom:15px;'>Enter the 6-digit code sent to your registered mobile number.</div>
                    </div>
                    <button id='btn-verify-otp' class='btn'>
                        <span>VERIFY &amp; ACCESS RECORDS</span>
                        <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2'><rect x='3' y='11' width='18' height='11' rx='2'/><path d='M7 11V7a5 5 0 0 1 10 0v4'/></svg>
                    </button>
                    <button id='btn-resend-otp' class='btn btn-outline' style='margin-top:12px;'>RESEND OTP</button>
                </div>
            </div>

            <!-- Step 3: Dashboard -->
            <div id='step-dashboard' class='hidden'>
                <div class='dashboard-header'>
                    <svg viewBox='0 0 24 24' width='26' height='26' fill='none' stroke='var(--success)' stroke-width='2.5'><path d='M22 11.08V12a10 10 0 1 1-5.93-9.14'/><polyline points='22 4 12 14.01 9 11.01'/></svg>
                    <div>
                        <h2 style='font-size:16px;font-weight:800;color:#fff;'>Property Verified</h2>
                        <div style='font-size:11px;color:var(--text-light);text-transform:uppercase;'>PMC Sparrow Holding Record</div>
                    </div>
                </div>
                <div id='dashboard-dues-box' class='dues-card'>
                    <div class='dues-row'>
                        <span class='dues-title'>Current Municipal Dues</span>
                        <span id='property-payment-status' class='status-badge'>PENDING</span>
                    </div>
                    <div class='dues-row'>
                        <span id='property-dues-amount' class='dues-amount'>&#8377;0.00</span>
                        <button id='btn-pay-dues' class='btn' style='width:auto;padding:8px 16px;font-size:12px;border-radius:8px;'>PAY TAX ONLINE</button>
                    </div>
                </div>
                <div class='detail-card'>
                    <div class='detail-row'><span class='detail-label'>Property ID (PID)</span><span id='detail-pid' class='detail-value highlight-value'></span></div>
                    <div class='detail-row'><span class='detail-label'>Registered Owner</span><span id='detail-owner' class='detail-value' style='color:var(--secondary);font-size:15px;'></span></div>
                    <div class='detail-row'><span class='detail-label'>Guardian/Father</span><span id='detail-guardian' class='detail-value'></span></div>
                    <div class='detail-row'><span class='detail-label'>Site Address</span><span id='detail-address' class='detail-value' style='font-size:13px;font-weight:500;'></span></div>
                    <div style='display:flex;gap:15px;margin-top:10px;'>
                        <div class='detail-row' style='flex:1;'><span class='detail-label'>Ward Number</span><span id='detail-ward' class='detail-value'></span></div>
                        <div class='detail-row' style='flex:1;'><span class='detail-label'>Revenue Circle</span><span id='detail-circle' class='detail-value'></span></div>
                    </div>
                </div>
                <div class='pvc-preview-container' onclick='flipPvcCard()'>
                    <div id='digital-pvc-card' class='pvc-card'>
                        <div class='pvc-card-header'>
                            <span class='pvc-card-title'>PATNA NAGAR NIGAM</span>
                            <span style='font-size:9px;color:var(--secondary);font-weight:800;border:1.5px solid var(--secondary);padding:1px 6px;border-radius:4px;'>SMART QR</span>
                        </div>
                        <div class='pvc-card-body'>
                            <div class='pvc-details'>
                                <span style='font-size:9px;color:rgba(255,255,255,.6);'>PROPERTY IDENTITY CARD</span>
                                <span id='pvc-card-owner' style='font-size:13.5px;font-weight:700;color:#fff;'></span>
                                <span id='pvc-card-pid' style='font-size:11px;font-weight:bold;color:var(--secondary);margin-top:4px;'>PID: </span>
                                <span id='pvc-card-qrid' style='font-size:9px;color:rgba(255,255,255,.5);'>QR ID: </span>
                            </div>
                            <div class='pvc-qr-sim'>
                                <svg viewBox='0 0 100 100'>
                                    <path d='M0 0 H25 V10 H10 V25 H0 Z' fill='#000'/>
                                    <path d='M75 0 H100 V25 H90 V10 H75 Z' fill='#000'/>
                                    <path d='M0 75 H10 V90 H25 V100 H0 Z' fill='#000'/>
                                    <path d='M90 75 H100 V100 H75 V90 H90 Z' fill='#000'/>
                                    <rect x='15' y='15' width='25' height='25' fill='#000'/>
                                    <rect x='15' y='60' width='25' height='25' fill='#000'/>
                                    <rect x='60' y='15' width='25' height='25' fill='#000'/>
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
                        <div class='pvc-tag-line'>Digital Address &amp; Tax Identity</div>
                    </div>
                </div>
                <button id='btn-logout' class='btn btn-outline' style='margin-top:20px;'>
                    <span>LOGOUT &amp; EXIT</span>
                    <svg viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='2'><path d='M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4'/><polyline points='16 17 21 12 16 7'/><line x1='21' y1='12' x2='9' y2='12'/></svg>
                </button>
            </div>
        </div>
    </div>

    <script>
        // ── State ──────────────────────────────────────────────────────────
        let scanToken = '";
        html += token;
        html += @"';
        let currentDuesAmount = 0;

        // ── Bootstrap ─────────────────────────────────────────────────────
        window.onload = function() {
            const urlParams = new URLSearchParams(window.location.search);
            const tokenParam = urlParams.get('token') || urlParams.get('value');
            if (tokenParam) {
                scanToken = tokenParam;
                const mi = document.getElementById('manual-token');
                if (mi) mi.value = tokenParam;
            }
            initFlow();
        };

        function initFlow() {
            hideAllAlerts();
            if (!scanToken) {
                showStep('scan');
                document.getElementById('portal-header-desc').innerText = 'Scan a valid PMC smart QR plate or enter your QR Token below.';
            } else {
                validateCardToken();
            }
        }

        // ── UI helpers ─────────────────────────────────────────────────────
        function showStep(stepName) {
            ['scan','login','dashboard'].forEach(s =>
                document.getElementById('step-' + s).classList.add('hidden'));
            document.getElementById('step-' + stepName).classList.remove('hidden');
        }

        function hideAllAlerts() {
            document.getElementById('error-banner').classList.add('hidden');
            document.getElementById('warning-banner').classList.add('hidden');
        }

        function showError(msg) {
            document.getElementById('error-message').innerText = msg;
            document.getElementById('error-banner').classList.remove('hidden');
            window.scrollTo({top:0,behavior:'smooth'});
        }

        function showWarning(msg) {
            document.getElementById('warning-message').innerText = msg;
            document.getElementById('warning-banner').classList.remove('hidden');
            window.scrollTo({top:0,behavior:'smooth'});
        }

        function extractCleanToken(val) {
            let clean = val.trim();
            if (clean.includes('/qr/'))     clean = clean.split('/qr/').pop();
            else if (clean.includes('token=')) clean = clean.split('token=').pop().split('&')[0];
            return clean;
        }

        // ── Step 1: Validate token via server ─────────────────────────────
        function validateCardToken() {
            hideAllAlerts();
            const cleanToken = extractCleanToken(scanToken);

            fetch('CardLookupHandler.ashx?value=' + encodeURIComponent(cleanToken))
                .then(r => r.json())
                .then(res => {
                    if (res.success) {
                        const card = res.data;
                        if (card.status !== 'ACTIVATED') {
                            showWarning('This QR code (' + card.qrId + ') is ' + card.status + '. It has not been linked to a Property ID yet.');
                            showStep('scan');
                            return;
                        }
                        document.getElementById('masked-mobile-text').innerText =
                            'QR card verified. Enter the mobile number registered with your PMC property holding.';
                        document.getElementById('masked-mobile-alert').classList.remove('hidden');
                        document.getElementById('div-mobile-input').classList.remove('hidden');
                        document.getElementById('div-otp-input').classList.add('hidden');
                        showStep('login');
                    } else {
                        showError(res.message || 'QR card check failed.');
                        showStep('scan');
                    }
                })
                .catch(err => {
                    showError('Server error: ' + err.message);
                    showStep('scan');
                });
        }

        // ── Step 2a: Send OTP ──────────────────────────────────────────────
        document.getElementById('btn-send-otp').onclick = function() {
            hideAllAlerts();
            const mobileInput = document.getElementById('citizen-mobile').value.trim();
            if (mobileInput.length !== 10 || isNaN(mobileInput)) {
                showError('Please enter a valid 10-digit mobile number.');
                return;
            }

            const cleanToken = extractCleanToken(scanToken);
            const btn = document.getElementById('btn-send-otp');
            btn.disabled = true;
            btn.querySelector('span').innerText = 'SENDING CODE...';

            fetch('CitizenPortal.ashx?action=send_otp', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: cleanToken, mobile: mobileInput })
            })
            .then(r => r.json())
            .then(res => {
                btn.disabled = false;
                btn.querySelector('span').innerText = 'GET VERIFICATION CODE';
                if (res.success) {
                    document.getElementById('div-mobile-input').classList.add('hidden');
                    document.getElementById('div-otp-input').classList.remove('hidden');
                    document.getElementById('otp-1').focus();
                } else {
                    showError(res.message || 'OTP delivery failed.');
                }
            })
            .catch(err => {
                btn.disabled = false;
                btn.querySelector('span').innerText = 'GET VERIFICATION CODE';
                showError('Server communication failure: ' + err.message);
            });
        };

        // ── OTP box navigation ─────────────────────────────────────────────
        window.moveNext = function(el, index) {
            if (el.value.length === 1 && index < 6)
                document.getElementById('otp-' + (index + 1)).focus();
        };

        window.moveBack = function(el, index) {
            if (event.key === 'Backspace' && el.value.length === 0 && index > 1)
                document.getElementById('otp-' + (index - 1)).focus();
        };

        window.verifyOtpAuto = function(el) {
            if (el.value.length === 1)
                setTimeout(() => document.getElementById('btn-verify-otp').click(), 100);
        };

        // ── Step 2b: Verify OTP ────────────────────────────────────────────
        document.getElementById('btn-verify-otp').onclick = function() {
            hideAllAlerts();
            let otpCode = '';
            for (let i = 1; i <= 6; i++)
                otpCode += document.getElementById('otp-' + i).value.trim();

            if (otpCode.length !== 6 || isNaN(otpCode)) {
                showError('Please enter the 6-digit OTP code.');
                return;
            }

            const cleanToken = extractCleanToken(scanToken);
            const btn = document.getElementById('btn-verify-otp');
            btn.disabled = true;
            btn.querySelector('span').innerText = 'VERIFYING...';

            fetch('CitizenPortal.ashx?action=verify_otp', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: cleanToken, otp: otpCode })
            })
            .then(r => r.json())
            .then(res => {
                btn.disabled = false;
                btn.querySelector('span').innerText = 'VERIFY & ACCESS RECORDS';
                if (res.success) {
                    const prop = res.data;
                    // Fetch QR ID for PVC card display
                    fetch('CardLookupHandler.ashx?value=' + encodeURIComponent(cleanToken))
                        .then(r2 => r2.json())
                        .then(res2 => {
                            loadDashboard(prop, res2.success ? res2.data.qrId : cleanToken);
                        })
                        .catch(() => loadDashboard(prop, cleanToken));
                } else {
                    showError(res.message || 'Incorrect OTP code.');
                }
            })
            .catch(err => {
                btn.disabled = false;
                btn.querySelector('span').innerText = 'VERIFY & ACCESS RECORDS';
                showError('OTP validation error: ' + err.message);
            });
        };

        // ── Step 3: Dashboard ──────────────────────────────────────────────
        function loadDashboard(prop, qrId) {
            hideAllAlerts();
            document.getElementById('detail-pid').innerText      = prop.pid;
            document.getElementById('detail-owner').innerText    = prop.ownerName;
            document.getElementById('detail-guardian').innerText = prop.guardianName || 'N/A';
            document.getElementById('detail-address').innerText  = prop.address;
            document.getElementById('detail-ward').innerText     = prop.wardNo   || '';
            document.getElementById('detail-circle').innerText   = prop.circle   || '';

            const dues = parseFloat(prop.totalDues) || 0;
            currentDuesAmount = dues;
            document.getElementById('property-dues-amount').innerText =
                '\u20B9' + dues.toLocaleString('en-IN', { minimumFractionDigits: 2 });

            const badge = document.getElementById('property-payment-status');
            badge.innerText = prop.paymentStatus.toUpperCase();

            if (prop.paymentStatus.toLowerCase() === 'paid' || dues === 0) {
                badge.className = 'status-badge status-paid';
                document.getElementById('dashboard-dues-box').style.background   = 'rgba(46,204,113,.08)';
                document.getElementById('dashboard-dues-box').style.borderColor  = 'rgba(46,204,113,.25)';
                document.getElementById('btn-pay-dues').style.display = 'none';
            } else {
                badge.className = 'status-badge status-pending';
                document.getElementById('dashboard-dues-box').style.background   = 'rgba(230,126,34,.08)';
                document.getElementById('dashboard-dues-box').style.borderColor  = 'rgba(230,126,34,.25)';
                document.getElementById('btn-pay-dues').style.display = 'block';
            }

            document.getElementById('pvc-card-owner').innerText = prop.ownerName;
            document.getElementById('pvc-card-pid').innerText   = 'PID: ' + prop.pid;
            document.getElementById('pvc-card-qrid').innerText  = 'QR ID: ' + qrId;

            showStep('dashboard');
            document.getElementById('portal-header-desc').innerText = 'Digital Address Card & Municipal Tax Summary';
            triggerConfetti();
        }

        // ── Pay dues ───────────────────────────────────────────────────────
        document.getElementById('btn-pay-dues').onclick = function() {
            if (typeof Razorpay === 'undefined') {
                showError('Payment gateway not loaded. Please check your internet connection.');
                return;
            }
            const dues = currentDuesAmount;
            var options = {
                key:         'rzp_live_REPLACE_WITH_REAL_KEY',
                amount:      Math.round(dues * 100),
                currency:    'INR',
                name:        'Patna Nagar Nigam',
                description: 'Municipal Tax — PID: ' + document.getElementById('detail-pid').innerText,
                prefill: {
                    name:    document.getElementById('detail-owner').innerText,
                    email:   'citizen@patnanagarnigam.in',
                    contact: document.getElementById('citizen-mobile').value || ''
                },
                theme: { color: '#0D3E73' },
                handler: async function(response) {
                    try {
                        const pid       = document.getElementById('detail-pid').innerText;
                        const ownerName = document.getElementById('detail-owner').innerText;
                        const mobileNo  = document.getElementById('citizen-mobile').value || '';
                        const paymentId = response.razorpay_payment_id;

                        const res = await fetch('/ProcessPaymentHandler.ashx', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ pid, amount: dues, paymentId, ownerName, mobileNo })
                        });
                        const data = await res.json();

                        if (data.success) {
                            document.getElementById('receipt-tx-id').innerText   = paymentId;
                            document.getElementById('receipt-date').innerText    = new Date().toLocaleString('en-IN');
                            document.getElementById('receipt-pid').innerText     = pid;
                            document.getElementById('receipt-owner').innerText   = ownerName;
                            document.getElementById('receipt-address').innerText = document.getElementById('detail-address').innerText;
                            document.getElementById('receipt-amount').innerText  = dues.toFixed(2);
                            document.getElementById('receipt-total').innerText   = '\u20B9' + dues.toFixed(2);

                            document.querySelector('.portal-container').style.display = 'none';
                            document.getElementById('receipt-container').style.display = 'block';
                            triggerConfetti();
                        } else {
                            showError('Payment recorded but database update failed: ' + data.message);
                        }
                    } catch(e) {
                        showError('Error contacting server after payment: ' + e.message);
                    }
                }
            };
            new Razorpay(options).open();
        };

        // ── Misc button handlers ───────────────────────────────────────────
        document.getElementById('btn-check-token').onclick = function() {
            const val = document.getElementById('manual-token').value.trim();
            if (!val) { showError('Please enter a QR Token or ID.'); return; }
            scanToken = val;
            validateCardToken();
        };

        document.getElementById('btn-resend-otp').onclick = function() {
            for (let i = 1; i <= 6; i++) document.getElementById('otp-' + i).value = '';
            document.getElementById('otp-1').focus();
            document.getElementById('btn-send-otp').click();
        };

        document.getElementById('btn-logout').onclick = function() {
            for (let i = 1; i <= 6; i++) document.getElementById('otp-' + i).value = '';
            document.getElementById('citizen-mobile').value = '';
            initFlow();
        };

        // ── PVC card flip ──────────────────────────────────────────────────
        function flipPvcCard() {
            const card = document.getElementById('digital-pvc-card');
            card.style.transform = card.style.transform === 'rotateY(180deg)' ? 'rotateY(0deg)' : 'rotateY(180deg)';
        }

        // ── Confetti ───────────────────────────────────────────────────────
        function triggerConfetti() {
            const canvas = document.getElementById('confetti-canvas');
            const ctx    = canvas.getContext('2d');
            canvas.width  = window.innerWidth;
            canvas.height = window.innerHeight;

            const colors = ['#E67E22','#0D3E73','#2ECC71','#3498DB','#9B59B6','#E74C3C'];
            const particles = Array.from({length:120}, () => ({
                x:     Math.random() * canvas.width,
                y:     Math.random() * canvas.height - canvas.height,
                r:     Math.random() * 6 + 4,
                d:     Math.random() * canvas.height,
                color: colors[Math.floor(Math.random() * colors.length)],
                tilt:  Math.random() * 10 - 5,
                tiltAngleIncremental: Math.random() * 0.07 + 0.02,
                tiltAngle: 0
            }));

            let counter = 0, rafId;
            function draw() {
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                particles.forEach((p, i) => {
                    p.tiltAngle += p.tiltAngleIncremental;
                    p.y += (Math.cos(p.d) + 3 + p.r / 2) / 2;
                    p.x += Math.sin(p.tiltAngle);
                    p.tilt = Math.sin(p.tiltAngle - i/3) * 15;
                    ctx.beginPath();
                    ctx.lineWidth   = p.r;
                    ctx.strokeStyle = p.color;
                    ctx.moveTo(p.x + p.tilt + p.r/2, p.y);
                    ctx.lineTo(p.x + p.tilt, p.y + p.tilt + p.r/2);
                    ctx.stroke();
                });
                if (counter++ < 180) rafId = requestAnimationFrame(draw);
                else { ctx.clearRect(0, 0, canvas.width, canvas.height); cancelAnimationFrame(rafId); }
            }
            draw();
        }

        window.addEventListener('resize', () => {
            const c = document.getElementById('confetti-canvas');
            c.width = window.innerWidth; c.height = window.innerHeight;
        });
    </script>
</body>
</html>";
        context.Response.Write(html);
    }

    public bool IsReusable { get { return false; } }
}
