using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for holding_admin_controller
/// </summary>
public class holding_admin_controller
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Adding new Administrator for the website
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int add_administrator(string first_name, string last_name, string user_name, string email_id, string contact_no, string password, string gender, string content_type, byte[] bytes)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_admin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID_Holding_Admin"].ToString());
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("h:mm:ss tt"));
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
        }
        return number;
    }
    /************************************************************
     * Purpose  ::  Adding new main Menu for the website
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_user_name(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,user_name from holding_admin where user_name=@user_name", con);
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
     * Purpose  ::  Admin Login Details
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int admin_login(string user_name,string password)
    {
        int number = 0;


        return number;
    }




    /*******************************************************
     * Till Here
     * *****************************************************/
}