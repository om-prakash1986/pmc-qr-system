using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// ActivateCardHandler.ashx.cs
/// Activates a QR card and links it to a Property ID.
/// POST: { "qrid": "...", "pid": "...", "staff_id": "...", "device_id": "...", "geo_lat": 0, "geo_long": 0 }
/// </summary>
public class ActivateCardHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS") { context.Response.StatusCode = 200; return; }
        if (context.Request.HttpMethod != "POST") { SendResponse(context, 405, false, "Only POST is allowed", null); return; }

        try
        {
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                requestBody = reader.ReadToEnd();

            if (string.IsNullOrEmpty(requestBody)) { SendResponse(context, 400, false, "Request body is empty", null); return; }

            var serializer = new JavaScriptSerializer();
            var payload    = serializer.Deserialize<Dictionary<string, object>>(requestBody);

            if (payload == null || (!payload.ContainsKey("qrid") && !payload.ContainsKey("qrtoken")) || !payload.ContainsKey("pid") || !payload.ContainsKey("staff_id"))
            { SendResponse(context, 400, false, "Missing parameters. Required: qrid or qrtoken, pid, staff_id", null); return; }

            string  qrid     = payload.ContainsKey("qrid") ? GetString(payload, "qrid").Trim() : "";
            string  qrtoken  = payload.ContainsKey("qrtoken") ? GetString(payload, "qrtoken").Trim() : "";
            string  pid      = GetString(payload, "pid").Trim();
            string  staffId  = GetString(payload, "staff_id").Trim();
            string  deviceId = payload.ContainsKey("device_id") ? GetString(payload, "device_id") : "";
            decimal geoLat = 0, geoLong = 0;
            if (payload.ContainsKey("geo_lat"))  decimal.TryParse(GetString(payload, "geo_lat"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out geoLat);
            if (payload.ContainsKey("geo_long")) decimal.TryParse(GetString(payload, "geo_long"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out geoLong);

            string clientIp = context.Request.UserHostAddress;
            string connStr  = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Check QR exists and status
                        string currentStatus = "", existingPid = "";
                        string resolvedQrId = "";
                        using (SqlCommand cmd = new SqlCommand("SELECT QRId, Status, PropertyId FROM QRMaster WHERE QRId=@QRId OR (QRToken=@Token AND @Token <> '')", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@QRId", qrid);
                            cmd.Parameters.AddWithValue("@Token", qrtoken.Length > 0 ? qrtoken : qrid); // Use qrid as fallback if qrtoken is empty
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                if (sdr.Read()) { 
                                    resolvedQrId = sdr["QRId"].ToString();
                                    currentStatus = sdr["Status"].ToString().ToUpper(); 
                                    existingPid = sdr["PropertyId"].ToString(); 
                                }
                                else            { sdr.Close(); trans.Rollback(); SendResponse(context, 404, false, "QR card not found in QRMaster.", null); return; }
                            }
                        }

                        if (currentStatus == "ACTIVATED")
                        { trans.Rollback(); SendResponse(context, 400, false, "This QR plate is already activated for PID: " + existingPid, null); return; }

                        // 2. Verify PID exists
                        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tbl_property_detail WHERE pid=@PID AND status IN (1,2,3,4)", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                            { trans.Rollback(); SendResponse(context, 404, false, "PID " + pid + " not found in municipal records.", null); return; }
                        }

                        // 3. Deactivate previous QR for this property
                        using (SqlCommand cmd = new SqlCommand("UPDATE QRMaster SET Status='REPLACED', ActivatedDate=GETDATE(), ActivatedBy=@StaffId WHERE PropertyId=@PID AND Status='ACTIVATED'", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID",     pid);
                            cmd.Parameters.AddWithValue("@StaffId", staffId);
                            cmd.ExecuteNonQuery();
                        }

                        // 4. Activate the new QR card using resolved QRId
                        using (SqlCommand cmd = new SqlCommand("UPDATE QRMaster SET Status='ACTIVATED', PropertyId=@PID, ActivatedDate=GETDATE(), ActivatedBy=@StaffId WHERE QRId=@QRId", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID",     pid);
                            cmd.Parameters.AddWithValue("@StaffId", staffId);
                            cmd.Parameters.AddWithValue("@QRId",    resolvedQrId);
                            cmd.ExecuteNonQuery();
                        }

                        // 5. Removed All_Demand update as it is deprecated

                        // 6. Log activation
                        string logSql = @"INSERT INTO tbl_QR_ScanLog (PID, Staff_Id, Scan_Type, Scan_Timestamp, Device_ID, Geo_Lat, Geo_Long, IP_Address, Is_Suspicious, Remarks)
                                          VALUES (TRY_CAST(@PID AS BIGINT), @StaffId, 'ACTIVATION', GETDATE(), @DeviceId, @GeoLat, @GeoLong, @IP, 0, 'PVC QR Plate Activated on site')";
                        using (SqlCommand cmd = new SqlCommand(logSql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID",      pid);
                            cmd.Parameters.AddWithValue("@StaffId",  staffId);
                            cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                            cmd.Parameters.AddWithValue("@GeoLat",   geoLat  == 0 ? DBNull.Value : (object)geoLat);
                            cmd.Parameters.AddWithValue("@GeoLong",  geoLong == 0 ? DBNull.Value : (object)geoLong);
                            cmd.Parameters.AddWithValue("@IP",       clientIp);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        SendResponse(context, 200, true, "QR card successfully linked to PID: " + pid, null);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SendResponse(context, 500, false, "Transaction failed: " + ex.Message, null);
                    }
                }
            }
        }
        catch (Exception ex) { SendResponse(context, 500, false, "Database error: " + ex.Message, null); }
    }

    private void SendResponse(HttpContext context, int statusCode, bool success, string message, object data)
    {
        context.Response.StatusCode = statusCode;
        var ser = new JavaScriptSerializer();
        context.Response.Write(ser.Serialize(new Dictionary<string, object>
        {
            { "success", success }, { "message", message }, { "data", data }
        }));
    }

    private string GetString(Dictionary<string, object> d, string key)
    {
        object val;
        return d.TryGetValue(key, out val) && val != null ? val.ToString() : "";
    }

    public bool IsReusable { get { return false; } }
}
