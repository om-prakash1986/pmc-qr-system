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
    public class FloorMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public FloorMasterController()
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
        public DataTable GetFloorMaster()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,floor_no,level from tbl_floor_master where status=1 order by ID ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;            
        }
        public DataTable GetFloorMaster(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,floor_no,level from tbl_floor_master where status=1 and id=@id order by ID ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;            
        }
        public DataTable GetFloorMaster(string FloorName)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@floor_no", FloorName));
            q = "select id,floor_no,level from tbl_floor_master where status=1 and floor_no=@floor_no order by ID ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt; 
        }
        
    }
}
