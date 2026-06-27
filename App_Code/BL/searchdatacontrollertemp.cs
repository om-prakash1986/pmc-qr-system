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
    /// Summary description for searchdatacontrollertemp
    /// </summary>
    public class searchdatacontrollertemp
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public searchdatacontrollertemp()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public DataTable SearchPropertyTemp(long sasid)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", sasid));
            q = "select property.id, property.ulb_id,property.uid,property.building_usage_id, property.application_no, property.ward_id,property.revenue_circle_id, " +
                "property.assessment_year as last_assessment_year,property.last_payment_receipt_no,property.last_payment_date,property.last_payment_amount,property.last_payment_year,property.PID, property.old_pid, property.new_holding_no, property.old_holding_no, property.co_mauja_name,property.co_circle_name," +
                "property.khata_no, property.plot_no,property.street_id, property.plot_area,property.constructed_area, property.property_type_id,property.construction_date as acquisition," +
                "property.water_harvesting,wft.facility_name,property.mutation_type,property.last_payment_quater, wtt.tax_type,wtt.rate, property.house_no, property.muhalla, property.street_name,property.street_type_id,property.[address],property.pin,property.cr_house_no, property.cr_muhalla, property.cr_street_name,property.cr_address,property.cr_pin," +
                "property.telephone_no as telephone_no, property.mobile_no,property.email_id,property.form_no,property.ro_ordersheet,property.indira_awaas,property.parent_property_id, property.ownership_type_id,property.status,sm.id as road_id,sm.street_name as road, ward.circle_id,circle.circle_name, ward.ward_no, isnull(revCirmast.rev_circle,'') as rev_circle," +
                "isnull(ptm.property_type,'')as property_type,pdot.building_name, isnull(stm.street_type,'')as road_type, isnull(osm.ownership_type,'') as ownership_type" +
                //"owd.owner_name, owd.guardian_name, owd.relation, owd.mobile_no 
                " from tbl_property_detail_tmp as property" +
                " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                " left join tbl_revenue_circle_master as revCirmast on revCirmast.id=property.revenue_circle_id" +
                " inner join tbl_property_type_master as ptm on ptm.id=property.property_type_id" +
                " inner join tbl_street_type_master as stm on stm.id=property.street_type_id" +
                " inner join tbl_ownership_type_master as osm on osm.id=property.ownership_type_id" +
                " inner join tbl_water_facility_type as wft on wft.id=property.water_facility_id" +
                " inner join tbl_water_tax_type as wtt on wtt.id=property.water_tax_id" +
                " left join tbl_street_master as sm on sm.id=property.street_id" +
                " left join tbl_property_detail_others_tmp as pdot on pdot.property_id=property.id" +
                //" inner join tbl_owner_detail as owd on owd.property_id=property.id" +
                " where property.id=@id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable SearchPropertyTempByMobile(string mobile)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@mobile_no", mobile));
            q = "select property.id, property.ulb_id,property.uid,property.building_usage_id, property.application_no, property.ward_id,property.revenue_circle_id, " +
                "property.assessment_year as last_assessment_year,property.last_payment_receipt_no,property.last_payment_date,property.last_payment_amount,property.last_payment_year,property.PID, property.old_pid, property.new_holding_no, property.old_holding_no, property.co_mauja_name,property.co_circle_name," +
                "property.khata_no, property.plot_no,property.street_id, property.plot_area,property.constructed_area, property.property_type_id,property.construction_date as acquisition," +
                "property.water_harvesting,wft.facility_name,property.mutation_type,property.last_payment_quater, wtt.tax_type,wtt.rate, property.house_no, property.muhalla, property.street_name,property.street_type_id,property.[address],property.pin,property.cr_house_no, property.cr_muhalla, property.cr_street_name,property.cr_address,property.cr_pin," +
                "property.telephone_no as telephone_no, property.mobile_no,property.email_id,property.form_no,property.ro_ordersheet,property.indira_awaas,property.parent_property_id, property.ownership_type_id,property.status,sm.id as road_id,sm.street_name as road, ward.circle_id,circle.circle_name, ward.ward_no, isnull(revCirmast.rev_circle,'') as rev_circle," +
                "isnull(ptm.property_type,'')as property_type,pdot.building_name, isnull(stm.street_type,'')as road_type, isnull(osm.ownership_type,'') as ownership_type,owd.owner_name" +
                //"owd.owner_name, owd.guardian_name, owd.relation, owd.mobile_no 
                " from tbl_property_detail_tmp as property" +
                " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                " left join tbl_revenue_circle_master as revCirmast on revCirmast.id=property.revenue_circle_id" +
                " inner join tbl_property_type_master as ptm on ptm.id=property.property_type_id" +
                " inner join tbl_street_type_master as stm on stm.id=property.street_type_id" +
                " inner join tbl_ownership_type_master as osm on osm.id=property.ownership_type_id" +
                " inner join (select property_id,STRING_AGG(owner_name,', ') as owner_name from tbl_owner_detail_tmp group by property_id) as owd on owd.property_id=property.id" +
                " inner join tbl_water_facility_type as wft on wft.id=property.water_facility_id" +
                " inner join tbl_water_tax_type as wtt on wtt.id=property.water_tax_id" +
                " left join tbl_street_master as sm on sm.id=property.street_id" +
                " left join tbl_property_detail_others_tmp as pdot on pdot.property_id=property.id" +
                //" inner join tbl_owner_detail as owd on owd.property_id=property.id" +
                " where property.mobile_no=@mobile_no and applied_from='NOLP' order by property.id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }

        public DataTable SearchOccupancyDetailsTemp(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select distinct ocd.floor_id, fl.floor_no as Floor_No,ocd.builtup_area as Built_Up_Area,ocd.from_year as Assesment_From_Year,ocd.upto_year as Assesment_Upto_Year,ustm.id as Type_Of_Use_ID,ustm.use_type as Type_Of_Use," +
                                " utm.id as Usage_Factor_ID,utm.usage_type as Usage_Factor, otm.id as Occupancy_Type_ID,otm.occupancy_type as Occupancy_Type, ctm.id as Construction_Type_ID,ctm.contruction_type as Construction_Type from tbl_occupancy_detail_tmp as ocd" +
                                " inner join tbl_floor_master as fl on fl.id=ocd.floor_id" +
                                " inner join tbl_usage_type_master as utm on utm.id=ocd.usage_type_id" +
                                " inner join tbl_occupancy_type_master as otm on otm.id=ocd.occupancy_type_id" +
                                " inner join tbl_construction_type_master as ctm on ctm.id=ocd.construction_type_id" +
                                " inner join tbl_use_type_master as ustm on ustm.id=ocd.use_type_id" +
                                " where property_id=@property_id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public void updateOccupancyToYear(long property_id,string upto_year)
        {
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@upto_year", upto_year));
            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@assessment_year", upto_year));
            param.Add(new SqlParameter("@id", property_id));
            q = "update tbl_occupancy_detail_tmp set upto_year=@upto_year,entry_date=GETDATE() where property_id=@property_id;update tbl_property_detail_tmp set assessment_year=@assessment_year,entry_date=GETDATE() where id=@id";
            dac = new DataAccessLayer();
            dac.update(q, param);
        }
        public DataTable SearchOwnerDetailsTemp(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select * from tbl_owner_detail_tmp where property_id=@property_id order by id Asc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public string ownerIdentity(long ownid)
        {
            string path = "";
            string q = "";
            param = new List<SqlParameter>();

            param.Add(new SqlParameter("@owner_id", ownid));
            q = "select doc_name from tbl_prop_document_details_tmp where owner_id=@owner_id";
            dac = new DataAccessLayer();
            path = Convert.ToString(dac.Scalar(q, param));
            return path;
        }
        public string document(long property_id,int doc_master_id,int doc_id)
        {
            string path = "";
            string q = "";
            param = new List<SqlParameter>();

            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@document_master_id", doc_master_id));
            param.Add(new SqlParameter("@document_detail_id", doc_id));
            q = "select doc_name from tbl_prop_document_details_tmp where property_id=@property_id and document_master_id=@document_master_id and document_detail_id=@document_detail_id";
            dac = new DataAccessLayer();
            path = Convert.ToString(dac.Scalar(q, param));
            return path;
        }
        public int holidays(DateTime frmDate,DateTime toDate)
        {
            int days = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@frm_Date", frmDate));
            param.Add(new SqlParameter("@to_date", toDate));
            q = "select count(distinct [date]) from tbl_holiday_master where [date]>@frm_Date and date<@to_date";
            dac = new DataAccessLayer();
            days = Convert.ToInt32(dac.Scalar(q, param));
            return days;
        }
    }
}