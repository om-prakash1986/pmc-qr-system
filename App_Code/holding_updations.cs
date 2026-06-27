using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for holding_updations
/// </summary>
/// 
public class holding_updations
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
    * Purpose  ::  Phase 1 Updatin
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public DataTable bind_available_data_step_one_pid(string PID)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from property_details WHERE id=@pid", con);
        cmd.Parameters.AddWithValue("@pid", PID);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }
	
	 public int update_sas_no(int custid, string sasno)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
             SqlCommand cmd = new SqlCommand("update_sas_no");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@custId", custid);
            cmd.Parameters.AddWithValue("@sasno", sasno);
            cmd.Connection = con;
            try
            {
                con.Open(); ;
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
    public DataTable shoe_sas_no(int cus_ID)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select SASNo,added_by from property_details where id=@id", con);
            cmd.Parameters.AddWithValue("@id", cus_ID);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable bind_available_owner_details_step_one_pid(string PID)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from owner_details WHERE Pro_ID=@pid", con);
        cmd.Parameters.AddWithValue("@pid", PID);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public int delete_old_property_record(string property_id)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("delete_old_owner_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", property_id);
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

// Updated by Gyan Chand Verma
    // date : 16 June 2019
    public int pmc_holding_property_details_update(int Pro_ID, string _circle, string _ward, string _ownershipType, string _corr_house, string _corr_street, string _corr, string _pincode, string _house_no, string _street_no, string _permanent, string _pincodee, string _propertyItem, string _acq_date, string _totalArea, string _builtupArea, string _roadType, string _streetType, string _water_harvest, string w_t_trype, string amount)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("holding_property_details_update"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID"].ToString());
            cmd.Parameters.AddWithValue("@Pro_ID", Pro_ID);
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

    public DataTable is_property_available(string pid)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from property_details WHERE id=@pid", con);
        cmd.Parameters.AddWithValue("@pid", pid);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public int delete_old_flat_record(string property_id)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("delete_old_floor_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@unique_no", property_id);
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

     public DataTable is_property_available_flat_and_house(string pid)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from floor_details WHERE Pro_ID= @pid", con);
        cmd.Parameters.AddWithValue("@pid", pid);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable calculate_floor_data(string PID)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from property_details WHERE id=@pid", con);
        cmd.Parameters.AddWithValue("@pid", PID);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable bind_vaccant_land_details(string PID)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_municipality](PID) as municipality from property_details WHERE id=@pid", con);
        cmd.Parameters.AddWithValue("@pid", PID);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public int update_vaccant_land_details(string PID, string SAS_no,string plot_area,string year)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_vaccant_land"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pid", PID);
            cmd.Parameters.AddWithValue("@sas_no", SAS_no);
            cmd.Parameters.AddWithValue("@plot_area", plot_area);
            cmd.Parameters.AddWithValue("@year", year);
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

    public DataTable find_circle_by_name(string circle_name)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,circle from circle where circle=@pid", con);
        cmd.Parameters.AddWithValue("@pid", circle_name);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_ward_by_name(string ward_name)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,circle_id,ward_no from tbl_ward_master where ward_no=@pid", con);
        cmd.Parameters.AddWithValue("@pid", ward_name);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_property_type(string property_type_name)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,property_type from tbl_property_type_master where property_type=@property_type_name", con);
        cmd.Parameters.AddWithValue("@property_type_name", property_type_name);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_watetype(string property_type_name)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,tax_type from water_tax_type where id=@property_type_name", con);
        cmd.Parameters.AddWithValue("@property_type_name", property_type_name);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_waterharvesting(string waterharves)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,rain_water from rain_water_harvesting where rain_water=@property_type_name and status=1", con);
        cmd.Parameters.AddWithValue("@property_type_name", waterharves);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_revenue_circle_by_name(string revenuecircle)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,rev_circle from tbl_revenue_circle_master where rev_circle= @revenuecircle", con);
        cmd.Parameters.AddWithValue("@revenuecircle", revenuecircle);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_Road_by_name(string roadname)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,Street_Name,Road_Type from road_type where Street_Name=@roadname", con);
        cmd.Parameters.AddWithValue("@roadname", roadname);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    // floor wise
    public DataTable find_construction_date_floor_wise(string PID, string ID)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select id,fromyear,PID from floor_details where Pro_ID=@pid and id=@id", con);
        cmd.Parameters.AddWithValue("@pid", PID);
        cmd.Parameters.AddWithValue("@id", ID);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }
    // overall
    // overall
    public DataTable find_construction_date(string PID, string id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from floor_details where Pro_ID= @pid and id=@id order by id desc", con);
        cmd.Parameters.AddWithValue("@pid", PID);
        cmd.Parameters.AddWithValue("@id", id);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_all_floors(string pid)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from floor_details where Pro_ID= @pid", con);
        cmd.Parameters.AddWithValue("@pid", pid);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable is_property_available_flat_and_house_id_wise(string pid, string id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        SqlCommand cmd = new SqlCommand("Select * from floor_details WHERE Pro_ID=@pid and id=@id", con);
        cmd.Parameters.AddWithValue("@pid", pid);
        cmd.Parameters.AddWithValue("@id", id);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    public DataTable find_floor_form_database()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select id, floorNo from floor_no", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bind_available_roads()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT id,street_name,Road_Type FROM road_type ORDER BY street_name ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bind_waterType()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT id,tax_type FROM water_tax_type order by id asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bind_waterharvesting()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT id,rain_water FROM rain_water_harvesting where status=1 order by id asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bind_available_roads_against_road(string id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT id,Road_Type FROM road_type where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
}