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
    /// Summary description for MobAppResponseController
    /// </summary>
    public class MobAppResponseController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public MobAppResponseController()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public int Insert(long request_id, decimal property_tax_amount, decimal waste_charges, decimal lateSubmission, decimal total_payable_amount, long property_detail_tmp_id, int status, DateTime response_date) 
        {
            int rowEffected = 0;
            string q = "";
            param=new List<SqlParameter>() ;
            param.Add(new SqlParameter("@request_id", request_id));
            param.Add(new SqlParameter("@property_tax_amount", property_tax_amount));
            param.Add(new SqlParameter("@waste_charges", waste_charges));
            param.Add(new SqlParameter("@late_submission_charge", lateSubmission));
            param.Add(new SqlParameter("@total_payable_amount", total_payable_amount));
            param.Add(new SqlParameter("@property_detail_tmp_id", property_detail_tmp_id));
            param.Add(new SqlParameter("@status", status));
            param.Add(new SqlParameter("@response_date", response_date));


            q = "insert into tbl_mobapp_response (request_id, 	property_tax_amount, 	waste_charges,late_submission_charge, 	total_payable_amount, 	property_detail_tmp_id, 	status, 	response_date)	Values	(@request_id, 	@property_tax_amount, 	@waste_charges, @late_submission_charge,	@total_payable_amount, 	@property_detail_tmp_id, 	@status, 	@response_date)	";
            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return rowEffected;
        }
        public DataTable GetMobAppResponseDetail(long request_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@request_id", request_id));
            q = "select * from tbl_mobapp_response where request_id=@request_id and status=1";
            dac = new DataAccessLayer();
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        
    }
}
