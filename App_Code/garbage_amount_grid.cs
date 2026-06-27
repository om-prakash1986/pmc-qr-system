using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for garbage_amount_grid
/// </summary>
public class garbage_amount_grid
{

    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Find All Garbage Collection Payment Details
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable find_all_payments_by_user(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            //SqlCommand cmd = new SqlCommand("select * from garbage_collection_user_details where gcc_user_id=@user_id and status='active'", con);
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](circle) as circle_name,[dbo].[find_amount_paid](transacton_no) as amount_paid_payment from garbage_collection_user_details where gcc_user_id=@user_id", con);
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

    public string find_usage_type(string pid)
    {
        string type = "0";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            //SqlCommand cmd = new SqlCommand("select * from garbage_collection_user_details where gcc_user_id=@user_id and status='active'", con);
            SqlCommand cmd = new SqlCommand("SELECT top 1 * from garbage_collection_user_details where gcc_id=@pid order by id desc", con);
            cmd.Parameters.AddWithValue("@pid", pid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                type = dt.Rows[0]["property_type"].ToString();
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
        return type;
    }

    /******************************************************************
     * Till Here
     * ***************************************************************/
}