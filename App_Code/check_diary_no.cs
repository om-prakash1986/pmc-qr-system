using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for check_diary_no
/// </summary>
public class check_diary_no
{
    string strcon = ConfigurationManager.ConnectionStrings["dashboard"].ConnectionString;
    /******************************************************************
     * Check QR Code
     * ****************************************************************/
    public int check_qr_code(string diary_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,dairy_no from letter_title where dairy_no=@dairy_no", con);
            cmd.Parameters.AddWithValue("@dairy_no", diary_no);
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

    /*******************************************************
     * Show all subjects
     * ********************************************************/

    public DataTable show_all_sub_subjects(string dairy_no)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select a.id as id,a.dairy_no,a.project_type,a.subject,a.status,b.id as b_id,b.project_type as project_type_name from letter_subjects a,letter_project_types b where a.dairy_no=@dairy_no and a.project_type=b.id", con);
            cmd.Parameters.AddWithValue("@dairy_no", dairy_no);
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

    /*********************************************************
     * Bind all data
     * *******************************************************/
    public DataTable bind_all_data(string id)
    {
        DataTable dt =new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select a.id as id,a.dairy_no,a.project_type,a.subject,a.status,b.id as b_id,b.project_type as project_type_name from letter_subjects a,letter_project_types b where a.id=@dairy_no and a.project_type=b.id", con);
            cmd.Parameters.AddWithValue("@dairy_no", id);
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

    /*****************************************************************
     * Update Data
     * ***************************************************************/
    public int update_project_data(string id,string dairy_no,string project_type,string subject)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update letter_subjects set dairy_no=@dairy_no,project_type=@project_type,subject=@subject where id=@id", con);
            cmd1.Parameters.AddWithValue("@dairy_no", dairy_no);
            cmd1.Parameters.AddWithValue("@project_type",project_type);
            cmd1.Parameters.AddWithValue("@subject",subject);
            cmd1.Parameters.AddWithValue("@id", id);
            cmd1.ExecuteNonQuery();
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
        return number;
    }



    /***********************************************************
     * Till here
     * *********************************************************/
}