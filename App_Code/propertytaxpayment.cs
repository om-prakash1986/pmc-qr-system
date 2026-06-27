using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Text.RegularExpressions;
using System.Collections.Specialized;
using com.awl.MerchantToolKit;

/// <summary>
/// Summary description for propertytaxpayment
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class propertytaxpayment : System.Web.Services.WebService {

    public propertytaxpayment () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod (EnableSession = true)]
    public string Sendtopaymentgateway(string Refrencenumber,string ApplicationNumber, string Amount, string Applicant, string Email, string MobileNo, string ApplicationType, string UserApplicationCode, string ReferenceCode, string ReferenceType, string ResponseURL)
    {
        string number = "0";


        ReqMsgDTO objReqMsgDTO = new ReqMsgDTO();
        //step 2
        objReqMsgDTO.OrderId = "" + Refrencenumber.ToString().Trim() + "";
        objReqMsgDTO.Mid = "WL0000000007384";
        objReqMsgDTO.Enckey = "7f2e93c63179a5ae9006f24dc6547ab4";
        objReqMsgDTO.MeTransReqType = "S";
        objReqMsgDTO.TrnAmt = Convert.ToString(Convert.ToDouble(Amount) * 100);
        objReqMsgDTO.ResponseUrl = "" + ResponseURL + "";
        objReqMsgDTO.TrnRemarks = "Payment from ICDS";
        objReqMsgDTO.TrnCurrency = "INR";
        objReqMsgDTO.AddField1 = Applicant;
        objReqMsgDTO.AddField2 = Email;
        objReqMsgDTO.AddField3 = MobileNo;
        objReqMsgDTO.AddField4 = ApplicationType;
        objReqMsgDTO.AddField5 = UserApplicationCode;
        objReqMsgDTO.AddField6 = ReferenceCode;
        objReqMsgDTO.AddField7 = ReferenceType;
        //Step 3: Call API to generate the message
        AWLMEAPI objawlmerchantkit = new AWLMEAPI();
        objReqMsgDTO = objawlmerchantkit.generateTrnReqMsg(objReqMsgDTO);
        string Message;
        if (objReqMsgDTO.StatusDesc == "Success")
        {
            Message = objReqMsgDTO.ReqMsg;
            HttpContext.Current.Session["MID"] = objReqMsgDTO.Mid;
            HttpContext.Current.Session["Msg"] = Message;
            number = "1";
            Server.Transfer("~/myTransaction.aspx");
             
        }
        return number;
    }
    
}
