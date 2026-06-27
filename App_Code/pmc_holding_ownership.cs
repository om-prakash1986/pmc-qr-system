using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for pmc_holding_ownership
/// </summary>
public class pmc_holding_ownership
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Add Ownership Details
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    // Updated by Gyan Chand Verma
    // date : 16 June 2019

    public int add_ownership_details(int unique_no, string _owner_name, string _relation_text, string _guardian_name, string _genderdropdown, string _pan_noo)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_owner_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@owner_name", _owner_name);
            cmd.Parameters.AddWithValue("@relation", _relation_text);
            cmd.Parameters.AddWithValue("@family_name", _guardian_name);
            cmd.Parameters.AddWithValue("@gender", _genderdropdown);
            cmd.Parameters.AddWithValue("@pan", _pan_noo);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now);
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

    /************************************************************
     * Purpose  ::  Add Property Details Part 1
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    // Updated by Gyan Chand Verma
        // date : 16 June 2019
    public int pmc_holding_property_detailsss(string unique_no, string _old_holding_no, string _new_holding_no, string _old_pid, string owner_ship_type, string property_type, string water_harvest)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID"].ToString());
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@old_holding_no", _old_holding_no);
            cmd.Parameters.AddWithValue("@new_holding_no", _new_holding_no);
            cmd.Parameters.AddWithValue("@old_pid", _old_pid);
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
    /************************************************************
    * Purpose  ::  Add Property Details Part 2
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public int pmc_holding_property_details_part_two(string unique_no, string circle, string ward,  string revenueCircle, string c_address, string c_pin, string p_address,string p_pin,string street_type,string acquision_date,string water_tax_type,string amount,string road_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details_part_two"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@circle", circle);
            cmd.Parameters.AddWithValue("@ward", ward);
            cmd.Parameters.AddWithValue("@revenuecircle", revenueCircle);
            cmd.Parameters.AddWithValue("@c_address", c_address);
            cmd.Parameters.AddWithValue("@c_pin", c_pin);
            cmd.Parameters.AddWithValue("@p_address", p_address);
            cmd.Parameters.AddWithValue("@p_pin", p_pin);
            cmd.Parameters.AddWithValue("@street_type", street_type);
            cmd.Parameters.AddWithValue("@acquision_date", acquision_date);
            cmd.Parameters.AddWithValue("@water_tax_type", water_tax_type);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@road_name", road_name);
            //cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

	public int update_ownership_details(int unique_no, string _owner_name, string _relation_text, string _guardian_name, string _genderdropdown, string _pan_noo)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_holding_owner_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@owner_name", _owner_name);
            cmd.Parameters.AddWithValue("@relation", _relation_text);
            cmd.Parameters.AddWithValue("@family_name", _guardian_name);
            cmd.Parameters.AddWithValue("@gender", _genderdropdown);
            cmd.Parameters.AddWithValue("@pan", _pan_noo);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

    /************************************************************
    * Purpose  ::  Update Floor Data
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ************************************************************/
    public int update_floor_data(string unique_no, string SAS_no, string total_area, string buildup_area)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details_update_floor_data"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@sas_no", SAS_no);
            cmd.Parameters.AddWithValue("@total_area", total_area);
            cmd.Parameters.AddWithValue("@buildup_area", buildup_area);
            //cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

   /************************************************************
   * Purpose  ::  Add Property Details Part 2 for House
   * Author   ::  Arnav
   * Date     ::  27-03-2017
   * ************************************************************/
     public int pmc_holding_property_details(string _circle, string _ward, string _ownershipType, string _corr_house, string _corr_street, string _corr, string _pincode, string _house_no, string _street_no, string _permanent, string _pincodee, string _propertyItem, string _acq_date, string _totalArea, string _builtupArea, string _roadType, string _streetType, string _water_harvest, string w_t_trype, string amount)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID"].ToString());
            cmd.Parameters.AddWithValue("@_circle", _circle);
            cmd.Parameters.AddWithValue("@_ward", _ward);
            cmd.Parameters.AddWithValue("@_ownershipType", _ownershipType);

            cmd.Parameters.AddWithValue("@_corr_house", _corr_house);
            cmd.Parameters.AddWithValue("@_corr_street", _corr_street);
            cmd.Parameters.AddWithValue("@_corr", _corr);
            cmd.Parameters.AddWithValue("@_pincode", _pincode);

            cmd.Parameters.AddWithValue("@_house_no", _house_no);
            cmd.Parameters.AddWithValue("@_street_no", _street_no);
            cmd.Parameters.AddWithValue("@_permanent", _permanent);
            cmd.Parameters.AddWithValue("@_pincodee", _pincodee);

            cmd.Parameters.AddWithValue("@_propertyItem", _propertyItem);
            cmd.Parameters.AddWithValue("@_acq_date", Convert.ToDateTime(_acq_date));
            cmd.Parameters.AddWithValue("@_totalArea", _totalArea);
            cmd.Parameters.AddWithValue("@_builtupArea", _builtupArea);
            cmd.Parameters.AddWithValue("@_roadType", _roadType);
            cmd.Parameters.AddWithValue("@_streetType", _streetType);
            cmd.Parameters.AddWithValue("@_water_harvest", _water_harvest);
            cmd.Parameters.AddWithValue("@w_t_trype", w_t_trype);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
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

    /************************************************************
   * Purpose  ::  Add Property Details Part 2
   * Author   ::  Arnav
   * Date     ::  27-03-2017
   * ************************************************************/
    public int pmc_holding_property_details_for_flat(string unique_no, string f_no, string u_type, string c_type, string fb_area, string c_use, string r_use,string flat_name,string flat_no,string creation_year)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details_for_flat"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@floor_no", f_no);

            cmd.Parameters.AddWithValue("@usage_type", u_type);
            cmd.Parameters.AddWithValue("@contruction_type", c_type);

            cmd.Parameters.AddWithValue("@fb_area", fb_area);
            cmd.Parameters.AddWithValue("@commercial_use", c_use);
            cmd.Parameters.AddWithValue("@residential_use", r_use);

            cmd.Parameters.AddWithValue("@flat_name", flat_name);
            cmd.Parameters.AddWithValue("@flat_no", flat_no);
            cmd.Parameters.AddWithValue("@creation_year", creation_year);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

    /************************************************************
    * Purpose  ::  Bnd commercial office type
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataTable bind_commercial_office_type()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select distinct(usagetype) from floor_details where usagetype != '' and usagetype is not null ORDER BY usagetype ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    /************************************************************
    * Purpose  ::  Find Last PID
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public string find_last_pid(string id)
    {
        string PID = "";
        Double number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 PID from property_details WHERE PID is not null order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                PID = dt.Rows[0]["PID"].ToString();

                number = Convert.ToDouble(PID);
                number = (number +1);
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
        return number.ToString();
    }

    /************************************************************
    * Purpose  ::  Update Property PID
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public int update_property_flat_area(string plot_area,string PID)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_owner_PID_Update"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", plot_area);
            cmd.Parameters.AddWithValue("@PID", PID);
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

	/************************************************************
    * Purpose  ::  Add Property Details Part 2 for House
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ************************************************************/
    public int pmc_holding_property_details_for_house(int unique_no, string f_no, string typeofuse, string occ_type, string cons_type, string built_area, string crea_year)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details_for_house"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", unique_no);
            cmd.Parameters.AddWithValue("@f_no", f_no);
            cmd.Parameters.AddWithValue("@typeofuse", typeofuse);
            //cmd.Parameters.AddWithValue("@usage_factor", usage_factor);
            cmd.Parameters.AddWithValue("@occ_type", occ_type);
            cmd.Parameters.AddWithValue("@cons_type", cons_type);
            cmd.Parameters.AddWithValue("@built_area", built_area);
            cmd.Parameters.AddWithValue("@crea_year", crea_year);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

    // Added by Gyan Chand Verma
    // Date : 16 June 2019
    public DataTable bindOwnershipType()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from tbl_ownership_type_master where status=1 order by id ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // Added by Gyan Chand Verma
    // Date : 16 June 2019
   public DataTable searchOwnership(string ownership)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, ownership_type from tbl_ownership_type_master where ownership_type=@ownership and status=1 order by id ASC", con);
            cmd.Parameters.AddWithValue("@ownership", ownership);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    
    /***********************************************************
     * Till Here
     * *********************************************************/
}