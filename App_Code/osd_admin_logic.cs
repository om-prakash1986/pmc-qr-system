using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Text;


public class osd_admin_logic
{
    string strcon = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
    /************************************************************
        * Purpose  ::  Adding new Admin for the website
        * Author   ::  Amrita singh
        * Date     ::  8-04-2019
        * ********************************************************/
    public int add_administrator(string first_name, string last_name, string user_name, string email_id, string contact_no, string password, string gender, string content_type, byte[] bytes)
    {
        int number = 0;

        osd_encryption en = new osd_encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_admin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["Admin_ID"].ToString());
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                number = 1;
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
            con.Close();
            con.Dispose();
        }
        return number;
    }

    /************************************************************
        * Purpose  ::  Checking availibility of the Admin username
        * Author   ::  Amrita singh
        * Date     ::  8-04-2019
        * ********************************************************/
    public int check_user_name(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from administrator where user_name=@user_name", con);
            cmd.Parameters.AddWithValue("@user_name", u_name);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
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

        return number;
    }
    /************************************************************
    * Purpose  ::  Login into admin panel authentication
    * Author   ::  Amrita singh
    * Date     ::  8-04-2019
    * ********************************************************/
    public int Admin_login_auth(string user_name, string password)
    {
        int number = 0;

        osd_encryption en = new osd_encryption();
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
                    HttpContext.Current.Session["Admin_Name"] = null;
                    HttpContext.Current.Session["Admin_ID"] = null;
                    HttpContext.Current.Session["Admin_Name"] = dt.Rows[0]["email_id"].ToString();
                    HttpContext.Current.Session["Admin_ID"] = dt.Rows[0]["id"].ToString();
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
}
