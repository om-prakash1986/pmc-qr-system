using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using PMC;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using PMC.DAL;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for FinalDetail
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[ScriptService]
public class PhonePeQR : System.Web.Services.WebService
{
    public PhonePeQR()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    JavaScriptSerializer js = new JavaScriptSerializer();
    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void sendresponse(string response)
    {
        data d;
        string orderid = "";
        string amount = "";
        string utr = "";
        string encodedText = response;

        var encodedTextBytes = Convert.FromBase64String(encodedText);

        string plainText = System.Text.Encoding.UTF8.GetString(encodedTextBytes);

        plainText = plainText.Replace("\"", "");
        plainText = plainText.Replace("{", "");
        plainText = plainText.Replace("}", "");
        plainText = plainText.Replace("[", "");
        plainText = plainText.Replace("]", "");
        plainText = plainText.Replace("data:", "");

        string[] arrData = plainText.Split(',');

        string successStatus = arrData[0].Split(':')[1];
        amount = arrData[6].Split(':')[1];
        double pamount = Convert.ToDouble(amount) / 100;
        if (successStatus == "true" & pamount > 0)
        {
            orderid = arrData[4].Split(':')[1];
            MobAppRequestController mrc = new MobAppRequestController();
            DataTable dtRequestDetail = mrc.GetMobAppRequestDetailOnRequestID(Convert.ToInt64(orderid));

            if(dtRequestDetail.Rows[0]["mid"].ToString()=="KIOSK")
            {
                sendresponseKIOSK(response);
            }
            else
            {
                sendresponsePHONEPE(response);
            }
        }
    }
    public void sendresponsePHONEPE(string response)
    {
        data d;
        string orderid = "";
        string amount = "";
        string utr = "";
        string encodedText = response;

        var encodedTextBytes = Convert.FromBase64String(encodedText);

        string plainText = System.Text.Encoding.UTF8.GetString(encodedTextBytes);

        plainText = plainText.Replace("\"", "");
        plainText = plainText.Replace("{", "");
        plainText = plainText.Replace("}", "");
        plainText = plainText.Replace("[", "");
        plainText = plainText.Replace("]", "");
        plainText = plainText.Replace("data:", "");

        string[] arrData = plainText.Split(',');

        string successStatus = arrData[0].Split(':')[1];
        amount = arrData[6].Split(':')[1];
        double pamount = Convert.ToDouble(amount) / 100;
        if (successStatus == "true" & pamount > 0)
        {
            orderid = arrData[4].Split(':')[1];
            amount = pamount.ToString();
            utr = arrData[11].Split(':')[1];
            d = new data();
            d.Mid = "PHONEPEQR";
            d.authoKey = "123456";
            d.Request_No = orderid;
            d.Transaction_Date = System.DateTime.Now.ToString();
            d.Transaction_ID = utr;
            d.Paid_Amount = amount;
            d.Payment_Status = "Success";
            d.Payment_Remarks = "Success";

            string url = "http://192.168.39.140/PtaxRequest.asmx/getDemandPhonePe";

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            myHttpWebRequest.Method = "POST";

            string postData = "{\"Mid\":\"PHONEPEQR\"" +
                               ",\"authoKey\":\"123456\"" + ",\"rid\":\"" + orderid + "\"}";


            byte[] data = Encoding.ASCII.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/json";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            myHttpWebRequest.ContentLength = data.Length;

            Stream requestStream = myHttpWebRequest.GetRequestStream();

            requestStream.Write(data, 0, data.Length);

            requestStream.Close();


            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            Stream responseStream = myHttpWebResponse.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

            string pageContent = myStreamReader.ReadToEnd();

            Console.WriteLine(pageContent);

            myStreamReader.Close();

            responseStream.Close();

            myHttpWebResponse.Close();

            pushTransaction(d);

        }
    }
    public void sendresponseKIOSK(string response)
    {
        data d;
        string orderid = "";
        string amount = "";
        string utr = "";
        string encodedText = response;

        var encodedTextBytes = Convert.FromBase64String(encodedText);

        string plainText = System.Text.Encoding.UTF8.GetString(encodedTextBytes);

        plainText = plainText.Replace("\"", "");
        plainText = plainText.Replace("{", "");
        plainText = plainText.Replace("}", "");
        plainText = plainText.Replace("[", "");
        plainText = plainText.Replace("]", "");
        plainText = plainText.Replace("data:", "");

        string[] arrData = plainText.Split(',');

        string successStatus = arrData[0].Split(':')[1];
        amount = arrData[6].Split(':')[1];
        double pamount = Convert.ToDouble(amount) / 100;
        if (successStatus == "true" & pamount > 0)
        {
            orderid = arrData[4].Split(':')[1];
            amount = pamount.ToString();
            utr = arrData[11].Split(':')[1];
            d = new data();
            d.Mid = "KIOSK";
            d.authoKey = "123456";
            d.Request_No = orderid;
            d.Transaction_Date = System.DateTime.Now.ToString();
            d.Transaction_ID = utr;
            d.Paid_Amount = amount;
            d.Payment_Status = "Success";
            d.Payment_Remarks = "Success";

            pushTransaction(d);

        }
    }
    public void pushTransaction(data t1)
    {
        returnStatus RStatus = null;
        RStatus = new returnStatus();
        string clientIp = HttpContext.Current.Request.UserHostAddress;

        //if (clientIp == "103.116.33.8" || clientIp == "103.116.33.9" || clientIp == "103.116.33.10" || clientIp == "103.116.33.11" || clientIp == "103.116.33.136" || clientIp == "103.116.33.137" || clientIp == "103.116.33.138" || clientIp == "103.116.33.139")
        //{

        /*
         public string Request_No { get; set; }
    public string Transaction_ID { get; set; }
    public string Transaction_Date { get; set; }
    public string Paid_Amount { get; set; }
    public string Payment_Status { get; set; }
    public
         */
        bool isAuthenticate = false;
        string Mid = t1.Mid;
        string authoKey = t1.authoKey;
        string Request_No;
        string Transaction_ID;
        string Transaction_Date;
        string Paid_Amount;
        string Payment_Status;
        string Payment_Remarks;
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();

        if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
        {
            isAuthenticate = true;
        }
        try
        {
            if (isAuthenticate)
            {

                Request_No = t1.Request_No;
                Transaction_ID = t1.Transaction_ID;
                Transaction_Date = t1.Transaction_Date;
                Paid_Amount = t1.Paid_Amount;
                Payment_Status = t1.Payment_Status;
                Payment_Remarks = t1.Payment_Remarks;

            }
            else
            {
                Request_No = t1.Request_No;
                Transaction_ID = t1.Transaction_ID;
                Transaction_Date = t1.Transaction_Date;
                Paid_Amount = t1.Paid_Amount;
                Payment_Status = "Failed";
                Payment_Remarks = "Invalid Access";
            }


            DataTable dtRequestDetail = mrc.GetMobAppRequestDetailOnRequestID(Convert.ToInt64(Request_No));
            if (dtRequestDetail.Rows.Count > 0)
            {
                MobAppPaymentController mapc = new MobAppPaymentController();
                DataTable dtPayment = mapc.chkPaymentStatusByRID(Convert.ToInt64(Request_No));
                if (dtPayment.Rows.Count <= 0)
                {


                    int msg = mapc.Insert(Convert.ToInt64(Request_No), Transaction_ID, Convert.ToDateTime(Transaction_Date), Convert.ToDecimal(Paid_Amount), Payment_Status, Payment_Remarks, 1, System.DateTime.Now, clientIp);


                    if (isAuthenticate)
                    {

                        if (Payment_Status == "Success")
                        {
                            YearlyTaxAssessmentController ytac = new YearlyTaxAssessmentController();

                            DataTable PDetail = new DataTable();
                            SearchDataController sdc = new SearchDataController();
                            if (dtRequestDetail.Rows[0]["pid"].ToString() == "")
                            {
                                PDetail = sdc.SearchProperty(Convert.ToInt64(dtRequestDetail.Rows[0]["property_id"].ToString()));
                            }
                            else
                            {
                                PDetail = sdc.SearchProperty(Convert.ToInt64(dtRequestDetail.Rows[0]["pid"].ToString()));
                            }
                            decimal lateSub = 0;
                            if (dtRequestDetail.Rows[0]["late_submission_charge"].ToString() == "")
                            {
                                lateSub = 0;
                            }
                            else
                            {
                                lateSub = Convert.ToDecimal(dtRequestDetail.Rows[0]["late_submission_charge"].ToString());
                            }
                            long ApplicationTranID = ytac.insertOnSuccessPayment(Convert.ToInt64(Request_No), Convert.ToInt64(PDetail.Rows[0]["id"].ToString()), Transaction_ID, Convert.ToDateTime(Transaction_Date), Convert.ToDecimal(Paid_Amount), lateSub, dtRequestDetail.Rows[0]["sas_no"].ToString(), Mid);

                            RStatus.Transaction = "Transaction ID: " + ApplicationTranID;
                            RStatus.Status = "Transaction Successful";
                            RStatus.Receipt_Url = "Receipt URL: https://pmc.bihar.gov.in/payment_receipt.aspx?uid=" + ApplicationTranID;

                            
                        }
                        else
                        {
                            RStatus.Transaction = "Transaction ID: " + Transaction_ID;
                            RStatus.Status = "Transaction Failed";
                            RStatus.Receipt_Url = "NA";
                        }
                    }
                    else
                    {

                        RStatus.Transaction = "NA";
                        RStatus.Status = "Invalid Access";
                        RStatus.Receipt_Url = "NA";
                    }


                }
                else
                {
                    RStatus.Transaction = "NA";
                    RStatus.Status = "This Request No. has already paid transaction";
                    RStatus.Receipt_Url = "NA";
                }
            }
            else
            {
                RStatus.Transaction = "NA";
                RStatus.Status = "Wrong Request No.";
                RStatus.Receipt_Url = "NA";
            }

        }
        catch (Exception ex)
        {

            RStatus.Transaction = "NA";
            RStatus.Status = "Error";
            RStatus.Receipt_Url = "NA";

        }
        Context.Response.Write(JsonConvert.SerializeObject(RStatus, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
        //}
        //else
        //{
        //    RStatus.Transaction = "NA";
        //    RStatus.Status = "Invalid Access";
        //    RStatus.Receipt_Url = "NA";
        //    Context.Response.Write(JsonConvert.SerializeObject(RStatus, Formatting.Indented));
        //    Context.Response.Flush();
        //    Context.Response.End();
        //}
    }
    public class data
    {
        public string Mid { get; set; }
        public string authoKey { get; set; }
        public string Request_No { get; set; }
        public string Transaction_ID { get; set; }
        public string Transaction_Date { get; set; }
        public string Paid_Amount { get; set; }
        public string Payment_Status { get; set; }
        public string Payment_Remarks { get; set; }
    }
    public class returnStatus
    {
        public string Transaction { get; set; }
        public string Status { get; set; }
        public string Receipt_Url { get; set; }
    }
}