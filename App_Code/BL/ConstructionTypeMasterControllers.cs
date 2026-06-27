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
    public class ConstructionTypeMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public ConstructionTypeMasterController()
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
        public DataTable GetConstructionType()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,contruction_type from tbl_construction_type_master where status=1 order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetConstructionType(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,contruction_type from tbl_construction_type_master where status=1 and id=@id order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetConstructionType(string ConstructionType)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@contruction_type", ConstructionType));
            q = "select id,contruction_type from tbl_construction_type_master where status=1 and contruction_type=@contruction_type order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }

    }
}

