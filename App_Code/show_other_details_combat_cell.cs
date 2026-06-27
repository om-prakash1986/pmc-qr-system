using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for show_other_details_combat_cell
/// </summary>
public class show_other_details_combat_cell
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Find category of complain
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string find_category_of_complain(string category_id)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from combatcategory where id=@id", con);
            cmd.Parameters.AddWithValue("@id", category_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["combatcatagory"].ToString();
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
        return name;
    }

    /************************************************************
    * Purpose  ::  Find aub category of complain
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public DataTable find_sub_category_of_complain(string sub_category_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from CombatCellSubcategory where id=@id", con);
            cmd.Parameters.AddWithValue("@id", sub_category_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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
        return dt;
    }

    /************************************************************
    * Purpose  ::  Find aub category of complain
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public string find_mode_of_complain(string mode)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from CombatCellMode where id=@id", con);
            cmd.Parameters.AddWithValue("@id", mode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["Mode"].ToString();
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
        return name;
    }

    /************************************************************
    * Purpose  ::  Find time Current Status
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public string find_current_status(string complain_no)
    {
        string name = "";

        return name;
    }
}