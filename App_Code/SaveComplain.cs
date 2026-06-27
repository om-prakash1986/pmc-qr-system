using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

/// <summary>
/// Summary description for SaveComplain
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/CombatCell/Service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class SaveComplain : System.Web.Services.WebService
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public SaveComplain()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void SaveRecord(string title, string categoryid, string subcategoryId, string Latitude, string Longitude, string name, string MobileNo, string Emailid)
    {
        string Returnid = "0";

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_Title_App"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", 0);
            cmd.Parameters["@id"].Direction = ParameterDirection.InputOutput;
            cmd.Parameters.AddWithValue("@ComplainNo", "");
            cmd.Parameters.AddWithValue("@added_by", "5");

            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@categoryid", categoryid);
            cmd.Parameters.AddWithValue("@subcategoryId", subcategoryId);
            cmd.Parameters.AddWithValue("@Latitude", Latitude);
            cmd.Parameters.AddWithValue("@Longitude", Longitude);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@MobileNo", MobileNo);
            cmd.Parameters.AddWithValue("@Emailid", Emailid);
            cmd.Parameters.AddWithValue("@message", "");
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                Returnid = cmd.Parameters["@id"].Value.ToString();
		// t
                Swashta_Data_new sd = new Swashta_Data_new();
                string Status = "";
                string status1 = "";
                string ResponseId = "";
                Status = sd.register_new_user(name, MobileNo, Longitude, Latitude);
                string[] st = new string[5];
                st = Status.Split(',');
                status1 = st[2].Split(':')[1].Replace('"', ' ');
                status1 = status1.TrimEnd();
                status1 = status1.TrimStart();
                ResponseId = st[3].Split(':')[2];
                 if (status1 == "You have been successfully registred")
                    {
                        sd.update_sent_details(MobileNo, status1, ResponseId, "1");
                    }
                    else
                    {
                        sd.update_sent_details(MobileNo, status1, ResponseId, "0");
                    }
                //// here
                categoryid = categoryid.Replace("\"", "");
                subcategoryId = subcategoryId.Replace("\"", "");
                Latitude = Latitude.Replace("\"", "");
                Longitude = Longitude.Replace("\"", "");
                name = name.Replace("\"", "");
                MobileNo = MobileNo.Replace("\"", "");
                Emailid = Emailid.Replace("\"", "");
                title = title.Replace("\"", "");

                try
                {

                    string filepath = "http://pmc.bihar.gov.in/CombatCell/complainImage/noimage.jpg".Trim();
                    SwachCityApiIntigration swach = new SwachCityApiIntigration();
                    swach.Call_Post_complain_Service_and_store_data(Returnid, Convert.ToInt64(MobileNo), int.Parse(subcategoryId), float.Parse(Latitude), float.Parse(Longitude), "Patna", "Patna", name, float.Parse(Latitude), float.Parse(Longitude), "Patna", filepath);

                }
                catch (Exception ex1)
                {
                }

                if (MobileNo != "")
                {
                    send_sms ss = new send_sms();
                    int auth = ss.sendSingleSMSNew(MobileNo, "Thank you, for submitting your request. You will get the Complain No. within next 2 Hrs. -Patna Municipal Corporation", "1307162308451612847");
                }
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
            con.Close();
            con.Dispose();
        }
        Context.Response.Write(JsonConvert.SerializeObject(Returnid, Newtonsoft.Json.Formatting.Indented));

    }


}
