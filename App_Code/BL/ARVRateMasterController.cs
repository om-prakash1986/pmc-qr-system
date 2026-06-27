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
    /// Summary description for FloorMaster
    /// </summary>
    public class ARVRateMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public ARVRateMasterController()
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
        public DataTable GetARVRate()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,effect_date from tbl_arv_rate_master where status=1 order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public int GetARVRate(string FY)
        {
            DateTime EffectDate = new DateTime();
            EffectDate = Convert.ToDateTime(("01-Apr-" + FY.Split('-')[0]).ToString());

            int arvRateMasterID = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@effect_date", EffectDate));
            q = "select top 1 id,effect_date from tbl_arv_rate_master where status=1 and effect_date<=@effect_date order by id DESC";
            dac = new DataAccessLayer();
            arvRateMasterID = Convert.ToInt32(dac.Scalar(q, param));
            return arvRateMasterID;
        }
        public DateTime GetARVRateEffectDate()
        {
            DateTime effectiveDate = new DateTime();
            string q = "";
            param = new List<SqlParameter>();
            q = "select effecr_date from tbl_arv_rate_master where status=1 order by id ASC";
            dac = new DataAccessLayer();
            effectiveDate = Convert.ToDateTime(dac.Scalar(q, param));
            return effectiveDate;
        }

    }
}
