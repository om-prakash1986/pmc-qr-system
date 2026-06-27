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
/// Summary description for process_shops_payment
/// </summary>
public class process_shops_payment
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public int insert_new_payment(string p_id, string unique_key, string assesment_year, string amount)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO shops_payment(pid,payment_code,assessment_year,amount,since) VAlUES(@p_id,@unique_key,@assessment_year,@amount,@since)", con);
            cmd.Parameters.AddWithValue("@p_id", p_id);
            cmd.Parameters.AddWithValue("@unique_key", unique_key);
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
//            objReqMsgDTO.ResponseUrl = "http://localhost:49944/thankyoupayment.aspx";
            objReqMsgDTO.TrnRemarks = "Shops monthly payment";
            objReqMsgDTO.TrnCurrency = "INR";
            objReqMsgDTO.AddField1 = p_id;
            objReqMsgDTO.AddField7 = "SP";
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
                //Response.Redirect("myTransaction.aspx", false);
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
        return number;
    }

    // till here
}