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
    public class VaccandLandTaxController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public VaccandLandTaxController()
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
        public DataTable GetVaccandLandTax()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id, street_type_id, rate, effect_date from tbl_vacant_land_tax order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public decimal GetVaccandLandTax(int streetTypeID, string FY)
        {
            DateTime EffectDate = new DateTime();
            EffectDate = Convert.ToDateTime(("01-Apr-" + FY.Split('-')[0]).ToString());

            decimal rate = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@effect_date", EffectDate));
            param.Add(new SqlParameter("@street_type_id", streetTypeID));

            q = "select isnull((select rate from tbl_vacant_land_tax where effect_date>=(select top 1 effect_date from tbl_vacant_land_tax) and effect_date<=@effect_date and street_type_id=@street_type_id), (select rate from tbl_vacant_land_tax where street_type_id=@street_type_id)) as rate from tbl_vacant_land_tax where street_type_id=@street_type_id";
            dac = new DataAccessLayer();
            rate = Convert.ToDecimal(dac.Scalar(q, param));
            return rate;
        }
        public int GetVaccandLandTaxRateID(int streetTypeID, string FY)
        {
            DateTime EffectDate = new DateTime();
            EffectDate = Convert.ToDateTime(("01-Apr-" + FY.Split('-')[0]).ToString());

            int rateid = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@effect_date", EffectDate));
            param.Add(new SqlParameter("@street_type_id", streetTypeID));

            q = "select isnull((select id from tbl_vacant_land_tax where effect_date>=(select top 1 effect_date from tbl_vacant_land_tax) and effect_date<=@effect_date and street_type_id=@street_type_id), (select rate from tbl_vacant_land_tax where street_type_id=@street_type_id)) as rate from tbl_vacant_land_tax where street_type_id=@street_type_id";
            dac = new DataAccessLayer();
            rateid = Convert.ToInt32(dac.Scalar(q, param));
            return rateid;
        }

    }
}
