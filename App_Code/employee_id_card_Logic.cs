using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

public class employee_id_card_Logic
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public int insert_new_employee_details(string employee_category, string _cmpName_Id, string employee_id, string _ward_Id, string _designation_Id, string name, string father_name, string permanet_address, string adhar_no, string mobile_no, string blood_group, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("id_insert_employee_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@employee_category", employee_category);
            cmd.Parameters.AddWithValue("@cmpName_Id", _cmpName_Id);
            cmd.Parameters.AddWithValue("@employee_id", employee_id);
            cmd.Parameters.AddWithValue("@ward_Id", _ward_Id);
            cmd.Parameters.AddWithValue("@designation_Id", _designation_Id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@father_name", father_name);
            cmd.Parameters.AddWithValue("@permanet_address", permanet_address);
            cmd.Parameters.AddWithValue("@adhar_no", adhar_no);
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@blood_group", blood_group);
            cmd.Parameters.AddWithValue("@admin_image", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
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

    // bind all images
    public DataTable bind_all_employee_details()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id, added_by, dbo.id_find_employee_category(employee_Category_Id) as employee_categoryy,employee_Category_Id,company_Id,dbo.id_find_Circle(circle_Id) as circle,dbo.id_find_Ward(ward_Id) as ward,designation_Id,name,father_name,address,adhar_no,mobile_no,blood_group,since from id_employee_details order by id desc", con);
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

    public DataTable bind_all_employee_details_order_by_category()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,added_by,dbo.id_find_employee_category(employee_Category_Id) as employee_categoryy, employee_Category_Id,name,father_name,address,adhar_no,mobile_no,blood_group,since from id_employee_details order by employee_category asc", con);
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

    // bind unique image
    public DataTable bind_unique_employee_details(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,added_by,dbo.id_find_employee_category(employee_Category_Id) as employee_categoryy,employee_Category_Id,dbo.id_find_company_name(company_Id) as company_name,company_Id,dbo.id_find_Circle(circle_Id) as circle,circle_Id,dbo.id_find_Ward(ward_Id) as ward,ward_Id,dbo.id_find_designation_name(designation_Id) as designation,designation_Id,name,father_name,address,adhar_no,mobile_no,blood_group,since from id_employee_details where id=@id", con);
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

    // update image details without image
    public int update_employee_details_with_image(string title, string employee_category, string _cmpName_Id, string employee_id, string _ward_Id, string _designation_Id, string name, string father_name, string permanet_address, string adhar_no, string mobile_no, string blood_group, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("id_update_employee_details_with_image"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", title);
            cmd.Parameters.AddWithValue("@employee_category", employee_category);
            cmd.Parameters.AddWithValue("@cmpName_Id", _cmpName_Id);
            cmd.Parameters.AddWithValue("@employee_id", employee_id);
            cmd.Parameters.AddWithValue("@ward_Id", _ward_Id);
            cmd.Parameters.AddWithValue("@designation_Id", _designation_Id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@father_name", father_name);
            cmd.Parameters.AddWithValue("@permanet_address", permanet_address);
            cmd.Parameters.AddWithValue("@adhar_no", adhar_no);
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@blood_group", blood_group);
            cmd.Parameters.AddWithValue("@admin_image", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
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

    // update image details with image
    public int update_employee_details_without_image(string title, string employee_category, string _cmpName_Id, string employee_id, string _ward_Id, string _designation_Id, string name, string father_name, string permanet_address, string adhar_no, string mobile_no, string blood_group)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("id_update_employee_details_without_image"))
        {

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", title);
            cmd.Parameters.AddWithValue("@employee_category", employee_category);
            cmd.Parameters.AddWithValue("@cmpName_Id", _cmpName_Id);
            cmd.Parameters.AddWithValue("@employee_id", employee_id);
            cmd.Parameters.AddWithValue("@ward_Id", _ward_Id);
            cmd.Parameters.AddWithValue("@designation_Id", _designation_Id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@father_name", father_name);
            cmd.Parameters.AddWithValue("@permanet_address", permanet_address);
            cmd.Parameters.AddWithValue("@adhar_no", adhar_no);
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@blood_group", blood_group);
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

    public DataSet bind_employee_category()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,category_name_e,category_name_h FROM id_employee_category WHERE status='active' ORDER BY category_name_e ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataTable BindAllCircles()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, circle from Circle where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindWard(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, ward_no from tbl_ward_master where status=1 and circle_id=@circleId order by ward_no ASC", con);
            cmd.Parameters.AddWithValue("@circleId", circle_id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(ds);
            dt = ds.Tables[0];
            con.Close();
        }
        return dt;
    }

    public DataTable BindEmployeeCategory(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,category_id ,company_name from id_campany_name where category_id= @id and status='active'", con);
            cmd.Parameters.AddWithValue("@id", circle_id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(ds);
            dt = ds.Tables[0];
            con.Close();
        }
        return dt;
    }

    public DataTable BindDesignation()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select designationId, designation from id_card_worker_designation where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
}