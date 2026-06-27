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
    /// Summary description for PropertyDetailTmpController
    /// </summary>
    public class PropertyDetailTmpController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public PropertyDetailTmpController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public long Insert(long pid,string ward_id,string requestID)
        {
            int rowEffected = 0;
            string q = "";
            long iden=0;
            
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@pid", pid));

            q = "update tbl_property_detail_tmp set [status]=0 where pid=@pid; ";
            q += "INSERT INTO [tbl_property_detail_tmp] ([ulb_id],[ward_id],[revenue_circle_id],[new_holding_no],[old_holding_no],[old_pid],[pid]";
            q += ",[parent_property_id],[uid],[house_no],[muhalla],[street_name],[address],[pin],[email_id],[telephone_no],[mobile_no],[cr_house_no]";
            q += ",[cr_muhalla],[cr_street_name],[cr_address],[cr_pin],[ownership_type_id],[property_type_id],[street_type_id],[building_usage_id]";
            q += ",[construction_date],[co_circle_name],[co_mauja_name],[khata_no],[plot_no],[plot_area],[constructed_area],[water_harvesting]";
            q += ",[water_facility_id],[water_tax_id],[application_type],[last_payment_receipt_no],[last_payment_date],[last_payment_amount],[last_payment_year]";
            q += ",[last_payment_quater],[assessment_year],[mutation_type],[street_id],[ro_ordersheet],[indira_awaas],[entry_date]";
            q += ",[ip_address],[status],[user_id],[applied_from],[property_detail_id]) ";
            q += "SELECT [ulb_id],[ward_id],[revenue_circle_id],[new_holding_no],[old_holding_no],[old_pid],[pid],[parent_property_id],[uid],[house_no]";
            q += ",[muhalla],[street_name],[address],[pin],[email_id],[telephone_no],[mobile_no],[cr_house_no],[cr_muhalla],[cr_street_name],[cr_address]";
            q += ",[cr_pin],[ownership_type_id],[property_type_id],[street_type_id],[building_usage_id],[construction_date],[co_circle_name],[co_mauja_name]";
            q += ",[khata_no],[plot_no],[plot_area],[constructed_area],[water_harvesting],[water_facility_id],[water_tax_id],3,[last_payment_receipt_no],[last_payment_date]";
            q += ",[last_payment_amount],[last_payment_year],[last_payment_quater],[assessment_year],[mutation_type],[street_id],[ro_ordersheet]";
            q += ",[indira_awaas],GETDATE(),convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),1,2,'OLP',[id] FROM [tbl_property_detail] where pid=@pid and ([status]=1 or [status]=3 or [status]=2); select @@IDENTITY;";

            dac = new DataAccessLayer();
            iden = Convert.ToInt64(dac.Scalar(q, param));

            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ward_id", ward_id));
            q = "select ward_no from tbl_ward_master where id=@ward_id";
            dac = new DataAccessLayer();
            string ward = Convert.ToString(dac.Scalar(q, param));


            string app_no = "PMC/" + ward + "/" + iden;
                       
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@application_no", app_no));
            param.Add(new SqlParameter("@id", iden));
            param.Add(new SqlParameter("@sas_no", app_no));
            param.Add(new SqlParameter("@requestid", requestID));

            q = "update tbl_property_detail_tmp set application_no=@application_no where id=@id; update tbl_mobapp_request set sas_no=@sas_no where request_id=@requestid";

            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return iden;
        }
        public long InsertSas(string id, string ward_id, string requestID)
        {
            int rowEffected = 0;
            string q = "";
            long iden = 0;

            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_detail_id", id));

            q = "update tbl_property_detail_tmp set [status]=0 where property_detail_id=@property_detail_id; ";
            q += "INSERT INTO [tbl_property_detail_tmp] ([ulb_id],[ward_id],[revenue_circle_id],[new_holding_no],[old_holding_no],[old_pid],[pid]";
            q += ",[parent_property_id],[uid],[house_no],[muhalla],[street_name],[address],[pin],[email_id],[telephone_no],[mobile_no],[cr_house_no]";
            q += ",[cr_muhalla],[cr_street_name],[cr_address],[cr_pin],[ownership_type_id],[property_type_id],[street_type_id],[building_usage_id]";
            q += ",[construction_date],[co_circle_name],[co_mauja_name],[khata_no],[plot_no],[plot_area],[constructed_area],[water_harvesting]";
            q += ",[water_facility_id],[water_tax_id],[application_type],[last_payment_receipt_no],[last_payment_date],[last_payment_amount],[last_payment_year]";
            q += ",[last_payment_quater],[assessment_year],[mutation_type],[street_id],[ro_ordersheet],[indira_awaas],[entry_date]";
            q += ",[ip_address],[status],[user_id],[applied_from],[property_detail_id]) ";
            q += "SELECT [ulb_id],[ward_id],[revenue_circle_id],[new_holding_no],[old_holding_no],[old_pid],[pid],[parent_property_id],[uid],[house_no]";
            q += ",[muhalla],[street_name],[address],[pin],[email_id],[telephone_no],[mobile_no],[cr_house_no],[cr_muhalla],[cr_street_name],[cr_address]";
            q += ",[cr_pin],[ownership_type_id],[property_type_id],[street_type_id],[building_usage_id],[construction_date],[co_circle_name],[co_mauja_name]";
            q += ",[khata_no],[plot_no],[plot_area],[constructed_area],[water_harvesting],[water_facility_id],[water_tax_id],3,[last_payment_receipt_no],[last_payment_date]";
            q += ",[last_payment_amount],[last_payment_year],[last_payment_quater],[assessment_year],[mutation_type],[street_id],[ro_ordersheet]";
            q += ",[indira_awaas],GETDATE(),convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),1,2,'OLP',[id] FROM [tbl_property_detail] where id=@property_detail_id and ([status]=1 or [status]=3 or [status]=2); select @@IDENTITY;";

            dac = new DataAccessLayer();
            iden = Convert.ToInt64(dac.Scalar(q, param));

            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ward_id", ward_id));
            q = "select ward_no from tbl_ward_master where id=@ward_id";
            dac = new DataAccessLayer();
            string ward = Convert.ToString(dac.Scalar(q, param));


            string app_no = "PMC/" + ward + "/" + iden;

            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@application_no", app_no));
            param.Add(new SqlParameter("@id", iden));
            param.Add(new SqlParameter("@sas_no", app_no));
            param.Add(new SqlParameter("@requestid", requestID));

            q = "update tbl_property_detail_tmp set application_no=@application_no where id=@id; update tbl_mobapp_request set sas_no=@sas_no where request_id=@requestid";

            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return iden;
        }
        public int chkActiveSAS(long pid)
        {
            int test = 0;
            DateTime frmDate = Convert.ToDateTime("01-"+System.DateTime.Now.ToString("dd-MMM-yyyy").Split('-')[1]+"-"+System.DateTime.Now.Year);
            DateTime toDate = System.DateTime.Now;
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@pid", pid));
            param.Add(new SqlParameter("@fromDate", frmDate));
            param.Add(new SqlParameter("@toDate", toDate));
            q = "select * from tbl_property_detail_tmp where pid=@pid and entry_date>=@fromDate and entry_date<=@toDate and status=1";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);

            if (dt.Rows.Count > 0)
            {
                test = 1;
            }
            else
            {
                test = 0;
            }

            return test;
        }
        
        public long chkActiveSASYear(long pid)
        {
            long test = 0;
            DateTime frmDate;
            if (System.DateTime.Now.Month < 4)
            {
                frmDate = Convert.ToDateTime("01-Apr-" + (System.DateTime.Now.Year - 1));
            }
            else
            {
                frmDate = Convert.ToDateTime("01-Apr-" + System.DateTime.Now.Year);
            }
            DateTime toDate = System.DateTime.Now;
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@pid", pid));
            param.Add(new SqlParameter("@fromDate", frmDate));
            param.Add(new SqlParameter("@toDate", toDate));
            q = "select * from tbl_property_detail_tmp where pid=@pid and entry_date>=@fromDate and entry_date<=@toDate and status=1 order by id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                test = Convert.ToInt64(dt.Rows[0]["id"].ToString());
            }
            else
            {
                test = 0;
            }
            return test;
        }
    }
}
