using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for page_two_controller
/// </summary>
public class page_two_controller
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
    * Purpose  ::  Bind First sub menu
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public string create_first_sub_menu(string menu_id, string page_type)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,has_links from category_2 where status='active' and category_1_id=@menu_id order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if (page_type == "1")
                    {
                        ////if(dt.Rows[i]["has_links"].ToString() =="yes")
                        ////{
                        //    sb.AppendFormat("<li>");
                        //        sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name"].ToString() + "</a>");
                        //    sb.AppendFormat("</li>");
                        //}
                        //else
                        //{
                            sb.AppendFormat("<li>");
                                sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=0'>" + dt.Rows[i]["name"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                       // }
                    }
                    else
                    {
                        if (dt.Rows[i]["has_links"].ToString() == "yes")
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                    }
                    i++;
                }
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
        menu = sb.ToString();
        return menu;
    }

    public string create_first_sub_menu_my_m(string menu_id, string page_type)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,has_links from category_2 where status='active' and category_1_id=@menu_id order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if (page_type == "1")
                    {
                        if (dt.Rows[i]["has_links"].ToString() == "yes")
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                    }
                    else
                    {
                        if (dt.Rows[i]["has_links"].ToString() == "yes")
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                    }
                    i++;
                }
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
        menu = sb.ToString();
        return menu;
    }


    public string create_first_sub_menu_three(string menu_id, string page_type)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,show_order,has_links from category_1 where status='active' and category_id=@menu_id order by show_order asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if (page_type == "1")
                    {
                        ////if(dt.Rows[i]["has_links"].ToString() =="yes")
                        ////{
                        //    sb.AppendFormat("<li>");
                        //        sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name"].ToString() + "</a>");
                        //    sb.AppendFormat("</li>");
                        //}
                        //else
                        //{
                        sb.AppendFormat("<li>");
                        sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name"].ToString() + "</a>");
                        sb.AppendFormat("</li>");
                        // }
                    }
                    else
                    {
                        if (dt.Rows[i]["has_links"].ToString() == "yes")
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                    }
                    i++;
                }
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
        menu = sb.ToString();
        return menu;
    }


    public string find_main_category_page_two(string menu_id)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,category_id from category_1 where status='active' and category_id=@menu_id order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    sb.AppendFormat("<li>");
                    sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=0'>" + dt.Rows[i]["name"].ToString() + "</a>");
                    sb.AppendFormat("</li>");
                    i++;
                }
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
        menu = sb.ToString();
        return menu;
    }

    /************************************************************
     * Purpose  ::  Find Page Name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_first_sub_menu_navigation(string menu_id, string page_id)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,show_order from category_1 where status='active' and id=@menu_id order by show_order asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if (page_id == "1")
                    {
                        sb.AppendFormat("" + dt.Rows[i]["name"].ToString() + "");
                    }
                    else
                    {
                        sb.AppendFormat("" + dt.Rows[i]["name_h"].ToString() + "");
                    }
                    i++;
                }
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
        menu = sb.ToString();
        return menu;
    }

    /************************************************************
     * Purpose  ::  Bind First sub Menu Data
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_first_sub_menu_data(string menu_id, string page_id)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,description,description_h from category_1 where status='active' and id=@menu_id  order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if (page_id == "1")
                    {
                        sb.AppendFormat("" + dt.Rows[i]["description"].ToString() + "");
                    }
                    else
                    {
                        sb.AppendFormat("" + dt.Rows[i]["description_h"].ToString() + "");
                    }
                    i++;
                }
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
        menu = sb.ToString();
        return menu;
    }

    /************************************************************
     * Purpose  ::  Find Page Name 
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_first_sub_menu_navigation_middle(string menu_id, string page_id)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT a.id,a.name,a.name_h,b.id,b.category_id from category a, category_1 b where a.id=b.category_id and b.status='active' and b.id=@menu_id  order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if (page_id == "1")
                    {
                        sb.AppendFormat("" + dt.Rows[i]["name"].ToString() + "");
                    }
                    else
                    {
                        sb.AppendFormat("" + dt.Rows[i]["name_h"].ToString() + "");
                    }
                    i++;
                }
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
        menu = sb.ToString();
        return menu;
    }

    /**********************************************************************
     * Create down Menu
     * ********************************************************************/
    public DataTable all_links(string menu_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,bg_image_path,has_links from category_2 where status='active' and category_1_id=@menu_id  order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
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

    public DataTable all_links_one(string menu_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,bg_image_path,has_links from category_2 where status='active' and category_1_id=@menu_id  order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
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

    public void find_first_category()
    {

    }

    public DataTable my_page_background(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,back_image from category_1 where status='active' and id=@id", con);
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

    // find is menu has links
    public DataTable find_has_links(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,has_links from category_1 where status='active' and id=@id", con);
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


    // find is menu has links
    public DataTable find_has_links_new(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,has_links from category_1 where status='active' and id=@id", con);
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

    // find main category
    public DataTable find_main_category(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,category_id from category_1 where status='active' and id=@id", con);
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

    /**************************************************************
     * Till Here
     * ***********************************************************/
	
}