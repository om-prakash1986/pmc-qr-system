using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for page_three_controller
/// </summary>
public class page_three_controller
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
    * Purpose  ::  Bind First Msub menu
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public string create_first_sub_menu(string menu_id, string page_type)
    {
        string menu = "";

        string data = find_find_page_two_id(menu_id);
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h from category_2 where status='active' and category_1_id=@menu_id order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", data);
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
                        sb.AppendFormat("<li>");
                        sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=0'>" + dt.Rows[i]["name"].ToString() + "</a>");
                        sb.AppendFormat("</li>");
                    }
                    else
                    {
                        sb.AppendFormat("<li>");
                        sb.AppendFormat("<a href='subpagethree.aspx?id=" + dt.Rows[i]["id"].ToString() + "&cont=0'>" + dt.Rows[i]["name_h"].ToString() + "</a>");
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
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h from category_2 where status='active' and id=@menu_id order by name asc", con);
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
            SqlCommand cmd = new SqlCommand("SELECT id,description,description_h from category_2 where status='active' and id=@menu_id order by name asc", con);
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
            SqlCommand cmd = new SqlCommand("SELECT id,name,name_h,title,title_h from category_2 where status='active' and id=@menu_id  order by name asc", con);
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
    * Purpose  ::  Find Page id
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public string find_find_page_two_id(string id)
    {
        string data = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,category_1_id from category_2 where status='active' and id=@menu_id order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    data = dt.Rows[i]["category_1_id"].ToString();
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
        return data;
    }

    /************************************************************
   * Purpose  ::  Find Page name
   * Author   ::  Arnav
   * Date     ::  24-03-2017
   * ********************************************************/
    public string find_find_page_two_name(string id,string page_id)
    {
        string data = "";
        SqlConnection con = new SqlConnection(strcon);
        StringBuilder sb = new StringBuilder();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT a.id,a.category_1_id,b.id, b.name,b.name_h from category_2 a, category_1 b where a.status='active' and a.id=@menu_id and a.category_1_id =b.id order by b.name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", id);
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
                        data = dt.Rows[i]["name"].ToString();
                    }
                    else
                    {
                        data = dt.Rows[i]["name_h"].ToString();
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
        return data;
    }

    /************************************************************
     * Purpose  ::  Find Page Name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable my_page_background(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,back_image from category_2 where status='active' and id=@menu_id order by name asc", con);
            cmd.Parameters.AddWithValue("@menu_id", id);
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

    /***********************************************************
     * Till Here
     * *********************************************************/
}