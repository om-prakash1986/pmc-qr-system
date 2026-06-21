<%@ WebHandler Language="C#" Class="CardLookupHandler" %>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class CardLookupHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        
        // CORS Headers for Flutter App
        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        context.Response.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

        if (context.Request.HttpMethod == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }

        if (context.Request.HttpMethod != "GET")
        {
            SendResponse(context, 405, false, "Only GET method is allowed", null);
            return;
        }

        string scannedValue = context.Request.QueryString["value"];
        if (string.IsNullOrEmpty(scannedValue))
        {
            scannedValue = context.Request.QueryString["token"];
        }
        if (string.IsNullOrEmpty(scannedValue))
        {
            scannedValue = context.Request.QueryString["qrid"];
        }

        if (string.IsNullOrEmpty(scannedValue))
        {
            SendResponse(context, 400, false, "Scanned value, token, or qrid query parameter is required", null);
            return;
        }

        string originalValue = scannedValue.Trim();
        string cleanToken = originalValue;

        // Extract GUID token from URL if a URL was scanned
        if (originalValue.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                Uri uri = new Uri(originalValue);
                string lastSegment = uri.Segments[uri.Segments.Length - 1].Trim('/');
                if (!string.IsNullOrEmpty(lastSegment))
                {
                    cleanToken = lastSegment;
                }
            }
            catch
            {
                // Fall back to original value if URL parsing fails
            }
        }

        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT QRId, QRToken, Status, PropertyId, CreatedDate, ActivatedDate, ActivatedBy, QRUrl 
                    FROM QRMaster 
                    WHERE QRToken = @Token OR QRId = @QRId OR QRUrl = @QRUrl";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", cleanToken);
                    cmd.Parameters.AddWithValue("@QRId", originalValue);
                    cmd.Parameters.AddWithValue("@QRUrl", originalValue);

                    conn.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            string rawPropId = sdr["PropertyId"] == DBNull.Value ? "" : sdr["PropertyId"].ToString().Trim();
                            if (rawPropId.Equals("NULL", StringComparison.OrdinalIgnoreCase)) rawPropId = "";

                            string rawActDate = sdr["ActivatedDate"] == DBNull.Value ? "" : sdr["ActivatedDate"].ToString().Trim();
                            if (rawActDate.Equals("NULL", StringComparison.OrdinalIgnoreCase)) rawActDate = "";

                            string formattedActDate = "";
                            if (!string.IsNullOrEmpty(rawActDate))
                            {
                                DateTime dt;
                                if (DateTime.TryParse(rawActDate, out dt))
                                {
                                    formattedActDate = dt.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                else
                                {
                                    formattedActDate = rawActDate;
                                }
                            }

                            string rawActBy = sdr["ActivatedBy"] == DBNull.Value ? "" : sdr["ActivatedBy"].ToString().Trim();
                            if (rawActBy.Equals("NULL", StringComparison.OrdinalIgnoreCase)) rawActBy = "";

                            var cardDetails = new Dictionary<string, object>
                            {
                                { "qrId", sdr["QRId"].ToString() },
                                { "qrToken", sdr["QRToken"].ToString() },
                                { "status", sdr["Status"].ToString().ToUpper() },
                                { "propertyId", rawPropId },
                                { "createdDate", Convert.ToDateTime(sdr["CreatedDate"]).ToString("yyyy-MM-dd HH:mm:ss") },
                                { "activatedDate", formattedActDate },
                                { "activatedBy", rawActBy },
                                { "qrUrl", sdr["QRUrl"].ToString() }
                            };

                            SendResponse(context, 200, true, "Card found successfully", cardDetails);
                        }
                        else
                        {
                            SendResponse(context, 404, false, "QR card details not found in system. Please verify if this card is registered.", null);
                        }
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

    public bool IsReusable { get { return false; } }
}
