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
/// Summary description for pmcSendSms
/// </summary>
[WebService(Namespace = "https://pmc.bihar.gov.in/service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class pmcsendsms : System.Web.Services.WebService {

    public pmcsendsms () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = false, Description = "Send single SMS")]
    public string SendSms(string mobileNo, string message, string Key)
    {
        string number = "0";
        if (Key == "7110EDA4D09E062AA5E4A390B0A572AC0D2C0220")
        {
            send_sms sms = new send_sms();
            number = sms.sendSingleSMSNew(mobileNo, message + " - Bihar Government.", "1307161770553497695").ToString();
        }
        else
        {
            number = "0";
        }
        return number;
    }    
}
