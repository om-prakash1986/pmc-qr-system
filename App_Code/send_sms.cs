using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for send_sms
/// </summary>
public class send_sms
{
    public int sendSingleSMS(string mobileNo, string Message)
    {
        int n = 0;
        try
        {

            String username = "BIHAREDISTRICT-pmc";
            String password = "pmcpat@#*2018";
            String senderid = "BRGOVT";
            String key = "f2475cb7-0406-4ca8-8e93-ebb16800d159";

            // generating hash
            new_single_sms ss = new new_single_sms();
            ss.sendSingleSMS(username, password, senderid, mobileNo, Message, key);

            n = 1;

            //Stream stream = new WebClient().OpenRead("http://sms.webdesigntechnology.in/API/WebSMS/Http/v1.0a/index.php?username=biharstate&password=123456&sender=BSHBON&to=" + mobileNo + "&message=" + Message + "&route_id=0");
            //string str = string.Empty;
            //StreamReader streamReader = new StreamReader(stream);
            //streamReader.ReadToEnd();
            //stream.Close();
            //streamReader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
        return n;
    }

    public int sendSingleSMSNew(string mobileNo, string Message, string tempateId)
    {
        int n = 0;
        try
        {
            String username = "BIHAREDISTRICT-pmc";
            String password = "pmcpat@#*2018";
            String senderid = "BRGOVT";
            String key = "f2475cb7-0406-4ca8-8e93-ebb16800d159";

            // generating hash
            new_single_sms ss = new new_single_sms();
            ss.sendSingleSMSNew(username, password, senderid, mobileNo, Message, key, tempateId);

            n = 1;

            //Stream stream = new WebClient().OpenRead("http://sms.webdesigntechnology.in/API/WebSMS/Http/v1.0a/index.php?username=biharstate&password=123456&sender=BSHBON&to=" + mobileNo + "&message=" + Message + "&route_id=0");
            //string str = string.Empty;
            //StreamReader streamReader = new StreamReader(stream);
            //streamReader.ReadToEnd();
            //stream.Close();
            //streamReader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
        return n;
    }

    public int sendSingleUnicodeSMS(string mobileNo, string Message)
    {
        int n = 0;
        try
        {

            String username = "BIHAREDISTRICT-pmc";
            String password = "pmcpat@#*2018";
            String senderid = "BHRGOV";
            String key = "f2475cb7-0406-4ca8-8e93-ebb16800d159";

            // generating hash
            new_single_sms ss = new new_single_sms();
            ss.sendUnicodeSMS(username, password, senderid, mobileNo, Message, key);

            n = 1;

            //Stream stream = new WebClient().OpenRead("http://sms.webdesigntechnology.in/API/WebSMS/Http/v1.0a/index.php?username=biharstate&password=123456&sender=BSHBON&to=" + mobileNo + "&message=" + Message + "&route_id=0");
            //string str = string.Empty;
            //StreamReader streamReader = new StreamReader(stream);
            //streamReader.ReadToEnd();
            //stream.Close();
            //streamReader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
        return n;
    }
}