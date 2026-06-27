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
    public class DocumentMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public DocumentMasterController()
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
        public DataTable GetDocumentMaster()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id,doc_name, from tbl_document_master where status=1 order by doc_name Desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetDocumentMaster(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id,doc_name from tbl_document_master where status=1 and id=@id order by doc_name DESC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetDocumentMaster(int applicationType, int propertyTypeID, int ownershipTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@application_type", applicationType));
            param.Add(new SqlParameter("@property_type_id", propertyTypeID));
            param.Add(new SqlParameter("@ownership_type_id", ownershipTypeID));
            q = "select id,doc_name from tbl_document_master where status=1 and application_type=@application_type and property_type_id=@property_type_id and ownership_type_id=@ownership_type_id order by doc_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetDocumentMaster(int applicationType, int propertyTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@application_type", applicationType));
            param.Add(new SqlParameter("@property_type_id", propertyTypeID));
            q = "select id,doc_name from tbl_document_master where status=1 and application_type=@application_type and property_type_id=@property_type_id order by doc_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
