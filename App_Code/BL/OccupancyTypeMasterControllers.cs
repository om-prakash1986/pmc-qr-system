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
    public class OccupancyTypeMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public OccupancyTypeMasterController()
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
        public DataTable GetOccupancyTypeMaster()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,occupancy_type from tbl_occupancy_type_master where status=1 order by occupancy_type ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetOccupancyTypeMaster(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,occupancy_type from tbl_occupancy_type_master where status=1 and id=@id order by occupancy_type ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetOccupancyTypeMaster(string OccupancyType)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@occupancy_type", OccupancyType));
            q = "select id, occupancy_type from tbl_occupancy_type_master where status=1 and occupancy_type=@occupancy_type order by occupancy_type ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        
    }
}
