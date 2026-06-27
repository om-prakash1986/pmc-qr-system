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
    /// Summary description for TaxRateMaster
    /// </summary>
    public class TaxRateMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public TaxRateMasterController()
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
        public DataTable GetTaxRate()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id, arv_master_id, holding_tax, water_tax, latrine_tax, education_cess, health_cess  from GetTaxRate where status=1 order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetTaxRate(int arvMasterID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@arv_master_id", arvMasterID));
            q = "select id, arv_master_id, holding_tax, water_tax, latrine_tax, education_cess, health_cess from tbl_tax_rate_master  where status=1 and arv_master_id=@arv_master_id order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }       
        
    }
}
