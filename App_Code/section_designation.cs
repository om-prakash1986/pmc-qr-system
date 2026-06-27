using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for section_designation
/// </summary>
public class section_designation
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Insert Section And Designation
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int add_new_data(string branch, string alloted_Place, string section, string designation, string name, string email_id, string contact_no, string status, int c_id, int d_id)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_section_designation"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@branch", branch);
            cmd.Parameters.AddWithValue("@alloted_Place", alloted_Place);
            cmd.Parameters.AddWithValue("@section", section);
            cmd.Parameters.AddWithValue("@designation", designation);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@c_id", c_id);
            cmd.Parameters.AddWithValue("@d_id", d_id);
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
    //show all data
    public DataTable show_all_officers(string filter, string filter1)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            if (filter == "0")
            {
                SqlCommand cmd = new SqlCommand("Select id, full_name, section_name, designation, contact_no, email_id,d_id,c_id from section_designation where status='active' ORDER BY alloted_Place ASC", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if (filter == "3" && filter1 == "0")
            {
                SqlCommand cmd = new SqlCommand("Select  id, full_name, section_name, designation, contact_no, email_id,d_id,c_id from section_designation where c_id !=0 and status='active' ORDER BY section_name ASC", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if (filter == "4" && filter1 == "0")
            {
                SqlCommand cmd = new SqlCommand("Select  id, full_name, section_name, designation, contact_no, email_id,d_id,c_id  from section_designation where d_id !=0 and  status='active' ORDER BY section_name ASC", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if (filter == "1")
            {
                SqlCommand cmd = new SqlCommand("Select  id, full_name, section_name, designation, contact_no, email_id,d_id,c_id from section_designation where branch=@branch and status='active' ORDER BY alloted_Place ASC", con);
                cmd.Parameters.AddWithValue("@branch", filter);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if (filter == "3")
            {
                SqlCommand cmd = new SqlCommand("Select  id, full_name, section_name, designation, contact_no, email_id,d_id,c_id from section_designation where c_id=@branch and status='active' ORDER BY alloted_Place ASC", con);
                cmd.Parameters.AddWithValue("@branch", filter1);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            else if (filter == "4")
            {
                SqlCommand cmd = new SqlCommand("Select  id, full_name, section_name, designation, contact_no, email_id,d_id,c_id  from section_designation where d_id=@branch and status='active' ORDER BY alloted_Place ASC", con);
                cmd.Parameters.AddWithValue("@branch", filter1);
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


    public DataSet bind_available_circles()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        string query = "Select * from Circle ORDER BY id ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        con.Open();
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds);
        con.Close();
        return ds;
    }

    public DataSet bind_Division_list()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "Select * from Division  ORDER BY id ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds);
        con.Close();
        return ds;
    }

    public DataTable rep_bind(string txt)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from section_designation where full_name like '" + txt + "%'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
    }
    /************************************************************
     * Till Here
     * *********************************************************/
}