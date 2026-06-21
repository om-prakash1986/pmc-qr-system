using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// LoginHandler.ashx.cs
/// Authenticates PMC staff users.
/// POST: { "username": "loginId", "password": "sha256hex" }
/// Returns user info on success.
/// </summary>
public class LoginHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS") { context.Response.StatusCode = 200; return; }
        if (context.Request.HttpMethod != "POST") { SendResponse(context, 405, false, "Only POST method is allowed", null); return; }

        try
        {
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                requestBody = reader.ReadToEnd();

            if (string.IsNullOrEmpty(requestBody)) { SendResponse(context, 400, false, "Request body is empty", null); return; }

            var serializer  = new JavaScriptSerializer();
            var credentials = serializer.Deserialize<Dictionary<string, object>>(requestBody);

            if (credentials == null || !credentials.ContainsKey("username") || !credentials.ContainsKey("password"))
            { SendResponse(context, 400, false, "Username and password are required", null); return; }

            string username     = GetString(credentials, "username").Trim();
            string passwordPlain = GetString(credentials, "password"); // Plain text sent by client

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT StaffId, LoginId, FullName, Role, Ward_No, Circle, Mobile, IsActive
                                 FROM tbl_QR_StaffUsers
                                 WHERE LoginId = @LoginId AND PasswordHash = @PasswordHash";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LoginId",      username);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordPlain);
                    conn.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            bool isActive = Convert.ToBoolean(sdr["IsActive"]);
                            if (!isActive) { SendResponse(context, 401, false, "Account is inactive", null); return; }

                            var userData = new Dictionary<string, object>
                            {
                                { "staffId",  sdr["StaffId"].ToString()  },
                                { "loginId",  sdr["LoginId"].ToString()  },
                                { "fullName", sdr["FullName"].ToString() },
                                { "role",     sdr["Role"].ToString()     },
                                { "wardNo",   sdr["Ward_No"] == DBNull.Value ? (object)null : Convert.ToInt32(sdr["Ward_No"]) },
                                { "circle",   sdr["Circle"]  == DBNull.Value ? "" : sdr["Circle"].ToString()  },
                                { "mobile",   sdr["Mobile"]  == DBNull.Value ? "" : sdr["Mobile"].ToString()  },
                            };

                            sdr.Close();
                            using (SqlCommand upd = new SqlCommand("UPDATE tbl_QR_StaffUsers SET Last_Login=GETDATE(), Login_Failures=0 WHERE LoginId=@LoginId", conn))
                            {
                                upd.Parameters.AddWithValue("@LoginId", username);
                                upd.ExecuteNonQuery();
                            }
                            SendResponse(context, 200, true, "Login successful", userData);
                        }
                        else
                        {
                            sdr.Close();
                            using (SqlCommand failCmd = new SqlCommand("UPDATE tbl_QR_StaffUsers SET Login_Failures=Login_Failures+1 WHERE LoginId=@LoginId", conn))
                            {
                                failCmd.Parameters.AddWithValue("@LoginId", username);
                                failCmd.ExecuteNonQuery();
                            }
                            SendResponse(context, 401, false, "Invalid username or password", null);
                        }
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
