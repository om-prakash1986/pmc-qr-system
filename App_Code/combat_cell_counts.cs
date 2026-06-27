using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for combat_cell_counts
/// </summary>
public class combat_cell_counts
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  All Complaints In Combat Cell
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string count_all_pending_complaints_in_combat_cell()
    {
        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT count(id) as number from combatcelltitle where ComplainStatus='Active' and status='true'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                number = dt.Rows[0]["number"].ToString();
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
        return number;
    }

    /************************************************************
     * Purpose  ::  All Complaints Forwarded to SI
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string count_all_forwarded_complaints_in_combat_cell()
    {
        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT count(id) as number from combatcelltitle where ComplainStatus!='Active' and status='true'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                number = dt.Rows[0]["number"].ToString();
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
        return number;
    }


    /************************************************************
     * Till Here
     * **********************************************************/
}