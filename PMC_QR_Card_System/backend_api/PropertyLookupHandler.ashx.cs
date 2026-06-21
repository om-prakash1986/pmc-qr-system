using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// PropertyLookupHandler.ashx.cs
/// Looks up a citizen property by PID from All_Demand.
/// GET: /PropertyLookupHandler.ashx?pid=&lt;PID&gt;
/// </summary>
public class PropertyLookupHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS") { context.Response.StatusCode = 200; return; }
        if (context.Request.HttpMethod != "GET") { SendResponse(context, 405, false, "Only GET is allowed", null); return; }

        string pid = context.Request.QueryString["pid"];
        if (string.IsNullOrEmpty(pid)) { SendResponse(context, 400, false, "Property ID (pid) is required", null); return; }
        pid = pid.Trim();

        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT TOP 1 
                        pd.pid AS PID, 
                        own.owner_name AS Owner_Name, 
                        own.guardian_name AS Guardian_Name, 
                        own.mobile_no AS Mobile_No, 
                        pd.address AS Address,
                        pd.last_payment_amount,
                        pd.last_payment_date,
                        pd.assessment_year,
                        wm.ward_no,
                        cm.circle_name,
                        rm.rev_circle,
                        ad.PID, ad.Owner_Name, ad.Guardian_Name, ad.Mobile_No, ad.Address,
                        ad.Total_Dues, ad.Current_Dues, ad.Payment_Status, ad.Circle, ad.Ward_No,
                        pd.application_no, pd.plot_area, pd.constructed_area,
                        stm.street_type, pd.id as property_pk
                    FROM All_Demand ad
                    LEFT JOIN tbl_property_detail pd ON CAST(ad.PID AS VARCHAR(50)) = CAST(pd.pid AS VARCHAR(50)) AND pd.status IN (1,2,3,4)
                    LEFT JOIN tbl_street_type_master stm ON pd.street_type_id = stm.id
                    WHERE ad.PID = @PID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", pid);
                    conn.Open();
                    
                    var details = new Dictionary<string, object>();
                    long propertyPk = 0;

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            propertyPk = sdr["property_pk"] != DBNull.Value ? Convert.ToInt64(sdr["property_pk"]) : 0;
                            
                            details.Add("pid", sdr["PID"].ToString());
                            details.Add("ownerName", sdr["Owner_Name"].ToString());
                            details.Add("guardianName", sdr["Guardian_Name"] == DBNull.Value ? "" : sdr["Guardian_Name"].ToString());
                            details.Add("mobileNo", sdr["Mobile_No"] == DBNull.Value ? "" : sdr["Mobile_No"].ToString());
                            details.Add("address", sdr["Address"] == DBNull.Value ? "" : sdr["Address"].ToString());
                            details.Add("totalDues", sdr["Total_Dues"] == DBNull.Value ? "0" : sdr["Total_Dues"].ToString());
                            details.Add("currentAmount", sdr["Current_Dues"] == DBNull.Value ? "0" : sdr["Current_Dues"].ToString());
                            details.Add("paymentStatus", sdr["Payment_Status"] == DBNull.Value ? "Pending" : sdr["Payment_Status"].ToString());
                            details.Add("circle", sdr["Circle"] == DBNull.Value ? "" : sdr["Circle"].ToString());
                            details.Add("wardNo", sdr["Ward_No"] == DBNull.Value ? "" : sdr["Ward_No"].ToString());
                            
                            // Extended fields
                            details.Add("sasNo", sdr["application_no"] == DBNull.Value ? "" : sdr["application_no"].ToString());
                            details.Add("plotArea", sdr["plot_area"] == DBNull.Value ? "" : sdr["plot_area"].ToString());
                            details.Add("constructedArea", sdr["constructed_area"] == DBNull.Value ? "" : sdr["constructed_area"].ToString());
                            details.Add("streetType", sdr["street_type"] == DBNull.Value ? "" : sdr["street_type"].ToString());
                        }
                    }

                    if (details.Count > 0)
                    {
                        // Floor details
                        var floorDetails = new List<Dictionary<string, object>>();
                        if (propertyPk > 0)
                        {
                            string floorQuery = @"
                                SELECT 
                                    fm.floor_no, od.builtup_area, ut.use_type, 
                                    ugt.usage_type, ctm.contruction_type, otm.occupancy_type
                                FROM tbl_occupancy_detail od
                                LEFT JOIN tbl_floor_master fm ON od.floor_id = fm.id
                                LEFT JOIN tbl_use_type_master ut ON od.use_type_id = ut.id
                                LEFT JOIN tbl_usage_type_master ugt ON od.usage_type_id = ugt.id
                                LEFT JOIN tbl_construction_type_master ctm ON od.construction_type_id = ctm.id
                                LEFT JOIN tbl_occupancy_type_master otm ON od.occupancy_type_id = otm.id
                                WHERE od.property_id = @PropertyPK AND od.status = 1";

                            using (SqlCommand fCmd = new SqlCommand(floorQuery, conn))
                            {
                                fCmd.Parameters.AddWithValue("@PropertyPK", propertyPk);
                                using (SqlDataReader fDr = fCmd.ExecuteReader())
                                {
                                    while (fDr.Read())
                                    {
                                        floorDetails.Add(new Dictionary<string, object>
                                        {
                                            { "floorNo", fDr["floor_no"] == DBNull.Value ? "" : fDr["floor_no"].ToString() },
                                            { "builtupArea", fDr["builtup_area"] == DBNull.Value ? "" : fDr["builtup_area"].ToString() },
                                            { "useType", fDr["use_type"] == DBNull.Value ? "" : fDr["use_type"].ToString() },
                                            { "usageType", fDr["usage_type"] == DBNull.Value ? "" : fDr["usage_type"].ToString() },
                                            { "constructionType", fDr["contruction_type"] == DBNull.Value ? "" : fDr["contruction_type"].ToString() },
                                            { "occupancyType", fDr["occupancy_type"] == DBNull.Value ? "" : fDr["occupancy_type"].ToString() }
                                        });
                                    }
                                }
                            }
                        }
                        details.Add("floorDetails", floorDetails);

                        SendResponse(context, 200, true, "Property found", details);
                    }
                    else
                    {
                        SendResponse(context, 404, false, "Property ID not found in records.", null);
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

    public bool IsReusable { get { return false; } }
}
