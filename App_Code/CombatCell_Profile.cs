using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for CombatCell_Profile
/// </summary>
public class CombatCell_Profile
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  District SP Profile
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_user_profile(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from sp_districts where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    string img1 = "../handlers/sp_district.ashx?id=" + user_id.ToString();
                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["district_name"].ToString() + "</b></a></li>");
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
     * Purpose  ::  Department Nodal Profile
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_user_profile_department(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from departments where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    string img1 = "../handlers/department.ashx?id=" + user_id.ToString();
                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["department_name"].ToString() + "</b></a></li>");
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
     * Purpose  ::  District District Profile
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_user_profile_district(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from districts where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    string img1 = "../handlers/districts.ashx?id=" + user_id.ToString();
                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["district_name"].ToString() + "</b></a></li>");
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
     * Purpose  ::  Division Profile
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_user_profile_division(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from divisions where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    string img1 = "../handlers/divisions.ashx?id=" + user_id.ToString();
                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["division_name"].ToString() + "</b></a></li>");
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
   * Purpose  ::  User's user Profile
   * Author   ::  Amrita Singh
   * Date     ::  26-05-2018
   * ********************************************************/
    public string create_user_profile_users(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from users where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    string img1 = "handlers/user.ashx?id=" + user_id.ToString();
                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["user_name"].ToString() + "</b></a></li>");
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
* Purpose  ::  User's user Profile
* Author   ::  Amrita Singh
* Date     ::  26-05-2018
* ********************************************************/
    public string create_user_profile_corporation(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from corporations where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    string img1 = "handlers/corporations.ashx?id=" + user_id.ToString();
                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["user_name"].ToString() + "</b></a></li>");
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
  * Purpose  ::  Report's user Profile
  * Author   ::  Amrita Singh
  * Date     ::  27-05-2018
  * ********************************************************/
    public string create_user_profile_report(string user_id)
    {
        string number = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * from users where id=@user_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    string img1 = "../handlers/user.ashx?id=" + user_id.ToString();
                    sb.AppendFormat("<a data-toggle='dropdown' class='dropdown-toggle' href='#'>");
                    sb.AppendFormat("<span><img alt='image' class='img-circle img-responsive new_show' src='" + img1 + "' style='height:30px;width:30px; margin:10px !important;'/></span>");
                    sb.AppendFormat("</a>");
                    sb.AppendFormat("<ul class='dropdown-menu animated fadeInRight m-t-xs'>");
                    sb.AppendFormat("<li><a href='#'><b>" + dt.Rows[i]["user_name"].ToString() + "</b></a></li>");
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


    public DataTable fetchNodalListdepartment()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as dept_id,a.department_name,a.status as dept_status,b.id as nodal_id,b.department_id,b.name,b.email_id,b.contact_no,b.designation,b.status from departments a left join nodal_officers b on a.id=b.department_id where a.status='active' order by a.department_name ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable fetchNodalListdivision()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as div_id,a.division_name,a.status as div_status,b.id as nodal_id,b.department_id,b.name,b.email_id,b.contact_no,b.designation,b.status from divisions a left join nodal_officers b on a.id=b.division_id where a.status='active' order by a.division_name ASC ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable fetchNodalListdistrict()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as dis_id,a.district_name,a.status as div_status,b.id as nodal_id,b.department_id,b.name,b.email_id,b.contact_no,b.designation,b.status from districts a  left join  nodal_officers b on a.id=b.district_id  where  a.status='active' order by a.district_name ASC ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable fetchNodalListsp()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as sp_id,a.district_name,a.status as div_status,b.id as nodal_id,b.department_id,b.name,b.email_id,b.contact_no,b.designation,b.status from sp_districts a left join nodal_officers b on a.id=b.sp_id order by a.district_name ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable fetchNodalListcorporation()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as corp_id,a.corporation_name,a.status as div_status,b.id as nodal_id,	b.department_id,b.name,b.email_id,b.contact_no,b.designation,b.status from corporations a  left join nodal_officers b on  a.id=b.corporation_id  where a.status='active' order by a.corporation_name ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    public string Update_nodal_officer_All_details(string nodalid, string departmentid, string divisionid, string districtid, string district_sp_id, string corporation_id, string name, string email_id, string contact_no, string designation, string username, string password)
    {
        string auth = "0";

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Update_nodal_officer_All_details"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nodalid", nodalid);
            cmd.Parameters.AddWithValue("@departmentid", departmentid);
            cmd.Parameters.AddWithValue("@divisionid", divisionid);
            cmd.Parameters.AddWithValue("@districtid ", districtid);
            cmd.Parameters.AddWithValue("@district_sp_id ", district_sp_id);

            cmd.Parameters.AddWithValue("@corporation_id", corporation_id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@designation", designation);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@user_name", username);

            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                auth = "1";
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
        return auth;
    }

}