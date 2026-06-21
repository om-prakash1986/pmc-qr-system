using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class ProcessPaymentHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }
        if (context.Request.HttpMethod != "POST")
        {
            SendResponse(context, 405, false, "Only POST is allowed", null);
            return;
        }

        try
        {
            string body;
            using (var rdr = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                body = rdr.ReadToEnd();
            }

            var serializer = new JavaScriptSerializer();
            var payload = serializer.Deserialize<Dictionary<string, object>>(body);

            if (payload == null || !payload.ContainsKey("pid") || !payload.ContainsKey("paymentId") || !payload.ContainsKey("amount"))
            {
                SendResponse(context, 400, false, "pid, paymentId, and amount parameters are required", null);
                return;
            }

            string pid = payload["pid"].ToString().Trim();
            string paymentId = payload["paymentId"].ToString().Trim();
            decimal amount = Convert.ToDecimal(payload["amount"]);
            string ownerName = payload.ContainsKey("ownerName") ? payload["ownerName"].ToString().Trim() : "";
            string mobileNo = payload.ContainsKey("mobileNo") ? payload["mobileNo"].ToString().Trim() : "";
            string paymentMode = payload.ContainsKey("paymentMode") ? payload["paymentMode"].ToString().Trim() : "ONLINE";
            string ipAddress = context.Request.UserHostAddress;

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert into tbl_QR_Citizen_Transaction
                        string insertSql = @"
                            INSERT INTO tbl_QR_Citizen_Transaction 
                            (PID, OwnerName, MobileNo, PaymentGatewayId, AmountPaid, PaymentMode, PaymentDate, Status, IPAddress, Remarks)
                            VALUES
                            (@PID, @OwnerName, @MobileNo, @PaymentId, @Amount, @PaymentMode, GETDATE(), 'SUCCESS', @IP, 'Citizen Portal Online Payment')";

                        using (SqlCommand cmd = new SqlCommand(insertSql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            cmd.Parameters.AddWithValue("@OwnerName", ownerName);
                            cmd.Parameters.AddWithValue("@MobileNo", mobileNo);
                            cmd.Parameters.AddWithValue("@PaymentId", paymentId);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);
                            cmd.Parameters.AddWithValue("@IP", ipAddress);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Mark older assessment years as paid
                        string getPropertyIdSql = "SELECT id FROM tbl_property_detail WHERE pid = @PID";
                        string internalPropId = null;
                        using (SqlCommand cmd = new SqlCommand(getPropertyIdSql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            object res = cmd.ExecuteScalar();
                            if (res != null && res != DBNull.Value)
                            {
                                internalPropId = res.ToString();
                            }
                        }

                        if (!string.IsNullOrEmpty(internalPropId))
                        {
                            string updateYearlySql = @"
                                UPDATE tbl_yearly_tax_assessment 
                                SET paid_status = 1, balance_amount = 0 
                                WHERE property_id = @InternalId AND paid_status = 0";
                            using (SqlCommand cmd = new SqlCommand(updateYearlySql, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@InternalId", internalPropId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. Update All_Demand
                        string updateDemandSql = @"
                            UPDATE All_Demand 
                            SET Payment_Status = 'Paid', Total_Dues = 0 
                            WHERE PID = @PID";
                        using (SqlCommand cmd = new SqlCommand(updateDemandSql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@PID", pid);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        SendResponse(context, 200, true, "Payment processed and database updated successfully", new { transactionId = paymentId });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SendResponse(context, 500, false, "Database transaction failed: " + ex.Message, null);
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
            { "success", success },
            { "message", message },
            { "data", data }
        }));
    }

    public bool IsReusable { get { return false; } }
}
