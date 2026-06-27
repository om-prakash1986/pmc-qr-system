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
    public class AssessmentApprovalController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public AssessmentApprovalController()
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
        public DataTable GetAssessmentApproval()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select *  from tbl_fy where status=1 order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetAssessmentApproval(int tmpPropertyID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@tmp_property_id", tmpPropertyID));
            q = "select * from tbl_fy where status=1 and tmp_property_id=@tmp_property_id order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetAssessmentApprovalByAppTypeID(int applicationTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@tmp_property_id", applicationTypeID));
            q = "select * from tbl_fy where status=1 and application_type=@application_type order by ID ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetAssessmentApprovalAppStatusByApplication(int applicationTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@application_type", applicationTypeID));
            q = "select * from tbl_fy where status=1 and application_type=@application_type order by ID ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetAssessmentApprovalAppStatusByProperty(int tmpPropertyID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@tmp_property_id", tmpPropertyID));
            q = "select * from tbl_fy where status=1 and tmp_property_id=@tmp_property_id order by ID ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
