using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using PMC;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using PMC.DAL;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for FinalDetail
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[ScriptService]
public class electricity_data : System.Web.Services.WebService
{
	public electricity_data()
	{
		//
		// TODO: Add constructor logic here
		//
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
	}
    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }
    JavaScriptSerializer js = new JavaScriptSerializer();
    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void pushData(string Mid,string authoKey,List<electricity_consumer> ec)
    {
        //return_data RData = null;
        saveStatus RStatus = null;
        List<saveStatus> lst;
        try
        {
            bool isAuthenticate = false;
            
            encryptdecrypt ed = new encryptdecrypt();
            MobAppRequestController mrc = new MobAppRequestController();
            if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
            {
                isAuthenticate = true;
            }

            if (isAuthenticate)
            {

                string connstring = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
                SqlConnection con = new SqlConnection(connstring);
                string q = "";
                //RData = new return_data();
                lst = new List<saveStatus>();
                for (int i = 0; i < ec.Count; i++)
                {
                    using (SqlCommand cmd = new SqlCommand("Electric_Consumer_Data"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CONSUMER_ID", ec[i].CONSUMER_ID.ToString());
                        cmd.Parameters.AddWithValue("@CONSUMER_NAME", ec[i].CONSUMER_NAME.ToString());
                        cmd.Parameters.AddWithValue("@FIRM_NAME", ec[i].FIRM_NAME.ToString());
                        cmd.Parameters.AddWithValue("@FA_HU_NAME", ec[i].FA_HU_NAME.ToString());
                        cmd.Parameters.AddWithValue("@CONNECTION_TYPE", ec[i].CONNECTION_TYPE.ToString());
                        cmd.Parameters.AddWithValue("@DATE_OF_CONNECTION", ec[i].DATE_OF_CONNECTION.ToString());
                        cmd.Parameters.AddWithValue("@BLOCK", ec[i].BLOCK.ToString());
                        cmd.Parameters.AddWithValue("@WARD", ec[i].WARD.ToString());
                        cmd.Parameters.AddWithValue("@ADDRESS_LINE_1", ec[i].ADDRESS_LINE_1.ToString());
                        if (ec[i].ADDRESS_LINE_2 == null)
                        {
                            cmd.Parameters.AddWithValue("@ADDRESS_LINE_2", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ADDRESS_LINE_2", ec[i].ADDRESS_LINE_2.ToString());
                        }
                        if (ec[i].ADDITIONAL_FIELD_1 == null)
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_1", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_1", ec[i].ADDITIONAL_FIELD_1.ToString());
                        }
                        if (ec[i].ADDITIONAL_FIELD_2 == null)
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_2", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_2", ec[i].ADDITIONAL_FIELD_2.ToString());
                        }
                        if (ec[i].ADDITIONAL_FIELD_3 == null)
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_3", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_3", ec[i].ADDITIONAL_FIELD_3.ToString());
                        }
                        if (ec[i].ADDITIONAL_FIELD_4 == null)
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_4", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ADDITIONAL_FIELD_4", ec[i].ADDITIONAL_FIELD_4.ToString());
                        }
                        SqlParameter parm = new SqlParameter("@Return", SqlDbType.Int);
                        parm.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(parm);
                        
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        
                        int id = Convert.ToInt32(parm.Value);
                        if (id == 1)
                        {
                            RStatus = new saveStatus()
                            {
                                KEY = ec[i].CONSUMER_ID.ToString(),
                                STATUS = "SUCCESS",
                                DESCRIPTION = "Successfully received"
                            };
                            
                            lst.Add(RStatus);
                           
                        }
                        else
                        {
                            RStatus = new saveStatus()
                            {
                                KEY = ec[i].CONSUMER_ID.ToString(),
                                STATUS = "DUPLICATE",
                                DESCRIPTION = "Duplicate Data"
                            };
                            
                            lst.Add(RStatus);
                            
                        }
                    }
                }
                //RData = new return_data()
                //{
                //    returnList = lst
                //};
            }
            else
            {
                RStatus = new saveStatus()
                {
                    KEY = ec[0].CONSUMER_ID.ToString(),
                    STATUS = "FAILED",
                    DESCRIPTION = "Invalid Access"
                };
                lst = new List<saveStatus>();
                lst.Add(RStatus);
                //RData = new return_data()
                //{
                //    returnList = lst
                //};
                
            }
        }
        catch (Exception ex)
        {
            
            RStatus = new saveStatus()
            {
                KEY = ec[0].CONSUMER_ID.ToString(),
                STATUS = "FAILED",
                DESCRIPTION = "Technical Issue"
            };
            lst = new List<saveStatus>();
            lst.Add(RStatus);
            //RData = new return_data()
            //{
            //    returnList=lst
            //};

        }
        Context.Response.Write(JsonConvert.SerializeObject(lst, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
    }

}
public class electricity_consumer
{
    public string CONSUMER_ID { get; set; }
    public string CONSUMER_NAME { get; set; }
    public string FIRM_NAME { get; set; }
    public string FA_HU_NAME { get; set; }
    public string CONNECTION_TYPE { get; set; }
    public string DATE_OF_CONNECTION { get; set; }
    public string BLOCK { get; set; }
    public string WARD { get; set; }
    public string ADDRESS_LINE_1 { get; set; }
    public string ADDRESS_LINE_2 { get; set; }
    public string ADDITIONAL_FIELD_1 { get; set; }
    public string ADDITIONAL_FIELD_2 { get; set; }
    public string ADDITIONAL_FIELD_3 { get; set; }
    public string ADDITIONAL_FIELD_4 { get; set; }
}
//public class consumer_data
//{
//    public string Mid { get; set; }
//    public string authoKey { get; set; }
//    public List<electricity_consumer> consumerList
//    {
//        get;
//        set;
//    }
//}
public class saveStatus
{
    public string KEY { get; set; }
    public string STATUS { get; set; }
    public string DESCRIPTION { get; set; }
}
//public class return_data
//{
//    public List<saveStatus> returnList
//    {
//        get;
//        set;
//    }
//}