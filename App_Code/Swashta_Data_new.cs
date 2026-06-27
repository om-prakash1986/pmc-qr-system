using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.IO;

using System.Web.Script.Serialization;
using System.StubHelpers;
using Newtonsoft.Json;

/// <summary>
/// Summary description for Swashta_Data_new
/// </summary>
public class Swashta_Data_new
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    // Register user in swashta application
    public string register_new_user(string name, string mobile_no, string longitude, string latitude)
    {
        string status = "";
        string location = "Patna";
        try
        {
            if (longitude == "")
            {
                longitude = "25.6094787";
            }
            if (latitude == "")
            {
                latitude = "85.1329996";
            }
            string vendor_name = "Patna";
            string access_key = "bkm2zvjf";
            string api_key = "af4e61d75d2782a33eac7641e42bba6f";
            /*Stream stream = new WebClient().OpenRead("http://api.swachh.city/sbm/v1/user?vendor_name=" + vendor_name + "&access_key=" + access_key + "&mobileNumber=" + mobile_no + "&macAddress=&deviceToken=&deviceOs=external&apiKey=" + api_key + "&lang=en&latitude=" + float.Parse(latitude) + "&longitude=" + float.Parse(longitude) + "&location=" + location + "");
            string str = string.Empty;
            StreamReader streamReader = new StreamReader(stream);
            streamReader.ReadToEnd();
            string data = streamReader.ReadToEnd();
            stream.Close();
            streamReader.Close();*/

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.swachh.city/sbm/v1/user");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string postData = "vendor_name=" + vendor_name + "&access_key=" + access_key + "&mobileNumber=" + mobile_no + "&macAddress=&deviceToken=&deviceOs=external&apiKey=" + api_key + "&lang=en&latitude=" + float.Parse(latitude) + "&longitude=" + float.Parse(longitude) + "&location=" + location + "";
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            var result = reader.ReadToEnd();
            //{"httpCode":200,"code":2000,"message":"Success","user":{"id":15982424,"session_lang_id":1,"activated":0}}
            status = Convert.ToString(result);
            stream.Dispose();
            reader.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return status;
    }

    // update sent details
    public int update_sent_details(string mobile_no, string resStatus, string ResponseIde, string value)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update combatcellTitle set is_sent=@value, RegistrationStatus=@resStatus,ResponseId=@ResponseIde where MobileNo=@mobile_no", con);
            cmd.Parameters.AddWithValue("@resStatus", resStatus);
            cmd.Parameters.AddWithValue("@ResponseIde", ResponseIde);
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.ExecuteNonQuery();
            con.Close();
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
}
