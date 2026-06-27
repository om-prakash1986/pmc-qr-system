using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Create by Gyan Chand Verma
/// Dated 15/01/2021:: 10:55 PM
/// </summary>
public class NewTemplate
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable BindNotice()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select top 10* from order_notice where (value=1 or value=7) and status='active' order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
}