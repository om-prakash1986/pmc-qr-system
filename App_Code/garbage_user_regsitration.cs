using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for garbage_user_regsitration
/// </summary>
public class garbage_user_regsitration
{
    encryption en = new encryption();
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public int insert_new_user(string contact_no,string email_id)
    {
        //string pass = en.GetMD5(password);
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_collection_user"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            //cmd.Parameters.AddWithValue("@password", pass);
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
        }
        return number;
    }

    public int find_if_mobile_registered(string mobile_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_user_registration where mobile_no=@mobile_no", con);
            cmd.Parameters.AddWithValue("@mobile_no",mobile_no);
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

    public DataTable garbage_login_user(string user_name)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_user_registration where mobile_no=@mobile_no", con);
            cmd.Parameters.AddWithValue("@mobile_no", user_name);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            da.Fill(dt);

            //if (dt.Rows.Count > 0)
            //{
            //    HttpContext.Current.Session["gar_User_Name"] = null;
            //    HttpContext.Current.Session["gar_User_ID"] = null;
            //    HttpContext.Current.Session["gar_User_email"] = null;
            //    HttpContext.Current.Session["gar_User_Name"] = dt.Rows[0]["mobile_no"].ToString();
            //    HttpContext.Current.Session["gar_User_ID"] = dt.Rows[0]["id"].ToString();
            //    HttpContext.Current.Session["gar_User_email"] = dt.Rows[0]["email_id"].ToString();
            //    number = 1;
            //}
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
        return dt;
    }

    public int is_property_type_available(string property_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_collection_house_types where property_type=@property_type", con);
            cmd.Parameters.AddWithValue("@property_type", property_type);
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

    public int insert_property_type(string property_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_property_type"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID_admin"].ToString());
            cmd.Parameters.AddWithValue("@property_type", property_type);
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
        }
        return number;
    }



	// till here
}