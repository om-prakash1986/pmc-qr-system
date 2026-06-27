using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

using System.Net;
using System.IO;

using System.Web.Script.Serialization;
using System.StubHelpers;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


/// <summary>
/// Summary description for sendconformationtosparrow
/// </summary>
public class sendconformationtosparrow
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    // find property_no
    public string find_pdf_from_application_no(string application_no)
    {
        string pid = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT pid from property_payment where payment_code=@application_no", con);
            cmd.Parameters.AddWithValue("@application_no", application_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    pid= dt.Rows[i]["pid"].ToString();
                    i++;
                }
            }
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
        return pid;
    }

	public string find_Id(string application_no)
    {
        string pid = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT pid from shops_payment where payment_code=@application_no", con);
            cmd.Parameters.AddWithValue("@application_no", application_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    pid = dt.Rows[i]["pid"].ToString();
                    i++;
                }
            }
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
        return pid;
    }

    // create new data to send to Sparrow
	public void sendconformationtosparrow_data(string property_id,string amount,string date_time)
	{
        my_class_new mcc = new my_class_new();
        string pid = property_id;
        Guid obj = Guid.NewGuid();
        mcc.property_no = pid;
        mcc.pmc_ref_no = obj.ToString();
        mcc.ulb_name = "Patna Municipal Corporation";
        mcc.paid_amount = amount;
        mcc.entry_datetime = date_time;

        var to_send = JsonConvert.SerializeObject(mcc);

        Crypto cry = new Crypto();
        var enc_request = cry.Encrypt(to_send);

        var request = (HttpWebRequest)WebRequest.Create("https://patnamunicipal.net/pmc/public/api/due_detail.html");
        // sarfraz changes

        var post_data = "clientid=PMCGOV249834";
        post_data += "&command=due_detail";
        post_data += "&enc_request=" + enc_request + "";

        var data = Encoding.ASCII.GetBytes(post_data);

        string Json = string.Empty;
        string Url = "https://patnamunicipal.net/pmc/public/api/due_detail.html?client_id=PMCGOV249834&command=payment&enc_request=" + to_send + "";
        Json = HttpPOSTNew(Url);
	}

    // post data to api
    public string HttpPOSTNew(string URI)
    {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);//
        request.Method = "POST";
        request.KeepAlive = true;
        request.ContentType = "appication/json";

        //request.ContentType = "application/x-www-form-urlencoded";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string myResponse = "";
        using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
        {
            myResponse = sr.ReadToEnd();
        }
        return myResponse;
    }

	public DataTable getPaytmCollectionInformationandSendBacktoSparrow(string property_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select top 1* from property_payment where pid=@propertyId order by id desc", con))
        {
            cmd.Parameters.AddWithValue("@propertyId", property_id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

	public DataTable getPaytmCollectionInformationandSendBacktoSparrowNew(string property_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select pid,amount,receive_date,since from property_payment where payment_code= @propertyId", con))
        {
            cmd.Parameters.AddWithValue("@propertyId", property_id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

	public DataTable getShopInformation(string property_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select pid,amount,receive_date,since from shops_payment where payment_code= @propertyId", con))
        {
            cmd.Parameters.AddWithValue("@propertyId", property_id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public void Update_Shop_Details(string paymentDate, string property_id)
    {
        DateTime datetime = Convert.ToDateTime(paymentDate);
        string month = datetime.Month.ToString();
        string year = datetime.Year.ToString();

        string fin_year = "";
        string CurrYear = (datetime.Year).ToString();
        string Preyear = (datetime.Year - 1).ToString();
        string NexYear = (datetime.Year + 1).ToString();
        if (datetime.Month > 3)
        {
            fin_year = CurrYear + "-" + NexYear;
        }
        else
        {
            fin_year = Preyear + "-" + CurrYear;
        }
        SqlConnection con = new SqlConnection(strcon);
        SqlCommand cmd = new SqlCommand("update Shop_Details set M_LastPaidMonth= @month,M_LastPaidYear= @year,G_R_fin_year= @fin_year Where Shop_Id= @property_id", con);
        cmd.Parameters.AddWithValue("@month", month);
        cmd.Parameters.AddWithValue("@year", year);
        cmd.Parameters.AddWithValue("@fin_year", fin_year);
        cmd.Parameters.AddWithValue("@property_id", property_id);
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
    }

    //Added by Gyan Chand Verma
    //Date 25-09-2020
    public void Update_Shop_Calculation_Details(string paymentDate, string SCD_Id)
    {
        DateTime datetime = Convert.ToDateTime(paymentDate);
        string month = datetime.Month.ToString();
        string year = datetime.Year.ToString();

        string fin_year = "";
        string CurrYear = (datetime.Year).ToString();
        string Preyear = (datetime.Year - 1).ToString();
        string NexYear = (datetime.Year + 1).ToString();
        if (datetime.Month > 3)
        {
            fin_year = CurrYear + "-" + NexYear;
        }
        else
        {
            fin_year = Preyear + "-" + CurrYear;
        }
        SqlConnection con = new SqlConnection(strcon);
        SqlCommand cmd = new SqlCommand("update ShopCalculationDeatils set C_M_Month= @month,C_M_Year= @year,C_P_GroundRent= @fin_year, status='active' Where id= @SCD_Id", con);
        cmd.Parameters.AddWithValue("@month", month);
        cmd.Parameters.AddWithValue("@year", year);
        cmd.Parameters.AddWithValue("@fin_year", fin_year);
        cmd.Parameters.AddWithValue("@SCD_Id", SCD_Id);
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
    }
}

public class my_class_new
{
    public string property_no { get; set; }
    public string ulb_name { get; set; }
    public string pmc_ref_no { get; set; }
    public string paid_amount { get; set; }
    public string entry_datetime { get; set; }
}
