using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for counter
/// </summary>
public class counter
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Main Menu Controller
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string select_counter()
    {
        string count = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,count from counter", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    count = dt.Rows[i]["count"].ToString();
                    i++;
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return count;
    }

    public int update_counter()
    {
        int number=0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update counter set count=count+1", con);
            cmd1.ExecuteNonQuery();
            con.Close();
            number = 1;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return number;
    }
}