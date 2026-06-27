using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for payment_processing_details
/// </summary>
public class payment_processing_details
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  All Payments
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_payments_for_property(string from_date,string to_date)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("all_payment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", from_date);
            cmd.Parameters.AddWithValue("@toDate", to_date);
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
     * Purpose  ::  Successful Payments
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_successful_payments_for_property(string from_date, string to_date)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("successfull_payment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", from_date);
            cmd.Parameters.AddWithValue("@toDate", to_date);
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
     * Purpose  ::  UnSuccessful Payments
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_unsuccessful_payments_for_property(string from_date, string to_date)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            //SqlCommand cmd = new SqlCommand("SELECT a.PID,a.CircleName,a.WardNo,a.OldHoldingNo,a.OldPID,a.NewHoldingNo,a.Address,a.LastAssessmentYear,b.PID,b.OwnerName,c.pid,c.payment_code,c.assessment_year,c.amount,c.status,c.payment_status,c.since,c.since,c.tran_id,c.authz_code,c.payment_status_description FROM property_details a, owner_details b,property_payment c WHERE c.pid = a.PID and a.PID = b.PID and a.PID is not null and b.PID is not null and a.PID !='NULL' and b.PID !='NULL' and c.since between @from_date and @to_date and c.status!='active'", con);
		SqlCommand cmd = new SqlCommand("failed_payment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", from_date);
            cmd.Parameters.AddWithValue("@toDate", to_date);
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