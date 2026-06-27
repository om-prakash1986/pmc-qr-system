using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for SendPOSMacDataToPMC
/// </summary>
[WebService(Namespace = "https://pmc.bihar.gov.in/service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class SendPOSMacDataToPMC : System.Web.Services.WebService
{
    string CS = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    Logger lg = new Logger();
    public SendPOSMacDataToPMC()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }
    //, string MerchantId, string IpAddress, string UserName, string Password, String Key
    //public string AuthenticationAndAuthorisation(string POSData)
    //{
    //    string strMsg = string.Empty;
    //    using (SqlConnection con = new SqlConnection(CS))
    //    {
    //        SqlCommand cmd = new SqlCommand("SELECT MerchantName,UserName,Password,MerchantKey,ContactPerson,ContactNumber,EmailId,IpAddress FROM [AXISBANK].[MerchantMaster] where IsActive=1 and MerchantId='BRPMC2022001'", con);
    //        SqlDataReader rdr = cmd.ExecuteReader();
    //        if (rdr.Read())
    //        {
    //            var serializeData = JsonConvert.DeserializeObject<List<ReturnFields>>(POSData);
    //            foreach (var data in serializeData)
    //            {
    //                //if (data.IpAddress == rdr["IpAddress"].ToString())
    //                //{
    //                //    if (data.UserName == rdr["UserName"].ToString())
    //                //    {
    //                //        if (data.Password == rdr["Password"].ToString())
    //                //        {
    //                //            strMsg = "AUTHENTICATED";
    //                //        }
    //                //        else
    //                //        {
    //                //            strMsg = "Wrong Password!";
    //                //        }
    //                //    }
    //                //    else
    //                //    {
    //                //        strMsg = "Invalid User!";
    //                //    }
    //                //}
    //                //else
    //                //{
    //                //    strMsg = "Invalid IpAddress!";
    //                //}
    //            }
    //        }
    //        else
    //        {
    //            strMsg = "Not authorised to access!";
    //        }
    //    }
    //    return strMsg;
    //}

    [WebMethod(EnableSession = false, Description = "Insert POS Machine Data Method")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string InsertPOSMachineData()
    {
        _returnFields ObjectName = null;

        string contentType = HttpContext.Current.Request.ContentType;

        if (false == contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)) return "";

        using (System.IO.Stream stream = HttpContext.Current.Request.InputStream)
        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
        {
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            string bodyText = reader.ReadToEnd(); bodyText = bodyText == "" ? "{}" : bodyText;

            var json = Newtonsoft.Json.Linq.JObject.Parse(bodyText);

            ObjectName = Newtonsoft.Json.JsonConvert.DeserializeObject<_returnFields>(json.ToString());
        }
        //string POSData = "";
        string strMsg = string.Empty;
        //ReturnFields objectReturnFields = new ReturnFields();
        try
        {
            strMsg = SavePOSData(ObjectName);
            Response response = new Response();
            if (strMsg == "SUCCESS")
            {
                response.Status = "OK";
                response.StatusCode = "200";
            }
            else
            {
                response.Status = "FAILURE";
                response.StatusCode = "201";
            }
            //JSONdesrilize(POSData);
            //strMsg = AuthenticationAndAuthorisation(POSData);
            //if (strMsg == "AUTHENTICATED")
            //{
            //    strMsg = SavePOSData(POSData);
            //}
            strMsg = JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        }
        catch (Exception ex)
        {
            strMsg = ex.Message;
            InsertIntoLogFile(strMsg.ToString());
        }
        return strMsg;
    }

    public string SavePOSData(_returnFields data)
    {
        //string str = "{\"user\":[" + POSData + "]}";

        //string str = POSData;
        JavaScriptSerializer js = new JavaScriptSerializer();
        string strMsg = string.Empty;
        //JToken root = JObject.Parse(str);
        //JToken rootData = root["user"];

        //string jsonText = File.ReadAllText(str);

        // Need to deserialize POSData

        //var serializeData = JsonConvert.DeserializeObject<List<ReturnFields>>(jsonText);// It required a json array to deserialize
        //var serializeData = js.Deserialize<List<_returnFields>>(rootData.ToString());

        //var serializeData = JsonConvert.DeserializeObject<List<AxisBankJsonData>>(rootData.ToString());// It required a json array to deserialize
        //var serializeData = JsonConvert.DeserializeObject<List>();
        //DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(ReturnFields));
        //MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(returnFields.ToString()));

        using (var con = new SqlConnection(CS))
        {
            //foreach (var data in POSData)
            //{
            using (var cmd = new SqlCommand("[AXISBANK].[spSavePOSData]", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (data.fineType != null)
                {
                    cmd.Parameters.AddWithValue("@fineType", data.fineType);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@fineType", DBNull.Value);
                }
                if (data.subFine != null)
                {
                    cmd.Parameters.AddWithValue("@subFine", data.subFine);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@subFine", DBNull.Value);
                }
                if (data.ssfine != null)
                {
                    cmd.Parameters.AddWithValue("@ssfine", data.ssfine);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ssfine", DBNull.Value);
                }
                if (data.totalArea != null)
                {
                    cmd.Parameters.AddWithValue("@totalArea", data.totalArea);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@totalArea", DBNull.Value);
                }
                if (data.numberofvehicles != null)
                {
                    cmd.Parameters.AddWithValue("@numberofvehicles", data.numberofvehicles);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@numberofvehicles", DBNull.Value);
                }
                if (data.fy != null)
                {
                    cmd.Parameters.AddWithValue("@fy", data.fy);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@fy", DBNull.Value);
                }
                if (data.parkingArea != null)
                {
                    cmd.Parameters.AddWithValue("@parkingArea", data.parkingArea);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@parkingArea", DBNull.Value);
                }
                if (data.vehicleNo != null)
                {
                    cmd.Parameters.AddWithValue("@vehicleNo", data.vehicleNo);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@vehicleNo", DBNull.Value);
                }
                if (data.address != null)
                {
                    cmd.Parameters.AddWithValue("@address", data.address);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@address", DBNull.Value);
                }
                if (data.username != null)
                {
                    cmd.Parameters.AddWithValue("@username", data.username);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@username", DBNull.Value);
                }
                if (data.txnAmount != null)
                {
                    cmd.Parameters.AddWithValue("@txnAmount", data.txnAmount);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@txnAmount", DBNull.Value);
                }
                if (data.totalhours != null)
                {
                    cmd.Parameters.AddWithValue("@totalhours", data.totalhours);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@totalhours", DBNull.Value);
                }
                if (data.totalDays != null)
                {
                    cmd.Parameters.AddWithValue("@totalDays", data.totalDays);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@totalDays", DBNull.Value);
                }
                if (data.specialnumberdays != null)
                {
                    cmd.Parameters.AddWithValue("@specialnumberdays", data.specialnumberdays);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@specialnumberdays", DBNull.Value);
                }
                if (data.noofanimals != null)
                {
                    cmd.Parameters.AddWithValue("@noofanimals", data.noofanimals);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@noofanimals", DBNull.Value);
                }
                if (data.wardNo != null)
                {
                    cmd.Parameters.AddWithValue("@wardNo", data.wardNo);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@wardNo", DBNull.Value);
                }
                if (data.citizenName != null)
                {
                    cmd.Parameters.AddWithValue("@citizenName", data.citizenName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@citizenName", DBNull.Value);
                }
                if (data.totalmeters != null)
                {
                    cmd.Parameters.AddWithValue("@totalmeters", data.totalmeters);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@totalmeters", DBNull.Value);
                }
                if (data.CircleName != null)
                {
                    cmd.Parameters.AddWithValue("@circleName", data.CircleName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@circleName", DBNull.Value);
                }
                if (data.guardianName != null)
                {
                    cmd.Parameters.AddWithValue("@guardianName", data.guardianName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@guardianName", DBNull.Value);
                }
                if (data.externalRefNumber2 != null)
                {
                    cmd.Parameters.AddWithValue("@externalRefNumber2", data.externalRefNumber2);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@externalRefNumber2", DBNull.Value);
                }
                if (data.externalRefNumber3 != null)
                {
                    cmd.Parameters.AddWithValue("@externalRefNumber3", data.externalRefNumber3);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@externalRefNumber3", DBNull.Value);
                }
                if (data.txnId != null)
                {
                    cmd.Parameters.AddWithValue("@txnId", data.txnId);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@txnId", DBNull.Value);
                }
                if (data.authCode != null)
                {
                    cmd.Parameters.AddWithValue("@authCode", data.authCode);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@authCode", DBNull.Value);
                }
                if (data.userNames != null)
                {
                    cmd.Parameters.AddWithValue("@userNames", data.userNames);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@userNames", DBNull.Value);
                }
                if (data.customerName != null)
                {
                    cmd.Parameters.AddWithValue("@customerMobile", data.customerName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@customerMobile", DBNull.Value);
                }
                if (data.rrNumber != null)
                {
                    cmd.Parameters.AddWithValue("@rrNumber", data.rrNumber);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@rrNumber", DBNull.Value);
                }
                if (data.paymentMode != null)
                {
                    cmd.Parameters.AddWithValue("@paymentMode", data.paymentMode);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@paymentMode", DBNull.Value);
                }
                if (data.states != null)
                {
                    cmd.Parameters.AddWithValue("@states", data.states);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@states", DBNull.Value);
                }
                if (data.status != null)
                {
                    cmd.Parameters.AddWithValue("@status", data.status);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@status", DBNull.Value);
                }
                if (data.tid != null)
                {
                    cmd.Parameters.AddWithValue("@tid", data.tid);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@tid", DBNull.Value);
                }
                if (data.receiptUrl != null)
                {
                    cmd.Parameters.AddWithValue("@receiptUrl", data.receiptUrl);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@receiptUrl", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@amount", data.amount);
                if (data.deviceSerial != null)
                {
                    cmd.Parameters.AddWithValue("@deviceSerial", data.deviceSerial);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@deviceSerial", DBNull.Value);
                }
                if (data.issuerCode != null)
                {
                    cmd.Parameters.AddWithValue("@issuerCode", data.issuerCode);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@issuerCode", DBNull.Value);
                }
                cmd.Connection = con;
                try
                {
                    int status = 0;
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        status = cmd.ExecuteNonQuery();
                        if (status == 1)
                        {
                            strMsg = "SUCCESS";
                        }
                        else
                        {
                            strMsg = "FAILURE";
                        }
                    }
                }
                catch (Exception ex)
                {
                    strMsg = ex.Message;
                    InsertIntoLogFile(strMsg.ToString());
                }
                finally
                {
                    con.Close();
                }
            }
        }
        return strMsg;
    }

    private void InsertIntoLogFile(string log)
    {
        SqlConnection con = new SqlConnection(CS);
        using (SqlCommand cmd = new SqlCommand("spInsertLog", con))
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter param = new SqlParameter("@ExceptionMessage", log);
            cmd.Parameters.Add(param);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }

    private string GetDataFromArray(string Data)
    {
        string strMsg = string.Empty;
        //string par = {"address":"chandpur bela patna"};

        return strMsg;
    }

    public class AxisBankJsonData
    {
        public _returnFields[] returnfields { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class _returnFields
    {
        public bool? success { get; set; }
        public Setting setting { get; set; }
        public List<object> apps { get; set; }
        public List<object> taskList { get; set; }
        public double? amount { get; set; }
        public double? amountAdditional { get; set; }
        public double? amountOriginal { get; set; }
        public double? amountCashBack { get; set; }
        public string authCode { get; set; }
        public string batchNumber { get; set; }
        public string cardLastFourDigit { get; set; }
        public string currencyCode { get; set; }
        public string customerName { get; set; }
        public string customerReceiptUrl { get; set; }
        public string deviceSerial { get; set; }
        public string externalRefNumber { get; set; }
        public string formattedPan { get; set; }
        public string merchantName { get; set; }
        public string mid { get; set; }
        public string nonceStatus { get; set; }
        public string orgCode { get; set; }
        public string merchantCode { get; set; }
        public string payerName { get; set; }
        public string paymentCardBin { get; set; }
        public string paymentCardBrand { get; set; }
        public string paymentCardType { get; set; }
        public string paymentMode { get; set; }
        public string pgInvoiceNumber { get; set; }
        public DateTime postingDate { get; set; }
        public string processCode { get; set; }
        public string rrNumber { get; set; }
        public string settlementStatus { get; set; }
        public string signatureId { get; set; }
        public string status { get; set; }
        public string states { get; set; }
        public string tid { get; set; }
        public string userMobile { get; set; }
        public string txnType { get; set; }
        public bool? dccOpted { get; set; }
        public int? cardHolderCurrencyExponent { get; set; }
        public string userAgreement { get; set; }
        public bool? signable { get; set; }
        public bool? voidable { get; set; }
        public bool? refundable { get; set; }
        public DateTime? chargeSlipDate { get; set; }
        public string readableChargeSlipDate { get; set; }
        public string cardTxnTypeDesc { get; set; }
        public string dxMode { get; set; }
        public string receiptUrl { get; set; }
        public bool? signReqd { get; set; }
        public string txnTypeDesc { get; set; }
        public string cardTxnType { get; set; }
        public string id { get; set; }
        public string paymentGateway { get; set; }
        public string txnRequestId { get; set; }
        public string acquirerCode { get; set; }
        public string referenceTransactionId { get; set; }
        public string stan { get; set; }
        public string additionalParamJson { get; set; }
        public DateTime createdTime { get; set; }
        public bool? customerNameAvailable { get; set; }
        public bool? callbackEnabled { get; set; }
        public double? additionalAmount { get; set; }//
        public string orderNumber { get; set; }
        public string reverseReferenceNumber { get; set; }
        public double? totalAmount { get; set; }//
        public string displayPAN { get; set; }
        public string nameOnCard { get; set; }
        public string invoiceNumber { get; set; }
        public string cardType { get; set; }
        public bool? tipEnabled { get; set; }
        public bool? callTC { get; set; }
        public string acquisitionId { get; set; }
        public string acquisitionKey { get; set; }
        public bool? externalDevice { get; set; }
        public bool? tipAdjusted { get; set; }
        public List<object> txnMetadata { get; set; }
        public bool? taxPresent { get; set; }
        public string issuerCode { get; set; }
        public string fineType { get; set; }
        public string subFine { get; set; }
        public string ssfine { get; set; }
        public string totalArea { get; set; }
        public string numberofvehicles { get; set; }
        public string fy { get; set; }
        public string parkingArea { get; set; }
        public string vehicleNo { get; set; }
        public string address { get; set; }
        public string username { get; set; }
        public string txnAmount { get; set; }
        public string totalhours { get; set; }
        public string totalDays { get; set; }
        public string specialnumberdays { get; set; }
        public string noofanimals { get; set; }
        public string wardNo { get; set; }
        public string citizenName { get; set; }
        public string totalmeters { get; set; }
        public string CircleName { get; set; }
        public string guardianName { get; set; }
        public string userNames { get; set; }
        public string externalRefNumber2 { get; set; }
        public string externalRefNumber3 { get; set; }
        public string txnId { get; set; }
    }

    public class Setting
    {
    }

    public class Response
    {
        public string Status { get; set; }
        public string StatusCode { get; set; }
    }
}

