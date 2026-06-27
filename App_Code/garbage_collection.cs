using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for garbage_collection
/// </summary>
public class garbage_collection
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
	public garbage_collection()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int insert_new_daily_garbage_collection_entry(string circle, string ward, string date, string total_h, string pick_h, string left_h, string total_s, string pick_s, string left_s,string remarks)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_daily_collection"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@circle", circle);
            cmd.Parameters.AddWithValue("@ward", ward);
            cmd.Parameters.AddWithValue("@date", date);

            cmd.Parameters.AddWithValue("@total_h", total_h);
            cmd.Parameters.AddWithValue("@pick_h", pick_h);
            cmd.Parameters.AddWithValue("@left_h", left_h);

            cmd.Parameters.AddWithValue("@total_s", total_s);
            cmd.Parameters.AddWithValue("@pick_s", pick_s);
            cmd.Parameters.AddWithValue("@left_s", left_s);

            cmd.Parameters.AddWithValue("@remarks", remarks);
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

    public DataTable bind_all_entries(string ward)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            if (ward == "0")
            {
                SqlCommand cmd = new SqlCommand("SELECT *,[dbo].[find_circle_name](circle_id) as circle_name from garbage_collection_details category where status='active' order by report_for desc", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else
            {
                SqlCommand cmd = new SqlCommand("SELECT *,[dbo].[find_circle_name](circle_id) as circle_name from garbage_collection_details category where status='active' and ward_id=@ward order by report_for desc", con);
                cmd.Parameters.AddWithValue("@ward",ward);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
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
        return dt;
    }

    public DataTable bind_single_data(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from garbage_collection_details category where status='active' and id=@id order by report_for desc", con);
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

    public int update_new_daily_garbage_collection_entry(string id,string circle, string ward, string date, string total_h, string pick_h, string left_h, string total_s, string pick_s, string left_s, string remarks)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Update_daily_collection"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id.ToString());
            cmd.Parameters.AddWithValue("@circle", circle);
            cmd.Parameters.AddWithValue("@ward", ward);
            cmd.Parameters.AddWithValue("@date", date);

            cmd.Parameters.AddWithValue("@total_h", total_h);
            cmd.Parameters.AddWithValue("@pick_h", pick_h);
            cmd.Parameters.AddWithValue("@left_h", left_h);

            cmd.Parameters.AddWithValue("@total_s", total_s);
            cmd.Parameters.AddWithValue("@pick_s", pick_s);
            cmd.Parameters.AddWithValue("@left_s", left_s);

            cmd.Parameters.AddWithValue("@remarks", remarks);
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

    public DataTable create_daily_collection_report(string circle_id,string date)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select ROW_NUMBER() OVER(ORDER BY WardNo ASC) AS id,circleid,WardNo,[dbo].[find_total_houses](id,@date) as total_houses,[dbo].[find_pick_houses](id,@date) as garbage_from_houses,[dbo].[find_left_houses](id,@date) as left_houses,[dbo].[find_total_shops](id,@date) as total_shops,[dbo].[find_pick_shops](id,@date) as garbage_from_shops,[dbo].[find_left_shops](id,@date) as left_shops from tblWard where status='1' and Circleid=@circle_id order by WardNo Asc", con);
            cmd.Parameters.AddWithValue("@circle_id", circle_id);
            cmd.Parameters.AddWithValue("@date", date);
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
    // till here
}