using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for morya_shops
/// </summary>
public class morya_shops
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    // bind all blocks
    public DataSet bind_all_blocks(string office_type)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT distinct(shops) as blocks FROM status_report ORDER BY shops ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    // bind all floors
    public DataSet bind_all_floors(string office_type,string block)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT distinct(floor_no) FROM status_report WHERE office_type=@office_type and shops=@block ORDER BY floor_no ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@office_type",office_type);
        cmd.Parameters.AddWithValue("@block", block);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataTable bind_all_offices(string block, string floor_no)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from master_details where block=@block and floor=@floor order by owner_name asc", con);
            cmd.Parameters.AddWithValue("@block", block);
            cmd.Parameters.AddWithValue("@floor", floor_no);
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

    public DataTable bind_all_offices_without_floor(string block)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from master_details where block=@block order by owner_name asc", con);
            cmd.Parameters.AddWithValue("@block", block);
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

    public DataTable bind_unique_shop_details(string shop_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from master_details where id=@id order by owner_name asc", con);
            cmd.Parameters.AddWithValue("@id", shop_id);
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