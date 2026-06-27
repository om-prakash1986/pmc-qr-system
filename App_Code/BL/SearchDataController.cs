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
    /// Summary description for SearchDataController
    /// </summary>
    public class SearchDataController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public SearchDataController()
        {
            //
            // TODO: Add constructor logic here
            //
            
        }
        public DataTable SearchProperty(long pid)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", pid));
            q = "select property.id, property.ulb_id,property.uid,property.building_usage_id, property.application_no, property.ward_id,property.revenue_circle_id, " +
                "property.assessment_year as last_assessment_year,property.last_payment_receipt_no,property.last_payment_date,property.last_payment_amount,property.last_payment_year,property.PID, property.old_pid, property.new_holding_no, property.old_holding_no, property.co_mauja_name,property.co_circle_name," +
                "property.khata_no, property.plot_no,property.street_id, property.plot_area,property.constructed_area, property.property_type_id,property.construction_date as acquisition," +
                "property.water_harvesting,property.water_facility_id,property.mutation_type,property.last_payment_quater, property.water_tax_id, property.house_no, property.muhalla, property.street_name,property.street_type_id,property.[address],property.pin,property.cr_house_no, property.cr_muhalla, property.cr_street_name,property.cr_address,property.cr_pin," +
                "property.telephone_no as telephone_no, property.mobile_no,property.email_id,property.form_no,property.ro_ordersheet,property.indira_awaas,property.parent_property_id, property.ownership_type_id,sm.street_name as road, ward.circle_id,circle.circle_name, ward.ward_no, isnull(revCirmast.rev_circle,'') as rev_circle," +
                "isnull(ptm.property_type,'')as property_type, isnull(stm.street_type,'')as road_type, isnull(osm.ownership_type,'') as ownership_type" +
                //"owd.owner_name, owd.guardian_name, owd.relation, owd.mobile_no 
                " from tbl_property_detail as property" +
                " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                " left join tbl_revenue_circle_master as revCirmast on revCirmast.id=property.revenue_circle_id" +
                " inner join tbl_property_type_master as ptm on ptm.id=property.property_type_id" +
                " inner join tbl_street_type_master as stm on stm.id=property.street_type_id" +
                " inner join tbl_ownership_type_master as osm on osm.id=property.ownership_type_id" +
                " left join tbl_street_master as sm on sm.id=property.street_id" +
                //" inner join tbl_owner_detail as owd on owd.property_id=property.id" +
                " where property.pid=@id and (property.status=1 or property.status=3 or property.status=2)";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;           
        }
        public DataTable SearchProperty(string id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", id));
            q = "select property.id, property.ulb_id,property.uid,property.building_usage_id, property.application_no, property.ward_id,property.revenue_circle_id, " +
                "property.assessment_year as last_assessment_year,property.last_payment_receipt_no,property.last_payment_date,property.last_payment_amount,property.last_payment_year,property.PID, property.old_pid, property.new_holding_no, property.old_holding_no, property.co_mauja_name,property.co_circle_name," +
                "property.khata_no, property.plot_no,property.street_id, property.plot_area,property.constructed_area, property.property_type_id,property.construction_date as acquisition," +
                "property.water_harvesting,property.water_facility_id,property.mutation_type,property.last_payment_quater, property.water_tax_id, property.house_no, property.muhalla, property.street_name,property.street_type_id,property.[address],property.pin,property.cr_house_no, property.cr_muhalla, property.cr_street_name,property.cr_address,property.cr_pin," +
                "property.telephone_no as telephone_no, property.mobile_no,property.email_id,property.form_no,property.ro_ordersheet,property.indira_awaas,property.parent_property_id, property.ownership_type_id,sm.street_name as road, ward.circle_id,circle.circle_name, ward.ward_no, isnull(revCirmast.rev_circle,'') as rev_circle," +
                "isnull(ptm.property_type,'')as property_type, isnull(stm.street_type,'')as road_type, isnull(osm.ownership_type,'') as ownership_type" +
                //"owd.owner_name, owd.guardian_name, owd.relation, owd.mobile_no 
                " from tbl_property_detail as property" +
                " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                " left join tbl_revenue_circle_master as revCirmast on revCirmast.id=property.revenue_circle_id" +
                " inner join tbl_property_type_master as ptm on ptm.id=property.property_type_id" +
                " inner join tbl_street_type_master as stm on stm.id=property.street_type_id" +
                " inner join tbl_ownership_type_master as osm on osm.id=property.ownership_type_id" +
                " left join tbl_street_master as sm on sm.id=property.street_id" +
                //" inner join tbl_owner_detail as owd on owd.property_id=property.id" +
                " where property.id=@id and (property.status=1 or property.status=3 or property.status=2)";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable SearchProperty()/*int cid,int wardno,long pid,string hold_no,string oname,string sas*/
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            //param.Add(new SqlParameter("@id", pid));
            q = "select ROW_NUMBER() OVER(ORDER BY property.id ASC) AS row_num, property.id, property.application_no as sas_no, circle.circle_name,ward.ward_no, " +
                "property.assessment_year as last_assessment_year,property.PID as property_no, property.old_pid, property.new_holding_no, property.old_holding_no," +
                "property.[address],osm.owner_name " +
                
                " from tbl_property_detail as property" +
                " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                " inner join (select property_id,STRING_AGG(owner_name,', ') as owner_name from tbl_owner_detail group by property_id) as osm on osm.property_id=property.id" +
                " where property.status>=1 order by property.id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;  
        }
        public DataTable SearchProperty(int cid, int wardno, long pid, string hold_no, string oname, string sas)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            //param.Add(new SqlParameter("@id", pid));
            q = "select ROW_NUMBER() OVER(ORDER BY property.id ASC) AS row_num, property.id, property.application_no as sas_no, circle.circle_name,ward.ward_no, " +
                "property.assessment_year as last_assessment_year,property.PID, property.old_pid, property.new_holding_no, property.old_holding_no," +
                "property.[address],osm.owner_name " +

                " from tbl_property_detail as property" +
                " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                " inner join (select property_id,STRING_AGG(owner_name,', ') as owner_name from tbl_owner_detail group by property_id) as osm on osm.property_id=property.id" +
                " where property.status>=1 ";

            if (cid != 0)
            {
                param.Add(new SqlParameter("@circleid", cid));
                q += " and circle.id=@circleid ";
            }
            if (wardno != 0)
            {
                param.Add(new SqlParameter("@wardid", wardno));
                q += " and property.ward_id=@wardid ";
            }
            if (pid != 0)
            {
                param.Add(new SqlParameter("@property_no", pid));
                q += " and PID=@property_no ";
            }
            if (hold_no != "")
            {
                param.Add(new SqlParameter("@holding_no", hold_no));
                q += " and (property.new_holding_no=@new_holding_no or  property.old_holding_no=@holding_no) ";
            }
            if (oname != "")
            {
                param.Add(new SqlParameter("@oname", oname));
                q += " and owner_name like '%'+@oname+'%' ";
            }
            if (sas != "")
            {
                param.Add(new SqlParameter("@sas", sas));
                q += " and sas_no=@sas_no ";
            }
            q+="order by property.id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable SearchOccupancyDetails(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select distinct ocd.floor_id, fl.floor_no as Floor_No,ocd.builtup_area as Built_Up_Area,ocd.from_year as Assesment_From_Year,ocd.upto_year as Assesment_Upto_Year,ustm.id as Type_Of_Use_ID,ustm.use_type as Type_Of_Use," +
                                " utm.id as Usage_Factor_ID,utm.usage_type as Usage_Factor, otm.id as Occupancy_Type_ID,otm.occupancy_type as Occupancy_Type, ctm.id as Construction_Type_ID,ctm.contruction_type as Construction_Type from tbl_occupancy_detail as ocd" +
                                " inner join tbl_floor_master as fl on fl.id=ocd.floor_id" +
                                " inner join tbl_usage_type_master as utm on utm.id=ocd.usage_type_id" +
                                " inner join tbl_occupancy_type_master as otm on otm.id=ocd.occupancy_type_id" +
                                " inner join tbl_construction_type_master as ctm on ctm.id=ocd.construction_type_id" +
                                " inner join tbl_use_type_master as ustm on ustm.id=ocd.use_type_id" +
                                " where property_id=@property_id and upto_year=(select max(upto_year) from tbl_occupancy_detail where property_id=@property_id)";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;            
        }
        public DataTable SearchTaxHistory(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select * from tbl_yearly_tax_assessment where property_id=@property_id order by id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable SearchOwnerDetails(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select * from tbl_owner_detail where property_id=@property_id order by id Asc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable SearchAdvancePayment(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));

            q = "select top 1 pr.id,pr.transaction_id,pr.advance_amount, pr.advance_adjusted,pr.total_dues,pr.current_payble_amount,pr.current_received_amount,pr.arrear_payble_amount,pr.arrear_received_amount,pr.receipt_fy,pr.current_panelty_payble_amount,pr.arrear_panelty_payble_amount, pr.arrear_panelty_received_amount, pr.current_panelty_received_amount,pr.status from tbl_payment_receipt as pr" +
                " where pr.transaction_id in (select top 1 transaction_id from tbl_transaction_detail where property_id=@property_id and [status]<>3 order by id desc) order by pr.id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public int ClearPayStatus(long property_id)
        {
            int payStatus = 1;
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select top 1 * from tbl_transaction_detail where property_id=@property_id and [status]<>3 order by id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["status"].ToString() == "2")
                {
                    payStatus = 0;
                }
                else
                {
                    payStatus = 1;
                }
            }
            return payStatus;
        }
	public decimal ArrearAdvance(long property_id)
        {
            decimal ArrAdvance = 0;
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select top 1 * from tbl_adjustment_arrear where property_id=@property_id and [status]=1 and adj_type='ADVANCE'";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                ArrAdvance = Convert.ToDecimal(dt.Rows[0]["balance_amt"].ToString());
            }
            return ArrAdvance;
        }
        public decimal ArrearDue(long property_id)
        {
            decimal ArrDue = 0;
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select top 1 * from tbl_adjustment_arrear where property_id=@property_id and [status]=1 and adj_type='ARREAR'";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                ArrDue = Convert.ToDecimal(dt.Rows[0]["balance_amt"].ToString());
            }
            return ArrDue;
        }
        public DataTable SearchTaxDue(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select top 1 pr.transaction_id,pr.total_dues, cd.fin_year from tbl_payment_receipt as pr" +
                " inner join tbl_collection_detail as cd on cd.transaction_id=pr.transaction_id" +
                " where pr.property_id=@property_id order by cd.id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable PaymentHistory(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select receipt_no,payment_mode,payment_date,amount from tbl_transaction_detail where property_id=@property_id and status=1 order by id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public decimal WaterTax(long property_no,int WType)
        {
            decimal Water_Tax_Value = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_no));
            q = "select top 1 paid_status from tbl_water_tax_detail where property_id=@property_id order by id desc";
            dac = new DataAccessLayer();
            int Wstatus = Convert.ToInt32(dac.Scalar(q, param));
            
            if (Wstatus == 1)
            {
                Water_Tax_Value = 0;
                
            }
            else
            {
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@id", property_no));
                q = "SELECT rate FROM tbl_water_tax_type where id=(select water_tax_id from tbl_property_detail where id=@id)";
                dac = new DataAccessLayer();
                Water_Tax_Value = Convert.ToDecimal(dac.Scalar(q, param));
            }
            
            return Water_Tax_Value;
        }
        public decimal WaterTaxFirstValue(long property_no)
        {
            decimal Water_Tax_Value = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", property_no));
            q = "SELECT rate FROM tbl_water_tax_type where id=(select water_tax_id from tbl_property_detail where id=@id)";
            dac = new DataAccessLayer();
            Water_Tax_Value = Convert.ToDecimal(dac.Scalar(q, param));
            
            return Water_Tax_Value;
        }
    }
}
