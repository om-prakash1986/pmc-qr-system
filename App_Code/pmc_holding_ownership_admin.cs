using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for pmc_holding_ownership_admin
/// </summary>
public class pmc_holding_ownership_admin
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Add Property Details Part 1
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public int pmc_holding_property_details_admin(string unique_no, string owner_ship_type, string property_type, string water_harvest)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details_admin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID_admin"].ToString());
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@owner_ship_type", owner_ship_type);
            cmd.Parameters.AddWithValue("@property_type", property_type);
            cmd.Parameters.AddWithValue("@water_harvest", water_harvest);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                int auth = Convert.ToInt32(cmd.ExecuteScalar());
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
        }
        return number;
    }


    /**********************************************************8
     * Till Here
     * ********************************************************/
}