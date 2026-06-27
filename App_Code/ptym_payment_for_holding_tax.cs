using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.StubHelpers;
using Newtonsoft.Json;
using System.Text;

/// <summary>
/// Summary description for ptym_payment_for_holding_tax
/// </summary>
public class ptym_payment_for_holding_tax
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public string ptym_payment_for_holding_tax_check(string holding_no)
    {
        string is_available = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from property_details where (PID=@holding_no or OldPID=@holding_no)", con);
            cmd.Parameters.AddWithValue("@holding_no", holding_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                is_available = dt.Rows[0]["PID"].ToString();
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return is_available;
    }

    public DataTable getDetails_for_paytm_payment(string holding_no)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 a.OwnerName,a.MobileNo,b.Address,b.PropertyType from owner_details a inner join property_details b on a.PID=b.PID where a.PID=@holding_no ", con);
            cmd.Parameters.AddWithValue("@holding_no", holding_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return dt;
    }

    public Boolean getalready_paytm_payment_details(string holding_no, string tran_id)
    {
        Boolean status = false;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 tran_id from property_payment where PID=@holding_no and tran_id=@tran_id ", con);
            cmd.Parameters.AddWithValue("@holding_no", holding_no);
            cmd.Parameters.AddWithValue("@tran_id", tran_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return status;
    }
    public Boolean getalready_paytm_payment__byassesment(string holding_no, string assessment_year, string amount)
    {
        Boolean status = false;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 assessment_year from property_payment where PID=@holding_no and assessment_year=@assessment_year and amount=@amount ", con);
            cmd.Parameters.AddWithValue("@holding_no", holding_no);
            cmd.Parameters.AddWithValue("@assessment_year", assessment_year);
            cmd.Parameters.AddWithValue("@amount", amount);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    status = true;
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return status;
    }


    public string insert_paytm_payment_details(string pid, string payment_code, string assessment_year, string amount, string status, string tran_id, string payment_status, string successful_status)
    {
        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_new_ptym_payment"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_id", pid);
            cmd.Parameters.AddWithValue("@payment_code", payment_code);
            cmd.Parameters.AddWithValue("@assessment_year", assessment_year);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@tran_id", tran_id);
            cmd.Parameters.AddWithValue("@receive_date", DateTime.Now);
            cmd.Parameters.AddWithValue("@payment_status", payment_status);
            cmd.Parameters.AddWithValue("@success_status", successful_status);

            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                number = "1";
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

    public string HttpPOST(string URI)
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

    public string new_request(string p_id)
    {
        string amount = "0";
        my_class_01 mcc = new my_class_01();

        string pid = p_id;
        Guid obj = Guid.NewGuid();
        mcc.property_no = pid;
        mcc.pmc_ref_no = obj.ToString();
        mcc.ulb_name = "Patna Municipal Corporation";

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
        string Url = "https://patnamunicipal.net/pmc/public/api/due_detail.html?client_id=PMCGOV249834&command=due_detail&enc_request=" + to_send + "";
        Json = HttpPOST(Url);
        JavaScriptSerializer serializer1 = new JavaScriptSerializer();
        //my_class rslt = JsonConvert.DeserializeObject<my_class>(Json);

        read_responce_01 rst = JsonConvert.DeserializeObject<read_responce_01>(Json);

        enc_response_01 encres = JsonConvert.DeserializeObject<enc_response_01>(rst.enc_response);
        //error_amount.Text = Url;
        string pro_charge = encres.property_tax.ToString();
        string waste_charge = encres.waste_user_charge.ToString();
        string pay_amount = encres.total_payble_amount.ToString();
        StringBuilder sb = new StringBuilder();
        if (pay_amount != "" && pay_amount != "0")
        {
            sb.Append(pro_charge + ",");
            sb.Append(waste_charge + ",");
            sb.Append(pay_amount + ",");
            amount = sb.ToString();
        }
        else
        {
            sb.Append(pro_charge + ",");
            sb.Append(waste_charge + ",");
            sb.Append(pay_amount + ",");
            amount = sb.ToString();
        }
        return amount;
    }

    public string create_fnancial_year(double year)
    {
        double CurrentYear = year;
        double PreviousYear = year - 1;
        double NextYear = year + 1;
        string PreYear = PreviousYear.ToString();
        string NexYear = NextYear.ToString();
        string CurYear = CurrentYear.ToString();
        string FinYear = null;

        if (DateTime.Today.Month > 3)
            FinYear = CurYear + "-" + NexYear;
        else
            FinYear = PreYear + "-" + CurYear;
        return FinYear.Trim();
    }
}

public class my_class_01
{
    // public "property_no\":\"" + pid + "\",\"ulb_name\":\"Patna Municipal Corporation\",\"pmc_ref_no\":\"bbps@123456\"
    public string property_no { get; set; }
    public string ulb_name { get; set; }
    public string pmc_ref_no { get; set; }
}

public class read_responce_01
{
    public int status { get; set; }
    public string enc_response { get; set; }
    public string error_description { get; set; }
    public string error_code { get; set; }
}

public class enc_response_01
{
    public string owner_name { get; set; }
    public string due_from { get; set; }
    public string due_upto { get; set; }
    public string previous_arrear { get; set; }


    public string arrear_penalty { get; set; }
    public string total_arrear { get; set; }
    public string current_year_tax { get; set; }


    public string vacant_land_tax { get; set; }
    public string rebate { get; set; }
    public string penalty { get; set; }


    public string total_due { get; set; }
    public string property_tax { get; set; }
    public string amount_payble { get; set; }
    public string water_charge { get; set; }

    public string rainwater_harvesting_rebate { get; set; }
    public string advance_amount { get; set; }
    public string waste_user_charge { get; set; }
    public string total_payble_amount { get; set; }

}