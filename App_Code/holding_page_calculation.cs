using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for holding_page_calculation
/// </summary>
public class holding_page_calculation
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  All Properties of user
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string all_properties_of_user(string user_id)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(PID) as number from property_details where added_by=@id", con);
            cmd.Parameters.AddWithValue("@id", user_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["number"].ToString();
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


    /**************************************************************
     * Till Here
     * ************************************************************/

}