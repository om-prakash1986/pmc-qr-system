using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for holding_payment_slip_details
/// </summary>
public class holding_payment_slip_details
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public DataTable find_payment_details(string payment_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM property_payment WHERE payment_code=@application_no", con);
            cmd.Parameters.AddWithValue("@application_no", payment_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            //con.Close();
            // con.Dispose();
        }
        return dt;
    }
}