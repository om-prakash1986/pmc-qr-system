using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for garbage_collection_user_account
/// </summary>
public class garbage_collection_user_account
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public DataTable find_user_details(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_user_registration where id=@id", con);
            cmd.Parameters.AddWithValue("@id", user_id);
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


    public int update_user_details(string user_id,string email_id,string mobile_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update garbage_user_registration set mobile_no=@mobile_no,email_id=@email_id WHERE id=@id", con);
            cmd1.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd1.Parameters.AddWithValue("@email_id", email_id);
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
    
    public garbage_collection_user_account()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}