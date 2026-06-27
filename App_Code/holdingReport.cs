using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for holdingReport
/// </summary>
public class holdingReport
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    //<holdinng tax>
    public DataTable findHolding(string _fromdate, string _todate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select count(amount) from property_payment where since between @_fromdate and @_todate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_fromdate", _fromdate);
            cmd.Parameters.AddWithValue("@_todate", _todate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findHoldingAmount(string _fromdate, string _todate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select SUM(CAST(amount as decimal)) from property_payment where since between @_fromdate and @_todate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_fromdate", _fromdate);
            cmd.Parameters.AddWithValue("@_todate", _todate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findMonthlyHolding(string startDate, string lastDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select count(amount) from property_payment where since between @startDate and @lastDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@lastDate", lastDate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findMonthlyHoldingAmount(string startDate, string lastDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select SUM(CAST(amount as decimal)) from property_payment where since between @startDate and @lastDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@lastDate", lastDate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findDailyHolding(string _selDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select count(amount) from property_payment where since =@_selDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_selDate", _selDate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findDailyHoldingAmount(string _selDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select SUM(CAST(amount as decimal)) from property_payment where since=@_selDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_selDate", _selDate);
            sda.Fill(dt);
        }
        return dt;
    }
    //end//
    //<prda tax>
    public DataTable findRent(string _fromdate, string _todate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select count(amount) from shops_payment where since between @_fromdate and @_todate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_fromdate", _fromdate);
            cmd.Parameters.AddWithValue("@_todate", _todate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findRentAmount(string _fromdate, string _todate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select SUM(CAST(amount as decimal)) from shops_payment where since between @_fromdate and @_todate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_fromdate", _fromdate);
            cmd.Parameters.AddWithValue("@_todate", _todate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findMonthlyRent(string startDate, string lastDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select count(amount) from shops_payment where since between @startDate and @lastDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@lastDate", lastDate);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable findMonthlyRentAmount(string startDate, string lastDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select SUM(CAST(amount as decimal)) from shops_payment where since between @startDate and @lastDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@lastDate", lastDate);
            sda.Fill(dt);
        }
        return dt;
    } 
    public DataTable findDailyRent(string _selDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select count(amount) from shops_payment where since =@_selDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_selDate", _selDate);
            sda.Fill(dt);
        }
        return dt;
    } 
    public DataTable findDailyRentAmount(string _selDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select SUM(CAST(amount as decimal)) from shops_payment where since=@_selDate and (status='active' or status='paid')", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@_selDate", _selDate);
            sda.Fill(dt);
        }
        return dt;
    }
    //end//
    public DataTable bindCircle()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select * from Circle", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }
    public DataTable bindGrid(string Circle, string pid, string fromdate, string uptodate, string PaymentMode, string PaymentStatus)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            string query = "select pp.pid, pp.payment_code, pd.CircleName, pp.amount, pp.status, pp.receive_date, pp.payment_status_description, convert(VARCHAR(10), pp.since, 103) as NewSince from property_payment pp,property_details pd where pp.pid = pd.PID and(pp.status = @PaymentMode or pp.status = @PaymentStatus)";

            //and((pp.since between '2019-10-13' and '2019-10-13') or(pd.CircleName = 'New Capital Circle') or(pp.pid = '2117143'))";
            string subquery = "";
            if (!string.IsNullOrEmpty(Circle) && !string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(uptodate))
            {
                subquery += " and ((pd.CircleName=@circle) and (pp.since between @fromdate and @uptodate))";
            }

            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(uptodate))
            {
                subquery += " and (pp.since between @fromdate and @uptodate)";
            }

            if (!string.IsNullOrEmpty(pid))
            {
                subquery += " and pp.pid=@pid";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery + " order by pp.id desc";
            }
            
                string newquery = query;
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@circle", Circle);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@PaymentMode", PaymentMode);
                cmd.Parameters.AddWithValue("@PaymentStatus", PaymentStatus);
                cmd.Parameters.AddWithValue("@fromdate", Convert.ToDateTime(fromdate));
                cmd.Parameters.AddWithValue("@uptodate", Convert.ToDateTime(uptodate));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

            
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