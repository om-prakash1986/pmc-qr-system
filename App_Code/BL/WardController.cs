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
    /// Summary description for WardController
    /// </summary>
    public class WardController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public WardController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public DataTable GetWard(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,ward_no from tbl_ward_master where status=1 and id=@id order by ward_no ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetWard(string WardNo)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ward_no", WardNo));
            q = "select id, ward_no from tbl_ward_master where status=1 and ward_no=@ward_no order by ward_no ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetWardByCircle(int Circleid)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@circle_id", Circleid));
            q = "select id, ward_no from tbl_ward_master where status=1 and circle_id=@circle_id order by ward_no ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
