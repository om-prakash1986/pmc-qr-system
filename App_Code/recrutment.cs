using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for recrutment
/// </summary>
public class recrutment
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
	public recrutment()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public DataTable show_job_details(string job_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from recruitment where id=@id", con);
            cmd.Parameters.AddWithValue("@id", job_id);
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