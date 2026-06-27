using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ShopReport
/// </summary>
public class ShopReport
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable bindShopType()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select * from Office_Type where status=1", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable bindGrid(string Circle, string pid, string fromdate, string uptodate, string PaymentMode, string PaymentStatus, string _status)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            string query = "select sp.id, sp.pid, mb.BLOCK_NO, bf.FLOOR_NO, ot.Office_Details, sd.Shop_No, sd.Shop_Holder_Name, sp.payment_code, sp.amount, sp.receive_date, convert(VARCHAR(10), sp.since, 105) as NewSince from shops_payment sp, Shop_Details sd, BLOCK_FLOOR bf, MAURYA_BLOCK mb, Office_Type ot where sp.pid = sd.Shop_Id and bf.FLOOR_ID = sd.Block_Floor and ot.Office_id = sd.Occupany_type and mb.block_id = bf.block_id  and(sp.status = @PaymentMode or sp.status = @PaymentStatus)";

            //and((pp.since between '2019-10-13' and '2019-10-13') or(pd.CircleName = 'New Capital Circle') or(pp.pid = '2117143'))";
            string subquery = "";
            if (Circle != "0" && _status != "2" && !string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(uptodate))
            {
                subquery += " and sd.Occupany_type = @circle and (sp.since between @fromdate and @uptodate)";
            }

            if (!string.IsNullOrEmpty(fromdate) && _status != "2" && !string.IsNullOrEmpty(uptodate))
            {
                subquery += " and (sp.since between @fromdate and @uptodate)";
            }

            if (!string.IsNullOrEmpty(pid))
            {
                subquery += " and sp.pid=@pid and (sp.since between @fromdate and @uptodate)";
            }
            if (_status != "1")
            {
                subquery += " and (sp.since between @fromdate and @uptodate)";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery + " order by sp.id desc";
            }

            string newquery = query;
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@circle", Circle);
            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.Parameters.AddWithValue("@status", _status);
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