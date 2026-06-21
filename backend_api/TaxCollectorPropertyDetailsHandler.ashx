<%@ WebHandler Language="C#" Class="TaxCollectorPropertyDetailsHandler" %>
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

public class TaxCollectorPropertyDetailsHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS") { context.Response.StatusCode = 200; return; }
        if (context.Request.HttpMethod != "GET") { SendResponse(context, 405, false, "Only GET is allowed", null); return; }

        string search = context.Request.QueryString["search"];

        if (string.IsNullOrEmpty(search))
        {
            SendResponse(context, 400, false, "Search query (PID or SAS) is required", null);
            return;
        }

        long parsedPid = 0;
        bool isNumeric = long.TryParse(search, out parsedPid);

        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT
                        pd.pid,
                        pd.application_no,
                        own.owner_name,
                        own.guardian_name,
                        pd.address as property_address,
                        pd.plot_area,
                        pd.constructed_area,
                        pd.assessment_year,
                        stm.street_type,
                        fm.floor_no,
                        od.builtup_area,
                        ut.use_type,
                        ugt.usage_type,
                        ctm.contruction_type,
                        otm.occupancy_type
                    FROM tbl_property_detail pd
                    INNER JOIN tbl_occupancy_detail od ON pd.id = od.property_id AND od.status = 1
                    LEFT JOIN tbl_floor_master fm ON od.floor_id = fm.id
                    LEFT JOIN tbl_use_type_master ut ON od.use_type_id = ut.id
                    LEFT JOIN tbl_usage_type_master ugt ON od.usage_type_id = ugt.id
                    LEFT JOIN tbl_construction_type_master ctm ON od.construction_type_id = ctm.id
                    LEFT JOIN tbl_occupancy_type_master otm ON od.occupancy_type_id = otm.id
                    LEFT JOIN tbl_street_type_master stm ON pd.street_type_id = stm.id
                    LEFT JOIN tbl_owner_detail own ON pd.id = own.property_id
                    WHERE pd.status IN (1,2,3,4)
                    AND (
                           (@isNumeric = 1 AND pd.pid = @pid)
                        OR (pd.application_no = @sas)
                    )";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@isNumeric", isNumeric ? 1 : 0);
                    cmd.Parameters.AddWithValue("@pid", isNumeric ? (object)parsedPid : DBNull.Value);
                    cmd.Parameters.AddWithValue("@sas", search);

                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        var results = new List<Dictionary<string, object>>();
                        while (sdr.Read())
                        {
                            results.Add(new Dictionary<string, object>
                            {
                                { "pid", sdr["pid"] == DBNull.Value ? "" : sdr["pid"].ToString() },
                                { "application_no", sdr["application_no"] == DBNull.Value ? "" : sdr["application_no"].ToString() },
                                { "owner_name", sdr["owner_name"] == DBNull.Value ? "" : sdr["owner_name"].ToString() },
                                { "guardian_name", sdr["guardian_name"] == DBNull.Value ? "" : sdr["guardian_name"].ToString() },
                                { "address", sdr["property_address"] == DBNull.Value ? "" : sdr["property_address"].ToString() },
                                { "plot_area", sdr["plot_area"] == DBNull.Value ? "" : sdr["plot_area"].ToString() },
                                { "constructed_area", sdr["constructed_area"] == DBNull.Value ? "" : sdr["constructed_area"].ToString() },
                                { "assessment_year", sdr["assessment_year"] == DBNull.Value ? "" : sdr["assessment_year"].ToString() },
                                { "street_type", sdr["street_type"] == DBNull.Value ? "" : sdr["street_type"].ToString() },
                                { "floor_no", sdr["floor_no"] == DBNull.Value ? "" : sdr["floor_no"].ToString() },
                                { "builtup_area", sdr["builtup_area"] == DBNull.Value ? "" : sdr["builtup_area"].ToString() },
                                { "use_type", sdr["use_type"] == DBNull.Value ? "" : sdr["use_type"].ToString() },
                                { "usage_type", sdr["usage_type"] == DBNull.Value ? "" : sdr["usage_type"].ToString() },
                                { "contruction_type", sdr["contruction_type"] == DBNull.Value ? "" : sdr["contruction_type"].ToString() },
                                { "occupancy_type", sdr["occupancy_type"] == DBNull.Value ? "" : sdr["occupancy_type"].ToString() }
                            });
                        }

                        if (results.Count > 0)
                        {
                            SendResponse(context, 200, true, "Details found", results);
                        }
                        else
                        {
                            SendResponse(context, 404, false, "No detailed records found for this Search query.", new List<object>());
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
