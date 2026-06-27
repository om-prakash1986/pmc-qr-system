using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

/// <summary>
/// Summary description for QRCodeProgram
/// </summary>
namespace Rextester
{
    public class Program
    {
        private const string PHONEPE_STAGE_BASE_URL = "https://mercury-t2.phonepe.com";
        private string merchantKey = "5e352f9b-afa6-419b-b881-ae7548f39244";
        private const string merchantId = "PATNAMUNICIPALCORPORATION";
        private string storeId = "PMC_New_SAS_Online";
        private string terminalId = "Online_PMC_Website";
        //private int expiresIn = 72000;

        

        public string SendPaymentRequest(string RequestId, string PID_SAS, int payableAmt,long expiresIn)
        {
            string result = string.Empty;
            PhonePeCollectRequest phonePeCollectRequest = new PhonePeCollectRequest();

            phonePeCollectRequest.merchantId = merchantId;
            phonePeCollectRequest.transactionId = RequestId;
            phonePeCollectRequest.merchantOrderId = PID_SAS;
            phonePeCollectRequest.amount = payableAmt*100;
            phonePeCollectRequest.expiresIn = expiresIn;
            phonePeCollectRequest.storeId = storeId;
            phonePeCollectRequest.terminalId = terminalId;

            //convert string to json
            String jsonStr = JsonConvert.SerializeObject(phonePeCollectRequest);
            Console.WriteLine(jsonStr);

	               //jsonStr = "{\"merchantId\":\"PATNAMUNICIPALCORPORATION\",\"transactionId\":\"" + RequestId + "\",\"merchantOrderId\":\"" +PID_SAS + "\",\"amount\":"+payableAmt+",\"expiresIn\":"+expiresIn+",\"storeId\":\"PMC_New_SAS_Online\",\"terminalId\":\"Online_PMC_Website\"}";
	    
		
            string base64Json = ConvertStringToBase64(jsonStr);
            Console.WriteLine(base64Json);

	    

            string jsonSuffixString = base64Json + "/v3/qr/init" + merchantKey;

            string checksum = GenerateSha256ChecksumFromBase64Json(jsonSuffixString);
            checksum = checksum + "###1";
            Console.WriteLine(checksum);
            
            string txnURL = PHONEPE_STAGE_BASE_URL + "/v3/qr/init";
            Console.WriteLine("txnURL : " + txnURL);
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(txnURL);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                
                webRequest.Headers.Add("x-verify", checksum);

                PhonePeCollectApiRequestBody phonePeCollectApiRequestBody = new PhonePeCollectApiRequestBody();
                phonePeCollectApiRequestBody.request = base64Json;
                String jsonBody = JsonConvert.SerializeObject(phonePeCollectApiRequestBody);

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(jsonBody);
                }

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    if (responseData.Length > 0)
                    {
                        //Dictionary<string, string> responseParam = JSONConvert.decode(responseData);
                        PhonePeCollectResponseBody responseBody = JsonConvert.DeserializeObject<PhonePeCollectResponseBody>(responseData);
                        result = responseData;
                        //result=responseBody.message;
                        
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
		
                result=e.Message+" Checksum- "+checksum+" JsonSuffix- "+jsonSuffixString;
            }
		
            return result;
        }

        public string SendCheckPaymentStatusRequest(string transactionId)
        {
            string headerString = String.Format("/v3/transaction/{0}/{1}/status{2}", merchantId, transactionId, merchantKey);
            Console.WriteLine("headerString: " + headerString);
            string checksum = GenerateSha256ChecksumFromBase64Json(headerString);
            checksum = checksum + "###1";
            Console.WriteLine(checksum);

            string txnURL = PHONEPE_STAGE_BASE_URL;
            String urlSuffix = String.Format("/v3/transaction/{0}/{1}/status", merchantId, transactionId);
            txnURL = txnURL + urlSuffix;

            Console.WriteLine("Url: " + txnURL);
            string result = "";
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(txnURL);

                webRequest.Method = "GET";
                webRequest.Headers.Add("x-verify", checksum);

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    if (responseData.Length > 0)
                    {
                        PhonePeCollectResponseBody responseBody = JsonConvert.DeserializeObject<PhonePeCollectResponseBody>(responseData);
                        result=responseData;
                        //Console.WriteLine(responseBody.message);
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                result = e.Message;
                
            }
            return result;
        }

        private bool CallPhonePeStatusApi(String xVerify, string transactionId)
        {
            Console.WriteLine("CallPhonePeStatusApi()");
            string txnURL = PHONEPE_STAGE_BASE_URL;
            String urlSuffix = String.Format("/v3/transaction/{0}/{1}/status", merchantId, transactionId);
            txnURL = txnURL + urlSuffix;

            Console.WriteLine("Url: " + txnURL);

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(txnURL);

                webRequest.Method = "GET";
                webRequest.Headers.Add("x-verify", xVerify);

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    if (responseData.Length > 0)
                    {
                        PhonePeCollectResponseBody responseBody = JsonConvert.DeserializeObject<PhonePeCollectResponseBody>(responseData);
                        Console.WriteLine(responseData);
                        Console.WriteLine(responseBody.message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return false;
        }
	
	public bool SendCancelRequest(string transactionId)
        {
            string headerString = String.Format("/v3/charge/{0}/{1}/cancel{2}", merchantId, transactionId, merchantKey);
            Console.WriteLine("headerString: " + headerString);
            string checksum = GenerateSha256ChecksumFromBase64Json(headerString);
            checksum = checksum + "###1";
            Console.WriteLine(checksum);

            bool result = CallCancelApi(checksum,transactionId);

            return result;
        }

        private bool CallCancelApi(String xVerify,string transactionId)
        {
            Console.WriteLine("CallPhonePeStatusApi()");
            string txnURL = PHONEPE_STAGE_BASE_URL;
            String urlSuffix = String.Format("/v3/charge/{0}/{1}/cancel", merchantId, transactionId);
            txnURL = txnURL + urlSuffix;

            Console.WriteLine("Url: " + txnURL);

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(txnURL);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add("x-verify", xVerify);

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    if (responseData.Length > 0)
                    {
                        PhonePeCollectResponseBody responseBody = JsonConvert.DeserializeObject<PhonePeCollectResponseBody>(responseData);
                        Console.WriteLine(responseData);
                        Console.WriteLine(responseBody.message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return false;
        }	

        //convert jsonBody to base64
        public string ConvertStringToBase64(string inputString)
        {
            string base64Json = null;
            byte[] requestBytes = Encoding.UTF8.GetBytes(inputString);
            base64Json = Convert.ToBase64String(requestBytes);
            return base64Json;
        }

        //convert jsonBody From Base64String
        public string ConvertStringFromBase64String(string inputString)
        {
            var encodedTextBytes = Convert.FromBase64String(inputString);

            string plainText = Encoding.UTF8.GetString(encodedTextBytes);
            return plainText;
        }

        // calculte SHA256
        private string GenerateSha256ChecksumFromBase64Json(string jsonSuffixString)
        {
            //string checksum = null;
            SHA256 sha256 = SHA256.Create();
            string checksumString = jsonSuffixString;
            byte[] checksumBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(checksumString));
            //checksum = BitConverter.ToString(checksumBytes).Replace("-", string.Empty);
            
              
            // Convert byte array to a string
            StringBuilder checksum = new StringBuilder();
            for (int i = 0; i < checksumBytes.Length; i++)
            {
                checksum.Append(checksumBytes[i].ToString("x2"));
            }
            return checksum.ToString();

            //return checksum;
        }
    }

    public class PhonePeCollectRequest
    {
        public string merchantId;
        public string transactionId;
        public string merchantOrderId;
        public int amount;
        public long expiresIn;
        public string storeId;
        public string terminalId;
    }

    public class PhonePeCollectApiRequestBody
    {
        public string request;
    }

    public class PhonePeCollectResponseBody
    {
        public bool success;
        public string code;
        public string message;
        public Data data;
    }
    public class Data
    {
        public string transactionId;
        public int amount;
        public string merchantId;
        public string providerReferenceId;
    }

    public class QRResponse
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }

    public class QRCodeResponse
    {
        public string merchantId { get; set; }
        public string transactionId { get; set; }
        public Int64 amount { get; set; }
        public string qrString { get; set; }
    }
}