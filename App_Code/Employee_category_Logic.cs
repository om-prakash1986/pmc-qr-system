using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

 
public class Employee_category_Logic
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    // insert new employee_category
    public int insert_new_employee_category(string category_name_e, string category_name_h)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("id_insert_employee_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@category_name_e", category_name_e);
            cmd.Parameters.AddWithValue("@category_name_h", category_name_h);
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

    // check employee_category name
    public int check_category_name(string category_name_e)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from id_employee_category where status='active' and category_name_e=@category_name_e", con);
            cmd.Parameters.AddWithValue("@category_name_e", category_name_e);
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

    // bind all districts
    public DataTable bind_all_employee_category()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from id_employee_category where status='active' order by category_name_e ASC", con);
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

  

    // bind unique district
    public DataTable bind_unique_employee_category(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from id_employee_category where status='active' and id=@id", con);
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

    // update district without password
    public int update_employee_category(string title, string category_name_e, string category_name_h)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("id_update_employee_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", title);
            cmd.Parameters.AddWithValue("@category_name_e", category_name_e);
            cmd.Parameters.AddWithValue("@category_name_h", category_name_h);
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
}