using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using PMC.DAL;


namespace PMC
{
    /// <summary>
    /// Summary description for MobAppRequestController
    /// </summary>
    public class MobAppRequestController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public MobAppRequestController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public bool verifyUser(string mid,string pswd)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@mid", mid));
            param.Add(new SqlParameter("@pswd", pswd));
            q = "select * from tbl_merchant_accesskey where mid=@mid and access_key=@pswd and status=1";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool chkRequest(string Request_No)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@request_id", Request_No));
            q = "select request_id from tbl_mobapp_request where request_id=@request_id and status=1";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string Insert( string mid, long pid, string sas_no, string type_of_request, int status, DateTime entry_date, string ip_address) 
        {
            string RequestId="";
            string q = "";
            param=new List<SqlParameter>() ;
            param.Add(new SqlParameter("@mid", mid));
            param.Add(new SqlParameter("@pid", pid));
            param.Add(new SqlParameter("@sas_no", sas_no));
            param.Add(new SqlParameter("@type_of_request", type_of_request));
            param.Add(new SqlParameter("@status", status));
            param.Add(new SqlParameter("@entry_date", entry_date));
            param.Add(new SqlParameter("@ip_address", ip_address));
            dac = new DataAccessLayer();
            q = "insert into tbl_mobapp_request	(mid, 	pid, 	sas_no, 	type_of_request, 	status, 	entry_date, 	ip_address) Values	(@mid, 	@pid, 	@sas_no, 	@type_of_request, 	@status, 	@entry_date, 	@ip_address); select @@IDENTITY";
            dac = new DataAccessLayer();
            RequestId = Convert.ToString(dac.Scalar(q, param));
            return RequestId;
        }
        public string InsertWithPid(string mid, string property_id, string pid, string sas_no, string type_of_request, int status, string ip_address)
        {
            string RequestId = "";
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@mid", mid));
            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@pid", pid));
            param.Add(new SqlParameter("@sas_no", sas_no));
            param.Add(new SqlParameter("@type_of_request", type_of_request));
            param.Add(new SqlParameter("@status", status));
            //param.Add(new SqlParameter("@entry_date", entry_date.ToString()));
            param.Add(new SqlParameter("@ip_address", ip_address));

            q = "insert into tbl_mobapp_request	(mid, property_id, 	pid, 	sas_no, 	type_of_request, 	status, 	entry_date, 	ip_address) Values	(@mid,@property_id, 	@pid, 	@sas_no, 	@type_of_request, 	@status, 	GETDATE(), 	@ip_address); select @@IDENTITY";
            dac = new DataAccessLayer();
            RequestId = Convert.ToString(dac.Scalar(q, param));
            return RequestId;
        }
        public DataTable GetMobAppRequestDetail(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select * from tbl_mobapp_request where property_id=@property_id and status=1";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetMobAppRequestDetailOnRequestID(long request_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@request_id", request_id));
            q = "select tbl_mobapp_request.request_id,pid,mid,sas_no,late_submission_charge,property_id from tbl_mobapp_request,tbl_mobapp_response where tbl_mobapp_request.request_id=tbl_mobapp_response.request_id and tbl_mobapp_request.request_id=@request_id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
