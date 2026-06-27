using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// 
/// </summary>
public class house_survey
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable BindHouseHoldSector()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, sector from id_house_hold_sector where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int insert_HouseHoldSurvey(string _ownername, string _no_member, string _no_renter, string _no_house, string _no_lane, string _area, string _sector, string _ward_no, string _mobile_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("id_insert_into_household_data"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@ownername", _ownername);
            cmd.Parameters.AddWithValue("@no_member", _no_member);
            cmd.Parameters.AddWithValue("@no_renter", _no_renter);
            cmd.Parameters.AddWithValue("@no_house", _no_house);
            cmd.Parameters.AddWithValue("@no_lane", _no_lane);
            cmd.Parameters.AddWithValue("@area", _area);
            cmd.Parameters.AddWithValue("@sector", _sector);
            cmd.Parameters.AddWithValue("@ward_no", _ward_no);
            cmd.Parameters.AddWithValue("@mobile_no", _mobile_no);
            cmd.Parameters.AddWithValue("@status", "1");
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

    public int update_HouseHoldSurvey(string title, string _ownername, string _no_member, string _no_renter, string _no_house, string _no_lane, string _area, string _sector, string _ward_no, string _mobile_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("id_update_into_household_data"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ide", title);
            cmd.Parameters.AddWithValue("@ownername", _ownername);
            cmd.Parameters.AddWithValue("@no_member", _no_member);
            cmd.Parameters.AddWithValue("@no_renter", _no_renter);
            cmd.Parameters.AddWithValue("@no_house", _no_house);
            cmd.Parameters.AddWithValue("@no_lane", _no_lane);
            cmd.Parameters.AddWithValue("@area", _area);
            cmd.Parameters.AddWithValue("@sector", _sector);
            cmd.Parameters.AddWithValue("@ward_no", _ward_no);
            cmd.Parameters.AddWithValue("@mobile_no", _mobile_no);
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

    public DataTable bind_all_employee_details()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id, owner_Name, no_of_member, no_of_renters, house_no, lane_name, Area,[dbo].[id_find_sector](sector) as sectors, sector, mobile_no, Ward_no,since from id_Create_Household_data", con);
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

    public DataTable bind_unique_employee_details(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id, owner_Name, no_of_member, no_of_renters, house_no, lane_name, Area,[dbo].[id_find_sector](sector) as sectors, sector, mobile_no, Ward_no,since from id_Create_Household_data where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
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
}