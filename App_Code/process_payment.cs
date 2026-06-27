using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using com.awl.MerchantToolKit;

/// <summary>
/// Summary description for process_payment
/// </summary>
public class process_payment     
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public int insert_new_payment(string p_id, string unique_key, string assesment_year, string amount, string waste_charge, string property_charge)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO property_payment(pid,payment_code,assessment_year,waste_charge,property_tax, amount, since) VAlUES(@p_id,@unique_key,@assessment_year,@waste_chargee,@property_chargee,@amount,@since)", con);
            cmd.Parameters.AddWithValue("@p_id", p_id);
            cmd.Parameters.AddWithValue("@unique_key", unique_key);
            cmd.Parameters.AddWithValue("@waste_chargee", waste_charge);
            cmd.Parameters.AddWithValue("@property_chargee", property_charge);
            cmd.Parameters.AddWithValue("@assessment_year", assesment_year);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();


            ReqMsgDTO objReqMsgDTO = new ReqMsgDTO();
            //step 2
            objReqMsgDTO.OrderId = "" + unique_key.ToString().Trim() + "";
            objReqMsgDTO.Mid = "WL0000000007384";
            objReqMsgDTO.Enckey = "7f2e93c63179a5ae9006f24dc6547ab4";
            objReqMsgDTO.MeTransReqType = "S";
            objReqMsgDTO.TrnAmt = Convert.ToString(Convert.ToDouble(amount) * 100);
            objReqMsgDTO.ResponseUrl = "https://www.pmc.bihar.gov.in/thankyoupayment.aspx";
            objReqMsgDTO.TrnRemarks = "Payment of property tax";
            objReqMsgDTO.TrnCurrency = "INR";

            //Step 3: Call API to generate the message
            AWLMEAPI objawlmerchantkit = new AWLMEAPI();
            objReqMsgDTO = objawlmerchantkit.generateTrnReqMsg(objReqMsgDTO);
            string Message;
            if (objReqMsgDTO.StatusDesc == "Success")
            {
                Message = objReqMsgDTO.ReqMsg;
                HttpContext.Current.Session["MID"] = objReqMsgDTO.Mid;
                HttpContext.Current.Session["Msg"] = Message;
                number = 1;
		HttpContext.Current.Response.Redirect("myTransaction.aspx", false);
            }
        }
        catch(Exception ex)
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

    // till here
}