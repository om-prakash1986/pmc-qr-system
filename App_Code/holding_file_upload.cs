using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for holding_file_upload
/// </summary>
public class holding_file_upload
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public int find_if_file_available(int pid)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select content_type1 from holding_files where Pro_ID= @pid", con);
            cmd.Parameters.AddWithValue("@pid", pid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = 1;
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

     public int insert_uploaded_files(byte[] file1, string content_type1, byte[] file2, string content_type2, byte[] file3, string content_type3, int pid)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            using (SqlCommand cmd = new SqlCommand("Insert_holding_files"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@file1", file1);
                cmd.Parameters.AddWithValue("@file2", file2);
                cmd.Parameters.AddWithValue("@file3", file3);
                cmd.Parameters.AddWithValue("@content_type1", content_type1);
                cmd.Parameters.AddWithValue("@content_type2", content_type2);
                cmd.Parameters.AddWithValue("@content_type3", content_type3);
                cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                number = 1;
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

     public int update_uploaded_files(byte[] file1, string content_type1, byte[] file2, string content_type2, byte[] file3, string content_type3, int pid)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            using (SqlCommand cmd = new SqlCommand("Update_holding_files"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@file1", file1);
                cmd.Parameters.AddWithValue("@file2", file2);
                cmd.Parameters.AddWithValue("@file3", file3);
                cmd.Parameters.AddWithValue("@content_type1", content_type1);
                cmd.Parameters.AddWithValue("@content_type2", content_type2);
                cmd.Parameters.AddWithValue("@content_type3", content_type3);
                cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                number = 1;
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


    // till here
}