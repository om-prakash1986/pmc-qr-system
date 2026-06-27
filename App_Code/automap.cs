using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using PMC;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using PMC.DAL;
using System.Configuration;

/// <summary>
/// Summary description for automap
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[ScriptService]
public class automap : System.Web.Services.WebService
{
    string cs = ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
    List<SqlParameter> param;
    DataAccessLayer dac;
    public automap()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    JavaScriptSerializer js = new JavaScriptSerializer();

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void pushData(string Mid, string authoKey, string PlanNumber, string BuilderName, string LandOwner, string MobileNumber,int UnitIdentifier)
    {
        string URL = "";
        bool isAuthenticate = false;
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();
        if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
        {
            isAuthenticate = true;
        }
        autoMap auto;
        if (isAuthenticate)
        {
            bool isDuplicate = CheckDuplicate(Mid, PlanNumber, BuilderName, LandOwner, MobileNumber,UnitIdentifier);

            if (!isDuplicate)
            {
                long rid;

                try
                {
                    rid = insertData(UnitIdentifier,Mid, PlanNumber, BuilderName, LandOwner, MobileNumber, "100", "Successful", "NA");
                    auto = new autoMap()
                    {
                        Status = "Successful",
                        Status_Code = "100",
                        URL = "https://pmc.bihar.gov.in/AutomapSAS.aspx?ID=" + rid
                    };
                }
                catch (Exception ex)
                {
                    auto = new autoMap()
                    {
                        Status = "Error",
                        Status_Code = "200",
                        URL = "NA"
                    };
                }
            }
            else
            {
                auto = new autoMap()
                {
                    Status = "Duplicate Data",
                    Status_Code = "400",
                    URL = "NA"
                };
            }
        }
        else
        {
            auto = new autoMap()
            {
                Status = "Invalid Access",
                Status_Code = "300",
                URL = "NA"
            };
        }

        Context.Response.Write(JsonConvert.SerializeObject(auto, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
    }
    public long insertData(int UnitIdentifier,string Mid, String PlanNumber, string BuilderName, string Landowner, string MobileNumber, string Status_Code, string Status, string Remarks)
    {
        long sm = 0;
        string query = @"INSERT INTO [dbo].[tbl_ReceivedDataAutoMap]
           ([UnitIdentifier],[Mid] ,[PlanNumber] ,[BuilderName] ,[LandOwner]  ,[MobileNumber] ,[Status_Code]
           ,[Status],[Ip_Address],[EntryDate],[Remarks]) Values 
         (@UnitIdentifier,@Mid,@PlanNumber,@BuilderName,@LandOwner,@MobileNumber,@Status_Code,@Status,convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),GETDATE(),@Remarks);SELECT CAST(SCOPE_IDENTITY() AS BIGINT); ";// SELECT CAST(SCOPE_IDENTITY() AS BIGINT); "select @@IDENTITY"

        param = new List<SqlParameter>();
        param.Add(new SqlParameter("@UnitIdentifier", Convert.ToInt32(UnitIdentifier)));
        param.Add(new SqlParameter("@Mid", Mid));
        param.Add(new SqlParameter("@PlanNumber", PlanNumber));
        param.Add(new SqlParameter("@BuilderName", BuilderName));
        param.Add(new SqlParameter("@LandOwner", Landowner));
        param.Add(new SqlParameter("@MobileNumber", MobileNumber));
        param.Add(new SqlParameter("@Status_Code", Status_Code));
        param.Add(new SqlParameter("@Status", Status));
        param.Add(new SqlParameter("@Remarks", Remarks));
        dac = new DataAccessLayer();
        sm = Convert.ToInt64(dac.Scalar(query, param));
        return sm;
    }
    public class autoMap
    {   
        public string Status { get; set; }
        public string Status_Code { get; set; }
        public string URL { get; set; }

    }
    public bool CheckDuplicate(string Mid, string PlanNumber, string BuilderName, string Landowner, string MobileNumber, int UnitIdentifier)
    {
        bool isDuplicate = false;
        string query = @"SELECT COUNT(*) FROM tbl_ReceivedDataAutoMap WHERE Mid = @Mid AND PlanNumber = @PlanNumber AND BuilderName = @BuilderName
        AND Landowner = @Landowner AND MobileNumber = @MobileNumber AND UnitIdentifier = @UnitIdentifier";

        using (SqlConnection connection = new SqlConnection(cs))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Mid", Mid);
                command.Parameters.AddWithValue("@PlanNumber", PlanNumber);
                command.Parameters.AddWithValue("@BuilderName", BuilderName);
                command.Parameters.AddWithValue("@Landowner", Landowner);
                command.Parameters.AddWithValue("@MobileNumber", MobileNumber);
                command.Parameters.AddWithValue("@UnitIdentifier", UnitIdentifier);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                isDuplicate = count > 0;
            }
        }
        return isDuplicate;
    }
}