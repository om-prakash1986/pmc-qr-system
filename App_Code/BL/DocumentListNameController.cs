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
    public class DocumentListNameController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public DocumentListNameController()
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
        public DataTable GetDocumentList()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,license_dtls_id, doc_name from tbl_document_list_name where status=1 order by id Desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetDocumentList(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,license_dtls_id, doc_name from tbl_document_list_name where status=1 and id=@id order by id DESC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetDocumentListByLicense(int LicenseDetailID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@license_dtls_id", LicenseDetailID));
            q = "select id,license_dtls_id, doc_name from tbl_document_list_name where status=1 and license_dtls_id=@license_dtls_id order by id ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
