using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for menu_controller
/// </summary>
public class menu_controller
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Main Menu Controller
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_main_menu(string page_type)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h from category where status='active'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                 int i = 0;
                 while (i < dt.Rows.Count)
                 {
                     if(page_type =="1")
                     {
                         sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpage.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name"].ToString() + "</a>");
                         sb.AppendFormat("</li>");
                     }
                     else
                     {
                         sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpage.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                         sb.AppendFormat("</li>");
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
     * Purpose  ::  Bind First Msub menu
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
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,show_order,has_links from category_1 where status='active' and category_id=@menu_id  order by show_order asc", con);
            cmd.Parameters.AddWithValue("@menu_id",menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if(page_type =="1")
                    {
                        if (dt.Rows[i]["has_links"].ToString() !="yes")
                        {
                            sb.AppendFormat("<li>");
                                sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li>");
                                sb.AppendFormat("<a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                    }
                    else
                    {
                        if (dt.Rows[i]["has_links"].ToString() != "yes")
                        {
                            sb.AppendFormat("<li>");
                                sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li>");
                                sb.AppendFormat("<a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
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

    /************************************************************
     * Purpose  ::  Find Page Name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_first_sub_menu_navigation(string menu_id,string page_id)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h from category where status='active' and id=@menu_id   order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if(page_id =="1")
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
    public string create_first_sub_menu_data(string menu_id,string page_id)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,description,description_h from category where status='active' and id=@menu_id  order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", menu_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if(page_id =="1")
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

    /**********************************************************************
     * Create down Menu
     * ********************************************************************/
    public DataTable all_links(string menu_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,bg_image_path,show_order from category_1 where status='active' and category_id=@menu_id  order by show_order asc", con);
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


    public DataTable my_page()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,title,description,bg_image_path from category where status='active'", con);
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

    public DataTable my_page_background(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,back_image from category where status='active' and id=@id", con);
            cmd.Parameters.AddWithValue("@id",id);
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