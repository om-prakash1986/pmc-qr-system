using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// TaxHistoryHandler.ashx.cs
/// Fetches the property tax history using the sp_temp_yearly_tax_test stored procedure.
/// GET: /TaxHistoryHandler.ashx?pid=<PID>
/// </summary>
public class TaxHistoryHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS") { context.Response.StatusCode = 200; return; }
        if (context.Request.HttpMethod != "GET") { SendResponse(context, 405, false, "Only GET is allowed", null); return; }

        string pidStr = context.Request.QueryString["pid"];

        if (string.IsNullOrEmpty(pidStr))
        {
            SendResponse(context, 400, false, "Search query (pid) is required", null);
            return;
        }

        long propertyId = 0;
        if (!long.TryParse(pidStr, out propertyId))
        {
            SendResponse(context, 400, false, "Invalid property ID format", null);
            return;
        }

        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                
                // Stored procedure expects primary key `id` (bigint) from tbl_property_detail, NOT the user-facing string `pid`
                long resolvedPropertyId = propertyId; // fallback
                string lookupSql = "SELECT TOP 1 id FROM tbl_property_detail WHERE pid = @PID AND status IN (1,2,3,4)";
                using (SqlCommand lookupCmd = new SqlCommand(lookupSql, conn))
                {
                    lookupCmd.Parameters.AddWithValue("@PID", pidStr);
                    object val = lookupCmd.ExecuteScalar();
                    if (val != null && val != DBNull.Value)
                    {
                        resolvedPropertyId = Convert.ToInt64(val);
                    }
                }

                using (SqlCommand cmd = new SqlCommand("sp_temp_yearly_tax_test", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@property_id", resolvedPropertyId);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        var results = new List<Dictionary<string, object>>();
                        while (sdr.Read())
                        {
                            results.Add(new Dictionary<string, object>
                            {
                                { "fin_year", sdr["fin_year"] == DBNull.Value ? "" : sdr["fin_year"].ToString() },
                                { "property_id", sdr["property_id"] == DBNull.Value ? "" : sdr["property_id"].ToString() },
                                { "floor_tax", sdr["floor_tax"] == DBNull.Value ? "0.00" : sdr["floor_tax"].ToString() },
                                { "vacant_tax", sdr["vacant_tax"] == DBNull.Value ? "0.00" : sdr["vacant_tax"].ToString() },
                                { "total_tax", sdr["total_tax"] == DBNull.Value ? "0.00" : sdr["total_tax"].ToString() }
                            });
                        }

                        if (results.Count > 0)
                        {
                            SendResponse(context, 200, true, "Tax history found", results);
                        }
                        else
                        {
                            SendResponse(context, 404, false, "No tax history found for this property ID.", new List<object>());
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
        context.Response.Write(ser.Serialize(new Dictionary<string, object>
        {
            { "success", success }, { "message", message }, { "data", data }
        }));
    }

    public bool IsReusable { get { return false; } }
}
