using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// All Login  logics added here
/// </summary>
public class login_logic
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Login into admin panel authentication
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int Admin_login_auth(string user_name, string password)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("admin_login"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Connection = con;
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["User_Name"] = null;
                    HttpContext.Current.Session["User_ID"] = null;
                    HttpContext.Current.Session["User_Name"] = dt.Rows[0]["email_id"].ToString();
                    HttpContext.Current.Session["User_ID"] = dt.Rows[0]["id"].ToString();
                    number = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        return number;
    }
    /***********************************************
     * Logic ends here
     * **********************************************/
}