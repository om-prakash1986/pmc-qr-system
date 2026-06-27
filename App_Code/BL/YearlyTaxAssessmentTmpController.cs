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
    /// Summary description for YearlyTaxAssessmentTmp
    /// </summary>
    public class YearlyTaxAssessmentTmpController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        DateTime frmDate;
        DateTime toDate;
        public YearlyTaxAssessmentTmpController()
        {
            //
            // TODO: Add constructor logic here
            //        
            frmDate = Convert.ToDateTime("01-" + System.DateTime.Now.ToString("dd-MMM-yyyy").Split('-')[1] + "-" + System.DateTime.Now.Year);
            toDate = System.DateTime.Now;
        }
        public void DeleteBeforeInsert(long property_id)
        {
            string q = "";

            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@FromDate", frmDate));
            param.Add(new SqlParameter("@ToDate", toDate));
            q = "select id from tbl_yearly_tax_assessment_tmp where property_id=@property_id and entry_date>=@FromDate and entry_date<=@ToDate";
            DataTable dtYearlyAssesmentID = new DataTable();
            dac = new DataAccessLayer();
            dtYearlyAssesmentID=dac.GetDataTable(q, param);
            string YearID = "";
            

            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@FromDate", frmDate));
            param.Add(new SqlParameter("@ToDate", toDate));
            param.Add(new SqlParameter("@yearly_taxes_id", toDate));
            
            q = "delete from tbl_yearly_vacant_land_assessment_tmp where property_id=@property_id and entry_date>=@FromDate and entry_date<=@ToDate; ";
            for (int i = 0; i < dtYearlyAssesmentID.Rows.Count; i++)
            {
                YearID = dtYearlyAssesmentID.Rows[i]["id"].ToString();
                q += "delete from tbl_yearly_floor_assessment_tmp where yearly_taxes_id="+YearID+"; ";
                q += "delete from tbl_assesment_rebate_penalty_tmp where yearly_tax_id=" + YearID + "; ";
            }
            q += "delete from tbl_water_tax_detail_tmp where property_id=@property_id and entry_date>=@FromDate and entry_date<=@ToDate; ";
            q += "delete from tbl_yearly_tax_assessment_tmp where property_id=@property_id and entry_date>=@FromDate and entry_date<=@ToDate; ";
            
            dac.Scalar(q, param);
        }
        public long InsertYearlyAssessment(long property_id, string fin_year, decimal total_fy_tax, decimal vacant_tax, decimal vacant_rate, int vacant_rate_id, decimal total_tax)
        {
            long rowEffected = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            //param.Add(new SqlParameter("@ulb_id", 1));
            param.Add(new SqlParameter("@fin_year", fin_year));
            param.Add(new SqlParameter("@total_fy_tax", total_fy_tax));
            param.Add(new SqlParameter("@vacant_tax", vacant_tax));
            param.Add(new SqlParameter("@vacant_rate", vacant_rate));
            param.Add(new SqlParameter("@vacant_rate_id", vacant_rate_id));
            param.Add(new SqlParameter("@total_tax", total_tax));
           // param.Add(new SqlParameter("@entry_date", System.DateTime.Now));
            //param.Add(new SqlParameter("@user_id", 2));//2
            //param.Add(new SqlParameter("@paid_status", 0));//0
            param.Add(new SqlParameter("@balance_amount", total_tax));//0
            //param.Add(new SqlParameter("@demand_type", demand_type));

            q = "insert into tbl_yearly_tax_assessment_tmp (property_id, 	ulb_id, 	fin_year, 	total_fy_tax, 	vacant_tax, 	vacant_rate, 	vacant_rate_id, 	total_tax, 	entry_date, 	user_id, 	paid_status,balance_amount) Values	(@property_id, 	1, 	@fin_year, 	@total_fy_tax, 	@vacant_tax, 	@vacant_rate, 	@vacant_rate_id, 	@total_tax, 	GETDATE(), 	2, 	0,@balance_amount); select @@IDENTITY";
            dac = new DataAccessLayer();
            rowEffected = Convert.ToInt64(dac.Scalar(q, param));
            return rowEffected;
        }
        public int InsertYearlyVacantAssessment(long property_id, decimal area_decimal, decimal area_sqft, decimal area_sqmtr, int vacant_rate_id, decimal tax, string effect_from)
        {
            int rowEffected = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@area_decimal", area_decimal));
            param.Add(new SqlParameter("@area_sqft", area_sqft));
            param.Add(new SqlParameter("@area_sqmtr", area_sqmtr));
            param.Add(new SqlParameter("@vacant_rate_id", vacant_rate_id));
            
            param.Add(new SqlParameter("@tax", tax));
            param.Add(new SqlParameter("@effect_from", effect_from));
            
            q = "insert into tbl_yearly_vacant_land_assessment_tmp ([property_id],[type],[area_decimal],[area_sqft],[area_sqmtr],[vacant_rate_id]";
            q += ",[tax],[effect_from],[user_id],[entry_date],[status]) ";
            q += " Values	(@property_id,'Vacant Land',@area_decimal,@area_sqft,@area_sqmtr,@vacant_rate_id,";
            q += " @tax,@effect_from,2,GETDATE(),1)";
            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return rowEffected;
        }
        public int InsertYearlyFloorAssessment(long yearly_taxes_id, int floor_id, int usage_type_id, int occupancy_type_id, int construction_type_id, decimal builtup_area,
                                              decimal carpet_area, string use_type, decimal occupancy_factor, decimal arv, decimal total_tax_factor, decimal total_tax, decimal arv_rate, int occupancy_type_detail_id)
        {
            int rowEffected = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@yearly_taxes_id", yearly_taxes_id));
            param.Add(new SqlParameter("@floor_id", floor_id));
            param.Add(new SqlParameter("@usage_type_id", usage_type_id));
            param.Add(new SqlParameter("@occupancy_type_id", occupancy_type_id));
            param.Add(new SqlParameter("@construction_type_id", construction_type_id));
            param.Add(new SqlParameter("@builtup_area", builtup_area));
            param.Add(new SqlParameter("@carpet_area", carpet_area));
            param.Add(new SqlParameter("@use_type", use_type));
            param.Add(new SqlParameter("@occupancy_factor", occupancy_factor));
            param.Add(new SqlParameter("@arv", arv));
            param.Add(new SqlParameter("@total_tax_factor", total_tax_factor));
            param.Add(new SqlParameter("@total_tax", total_tax));
            param.Add(new SqlParameter("@arv_rate", arv_rate));
            param.Add(new SqlParameter("@occupancy_type_detail_id", occupancy_type_detail_id));

            q = "INSERT INTO [dbo].[tbl_yearly_floor_assessment_tmp]([yearly_taxes_id],[floor_id],[usage_type_id],[occupancy_type_id],[construction_type_id]";
            q += ",[builtup_area],[carpet_area],[use_type],[occupancy_factor],[arv],[total_tax_factor],[total_tax],[arv_rate],[occupancy_type_detail_id]) ";
            q += " VALUES(@yearly_taxes_id,@floor_id,@usage_type_id,@occupancy_type_id,@construction_type_id,@builtup_area,@carpet_area,@use_type,@occupancy_factor,";
            q += " @arv,@total_tax_factor,@total_tax,@arv_rate,@occupancy_type_detail_id)";
            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return rowEffected;
        }
        public int InsertYearlyRebatePenaltyAssessment(long yearly_taxes_id, decimal penalty, decimal tax_with_penalty, decimal current_tax_rebate,
            decimal water_harvt_rebate, decimal total_rebate, decimal final_tax, long property_id)
        {
            int rowEffected = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@yearly_tax_id", yearly_taxes_id));
            param.Add(new SqlParameter("@penalty", penalty));
            param.Add(new SqlParameter("@tax_with_penalty", tax_with_penalty));
            param.Add(new SqlParameter("@current_tax_rebate", current_tax_rebate));
            param.Add(new SqlParameter("@water_harvt_rebate", water_harvt_rebate));
            param.Add(new SqlParameter("@total_rebate", total_rebate));
            param.Add(new SqlParameter("@final_tax", final_tax));
            param.Add(new SqlParameter("@property_id", property_id));

            q = "INSERT INTO [dbo].[tbl_assesment_rebate_penalty_tmp]([yearly_tax_id],[penalty],[tax_with_penalty],[current_tax_rebate],[water_harvt_rebate] ";
            q += ",[total_rebate],[final_tax],[property_id])  VALUES(@yearly_tax_id,@penalty,@tax_with_penalty,@current_tax_rebate,@water_harvt_rebate,@total_rebate,";
            q += "@final_tax,@property_id)";
            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return rowEffected;
        }
        public int InsertWaterTax(long property_id,decimal amount)
        {
            int rowEffected = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@amount", amount));
            q = "INSERT INTO [dbo].[tbl_water_tax_detail_tmp](property_id,[amount],entry_date,user_id,paid_status,[status]) ";
            q += "values(@property_id,@amount,GETDATE(),2,0,1)";
            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return rowEffected;
        }
    }
}
