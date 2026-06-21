<%@ WebHandler Language="C#" Class="ActivateCardHandler" %>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class ActivateCardHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        
        // CORS Headers for Flutter App
        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        context.Response.AddHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

        if (context.Request.HttpMethod == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }

        if (context.Request.HttpMethod != "POST")
        {
            SendResponse(context, 405, false, "Only POST method is allowed", null);
            return;
        }

        try
        {
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                requestBody = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(requestBody))
            {
                SendResponse(context, 400, false, "Request body is empty", null);
                return;
            }

            var serializer = new JavaScriptSerializer();
            var payload = serializer.Deserialize<Dictionary<string, object>>(requestBody);

            if (payload == null || !payload.ContainsKey("qrid") || !payload.ContainsKey("pid") || !payload.ContainsKey("staff_id"))
            {
                SendResponse(context, 400, false, "Missing parameters. Required: qrid, pid, staff_id", null);
                return;
            }

            string qrid = GetString(payload, "qrid").Trim();
            string pid = GetString(payload, "pid").Trim();
            string staffId = GetString(payload, "staff_id").Trim();
            string deviceId = payload.ContainsKey("device_id") ? GetString(payload, "device_id") : "";
            
            decimal geoLat = 0;
            decimal geoLong = 0;
            if (payload.ContainsKey("geo_lat")) decimal.TryParse(GetString(payload, "geo_lat"), out geoLat);
            if (payload.ContainsKey("geo_long")) decimal.TryParse(GetString(payload, "geo_long"), out geoLong);

            string clientIp = context.Request.UserHostAddress;
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                
                // Start SQL Transaction to ensure atomicity
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Verify that the QR Card exists and check its current status
                        string qrCheckQuery = @"
                            SELECT Status, PropertyId 
                            FROM QRMaster 
                            WHERE QRId = @QRId";
                        
                        string currentStatus = "";
                        string existingPropertyId = "";
                        
                        using (SqlCommand cmd = new SqlCommand(qrCheckQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@QRId", qrid);
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                if (sdr.Read())
                                {
                                    currentStatus = sdr["Status"].ToString().ToUpper();
                                    existingPropertyId = sdr["PropertyId"].ToString();
                                }
                                else
                                {
                                    sdr.Close();
                                    trans.Rollback();
                                    SendResponse(context, 404, false, "Scanned QR card was not found in the QRMaster database. Please verify the code.", null);
                                    return;
                                }
                            }
                        }

                        // Security check: Only UNASSIGNED cards can be activated
                        if (currentStatus == "ACTIVATED")
                        {
                            trans.Rollback();
                            SendResponse(context, 400, false, "This QR plate has already been activated and is linked to Property ID: " + existingPropertyId + ". It cannot be re-linked without supervisor overrides.", null);
                            return;
                        }

                        // 2. Verify that the PID exists in All_Demand
                        string pidCheckQuery = "SELECT COUNT(*) FROM All_Demand WHERE PID = @PID";
                        using (SqlCommand cmd = new SqlCommand(pidCheckQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            int pidCount = Convert.ToInt32(cmd.ExecuteScalar());
                            if (pidCount == 0)
                            {
                                trans.Rollback();
                                SendResponse(context, 404, false, "The Property ID (PID) entered: " + pid + " does not exist in municipal records. Please check the spelling.", null);
                                return;
                            }
                        }

                        // 3. Deactivate any previously active QR plates for this Property (only 1 active plate per property)
                        string deactivateOldQuery = @"
                            UPDATE QRMaster 
                            SET Status = 'REPLACED', ActivatedDate = GETDATE(), ActivatedBy = @StaffId 
                            WHERE PropertyId = @PID AND Status = 'ACTIVATED'";
                        
                        using (SqlCommand cmd = new SqlCommand(deactivateOldQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            cmd.Parameters.AddWithValue("@StaffId", staffId);
                            cmd.ExecuteNonQuery();
                        }

                        // 4. Update status in QRMaster to ACTIVATED and save PID mapping
                        string activateQRQuery = @"
                            UPDATE QRMaster 
                            SET Status = 'ACTIVATED', 
                                PropertyId = @PID, 
                                ActivatedDate = GETDATE(), 
                                ActivatedBy = @StaffId 
                            WHERE QRId = @QRId";
                        
                        using (SqlCommand cmd = new SqlCommand(activateQRQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            cmd.Parameters.AddWithValue("@StaffId", staffId);
                            cmd.Parameters.AddWithValue("@QRId", qrid);
                            cmd.ExecuteNonQuery();
                        }

                        // 5. Stamp the All_Demand table to show it is ACTIVE
                        string updateDemandQuery = @"
                            UPDATE [dbo].[All_Demand]
                            SET Card_Status = 'ACTIVE',
                                Card_Print_Date = GETDATE(),
                                Last_Scanned = GETDATE(),
                                Scan_Count = ISNULL(Scan_Count, 0) + 1
                            WHERE PID = @PID";
                        
                        using (SqlCommand cmd = new SqlCommand(updateDemandQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            cmd.ExecuteNonQuery();
                        }

                        // 6. Log this activation in the scans log table (tbl_QR_ScanLog)
                        string logQuery = @"
                            INSERT INTO tbl_QR_ScanLog 
                                (PID, Staff_Id, Scan_Type, Scan_Timestamp, Device_ID, Geo_Lat, Geo_Long, IP_Address, Is_Suspicious, Remarks)
                            VALUES 
                                (TRY_CAST(@PID AS BIGINT), @StaffId, 'ACTIVATION', GETDATE(), @DeviceId, @GeoLat, @GeoLong, @IP, 0, 'PVC QR Plate Activated on site')";
                        
                        using (SqlCommand cmd = new SqlCommand(logQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            cmd.Parameters.AddWithValue("@StaffId", staffId);
                            cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                            cmd.Parameters.AddWithValue("@GeoLat", geoLat == 0 ? DBNull.Value : (object)geoLat);
                            cmd.Parameters.AddWithValue("@GeoLong", geoLong == 0 ? DBNull.Value : (object)geoLong);
                            cmd.Parameters.AddWithValue("@IP", clientIp);
                            cmd.ExecuteNonQuery();
                        }

                        // Commit Transaction
                        trans.Commit();
                        SendResponse(context, 200, true, "QR card successfully linked to Property ID: " + pid, null);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SendResponse(context, 500, false, "Transaction failed: " + ex.Message, null);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SendResponse(context, 500, false, "Database error: " + ex.Message, null);
        }
    }

    private void SendResponse(HttpContext context, int statusCode, bool success, string message, object data)
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

    private string GetString(Dictionary<string, object> d, string key)
    {
        object val;
        return d.TryGetValue(key, out val) && val != null ? val.ToString() : "";
    }

    public bool IsReusable { get { return false; } }
}
