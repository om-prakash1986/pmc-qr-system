using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// All Repeater data
/// </summary>
public class repeater_counts
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    /***********************************************
     * Author  : Arnav
     * Date    : 15-06-2018
     * Purpose : Find News Content
     * ***********************************************/
    public string find_image_content(string image_id)
    {
        string title = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from news where status='active' and id=@id Order by id DESC", con);
            cmd.Parameters.AddWithValue("@id", image_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    title = HttpUtility.HtmlEncode(dt.Rows[i]["news_title"].ToString().Trim());
                    if (title.Length > 50)
                    {
                        title = title.Substring(0, 50);
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
        return title;
    }

    /***********************************************
     * Author  : Arnav
     * Date    : 15-06-2018
     * Purpose : Find News Content
     * ***********************************************/
    public DataTable find_news_data(string news_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from news where status='active' and id=@id Order by id DESC", con);
            cmd.Parameters.AddWithValue("@id", news_id);
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

    /***********************************************
     * Author  : Arnav
     * Date    : 15-06-2018
     * Purpose : Find News Content
     * ***********************************************/
    public DataTable bind_latest_news(string news_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select top 5 id,news_title,news_title_h,status,since where status='active' and id<>@id Order by id DESC", con);
            cmd.Parameters.AddWithValue("@id", news_id);
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