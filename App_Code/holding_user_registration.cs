using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for holding_user_registration
/// </summary>
public class holding_user_registration
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    // insert new user
    public int insert_new_user(string f_name, string m_name, string l_name, string email_id, string contact_no, string aadhar_no, string password)
    {
        int number = 0;
        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Holding_insert_user"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@first_name", f_name);
            cmd.Parameters.AddWithValue("@middle_name", m_name);
            cmd.Parameters.AddWithValue("@last_name", l_name);
            //cmd.Parameters.AddWithValue("@user_name", u_name);
            //cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@aadhar_no", aadhar_no);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            //cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("h:mm:ss tt"));
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

    // check user name for user
    public int check_emailid(string email_id)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from holding_user where email_id=@emailid order by id desc", con);
            cmd.Parameters.AddWithValue("@emailid", email_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = 1;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return number;
    }

    public int check_contactNo(string contact_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from holding_user where contact_no=@contactno order by id desc", con);
            cmd.Parameters.AddWithValue("@contactno", contact_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = 1;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return number;
    }

    public int check_aadharno(string aadharno)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from holding_user where aadhar_no=@aadharno order by id desc", con);
            cmd.Parameters.AddWithValue("@aadharno", aadharno);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = 1;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return number;
    }

    // Login for user

    public int holding_user_login(string user_name, string password)
    {
        int number = 0;
        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_user_login"))
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
                    HttpContext.Current.Session["holding_U_Name"] = null;
                    HttpContext.Current.Session["holding_U_ID"] = null;
                    HttpContext.Current.Session["holding_U_Name"] = dt.Rows[0]["user_name"].ToString();
                    HttpContext.Current.Session["holding_U_ID"] = dt.Rows[0]["id"].ToString();
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

    public int holding_admin_login(string user_name, string password)
    {
        int number = 0;
        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_admin_login"))
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
                    HttpContext.Current.Session["holding_U_Name_admin"] = null;
                    HttpContext.Current.Session["holding_U_ID_admin"] = null;
                    HttpContext.Current.Session["holding_U_ID_admin_type"] = null;
                    HttpContext.Current.Session["holding_U_Name_admin"] = dt.Rows[0]["user_name"].ToString();
                    HttpContext.Current.Session["holding_U_ID_admin"] = dt.Rows[0]["id"].ToString();
                    HttpContext.Current.Session["holding_U_ID_admin_type"] = dt.Rows[0]["user_type"].ToString();
                    HttpContext.Current.Session["holding_U_Name_Circle"] = dt.Rows[0]["circle"].ToString();
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

    public holding_user_registration()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}