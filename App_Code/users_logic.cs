using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

public class users_logic
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /*************************************************
   Author : Amrita Singh
   Date :  23-06-2108
   Purpose : Binding All Tender(tenders.aspx)
   *************************************************/
    public string bind_all_tender()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from tender order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int j = 1;
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>");
                    sb.AppendFormat(j.ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["tender_no"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["start_date"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["end_date"].ToString());
                    sb.AppendFormat("</td>");


                    sb.AppendFormat("<td>");
                    sb.Append("<a href='downloadtender.aspx?id=" + dt.Rows[i]["id"].ToString() + "' target='_blank'><img src='assets/images/download.png'></a>");               
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("</tr>");
                    j++;
                    i++;
                }
            }
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
        data = sb.ToString();
        return data;
    }
    /*****************End(noticeboard.aspx)************/
    /*************************************************
  Author : Amrita Singh
  Date :  23-06-2108
  Purpose : Binding All Notice(noticeboard.aspx)
  *************************************************/
    public string bind_all_notice()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from notice_board order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int j = 1;
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>");
                    sb.AppendFormat(j.ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["start_date"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["end_date"].ToString());
                    sb.AppendFormat("</td>");


                    sb.AppendFormat("<td>");
                    sb.Append("<a href='downloadnotice.aspx?id=" + dt.Rows[i]["id"].ToString() + "' target='_blank'><img src='assets/images/download.png'></a>");
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("</tr>");
                    j++;
                    i++;
                }
            }
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
        data = sb.ToString();
        return data;
    }
    /*****************End(noticeboard.aspx)************/
    /*************************************************
      Author : Amrita Singh
      Date :  09-07-2108
      Purpose : Binding All Recruitment(recruitment.aspx)
    *************************************************/
    public DataTable bind_all_recruitment()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from recruitment where status='active' order by id desc", con);
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

    /*************************************************
        Author : Amrita Singh
        Date :  09-07-2108
        Purpose : Binding All JOB Description(jobdescription.aspx)
    *************************************************/
    public string bind_all_jobdescription_title(string id)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select title from recruitment where id=@id order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", id);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int j = 1;
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    
                    sb.AppendFormat("<h3 style='text-align:center;'>"+dt.Rows[i]["title"].ToString()+"</hr>");

                    
                    j++;
                    i++;
                }
            }
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
        data = sb.ToString();
        return data;
    }
    public string bind_all_jobdescription_description(string id)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select description from recruitment where id=@id order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", id);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int j = 1;
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    
                    sb.AppendFormat("<p style='color:#000 !important'>"+dt.Rows[i]["description"].ToString()+"</p>");

                    
                    j++;
                    i++;
                }
            }
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
        data = sb.ToString();
        return data;
    }
}