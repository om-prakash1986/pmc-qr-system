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
    /// Summary description for StreetTypeController
    /// </summary>
    public class StreetTypeMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public StreetTypeMasterController()
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
        public DataTable GetStreetType()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,street_type from tbl_street_type_master where status=1 order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetStreetType(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,street_type from tbl_street_type_master where status=1 and id=@id order by street_type ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetStreetType(string StreetType)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@street_type", StreetType));
            q = "select id,street_type from tbl_street_type_master where status=1 and street_type=@street_type order by street_type ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
