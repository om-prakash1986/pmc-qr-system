using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for sending_sms_to_users
/// </summary>
public class sending_sms_to_users
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Find Circle ID
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable find_all_users()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select distinct(PID) as id,mobileno from property_details where is_sent is null", con);
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

    public int update_sent_sms(string pid,string mobile_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        try
        {
            SqlCommand cmd1 = new SqlCommand("update  property_details set is_sent='yes' WHERE pid=@id and mobileno=@mobile_no", con);
            cmd1.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd1.Parameters.AddWithValue("@id", pid);
            cmd1.ExecuteNonQuery();
            con.Close();
            number = 1;
        }
        catch(Exception ex)
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
}