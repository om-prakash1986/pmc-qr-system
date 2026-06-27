using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for mobile_logins
/// </summary>
public class mobile_logins
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public int insert_mobile_login(string mobile_no,string otp)
    {
        string time = DateTime.Now.ToString("h:mm:ss tt");
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_mobile_login"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@contact_no", mobile_no);
            cmd.Parameters.AddWithValue("@otp", otp);
            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                HttpContext.Current.Session["con_no"] = null;
                HttpContext.Current.Session["con_no"] = mobile_no.ToString();
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

    // show all contact us
    public DataTable show_all_logins()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from mobile_logins order by id desc", con);
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

    // show unnique data

    public DataTable show_unique_data(string mobile_no, string otp)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            string query = "SELECT top 1 id,mobile_no,otp FROM mobile_logins where mobile_no=@mobile_no and otp=@otp ORDER BY id desc";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@otp", otp);
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

}