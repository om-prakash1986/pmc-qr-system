using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text;

/// <summary>
/// Summary description for create_main_menu
/// </summary>
public class create_main_menu
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Create About The City
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_about_the_city(string menu_id,string page_type)
    {
        string menu = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,has_links from category_1 where status='active' and category_id=@menu_id order by show_order asc", con);
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
                        if (dt.Rows[i]["has_links"].ToString() != "yes")
                        {
                            sb.AppendFormat("<li>");
                                sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li>");
                                sb.AppendFormat("<a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name"].ToString() + "<i class='fa fa-angle-right' aria-hidden='true'></i></a>");
                                    sb.Append("<ul class='sub-menu'>");
                                        DataTable link = find_all_sub_menu(dt.Rows[i]["id"].ToString());
                                        if (link != null)
                                        {
                                            if (link.Rows.Count > 0)
                                            {
                                                int j = 0;
                                                while (j < link.Rows.Count)
                                                {
                                                    sb.AppendFormat("<li><a href='subpagethree.aspx?id=" + link.Rows[j]["id"].ToString() + "&cont=0'>" + link.Rows[j]["name"].ToString() + "</a></li>");
                                                    j++;
                                                }
                                            }
                                        }
                                    sb.Append("</ul>");
                            sb.AppendFormat("</li>");
                        }
                    }
                    else
                    {
                        if (dt.Rows[i]["has_links"].ToString() != "yes")
                        {
                            sb.AppendFormat("<li>");
                            sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=1'>" + dt.Rows[i]["name"].ToString() + "</a>");
                            sb.AppendFormat("</li>");
                        }
                        else
                        {
                            sb.AppendFormat("<li><a href='subpagetwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'>" + dt.Rows[i]["name"].ToString() + "<i class='fa fa-angle-right' aria-hidden='true'></i></a>");
                                sb.Append("<ul class='sub-menu'>");
                                    DataTable link = find_all_sub_menu(dt.Rows[i]["id"].ToString());
                                    int j = 0;
                                    while (j < link.Rows.Count)
                                    {
                                        sb.AppendFormat("<a href='subpagethree.aspx?id=" + link.Rows[j]["id"].ToString() + "&cont=0'>" + link.Rows[j]["name"].ToString() + "</a>");
                                        j++;
                                    }
                                sb.Append("</ul>");
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

    public DataTable find_all_sub_menu(string menu_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h,has_links from category_2 where status='active' and category_1_id=@menu_id order by name asc", con);
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
            //con.Close();
           // con.Dispose();
        }
        return dt;
    }
}