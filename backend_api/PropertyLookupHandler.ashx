<%@ WebHandler Language="C#" Class="PropertyLookupHandler" %>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class PropertyLookupHandler : IHttpHandler
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

        string pid = context.Request.QueryString["pid"];
        if (string.IsNullOrEmpty(pid))
        {
            SendResponse(context, 400, false, "Property ID (pid) query parameter is required", null);
            return;
        }

        pid = pid.Trim();

        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // Fetch property info from All_Demand
                string query = @"
                    SELECT TOP 1 
                        PID, Owner_Name, Guardian_Name, Mobile_No, Address, Total_Dues, Payment_Status, Circle, Ward_No
                    FROM All_Demand 
                    WHERE PID = @PID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", pid);

                    conn.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            var propertyDetails = new Dictionary<string, object>
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

                            SendResponse(context, 200, true, "Property found successfully", propertyDetails);
                        }
                        else
                        {
                            SendResponse(context, 404, false, "Property ID (PID) not found in system records. Please verify the PID.", null);
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
