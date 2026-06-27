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

/// <summary>
/// Summary description for GetData
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/CombatCell/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class GetData : System.Web.Services.WebService {
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public GetData () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string GetCatagoryDetails()
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


        return DataTableToJSONWithJavaScriptSerializer(dt);
    }
    public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
    {
        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
        Dictionary<string, object> childRow;
        foreach (DataRow row in table.Rows)
        {
            childRow = new Dictionary<string, object>();
            foreach (DataColumn col in table.Columns)
            {
                childRow.Add(col.ColumnName, row[col]);
            }
            parentRow.Add(childRow);
        }
        return jsSerializer.Serialize(parentRow);
    }


    [WebMethod]
    public string GetSubCatagoryDetails(string Catid)
    {


        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,SubcatagoryName from CombatCellSubcategory where status=1 and categoryid=@catagoryid order by SubcatagoryName ASC", con);
            cmd.Parameters.Add("@catagoryid", Catid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }


        return DataTableToJSONWithJavaScriptSerializer(dt);
    }


    //[WebMethod]
    //public string GetSubCatagoryDetails(string Catid)
    //{


    //    DataTable dt = new DataTable();
    //    DataSet ds = new DataSet();
    //    using (SqlConnection con = new SqlConnection(strcon))
    //    {
    //        con.Open();
    //        SqlCommand cmd = new SqlCommand("select id,SubcatagoryName from CombatCellSubcategory where status=1 and categoryid=@catagoryid order by SubcatagoryName ASC", con);
    //        cmd.Parameters.Add("@catagoryid", Catid);
    //        SqlDataAdapter da = new SqlDataAdapter(cmd);
    //        da.Fill(ds);
    //        dt = ds.Tables[0];
    //    }


    //    return DataTableToJSONWithJavaScriptSerializer(dt);
    //}


}
