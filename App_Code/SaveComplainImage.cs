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
using System.IO;

/// <summary>
/// Summary description for SaveComplainImage
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/CombatCell/Service")]
//[WebService(Namespace = "http://tempuri.org/")]

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class SaveComplainImage : System.Web.Services.WebService
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public SaveComplainImage()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void UploadImage()
    {
        string Id = HttpContext.Current.Request.QueryString["Id"].ToString();
        //string Id = HttpContext.Current.Request.QueryString["Id"].ToString();
        string filepath = "";
        HttpFileCollection uploadedFiles = HttpContext.Current.Request.Files;
        for (int i = 0; i < uploadedFiles.Count; i++)
        {
            HttpPostedFile userPostedFile = uploadedFiles[i];
            if (userPostedFile.ContentLength > 0)
            {
                filepath = "~/CombatCell/complainImage/" + Id + "-" + System.DateTime.Now.Hour + System.DateTime.Now.Millisecond + Path.GetExtension(userPostedFile.FileName);
                userPostedFile.SaveAs(Server.MapPath(filepath));

            }

            SqlConnection con1 = new SqlConnection(strcon);
            using (SqlCommand cmd1 = new SqlCommand("Insert_Combat_Cell_Imageapp"))
            {
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@combatcelltitleid", Convert.ToInt32(Id));
                cmd1.Parameters.AddWithValue("@ImagePath", filepath);
                cmd1.Parameters.AddWithValue("@Contenttype", "image/jpeg");

                cmd1.Parameters.AddWithValue("@status", true);
                cmd1.Connection = con1;
                try
                {
                    con1.Open();
                    cmd1.ExecuteNonQuery();
                    con1.Close();
                }
                catch (Exception ex)
                {


                }
                finally
                {
                    con1.Close();
                    con1.Dispose();
                }
            }
        }
        //string Returnid = "";
        //string categoryid = "";
        //string subcategoryId = "";
        //string Latitude = "";
        //string Longitude = "";
        //string name = "";
        //string MobileNo = "";
        //string Emailid = "";
        //string title = "";
        // string complaintLocation="";
        //string complaintLandmark="";
        //DataTable dt = new DataTable();
        //SqlConnection con = new SqlConnection(strcon);
        //try
        //{
        //    SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and id=@id", con);
        //    cmd.Parameters.AddWithValue("@id", Id);
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    da.Fill(dt);
        //    if (dt != null)
        //    {
        //        Returnid = Id;

        //        categoryid = dt.Rows[0]["categoryid"].ToString().Replace("\"", "");
        //        subcategoryId = dt.Rows[0]["subcategoryId"].ToString().Replace("\"", "");
        //        Latitude = dt.Rows[0]["Latitude"].ToString().Replace("\"", "");
        //        Longitude = dt.Rows[0]["Longitude"].ToString().Replace("\"", "");
        //        name = dt.Rows[0]["name"].ToString().Replace("\"", "");
        //        MobileNo = dt.Rows[0]["MobileNo"].ToString().Replace("\"", "");
        //        Emailid = dt.Rows[0]["EmailId"].ToString().Replace("\"", "");
        //        title = dt.Rows[0]["title"].ToString().Replace("\"", "");
        //        filepath="http://pmc.bihar.gov.in"+filepath.Replace("~","");
        //        complaintLocation = dt.Rows[0]["complaintLandmark"].ToString().Replace("\"", "");
        //        complaintLandmark = dt.Rows[0]["complaintLocation"].ToString().Replace("\"", "");

        //        try
        //        {
        //            SwachCityApiIntigration swach = new SwachCityApiIntigration();
        //            swach.Call_Post_complain_Service_and_store_data(Returnid, Convert.ToInt64(MobileNo), int.Parse(subcategoryId), float.Parse(Latitude), float.Parse(Longitude), complaintLocation, complaintLandmark, name, float.Parse(Latitude), float.Parse(Longitude), complaintLocation, filepath);
        //        }
        //        catch (Exception ex)
        //        {
        //        }


        //        if (MobileNo != "")
        //        {
        //            send_sms ss = new send_sms();
        //            int auth = ss.sendSingleSMS(MobileNo, "Thank you " + name.ToString().Trim() + ", for submitting your request.You will get the Complain No within next 2 hrs.");
        //        }

        //    }
        //    else
        //    {

        //    }
        //}
        //catch (Exception ex)
        //{

        //}
        //finally
        //{

        //}



    }

}
