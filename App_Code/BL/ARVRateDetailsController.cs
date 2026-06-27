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
    /// <summary>
    /// Summary description for ARVRateDetails
    /// </summary>
    public class ARVRateDetailsController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public ARVRateDetailsController()
        {
            //
            // TODO: Add constructor logic here
            //        
        }
        public DataTable GetARVRateDetail()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,arv_master_id, street_type_id, use_type_id, construction_type_id, rate  from tbl_arv_rate_detail where status=1 order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetARVRateDetail(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,arv_master_id, street_type_id, use_type_id, construction_type_id, rate  from tbl_arv_rate_detail where status=1 and id=@id order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public decimal GetARVRateDetail(int arvMasterID, int streetTypeID, int useTypeID, int consTypeID)
        {
            decimal rate = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@arv_master_id", arvMasterID));
            param.Add(new SqlParameter("@street_type_id", streetTypeID));
            param.Add(new SqlParameter("@use_type_id", useTypeID));
            param.Add(new SqlParameter("@construction_type_id", consTypeID));
            q = "select rate  from tbl_arv_rate_detail where status=1 and arv_master_id=@arv_master_id and street_type_id=@street_type_id and use_type_id=@use_type_id and construction_type_id=@construction_type_id order by id ASC";
            dac = new DataAccessLayer();
            rate = Convert.ToDecimal(dac.Scalar(q, param));
            return rate;
        }

    }
}
