using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Summary description for new_single_sms
/// </summary>
public class new_single_sms
{
    /// <summary>
    /// Method for sending single SMS.
    /// </summary>
    /// <param name="username"> Registered user name
    /// <param name="password"> Valid login password
    /// <param name="senderid">Sender ID 
    /// <param name="mobileNo"> valid Single Mobile Number 
    /// <param name="message">Message Content 
    /// <param name="secureKey">Department generate key by login to services portal
    /// <param name="templateid">templateid unique for each template message content

    // Method for sending single SMS.

	public String sendSingleSMSNew(String username, String password, String senderid, String mobileNo, String message, String secureKey, String templateid)
    {
        // Force TLS 1.2 — required by msdgweb.mgov.gov.in
        System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072; // Tls12

        String encryptedPassword = encryptedPasswod(password);
        String NewsecureKey = hashGenerator(username.Trim(), senderid.Trim(), message.Trim(), secureKey.Trim());

        String query = "username=" + HttpUtility.UrlEncode(username.Trim()) +
            "&password="      + HttpUtility.UrlEncode(encryptedPassword) +
            "&smsservicetype=singlemsg" +
            "&content="       + HttpUtility.UrlEncode(message.Trim()) +
            "&mobileno="      + HttpUtility.UrlEncode(mobileNo) +
            "&senderid="      + HttpUtility.UrlEncode(senderid.Trim()) +
            "&key="           + HttpUtility.UrlEncode(NewsecureKey.Trim()) +
            "&templateid="    + HttpUtility.UrlEncode(templateid.Trim());

        byte[] byteArray = Encoding.UTF8.GetBytes(query);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequestDLT");
        request.Method          = "POST";
        request.ContentType     = "application/x-www-form-urlencoded";
        request.ContentLength   = byteArray.Length;
        request.Timeout         = 30000; // 30 seconds
        request.KeepAlive       = false;
        request.UserAgent       = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";

        using (Stream dataStream = request.GetRequestStream())
            dataStream.Write(byteArray, 0, byteArray.Length);

        using (WebResponse response = request.GetResponse())
        using (Stream dataStream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(dataStream))
            return reader.ReadToEnd();
    }

    public String sendSingleSMS(String username, String password, String senderid, String mobileNo, String message, String secureKey)
    {
        // Force TLS 1.2 — required by msdgweb.mgov.gov.in
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // Tls12

        String encryptedPassword = encryptedPasswod(password);
        String NewsecureKey = hashGenerator(username.Trim(), senderid.Trim(), message.Trim(), secureKey.Trim());

        String query = "username=" + HttpUtility.UrlEncode(username.Trim()) +
            "&password="      + HttpUtility.UrlEncode(encryptedPassword) +
            "&smsservicetype=singlemsg" +
            "&content="       + HttpUtility.UrlEncode(message.Trim()) +
            "&mobileno="      + HttpUtility.UrlEncode(mobileNo) +
            "&senderid="      + HttpUtility.UrlEncode(senderid.Trim()) +
            "&key="           + HttpUtility.UrlEncode(NewsecureKey.Trim());

        byte[] byteArray = Encoding.UTF8.GetBytes(query);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequest");
        request.Method        = "POST";
        request.ContentType   = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;
        request.Timeout       = 30000; // 30 seconds
        request.KeepAlive     = false;
        request.UserAgent     = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";

        using (Stream dataStream = request.GetRequestStream())
            dataStream.Write(byteArray, 0, byteArray.Length);

        using (WebResponse response = request.GetResponse())
        using (Stream dataStream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(dataStream))
            return reader.ReadToEnd();
    }

    public String sendUnicodeSMS(String username, String password, String senderid, String mobileNos, String Unicodemessage, String secureKey)
    {
        Stream dataStream;
       // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //forcing .Net framework to use TLSv1.2

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequest");
        request.ProtocolVersion = HttpVersion.Version10;
        request.KeepAlive = false;
        request.ServicePoint.ConnectionLimit = 1;

        //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
        ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";

        request.Method = "POST";

        System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
        String U_Convertedmessage = "";

        foreach (char c in Unicodemessage)
        {
            int j = (int)c;
            String sss = "&#" + j + ";";
            U_Convertedmessage = U_Convertedmessage + sss;
        }
        String encryptedPassword = encryptedPasswod(password);
        String NewsecureKey = hashGenerator(username.Trim(), senderid.Trim(), U_Convertedmessage.Trim(), secureKey.Trim());


        String smsservicetype = "unicodemsg"; // for unicode msg
        String query = "username=" + HttpUtility.UrlEncode(username.Trim()) +
            "&password=" + HttpUtility.UrlEncode(encryptedPassword) +
            "&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +
            "&content=" + HttpUtility.UrlEncode(U_Convertedmessage.Trim()) +
            "&bulkmobno=" + HttpUtility.UrlEncode(mobileNos) +
            "&senderid=" + HttpUtility.UrlEncode(senderid.Trim()) +
            "&key=" + HttpUtility.UrlEncode(NewsecureKey.Trim());


        byte[] byteArray = Encoding.ASCII.GetBytes(query);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;
        dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();
        WebResponse response = request.GetResponse();
        String Status = ((HttpWebResponse)response).StatusDescription;
        dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        String responseFromServer = reader.ReadToEnd();
        reader.Close();
        dataStream.Close();
        response.Close();
        return responseFromServer;
    }

    protected String encryptedPasswod(String password)
    {

        byte[] encPwd = Encoding.UTF8.GetBytes(password);
        //static byte[] pwd = new byte[encPwd.Length];
        HashAlgorithm sha1 = HashAlgorithm.Create("SHA1");
        byte[] pp = sha1.ComputeHash(encPwd);
        // static string result = System.Text.Encoding.UTF8.GetString(pp);
        StringBuilder sb = new StringBuilder();
        foreach (byte b in pp)
        {

            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();

    }

    protected String hashGenerator(String Username, String sender_id, String message, String secure_key)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Username).Append(sender_id).Append(message).Append(secure_key);
        byte[] genkey = Encoding.UTF8.GetBytes(sb.ToString());
        //static byte[] pwd = new byte[encPwd.Length];
        HashAlgorithm sha1 = HashAlgorithm.Create("SHA512");
        byte[] sec_key = sha1.ComputeHash(genkey);

        StringBuilder sb1 = new StringBuilder();
        for (int i = 0; i < sec_key.Length; i++)
        {
            sb1.Append(sec_key[i].ToString("x2"));
        }
        return sb1.ToString();
    }

    /*****************************************************
     * Till Here
     * ***************************************************/
}

class MyPolicy : ICertificatePolicy
{
    public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
    {
        return true;
    }
}