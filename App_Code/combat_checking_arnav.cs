using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for combat_checking_arnav
/// </summary>
public class combat_checking_arnav
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Checking category for the combat cell
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_user_name(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,combatcatagory from CombatCategory where combatcatagory=@user_name", con);
            cmd.Parameters.AddWithValue("@user_name", u_name);
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

    /************************************************************
     * Purpose  ::  Add New category for the combat cell
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int add_new_combat_category(string category_name,string category_code)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_combat_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@category_name", category_name);
            cmd.Parameters.AddWithValue("@category_code", category_code);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            //cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("h:mm:ss tt"));
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

    /************************************************************
    * Purpose  ::  Update Caregory
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public int update_combat_category(string id,string category_name,string category_code)
    {
        int nnumber = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_combat_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@category_name", category_name);
            cmd.Parameters.AddWithValue("@category_code", category_code);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                nnumber = 1;
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
        return nnumber;
    }

    /************************************************************
    * Purpose  ::  Forward Issue
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public DataTable show_issue_details(string issue_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and id=@id", con);
            cmd.Parameters.AddWithValue("@id", issue_id);
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

    /************************************************************
    * Purpose  ::  Binding All section data
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataSet bind_available_sanitization_officers(string ward_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select id, (first_name+' '+ last_name) as name from users where Ward=@ward and UserTypeid='4' and status='active' ORDER BY first_name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@ward",ward_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    /************************************************************
     * Purpose  ::  Find Circle Name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string check_circle_name(string id)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,Circle from circle where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = dt.Rows[0]["circle"].ToString();
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

    /************************************************************
     * Purpose  ::  Find Circle Name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string check_ward_name(string id)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,WardNo from tblWard where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = dt.Rows[0]["WardNo"].ToString();
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

    /************************************************************
    * Purpose  ::  Binding All section data
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataSet bind_available_chief_sanitization_officers(string circle_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select id, (first_name+' '+ last_name) as name from users where Circle=@circle and UserTypeid='3' and status='active' ORDER BY first_name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@circle", circle_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }


    public string find_sender_name(string complain_no)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 complainno,added_by,[dbo].[find_forwarder_name](added_by)as s_name,status as current_status,fw_DateTime,fw_officer_Id from CombatCell_forward_list where ComplainNo=@complain_no order by id asc", con);
            cmd.Parameters.AddWithValue("@complain_no", complain_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["s_name"].ToString();
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
        return name;
    }


    public string find_receiver_name(string complain_no)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 complainno,added_by,[dbo].[find_forwarder_name](fw_officer_Id)as f_name,status as current_status,fw_DateTime,fw_officer_Id from CombatCell_forward_list where ComplainNo=@complain_no order by id asc", con);
            cmd.Parameters.AddWithValue("@complain_no", complain_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["f_name"].ToString();
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
        return name;
    }

    public string find_sent_date_time(string complain_no)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from CombatCell_forward_list where ComplainNo=@complain_no order by id asc", con);
            cmd.Parameters.AddWithValue("@complain_no", complain_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["fw_DateTime"].ToString();
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
        return name;
    }


    public string find_category_type(string id)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from CombatCategory where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["combatcatagory"].ToString() + " - " + dt.Rows[0]["CatCode"].ToString();
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
        return name;
    }

    public string find_sub_category_type(string id)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from CombatCellSubcategory where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["SubcatagoryName"].ToString() + " - " + dt.Rows[0]["SubCatCode"].ToString();
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
        return name;
    }

    public string find_combat_cell_mode(string id)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from CombatCellMode where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["Mode"].ToString();
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
        return name;
    }

    public DataTable bind_all_operators()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT id,(first_name + ' ' + last_name) as name FROM users WHERE status='active' and usertypeid='5' ORDER BY name ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    /*************************************************************
     * Till Here
     * ************************************************************/
}