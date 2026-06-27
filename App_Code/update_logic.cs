using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
public class update_logic
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /*****************************START*******************************
     Purpose  ::  Update All Draft's Letters(updateletter.aspx)
     Author   ::  Amrita Singh
     Date     ::  24-04-2018    
    * ******************************************************************/
    public DataTable bind_all_admin(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from administrator where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }

    //Added By Amrita Singh
    public int update_new_admin_image(string id,string f_name, string l_name, string u_name, string email_id, string contact, string password, string gen, string contentType,byte[] bytes)
    {
        int number = 0;
        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_admin_image"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@first_name", f_name);
            cmd.Parameters.AddWithValue("@last_name", l_name);
            cmd.Parameters.AddWithValue("@user_name", u_name);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@gender", gen);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact);
            cmd.Parameters.AddWithValue("@content_type", contentType);
            cmd.Parameters.AddWithValue("@admin_image", bytes);
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
            con.Close();
            con.Dispose();
        }
        return number;
    }
    //Added By Amrita Singh
    public int update_new_admin_text(string id,string f_name, string l_name, string u_name, string email_id, string contact, string password, string gen)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(password);
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_admin_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@first_name", f_name);
            cmd.Parameters.AddWithValue("@last_name", l_name);
            cmd.Parameters.AddWithValue("@user_name", u_name);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@gender", gen);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", contact);
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
            con.Close();
            con.Dispose();
        }
        return number;
    }
    public string show_admin_image(string admin_image)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        string img1 = "../handlers/admin.ashx?id=" + admin_image.ToString();
        sb.AppendFormat("<img src='" + img1 + "' class='img-responsive img-thumbnail'/>");
        data = sb.ToString();
        return data;
    }
    /************************************************************
  * Purpose  ::  update main menu(updatemainmenu.aspx)
  * Author   ::  Amrita Singh
  * Date     ::  18-06-2018
  * ********************************************************/
    public DataTable bind_all_main_category(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    public int update_main_menu(string id,string name, string name_h, string title, string title_h, string description, string description_h, string css, string path, string path1)
    {

        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_main_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
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
    public int update_main_menu_text(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css)
    {

        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_main_category_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
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
 * Purpose  ::  update submenuone(updatsubmenuone.aspx)
 * Author   ::  Amrita Singh
 * Date     ::  18-06-2018
 * ********************************************************/
    public DataTable bind_sub_category1(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_1 where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    public int update_sub_menu(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path, string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
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
    public int update_sub_menu_text(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
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
 * Purpose  ::  update submenutwo(updatsubmenutwo.aspx)
 * Author   ::  Amrita Singh
 * Date     ::  18-06-2018
 * ********************************************************/
    public DataTable bind_sub_category2(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_2 where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    public int update_sub_menu_two(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path,string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_two"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
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
    public int update_sub_menu_two_text(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_two_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
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
* Purpose  ::  update submenuthree(updatsubmenuthree.aspx)
* Author   ::  Amrita Singh
* Date     ::  20-06-2018
* ********************************************************/
    public DataTable bind_sub_category3(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_3 where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    public int update_sub_menu_three(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path,string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_three"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
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
    public int update_sub_menu_three_text(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_three_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
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
* Purpose  ::  update submenufour(updatsubmenufour.aspx)
* Author   ::  Amrita Singh
* Date     ::  20-06-2018
* ********************************************************/
    public DataTable bind_sub_category4(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_4 where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    public int update_sub_menu_four(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path,string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_four"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
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
    public int update_sub_menu_four_text(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_four_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
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
* Purpose  ::  update submenufive(updatsubmenufive.aspx)
* Author   ::  Amrita Singh
* Date     ::  20-06-2018
* ********************************************************/
    public DataTable bind_sub_category5(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_5 where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    public int update_sub_menu_five(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path,string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_five"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
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
    public int update_sub_menu_five_text(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_five_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
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
* Purpose  ::  update submenusix(updatsubmenusix.aspx)
* Author   ::  Amrita Singh
* Date     ::  20-06-2018
* ********************************************************/
    public DataTable bind_sub_category6(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_6 where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    public int update_sub_menu_six(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css, string path,string path1)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_six"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
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
    public int update_sub_menu_six_text(string id, string name, string name_h, string title, string title_h, string description, string description_h, string css)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_sub_category_six_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            //cmd.Parameters.AddWithValue("@main_menu", main_menu);
            cmd.Parameters.AddWithValue("@name_h", name_h);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@title_h", title_h);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@description_h", description_h);
            cmd.Parameters.AddWithValue("@css", css);
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
* Purpose  ::  Updating Recruitment(updaterecruitment.aspx)
* Author   ::  Amrita
* Date     ::  09-07-2018
* ********************************************************/
    public int update_recruitment(string id,string title, string description)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_recruitment"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
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
* Purpose  ::  Updating Recruitment(updaterecruitment.aspx)
* Author   ::  Amrita
* Date     ::  26-11-2018
* ********************************************************/
    public int update_recruitment_pdf(string id, string title, string description, byte[] bytes)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_recruitment_pdf"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@pdf_data", bytes);
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
* Purpose  ::  update recruitment(updaterecruitment.aspx)
* Author   ::  Amrita Singh
* Date     ::  09-07-2018
* ********************************************************/
    public DataTable bind_recruitment(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from recruitment where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
    /************************************************************
    * Purpose  ::  Select wardcouncillors 
    * Author   ::  Amrita Singh
    * Date     ::  07-10-2018
    * ********************************************************/
    public DataTable select_WardCauncilors(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,Circleid,Wardid,first_name,last_name,MotherName,FatherName,DOB,Address,email_id,contact_no,HighestQualification,ExperianceDetails,gender from WardCauncilors where id=@id", con);
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

    /************************************************************
   * Purpose  ::  Update Ward Councillors profile
   * Author   ::  Amrita Singh
   * Date     ::  07-10-2018
   * ********************************************************/
    public int update_new_wardCouncillor(string id, string Circleid, string Wardid, string first_name, string last_name, string MotherName, string FatherName, string DOB, string Address, string email_id, string contact_no, string HighestQualification, string ExperianceDetails, string gender)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_Ward_Councillor"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
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
    public int updatepage_ward_image(string id, string content_type, byte[] bytes)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update_Ward_Councillor_image"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@image_data_content_type", content_type);
            cmd.Parameters.AddWithValue("@image_date", bytes);
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
    public string show_ward_image(string admin_image)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        string img1 = "../handlers/wardprofile_image.ashx?id=" + admin_image.ToString();
        sb.AppendFormat("<img src='" + img1 + "' class='img-responsive img-thumbnail'/>");
        data = sb.ToString();
        return data;
    }
    /************************************************************
* Purpose  ::  update section Designation(updateinsertsectiondesignation.aspx)
* Author   ::  Amrita Singh
* Date     ::  02-11-2018
* ********************************************************/
    public DataTable bind_sectiondesignation(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from section_designation where id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }

        return dt;
    }
  
    /************************************************************
     *
* Purpose  ::  Updating Recruitment(updaterecruitment.aspx)
* Author   ::  Amrita
* Date     ::  09-07-2018
* ********************************************************/
    public int update_sectiondesignation(string id,string branch,string alloted_Place, string section_name, string designation, string full_name, string contact_no, string email_id, string status,int c_id, int d_id)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("update section_designation set added_by=@added_by,alloted_Place=@alloted_Place,branch=@branch,section_name=@section_name,designation=@designation,full_name=@full_name,contact_no=@contact_no,email_id=@email_id,since=@since,status=@status, c_id=@c_id,d_id=@d_id where id=@id", con))
        {
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@branch", branch);
            cmd.Parameters.AddWithValue("@alloted_Place", alloted_Place);
            cmd.Parameters.AddWithValue("@section_name", section_name);
            cmd.Parameters.AddWithValue("@designation", designation);
            cmd.Parameters.AddWithValue("@full_name", full_name);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@c_id", c_id);
            cmd.Parameters.AddWithValue("@d_id", d_id);
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


  
}