using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for forgot_password_holding
/// </summary>
public class forgot_password_holding
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public DataTable check_user_available(string user_name)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT top 1 * from holding_user where (user_name=@id or contact_no=@id or email_id=@id)", con);
            cmd.Parameters.AddWithValue("@id", user_name);
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

    public int update_details(string user_id,string password)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update holding_user set password=@password WHERE id=@id", con);
            cmd1.Parameters.AddWithValue("@password", password);
            cmd1.Parameters.AddWithValue("@id", user_id);
            cmd1.ExecuteNonQuery();
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
        return number;
    }
}