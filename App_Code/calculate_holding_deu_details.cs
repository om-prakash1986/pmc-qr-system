using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for calculate_holding_deu_details
/// </summary>
public class calculate_holding_deu_details
{
	string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  All Wards Wise Report
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable find_vaccant_land(string PID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from property_details where id=@pid", con);
            cmd.Parameters.AddWithValue("@pid",PID);
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
    /*********************************************************
     * Till Here
     * ********************************************************/
}