using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Participants Admin Page
/// Added by Gyan Chand Verma
/// Dated 06-11-2020
/// </summary>
public class participantAdmin2020
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable BindGrid()
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, type, full_name, father_name, address, mobile_no, alt_mobile_no,team_name, email_id, id_type, id_no, status, CONVERT(VARCHAR(10),since,103) as since from participants_2021 where status='active' order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable BindGridbyTypeId(string _dropdownValue)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, type, full_name, father_name, address, mobile_no, alt_mobile_no, email_id,team_name,  id_type, id_no, status, Since from participants_2021 where type=@typeId and status='active' order by id desc", con);
            cmd.Parameters.AddWithValue("@typeId", _dropdownValue);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable BindGridbyId(String id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, type, full_name, father_name, address, mobile_no, alt_mobile_no, email_id, id_type,team_name,  id_no, status, address, youtube_link from participants_2021 where id=@_id order by id desc", con);
            cmd.Parameters.AddWithValue("@_id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public int updateStatus(string title, string _status, string _msg)
    {
        int num = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update participants_2021 set current_status=@status, message=@msg where id=@_id", con);
            cmd.Parameters.AddWithValue("@_id", title);
            cmd.Parameters.AddWithValue("@status", _status);
            cmd.Parameters.AddWithValue("@msg", _msg);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                num = 1;
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

        return num;
    }
}