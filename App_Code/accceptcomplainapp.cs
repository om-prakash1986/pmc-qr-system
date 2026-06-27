using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

/// <summary>
/// Summary description for accceptcomplainapp
/// </summary>
//[WebService(Namespace = "http://tempuri.org/")]
[WebService(Namespace = "http://pmc.bihar.gov.in/CombatCell/Service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class accceptcomplainapp : System.Web.Services.WebService
{
    string strcon = ConfigurationManager.ConnectionStrings["elms"].ConnectionString;
    public accceptcomplainapp()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string SaveRecord(string title, string categoryid, string subcategoryId, string Latitude, string Longitude, string name, string MobileNo, string Emailid)
    {
        string Returnid = "0";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_Title_App"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
	    cmd.Parameters.Add("@id", SqlDbType.Int, 0);
            cmd.Parameters["@id"].Direction = ParameterDirection.Output;
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
                con.Close();

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
        return Returnid;
    }


    [WebMethod]
    public string SaveImage(string combatcelltitleid, byte[] Attachement)
    {


        string Returnid = "0";

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_Image"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@combatcelltitleid", combatcelltitleid);
            cmd.Parameters.AddWithValue("@Attachement", Attachement);
            cmd.Parameters.AddWithValue("@attatchmenttype", "image/jpeg");

            cmd.Parameters.AddWithValue("@status", true);
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                Returnid = "1";
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
        return Returnid;
    }
}
