using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for admin_holding_reports
/// </summary>
public class admin_holding_reports
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  All Pending Property For All user Types
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/

    public DataTable all_pending_property_details_admin(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from holding_user WHERE (status ='pending' or status is null) ORDER by since DESC", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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

    /************************************************************
     * Purpose  ::  All Pending Property For All user Types
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_pending_property_details_admin_pending(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from property_details WHERE (form_status !='completed' or form_status is null) and admin_id is not null ORDER by since DESC", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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

    /************************************************************
     * Purpose  ::  All Pending Property For All user Types
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_pending_property_details_circle_pending(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from property_details WHERE (form_status !='completed' or form_status is null) and admin_id is not null ORDER by since DESC", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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

    /************************************************************
 * Purpose  ::  All Pending Property For All user Types
 * Author   ::  Arnav
 * Date     ::  24-03-2017
 * ********************************************************/
    public DataTable all_pending_property_details_user_pending(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from property_details WHERE (form_status !='completed' or form_status is null) and added_by is not null ORDER by since DESC", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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

    /********************************************************88
     * Till Here
     * *******************************************************/
}