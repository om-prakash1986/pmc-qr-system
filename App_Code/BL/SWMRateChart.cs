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
/// Summary description for SWMRateChart
/// </summary>
public class SWMRateChart
{
    DataTable dt;
    List<SqlParameter> param;
    DataAccessLayer dac;
    public SWMRateChart()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public DataTable GetRateChart()
    {
        dt = new DataTable();
        string q = "";
        param = new List<SqlParameter>();
        q = "select id,range_mstr_id,amount from tbl_waste_rate_chart where status=1 order by ID ASC";
        dac = new DataAccessLayer();
        dt = dac.GetDataTable(q, param);
        return dt;
    }
    public DataTable GetRateChart(int id)
    {
        dt = new DataTable();
        string q = "";
        param = new List<SqlParameter>();
        param.Add(new SqlParameter("@range_mstr_id", id));
        q = "select id,range_mstr_id,amount from tbl_waste_rate_chart where status=1 and range_mstr_id =@range_mstr_id order by ID ASC";
        dac = new DataAccessLayer();
        dt = dac.GetDataTable(q, param);
        return dt;
    }
}