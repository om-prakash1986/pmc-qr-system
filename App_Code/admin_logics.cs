using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for admin_logics
/// </summary>
public class admin_logics
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
    public int check_user_name(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,user_name from administrator where user_name=@user_name", con);
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

    public int check_main_menu_name(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name from category where name=@user_name", con);
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
    public int check_main_menu_name_hindi(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name_h from category where name=@user_name", con);
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
    public int check_main_menu_title(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from category where name=@user_name", con);
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
    public int check_main_menu_title_hindi(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from category where name=@user_name", con);
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
    public int add_new_main_menu(string name, string name_h, string title, string title_h, string description, string description_h, string css, string path, string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_main_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@path1", path1);
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

    ///*********************************************************
    ///Sub Menu 1
    ///**********************************************************/

    /************************************************************
     * Purpose  ::  Checking Sub menu one(submenuone.aspx)
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_name_sub_menu(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name from category_1 where name=@user_name", con);
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
     * Purpose  ::  Checking Main menu name hindi
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_name_hindi_sub_menu(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name_h from category_1 where name=@user_name", con);
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
     * Purpose  ::  Checking Main menu name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_title_sub_menu(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from category_1 where name=@user_name", con);
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
     * Purpose  ::  Checking Main menu name hindi
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_title_hindi_sub_menu(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from category_1 where name=@user_name", con);
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
    * Purpose  ::  Add New Main Menu
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public int add_new_sub_menu(string main_menu, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path, string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_sub_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@path1", path1);
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


    ///*********************************************************
    ///Sub Menu 2
    ///**********************************************************/

    /************************************************************
     * Purpose  ::  Checking Main menu name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_name_sub_menu_two(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name from category_2 where name=@user_name", con);
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
     * Purpose  ::  Checking Main menu name hindi
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_name_hindi_sub_menu_two(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name_h from category_2 where name=@user_name", con);
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
     * Purpose  ::  Checking Main menu name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_title_sub_menu_two(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from category_2 where name=@user_name", con);
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
     * Purpose  ::  Checking Main menu name hindi
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_title_hindi_sub_menu_two(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from category_2 where name=@user_name", con);
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
    * Purpose  ::  Add New sub Menu Two
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public int add_new_sub_menu_two(string main_menu, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path, string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_sub_category_two"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@path1", path1);
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

    ///*********************************************************
    ///Sub Menu 3
    ///**********************************************************/

    /************************************************************
     * Purpose  ::  Checking Sub menu three name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_name_sub_menu_three(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name from category_3 where name=@user_name", con);
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
     * Purpose  ::  Checking sub menu name hindi
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_name_hindi_sub_menu_three(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name_h from category_3 where name=@user_name", con);
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
     * Purpose  ::  Checking sub menu three title
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_title_sub_menu_three(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from category_3 where name=@user_name", con);
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
     * Purpose  ::  Checking sub menu title hindi
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int check_main_menu_title_hindi_sub_menu_three(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from category_3 where name=@user_name", con);
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
    * Purpose  ::  Add New sub Menu three
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public int add_new_sub_menu_three(string main_menu, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path, string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_sub_category_three"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@path1", path1);
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

    ///*********************************************************
    ///Sub Menu 4
    ///**********************************************************/

    /************************************************************
     * Purpose  ::  Checking Sub menu four name
     * Author   ::  Amrita
     * Date     ::  20-06-2018
     * ********************************************************/
    public int check_main_menu_name_sub_menu_four(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name from category_4 where name=@user_name", con);
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

    public int check_main_menu_name_hindi_sub_menu_four(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name_h from category_4 where name=@user_name", con);
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
    public int check_main_menu_title_sub_menu_four(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from category_4 where name=@user_name", con);
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
    public int check_main_menu_title_hindi_sub_menu_four(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from category_4 where name=@user_name", con);
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
    public int add_new_sub_menu_four(string main_menu, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path, string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_sub_category_four"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@path1", path1);
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
    * Purpose  ::  Checking Sub menu four (submenufive.aspx)
    * Author   ::  Amrita
    * Date     ::  20-06-2018
    * ********************************************************/
    public int check_main_menu_name_sub_menu_five(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name from category_5 where name=@user_name", con);
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

    public int check_main_menu_name_hindi_sub_menu_five(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name_h from category_5 where name=@user_name", con);
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
    public int check_main_menu_title_sub_menu_five(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from category_5 where name=@user_name", con);
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
    public int check_main_menu_title_hindi_sub_menu_five(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from category_5 where name=@user_name", con);
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
    public int add_new_sub_menu_five(string main_menu, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path,string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_sub_category_five"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@path1", path1);
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
   * Purpose  ::  Checking Sub menu six (submenusix.aspx)
   * Author   ::  Amrita
   * Date     ::  20-06-2018
   * ********************************************************/
    public int check_main_menu_name_sub_menu_six(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name from category_6 where name=@user_name", con);
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

    public int check_main_menu_name_hindi_sub_menu_six(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name_h from category_6 where name=@user_name", con);
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
    public int check_main_menu_title_sub_menu_six(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from category_6 where name=@user_name", con);
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
    public int check_main_menu_title_hindi_sub_menu_six(string u_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from category_6 where name=@user_name", con);
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
    public int add_new_sub_menu_six(string main_menu, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path,string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_sub_category_six"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@path1", path1);
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
    /*************************Start Image Gallery insertion***********************************/
    /************************************************************
     * Purpose  ::  Adding New Image Gallery 
     * Author   ::  Amrita Singh
     * Date     ::  21-06-2018
     * *********************************************************/
    public int add_image_gallary(string content_type, byte[] bytes, string content_e, string content_h)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_image_gallary"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@title", content_e);
            cmd.Parameters.AddWithValue("@title_h", content_h);
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
            con.Close();
            con.Dispose();
        }
        return number;
    }
    /*************************Start Video Gallery insertion***********************************/
    /************************************************************
    * Purpose  ::  Adding New Video Gallery 
     * Author   ::  Amrita Singh
     * Date     ::  20-06-2018
     * *********************************************************/
    public int add_video_url_gallary(string content_type, byte[] image, string title, string title_h, string y_url, string description_e, string description_h)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);

        using (SqlCommand cmd = new SqlCommand("insert_video_url"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@video_url", y_url);
            cmd.Parameters.AddWithValue("@image_data", image);
            cmd.Parameters.AddWithValue("@image_content_type", content_type);
            cmd.Parameters.AddWithValue("@description", description_e);
            cmd.Parameters.AddWithValue("@description_h", description_h);
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
            con.Close();
            con.Dispose();
        }
        return number;
    }
    /*************************Start Video Gallery insertion***********************************/
    /************************************************************
    * Purpose  ::  Adding New local Video Gallery 
     * Author   ::  Amrita Singh
     * Date     ::  20-06-2018
     * *********************************************************/
    public int add_local_video_gallary(string content_type_image, byte[] image, string content_type_video, byte[] local_video, string title, string title_h, string description_e, string description_h)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);

        using (SqlCommand cmd = new SqlCommand("insert_local_video"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());

            cmd.Parameters.AddWithValue("@image_data", image);
            cmd.Parameters.AddWithValue("@image_content_type", content_type_image);
            cmd.Parameters.AddWithValue("@video_data", local_video);
            cmd.Parameters.AddWithValue("@video_content_type", content_type_video);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description_e);
            cmd.Parameters.AddWithValue("@description_h", description_h);
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
            con.Close();
            con.Dispose();
        }
        return number;
    }
    /************************************************************
   * Purpose  ::  Adding tender(newtender.aspx)
   * Author   ::  Amrita
   * Date     ::  21-06-2018
   * ********************************************************/
    public int check_tender_title_e(string t_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from tender where title=@t_name", con);
            cmd.Parameters.AddWithValue("@t_name", t_name);
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
    public int check_tender_title_h(string t_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from tender where title_h=@t_name", con);
            cmd.Parameters.AddWithValue("@t_name", t_name);
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
    public int add_tender(string title,string title_h,string start_date,string end_date,byte[] bytes,string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_tender"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@start_date", start_date);
            cmd.Parameters.AddWithValue("@end_date", end_date);
            cmd.Parameters.AddWithValue("@pdf_data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
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
  * Purpose  ::  Adding Notice Board(noticeboard.aspx)
  * Author   ::  Amrita
  * Date     ::  21-06-2018
  * ********************************************************/
    public int check_notice_title_e(string t_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title from notice_board where title=@t_name", con);
            cmd.Parameters.AddWithValue("@t_name", t_name);
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
    public int check_notice_title_h(string t_name)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,title_h from notice_board where title_h=@t_name", con);
            cmd.Parameters.AddWithValue("@t_name", t_name);
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
    public int add_notice_board(string title, string title_h, string start_date, string end_date, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_notice"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@start_date", start_date);
            cmd.Parameters.AddWithValue("@end_date", end_date);
            cmd.Parameters.AddWithValue("@pdf_data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
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
 * Purpose  ::  Adding Recruitment(recruitment.aspx)
 * Author   ::  Amrita
 * Date     ::  08-07-2018
 * ********************************************************/
    public int add_recruitment(string title, string description,byte[] bytes)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_retruitment"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@data", bytes);
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
            con.Close();
            con.Dispose();
            return number;
        }
    }





    /************************************************************
    * Purpose  ::  Adding new User for the website
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public int add_new_wardCouncillor(string Circleid, string Wardid, string first_name, string last_name, string MotherName, string FatherName, string DOB, string Address, string email_id, string contact_no, string HighestQualification, string ExperianceDetails, string gender, string content_type, byte[] bytes)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_Ward_Councillor"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@Circleid", Circleid);
            cmd.Parameters.AddWithValue("@Wardid", Wardid);
            cmd.Parameters.AddWithValue("@first_name", first_name);
            cmd.Parameters.AddWithValue("@last_name", last_name);
            cmd.Parameters.AddWithValue("@MotherName", MotherName);
            cmd.Parameters.AddWithValue("@FatherName", FatherName);
            cmd.Parameters.AddWithValue("@DOB", DOB);
            cmd.Parameters.AddWithValue("@Address", Address);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
           
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@HighestQualification", HighestQualification);
            cmd.Parameters.AddWithValue("@ExperianceDetails", ExperianceDetails);
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
            con.Close();
            con.Dispose();
        }
        return number;
    }

    public int add_new_wardCouncillor_Image(string wardcId, byte[] Image, string contentType)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_Ward_Councillor_image"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id",0);
            cmd.Parameters.AddWithValue("@wardcId", wardcId);
            cmd.Parameters.AddWithValue("@Imageurl", "");
            cmd.Parameters.AddWithValue("@Image", Image);
            cmd.Parameters.AddWithValue("@contentType", contentType);
            cmd.Parameters.AddWithValue("@status", "active");
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
            con.Close();
            con.Dispose();
        }
        return number;
    }


    public int add_new_wardCouncillor_Video(string wardcId,  byte[] video, string contentType, byte[] VideoImage)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_Ward_Councillor_Video"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", 0);
            cmd.Parameters.AddWithValue("@wardcId", wardcId);
            cmd.Parameters.AddWithValue("@video", video);
            cmd.Parameters.AddWithValue("@contentType", contentType);
            cmd.Parameters.AddWithValue("@VideoImage", VideoImage);
            cmd.Parameters.AddWithValue("@status", "active");
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
            con.Close();
            con.Dispose();
        }
        return number;
    }


    public int add_new_wardCouncillor_Video_url(string wardcId, string VideoUrl, string contentType, byte[] VideoImage)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_Ward_Councillor_Video_url"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", 0);
            cmd.Parameters.AddWithValue("@wardcId", wardcId);
            cmd.Parameters.AddWithValue("@VideoUrl", VideoUrl);
            cmd.Parameters.AddWithValue("@contentType", contentType);
            cmd.Parameters.AddWithValue("@VideoImage", VideoImage);
            cmd.Parameters.AddWithValue("@status", "active");
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
            con.Close();
            con.Dispose();
        }
        return number;
    }


    public DataTable FetchWardCouncillor_Image()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select id from wardCouncillorImage where status='active' order by id asc", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    public DataTable FetchWardCouncillor_Image_byId(string Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select id from wardCouncillorImage where status='active' and wardcId=@id order by id asc", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@id", Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    public DataTable FetchWardCouncillor_Video()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select id from wardConcillorVideo where status='active' order by id asc", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    public DataTable FetchWardCouncillor_Video_byid(string id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select id from wardConcillorVideo where status='active' and wardcId=@id   order by id asc", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    public DataTable FetchWardCouncillor(string Clause)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("show_Ward_Councillor", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@clause", Clause);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    /************************************************************
* Purpose  ::  Adding new updateinsertsectiondesignation employee (updateinsertsectiondesignation.aspx)
* Author   ::  Amrita Singh
* Date     ::  04-04-2019
* ********************************************************/

    public DataTable select_section_designation()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from section_designation ORDER BY alloted_Place ASC", con);/* where status = 'active'*/
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

    public DataTable select_Circle()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from Circle ORDER BY Circle ASC", con);/* where status = 'active'*/
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
    public DataTable select_division()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from Division ORDER BY Division ASC", con);/* where status = 'active'*/
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