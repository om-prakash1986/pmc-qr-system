using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
/// <summary>
/// Summary description for CombatCellAdmin
/// </summary>
public class CombatCellAdmin
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Adding new Administrator for the website
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int add_administrator(string first_name, string last_name, string user_name, string email_id, string contact_no, string password, string gender, string content_type, byte[] bytes)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_admin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@path", "");
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("h:mm:ss tt"));
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
     * Purpose  ::  Adding new main Menu for the website
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int Admin_login_auth(string user_name, string password)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_admin_login"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Connection = con;
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["User_Name"] = null;
                    HttpContext.Current.Session["User_ID"] = null;
                    HttpContext.Current.Session["User_Name"] = dt.Rows[0]["email_id"].ToString();
                    HttpContext.Current.Session["User_ID"] = dt.Rows[0]["id"].ToString();
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
        }
        return number;
    }

    public int check_user_name(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,user_name from users where user_name=@user_name", con);
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

    public int add_User(string usertype,string circle, string ward, string first_name, string last_name, string user_name, string email_id, string contact_no, string password, string gender, string content_type, byte[] bytes)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_Combat_User"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserTypeid", usertype);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@Ward", ward);
            cmd.Parameters.AddWithValue("@Circle", circle);
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
    //Added By Amrita Singh for update users pannel
    public int update_Combat_User_all(string id,string usertype, string circle, string ward, string first_name, string last_name, string user_name, string email_id, string contact_no, string password, string gender, string content_type, byte[] bytes)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_Combat_User_all"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@UserTypeid", usertype);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@Ward", ward);
            cmd.Parameters.AddWithValue("@Circle", circle);
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
    //Added By Amrita Singh for update users pannel
    public int update_Combat_User_text(string id,string usertype, string circle, string ward, string first_name, string last_name, string user_name, string email_id, string contact_no, string password, string gender)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_Combat_User_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@UserTypeid", usertype);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@Ward", ward);
            cmd.Parameters.AddWithValue("@Circle", circle);
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
    //Added By Amrita Singh for update users pannel
    public int update_Combat_User_pdf(string id,string usertype, string circle, string ward, string first_name, string last_name, string user_name, string email_id, string contact_no, string gender, string content_type, byte[] bytes)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_Combat_User_pdf"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@UserTypeid", usertype);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@Ward", ward);
            cmd.Parameters.AddWithValue("@Circle", circle);
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
    //till here

    public DataTable BindUserType()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,authority_name from authority_details where status='active' order by authority_name ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    
    public DataTable BindCircle()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,Circle from Circle where status=1 order by Circle ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindWard()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.*,b.Circle from tblWard a inner join Circle b on a.Circleid=b.id  where a.status=1 order by Circle ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public Int32 AddCCWard(string id, string Circleid, string WardNo, string WardCode)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_Ward"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@Circleid", Circleid);
            cmd.Parameters.AddWithValue("@WardNo", WardNo);
            cmd.Parameters.AddWithValue("@WardCode", WardCode);
            cmd.Parameters.AddWithValue("@status", 1);
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
    
    public DataTable bindCategory()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,combatcatagory,CatCode from CombatCategory where status='Active' order by combatcatagory ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    
    public DataTable BindSubCatagory()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.*,b.combatcatagory from CombatCellSubcategory a inner join CombatCategory b on a.categoryid=b.id  where a.status=1 order by a.SubcatagoryName ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    
    public Int32 AddSubCatgory(string id, string categoryid, string SubcatagoryName, string SubCatCode, string TimelimitinhrsforSI, string TimeLimitforCSI)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_SubCatagory"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@categoryid", categoryid);
            cmd.Parameters.AddWithValue("@SubcatagoryName", SubcatagoryName);
            cmd.Parameters.AddWithValue("@SubCatCode", SubCatCode);
            cmd.Parameters.AddWithValue("@TimelimitinhrsforSI", TimelimitinhrsforSI);
            cmd.Parameters.AddWithValue("@TimeLimitforCSI", TimeLimitforCSI);
            cmd.Parameters.AddWithValue("@status", 1);
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
    /*Combat cell view user of admin panel
     *Added By Amrita Singh
    */
    //users(added_by,UserTypeid,first_name,last_name,user_name,gender,email_id,password,contact_no,image_data_content_type,image_date,since,Ward,Circle
    public DataTable Bind_admin_user()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from users where status='active' order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }



}