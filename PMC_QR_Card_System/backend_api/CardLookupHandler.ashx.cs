using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// CardLookupHandler.ashx.cs
/// Looks up a QR card by token, QRId, or URL from QRMaster.
/// GET: /CardLookupHandler.ashx?value=&lt;token_or_url&gt;
/// </summary>
public class CardLookupHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS") { context.Response.StatusCode = 200; return; }
        if (context.Request.HttpMethod != "GET") { SendResponse(context, 405, false, "Only GET is allowed", null); return; }

        // Accept value / token / qrid query params
        string scannedValue = context.Request.QueryString["value"]
                           ?? context.Request.QueryString["token"]
                           ?? context.Request.QueryString["qrid"];

        if (string.IsNullOrEmpty(scannedValue)) { SendResponse(context, 400, false, "value, token, or qrid parameter is required", null); return; }

        string originalValue = scannedValue.Trim();
        string cleanToken    = ExtractToken(originalValue);

        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT QRId, QRToken, Status, PropertyId, CreatedDate, ActivatedDate, ActivatedBy, QRUrl
                                 FROM QRMaster
                                 WHERE QRToken = @Token OR QRId = @QRId OR QRUrl = @QRUrl";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", cleanToken);
                    cmd.Parameters.AddWithValue("@QRId",  originalValue);
                    cmd.Parameters.AddWithValue("@QRUrl", originalValue);
                    conn.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            string rawPropId  = sdr["PropertyId"]    == DBNull.Value ? "" : sdr["PropertyId"].ToString().Trim();
                            string rawActDate = sdr["ActivatedDate"] == DBNull.Value ? "" : sdr["ActivatedDate"].ToString().Trim();
                            string rawActBy   = sdr["ActivatedBy"]   == DBNull.Value ? "" : sdr["ActivatedBy"].ToString().Trim();

                            if (rawPropId.Equals("NULL",  StringComparison.OrdinalIgnoreCase)) rawPropId  = "";
                            if (rawActDate.Equals("NULL", StringComparison.OrdinalIgnoreCase)) rawActDate = "";
                            if (rawActBy.Equals("NULL",   StringComparison.OrdinalIgnoreCase)) rawActBy   = "";

                            string formattedActDate = "";
                            if (!string.IsNullOrEmpty(rawActDate))
                            {
                                DateTime dt;
                                formattedActDate = DateTime.TryParse(rawActDate, out dt)
                                    ? dt.ToString("yyyy-MM-dd HH:mm:ss") : rawActDate;
                            }

                            var cardDetails = new Dictionary<string, object>
                            {
                                { "qrId",          sdr["QRId"].ToString()                                                },
                                { "qrToken",       sdr["QRToken"].ToString()                                             },
                                { "status",        sdr["Status"].ToString().ToUpper()                                    },
                                { "propertyId",    rawPropId                                                             },
                                { "createdDate",   Convert.ToDateTime(sdr["CreatedDate"]).ToString("yyyy-MM-dd HH:mm:ss") },
                                { "activatedDate", formattedActDate                                                      },
                                { "activatedBy",   rawActBy                                                              },
                                { "qrUrl",         sdr["QRUrl"].ToString()                                              },
                            };
                            SendResponse(context, 200, true, "Card found", cardDetails);
                        }
                        else
                        {
                            SendResponse(context, 404, false, "QR card not found. Please verify the card is registered.", null);
                        }
                    }
                }
            }
        }
        catch (Exception ex) { SendResponse(context, 500, false, "Database error: " + ex.Message, null); }
    }

    private string ExtractToken(string value)
    {
        if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                Uri uri = new Uri(value);
                string seg = uri.Segments[uri.Segments.Length - 1].Trim('/');
                if (!string.IsNullOrEmpty(seg)) return seg;
            }
            catch { }
        }
        return value;
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

    public bool IsReusable { get { return false; } }
}
