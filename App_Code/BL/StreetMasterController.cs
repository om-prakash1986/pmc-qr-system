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
    /// Summary description for StreetMasterController
    /// </summary>
    public class StreetMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public StreetMasterController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public DataTable GetStreet(int street_type_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id, street_name from tbl_street_master where street_type_id=@street_type_id and status=1 order by street_name ASC";

            param.Add(new SqlParameter("@street_type_id", street_type_id));
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetOneStreet(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id, street_type_id, street_name, street_code from tbl_street_master where status=1 and id=@id order by street_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetStreet(string StreetName)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@street_name", StreetName));
            q = "select id, street_type_id, street_name, street_code from tbl_street_master where status=1 and street_name=@street_name order by street_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetStreetByType(int StreetTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@street_type_id", StreetTypeID));
            q = "select id, street_type_id, street_name, street_code from tbl_street_master where status=1 and street_type_id=@street_type_id order by street_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetAllStreet()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();

            q = "SELECT tbl_street_type_master.ID as StreetTypeId,tbl_street_master.ID as StreetID,tbl_street_master.street_name, tbl_street_type_master.street_type, tbl_street_master.detail FROM   tbl_street_master INNER JOIN tbl_street_type_master ON tbl_street_master.street_type_id = tbl_street_type_master.id where tbl_street_master.status=1 order by street_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
