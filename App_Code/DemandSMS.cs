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


/// <summary>
/// Summary description for smsdemand
/// </summary>
/// [WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[ScriptService]
public class DemandSMS : System.Web.Services.WebService
{
    public DemandSMS()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    JavaScriptSerializer js = new JavaScriptSerializer();
    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void getDemand(string mobileNumber,string timeStamp,string operatorName,string areaCode,string message)
    {
        //string receiverShortCode = sc;
        demand d;
        if (message != null || message !="")
        {
            //if (receiverShortCode != null || receiverShortCode !="")
            //{
            //    if ((receiverShortCode.Equals("9223166166", StringComparison.InvariantCultureIgnoreCase)))
            //    {
                    string[] getMsg = message.Split(' ');
                    if (getMsg.Length == 3)
                    {
                        if (getMsg[0] == "GET" & getMsg[1] == "DEMAND")
                        {
                            try
                            {
                                demandbysms dbs = new demandbysms();
                                dbs.getDemandBySMS(mobileNumber, getMsg[2]);
                                d = new demand()
                                {
                                    Status="Success"
                                };

                            }
                            catch (Exception ex)
                            {
                                d = new demand()
                                {
                                    Status = ex.Message
                                };
                            }
                            Context.Response.Write(JsonConvert.SerializeObject(d, Formatting.Indented));
                            Context.Response.Flush();
                            Context.Response.End();
                        }
                        
                    }
                //}
            //}

        }
        
    }
}
public class demand
{
    public string Status { get; set; }
}