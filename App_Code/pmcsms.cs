using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for pmcsms
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class pmcsms : System.Web.Services.WebService {

    public pmcsms () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string SendSms(string key,string mobileNo, string message)
    {
        string number = "error";
        if (key == "2cb962ac59075b964b07152d234b70")
        {
            send_sms sms = new send_sms();
            number = sms.sendSingleSMS(mobileNo, message).ToString();
            if(number=="1")
            {
                number = "success";
            }
        }
        else
        {
            number = "Invalid Key";
        }
        return number;
    }
    
}
