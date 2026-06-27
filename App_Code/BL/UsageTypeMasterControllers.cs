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
    /// Summary description for UsageTypeMaster
    /// </summary>
    public class UsageTypeMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public UsageTypeMasterController()
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
        public DataTable GetUsageTypeMaster()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id, usage_type, use_type_id, gov_multipication_factor from tbl_usage_type_master where status=1 order by ID ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetUsageTypeMaster(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id, usage_type, use_type_id, gov_multipication_factor from tbl_usage_type_master where status=1 and id=@id order by usage_type ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public decimal GetUsageTypeMasterByUseTypeID(string usageType, int useTypeID)
        {
            decimal gov_multipication_factor = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@use_type_id", useTypeID));
            q = "select id, usage_type, use_type_id, gov_multipication_factor from tbl_usage_type_master where status=1 and use_type_id=@use_type_id order by usage_type ASC";
            dac = new DataAccessLayer();
            gov_multipication_factor = Convert.ToDecimal(dac.Scalar(q, param));
            return gov_multipication_factor;
        }
        public DataTable GetUsageTypeMasterByUseTypeID(int useTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@use_type_id", useTypeID));
            q = "select id, usage_type, use_type_id, gov_multipication_factor from tbl_usage_type_master where status=1 and use_type_id=@use_type_id order by usage_type ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        
    }
}
