using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// PropertyOutstandingHandler.ashx.cs
/// Returns property details along with dynamic municipal dues calculated via PMC.demandbysms.
/// GET: /PropertyOutstandingHandler.ashx?token=<QR_TOKEN>
/// </summary>
public class PropertyOutstandingHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request.HttpMethod == "OPTIONS") { context.Response.StatusCode = 200; return; }
        if (context.Request.HttpMethod != "GET") { SendResponse(context, 405, false, "Only GET is allowed", null); return; }

        string token = context.Request.QueryString["token"];
        if (string.IsNullOrEmpty(token)) { SendResponse(context, 400, false, "Token is required", null); return; }
        token = token.Trim();

        // Resolve token to property ID using existing lookup logic (reuse PropertyLookupHandler query)
        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // First, get the QR record to fetch linked PropertyId from QRMaster
                string qrQuery = @"SELECT PropertyId FROM QRMaster WHERE (QRToken = @Token OR QRId = @Token OR QRUrl = @Token) AND Status = 'ACTIVATED'";
                using (SqlCommand qrCmd = new SqlCommand(qrQuery, conn))
                {
                    qrCmd.Parameters.AddWithValue("@Token", token);
                    conn.Open();
                    object pidObj = qrCmd.ExecuteScalar();
                    if (pidObj == null || pidObj == DBNull.Value)
                    {
                        SendResponse(context, 404, false, "QR token not found or not activated", null);
                        return;
                    }
                    string propertyId = pidObj.ToString();

                    // Reuse the property lookup query from PropertyLookupHandler
                    string propQuery = @"
                        SELECT TOP 1 
                            pd.PID,
                            own.owner_name AS Owner_Name,
                            own.guardian_name AS Guardian_Name,
                            own.mobile_no AS Mobile_No,
                            pd.address,
                            pd.application_no,
                            pd.plot_area,
                            pd.constructed_area,
                            stm.street_type,
                            wm.ward_no AS Ward_No,
                            cm.circle_name AS Circle,
                            pd.id as property_pk 
                        FROM tbl_property_detail pd 
                        LEFT JOIN tbl_owner_detail own ON pd.id = own.property_id 
                        LEFT JOIN tbl_street_type_master stm ON pd.street_type_id = stm.id 
                        LEFT JOIN tbl_ward_master wm ON pd.ward_id = wm.id
                        LEFT JOIN tbl_circle_master cm ON wm.circle_id = cm.id
                        WHERE pd.PID = @PID AND pd.status IN (1,2,3,4)";
                    var details = new Dictionary<string, object>();
                    long propertyPk = 0;

                    using (SqlCommand propCmd = new SqlCommand(propQuery, conn))
                    {
                        propCmd.Parameters.AddWithValue("@PID", propertyId);
                        using (SqlDataReader sdr = propCmd.ExecuteReader())
                        {
                            if (sdr.Read())
                            {
                                propertyPk = sdr["property_pk"] != DBNull.Value ? Convert.ToInt64(sdr["property_pk"]) : 0;
                                details["pid"] = sdr["PID"].ToString();
                                details["ownerName"] = sdr["Owner_Name"].ToString();
                                details["guardianName"] = sdr["Guardian_Name"] == DBNull.Value ? "" : sdr["Guardian_Name"].ToString();
                                details["mobileNo"] = sdr["Mobile_No"] == DBNull.Value ? "" : sdr["Mobile_No"].ToString();
                                details["address"] = sdr["Address"] == DBNull.Value ? "" : sdr["Address"].ToString();
                                details["applicationNo"] = sdr["application_no"] == DBNull.Value ? "" : sdr["application_no"].ToString();
                                details["plotArea"] = sdr["plot_area"] == DBNull.Value ? "" : sdr["plot_area"].ToString();
                                details["constructedArea"] = sdr["constructed_area"] == DBNull.Value ? "" : sdr["constructed_area"].ToString();
                                details["streetType"] = sdr["street_type"] == DBNull.Value ? "" : sdr["street_type"].ToString();
                                details["circle"] = sdr["Circle"] == DBNull.Value ? "" : sdr["Circle"].ToString();
                                details["wardNo"] = sdr["Ward_No"] == DBNull.Value ? "" : sdr["Ward_No"].ToString();
                            }
                        }
                    }

                    if (details.Count > 0)
                    {
                        // Calculate dynamic dues using existing business logic
                        decimal dynamicDues = 0;
                        try
                        {
                            PMC.demandbysms dbs = new PMC.demandbysms();
                            dynamicDues = dbs.getDemandForMutation(propertyId);
                        }
                        catch (Exception ex)
                        {
                            // Log but continue with zero dues
                            System.Diagnostics.Debug.WriteLine("[PropertyOutstandingHandler] Dues calculation error: " + ex);
                        }
                        details["totalDues"] = Math.Round(dynamicDues, 2);
                        details["paymentStatus"] = (dynamicDues > 0) ? "Pending" : "Paid";

                        // Floor details (reuse same logic as PropertyLookupHandler for completeness)
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
                                            {"floorNo", fDr["floor_no"] == DBNull.Value ? "" : fDr["floor_no"].ToString()},
                                            {"builtupArea", fDr["builtup_area"] == DBNull.Value ? "" : fDr["builtup_area"].ToString()},
                                            {"useType", fDr["use_type"] == DBNull.Value ? "" : fDr["use_type"].ToString()},
                                            {"usageType", fDr["usage_type"] == DBNull.Value ? "" : fDr["usage_type"].ToString()},
                                            {"constructionType", fDr["contruction_type"] == DBNull.Value ? "" : fDr["contruction_type"].ToString()},
                                            {"occupancyType", fDr["occupancy_type"] == DBNull.Value ? "" : fDr["occupancy_type"].ToString()}
                                        });
                                    }
                                }
                            }
                        }
                        details["floorDetails"] = floorDetails;

                        SendResponse(context, 200, true, "Property data retrieved", details);
                    }
                    else
                    {
                        SendResponse(context, 404, false, "Property ID not found", null);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SendResponse(context, 500, false, "Server error: " + ex.Message, null);
        }
    }

    private void SendResponse(HttpContext context, int statusCode, bool success, string message, object data)
    {
        context.Response.StatusCode = statusCode;
        var ser = new JavaScriptSerializer();
        context.Response.Write(ser.Serialize(new Dictionary<string, object>
        {
            {"success", success},
            {"message", message},
            {"data", data}
        }));
    }

    public bool IsReusable { get { return false; } }
}
