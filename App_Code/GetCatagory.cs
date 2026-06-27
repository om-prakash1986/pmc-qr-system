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
/// Summary description for GetCatagory
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/CombatCell/Service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class GetCatagory : System.Web.Services.WebService {
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public GetCatagory () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]  
    public void GetCatagoryDetails()
    {


        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,combatcatagory,CatCode from CombatCategory where status='Active' order by combatcatagory ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
     Context.Response.Write( JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.Indented)); 
    }
   


}
