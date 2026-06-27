using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for SwachCityApiIntigration
/// </summary>
public class SwachCityApiIntigration
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public string HttpPOST(string URI)
    {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);//
        request.Method = "POST";
        request.KeepAlive = true;
        request.ContentType = "appication/json";
      
        //request.ContentType = "application/x-www-form-urlencoded";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string myResponse = "";
        using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
        {
            myResponse = sr.ReadToEnd();
        }
        return myResponse;
    }
    public string HttpGet(string URI)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);//
        request.Method = "GET";
        request.KeepAlive = true;
        request.ContentType = "appication/json";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string myResponse = "";
        using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
        {
            myResponse = sr.ReadToEnd();
        }
        return myResponse;
    }

    public string HttpPut(string URI)
    {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);//
        request.Method = "PUT";
        request.KeepAlive = true;
        request.ContentType = "appication/json";

        //request.ContentType = "application/x-www-form-urlencoded";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string myResponse = "";
        using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
        {
            myResponse = sr.ReadToEnd();
        }
        return myResponse;
    }
    public class Complaint
    {
        public string generic_id { get; set; }
    }

    public class RootObject
    {
        public int httpCode { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public Complaint complaint { get; set; }
    }

    public string Call_Post_complain_Service_and_store_data(string Returnid, Int64 mobileNumber, int categoryId, float complaintLatitude, float complaintLongitude, string complaintLocation, string complaintLandmark, string fullName, float userLatitude, float userLongitude, string userLocation,string Filepath)
    {
        try
        {
            string Json1 = string.Empty;
            string Url1 = "http://api.swachh.city/sbm/v1/user?vendor_name=Patna&access_key=bkm2zvjf&mobileNumber=" + mobileNumber + "&macAddress=&deviceToken=&deviceOs=external&apiKey=af4e61d75d2782a33eac7641e42bba6f&lang=en";
            Json1 = HttpPOST(Url1);
            JavaScriptSerializer serializer12 = new JavaScriptSerializer();
            RootObject rslt2 = JsonConvert.DeserializeObject<RootObject>(Json1);

        }
        catch (Exception ex)
        {
        }



        string Json = string.Empty;
        string Url = "http://api.swachh.city/sbm/v1/post-complaint?vendor_name=patna&access_key=bkm2zvjf&mobileNumber=" + mobileNumber + "&categoryId=" + categoryId + "&complaintLatitude=" + complaintLatitude + "&complaintLongitude=" + complaintLongitude + "&complaintLocation=" + complaintLocation + "&complaintLandmark=" + complaintLandmark + "&fullName=" + fullName + "&userLatitude=" + userLatitude + "&userLongitude=" + userLongitude + "&userLocation=" + userLocation + "&macAddress=&deviceToken=&deviceOs=external&file="+Filepath+"";
        Json = HttpPOST(Url);
        JavaScriptSerializer serializer1 = new JavaScriptSerializer();
        RootObject rslt = JsonConvert.DeserializeObject<RootObject>(Json);
        
        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_Combat_Cell_Title_Generic"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", Returnid);
            cmd.Parameters.AddWithValue("@Location", complaintLocation);
            cmd.Parameters.AddWithValue("@GenericId", rslt.complaint.generic_id);
            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToString(cmd.ExecuteScalar());
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
        }
        return number;
    }

    public string Call_API_for_change_complaint_status (Int64 statusid,Int64 complaintid)
    {
        string number = "0";
        if (complaintid != 0)
        {
            string Json = string.Empty;
            string Url = "http://api.swachh.city/engineer/v1/complaint-status-update?statusId=" + statusid + "&complaintId=" + complaintid + "&commentDescription=ForwardedforProcessing&deviceOs=external&vendor_name=Patna&access_key=bkm2zvjf&apiKey=af4e61d75d2782a33eac7641e42bba6f";
            Json = HttpPut(Url);
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            RootObject rslt = JsonConvert.DeserializeObject<RootObject>(Json);

           
        }

        return number;
    }

    public string Get_genericid(string complainid)
    {
        string genericid = "0";
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select GenericId from combatcellTitle where status='true' and id=@id", con);
            cmd.Parameters.AddWithValue("@id", complainid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt != null)
            {
               
                genericid = dt.Rows[0]["GenericId"].ToString().Replace("W03000C", "");
                if (dt.Rows[0]["GenericId"].ToString() == "")
                {
                    genericid = "0";
                }
            }
            else
            {
                genericid = "0";
            }
            

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
        return genericid;
    }



    public string Call_Post_complain_Service_and_store_data_Back(string Returnid, Int64 mobileNumber, int categoryId, float complaintLatitude, float complaintLongitude, string complaintLocation, string complaintLandmark, string fullName, float userLatitude, float userLongitude, string userLocation)
    {

        string Json1 = string.Empty;
        string Url1 = "http://api.swachh.city/sbm/v1/user?vendor_name=Patna&access_key=bkm2zvjf&mobileNumber=" + mobileNumber + "&macAddress=&deviceToken=&deviceOs=external&apiKey=af4e61d75d2782a33eac7641e42bba6f&lang=en";
        Json1 = HttpPOST(Url1);
        JavaScriptSerializer serializer12 = new JavaScriptSerializer();
        RootObject rslt2 = JsonConvert.DeserializeObject<RootObject>(Json1);



        string Json = string.Empty;
        string Url = "http://api.swachh.city/sbm/v1/post-complaint?vendor_name=patna&access_key=bkm2zvjf&mobileNumber=" + mobileNumber + "&categoryId=" + categoryId + "&complaintLatitude=" + complaintLatitude + "&complaintLongitude=" + complaintLongitude + "&complaintLocation=" + complaintLocation + "&complaintLandmark=" + complaintLandmark + "&fullName=" + fullName + "&userLatitude=" + userLatitude + "&userLongitude=" + userLongitude + "&userLocation=" + userLocation + "&macAddress=&deviceToken=&deviceOs=external&file";
        Json = HttpPOST(Url);
        JavaScriptSerializer serializer1 = new JavaScriptSerializer();
        RootObject rslt = JsonConvert.DeserializeObject<RootObject>(Json);

        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_Combat_Cell_Title_Generic"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", Returnid);
            cmd.Parameters.AddWithValue("@Location", complaintLocation);
            cmd.Parameters.AddWithValue("@GenericId", rslt.complaint.generic_id);
            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToString(cmd.ExecuteScalar());
                number = rslt.complaint.generic_id; 
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
        }
        return number;
    }



    public DataTable BindAllComplains_received_bydatebetween_andpage(string PageIndex,string datefrom, string dateto)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
           
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_FetchAllComplainwithPaging", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", 15);
            cmd.Parameters.Add("@RecordCount", SqlDbType.Int);
            cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@datefrom", datefrom);
            cmd.Parameters.AddWithValue("@dateto", dateto);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
            int recordCount = Convert.ToInt32(cmd.Parameters["@RecordCount"].Value);
        }
        return dt;
    }

}