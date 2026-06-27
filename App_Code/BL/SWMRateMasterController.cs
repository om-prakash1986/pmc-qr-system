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
/// <summary>
/// Summary description for SWMRateMasterController
/// </summary>
public class SWMRateMasterController
{
    DataTable dt;
    List<SqlParameter> param;
    DataAccessLayer dac;
    public SWMRateMasterController()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public DataTable GetRangeMaster(int id)
    {
        dt = new DataTable();
        string q = "";
        param = new List<SqlParameter>();
        q = "select id, range_name from tbl_waste_range_master where status=1 and category_mstr_id =@category_mstr_id  order by ID ASC";
        dac = new DataAccessLayer();
        param.Add(new SqlParameter("@category_mstr_id",id));
        dt = dac.GetDataTable(q, param);
        return dt;
    }
}