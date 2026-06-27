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
    /// Summary description for WaterharvestingRebateMaster
    /// </summary>
    public class WaterharvestingRebateMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public WaterharvestingRebateMasterController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static int Insert()
        {
            int ID = 0;

            try
            {


            }
            catch (Exception ex) { string mes = ex.Message.ToString(); }
            return ID;
        }
        public DataTable GetWaterharvestingRebat()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,calculation_type, rate, effect_from  from tbl_waterharvesting_rebate_master where status=1 order by facility_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetWaterharvestingRebat(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,calculation_type, rate, effect_from  from tbl_waterharvesting_rebate_master where status=1 and id=@id order by facility_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public decimal GetWaterharvestingRebate(string AssessmentYear)
        {
            decimal rebatePercentage = 0;
            DateTime assDate = new DateTime();
            
            if(!string.IsNullOrEmpty(AssessmentYear))
            {
                assDate = Convert.ToDateTime("01-Apr-"+(AssessmentYear.Substring(1, 4)).ToString());
                string q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@effect_from", assDate));
                q = "select rate, effect_from  from tbl_waterharvesting_rebate_master where status=1 and effect_from=@effect_from";
                dac = new DataAccessLayer();
                rebatePercentage = Convert.ToDecimal(dac.Scalar(q, param));                
            }
            return rebatePercentage;
        }
        
    }
}
