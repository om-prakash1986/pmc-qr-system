using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text;
public class ward_councillor
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

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
    public DataTable bindWard(string Circleid)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,WardNo from tblWard where status=1 and Circleid=@Circleid order by WardNo ASC", con);
            cmd.Parameters.AddWithValue("@Circleid", Circleid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
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

}