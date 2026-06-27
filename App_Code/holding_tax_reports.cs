using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for holding_tax_reports
/// </summary>
public class holding_tax_reports
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  All Pending Property For USers
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_pending_property_details(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from property_details WHERE added_by=@user_id and (form_status !='completed' or form_status is null) ORDER by since DESC", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
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
     * Purpose  ::  Find Owner NAme
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable owner_name(string PID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select OwnerName from owner_details where pid= @pid", con);
            cmd.Parameters.AddWithValue("@pid", PID);
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

    public string create_user_profile_users(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            //change
            SqlCommand cmd = new SqlCommand("SELECT * from holding_user where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                string img1 = "";
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    //if (dt.Rows[i]["content_type"].ToString() != "")
                    //{
                    //    img1 = "handlers/user.ashx?id=" + user_id.ToString();
                    //}
                    //else
                    {
                        img1 = "assets/images/users/3.png";
                    }

                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["first_name"].ToString() + "</b></a></li>");
                    sb.AppendFormat("<li class='divider'></li>");
                    sb.AppendFormat("<li><a href='viewaccount.aspx'> View Account </a></li>");
                    sb.AppendFormat("<li class='divider'></li>");
                    sb.AppendFormat("<li><a href='logout.aspx'>Logout</a></li>");
                    sb.AppendFormat("</ul >");
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
        number = sb.ToString();
        return number;
    }

    public string create_admin_profile_users(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            //change
            SqlCommand cmd = new SqlCommand("SELECT * from holding_admin where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                string img1 = "";
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    //if (dt.Rows[i]["content_type"].ToString() != "")
                    //{
                    //    img1 = "handlers/user.ashx?id=" + user_id.ToString();
                    //}
                    //else
                    {
                        img1 = "../assets/images/users/3.png";
                    }

                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["first_name"].ToString() + "</b></a></li>");
                    sb.AppendFormat("<li class='divider'></li>");
                    sb.AppendFormat("<li><a href='viewaccount.aspx'> View Account </a></li>");
                    sb.AppendFormat("<li class='divider'></li>");
                    sb.AppendFormat("<li><a href='logout.aspx'>Logout</a></li>");
                    sb.AppendFormat("</ul >");
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
        number = sb.ToString();
        return number;
    }

    /************************************************************
     * Purpose  ::  All Completed Property Property For USers
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_completed_property_details(string user_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from property_details WHERE added_by=@user_id and form_status='completed' ORDER by since DESC", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
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

    // find forward to page
    public DataTable find_current_status(string PID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select PlotArea,ConstructedArea,form_status,propertytype from property_details WHERE CustPID=@pid", con);
            cmd.Parameters.AddWithValue("@pid", PID);
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
     * Purpose  ::  All unverified Property Details
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_unverified_property_details(string user_id, string circle_name)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from property_details WHERE form_status='completed' and CircleName=@Circle_name and (is_verified is null or is_verified !='yes') ORDER by since DESC", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            cmd.Parameters.AddWithValue("@Circle_name", circle_name);
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

    /**********************************************************
     * Till Here
     * *******************************************************/
}