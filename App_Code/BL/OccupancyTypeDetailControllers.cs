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
    /// Summary description for OccupancyTypeMaster
    /// </summary>
    public class OccupancyTypeDetailController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public OccupancyTypeDetailController()
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
        public DataTable GetOccupancyTypeDetail()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,occupancy_type_id, multipication_factor, effect_date from tbl_occupancy_type_detail order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetOccupancyTypeDetail(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,occupancy_type_id, multipication_factor, effect_date from tbl_occupancy_type_detail where id=@id order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public decimal GetOccupancyTypeDetail(int OccupancyTypeID, string AssessmentYear)
        {
            DateTime EffectDate = new DateTime();
            EffectDate = Convert.ToDateTime(("01-Apr-"+AssessmentYear.Split('-')[0]).ToString());
            decimal multiplyingFactor = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@effect_date", EffectDate));
            param.Add(new SqlParameter("@occupancy_type_id", OccupancyTypeID));
            q = "select top 1 multipication_factor from tbl_occupancy_type_detail where effect_date>=(select top 1 effect_date from tbl_occupancy_type_detail)  and effect_date<=@effect_date and occupancy_type_id=@occupancy_type_id order by effect_date desc";
            dac = new DataAccessLayer();
            multiplyingFactor = Convert.ToDecimal(dac.Scalar(q, param));
            return multiplyingFactor;
        }
        public int GetOccupancyTypeDetailID(int OccupancyTypeID, string AssessmentYear)
        {
            DateTime EffectDate = new DateTime();
            EffectDate = Convert.ToDateTime(("01-Apr-" + AssessmentYear.Split('-')[0]).ToString());
            int Occup_Type_Detail_ID = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@effect_date", EffectDate));
            param.Add(new SqlParameter("@occupancy_type_id", OccupancyTypeID));
            q = "select top 1 id from tbl_occupancy_type_detail where effect_date>=(select top 1 effect_date from tbl_occupancy_type_detail)  and effect_date<=@effect_date and occupancy_type_id=@occupancy_type_id order by effect_date desc";
            dac = new DataAccessLayer();
            Occup_Type_Detail_ID = Convert.ToInt32(dac.Scalar(q, param));
            return Occup_Type_Detail_ID;
        }
        
    }
}
