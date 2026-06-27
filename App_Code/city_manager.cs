using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for city_manager
/// </summary>
public class city_manager
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Find Circle ID
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string find_circle_id(string user_id)
    {
        string circle = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT id,circle from users where id=@id", con);
            cmd.Parameters.AddWithValue("@id", user_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                circle=dt.Rows[0]["circle"].ToString();
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
        return circle;
    }

    /************************************************************
     * Purpose  ::  City manager Count
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable count_all_complaints_in_circle(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name from combatcellTitle where status='true' and circle=@circle_id order by id DESC", con);
            cmd.Parameters.AddWithValue("@circle_id", circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public string getsender(string userid)
    {
        string sender = string.Empty;

        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,first_name+''+last_name as Fullname   from users where status='Active'  and id=@id", con);
            cmd.Parameters.Add("@id", userid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            if (ds.Tables[0] != null)
            {
                sender = ds.Tables[0].Rows[0]["Fullname"].ToString();
            }
        }
        return sender;

    }

    // pending at si
    public DataTable BindAllComplains_received_all_pending_combat_si_pending_city_manager(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@circle_id order by b.id desc", con);
            cmd.Parameters.AddWithValue("@circle_id", circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    // pending at csi
    public DataTable BindAllComplains_received_all_pending_combat_csi_pending_city_manager(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.ComplainNo,b.circle,b.ward,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@circle_id order by b.id desc", con);
            cmd.Parameters.AddWithValue("@circle_id",circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // closed at list for cirlcce
    public DataTable all_closed_complaints_in_circle(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name from combatcellTitle where status='true' and complainstatus='closed' and circle=@circle_id order by id DESC", con);
            cmd.Parameters.AddWithValue("@circle_id", circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // overtime at Csi
    public DataTable BindAllComplains_sanitization_officer_overtime_at_csi(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo as comp_no,b.title,b.ComplainStatus,b.circle,c.id,c.ComplainNo,c.fw_DateTime,(DATEDIFF(HOUR ,(c.fw_DateTime) , getdate()))-( d.TimelimitinhrsforSI) as AboveTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c, CombatCellSubcategory d where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.subcategoryId=d.id and DATEDIFF(HOUR ,(c.fw_DateTime) , getdate())  >d.TimelimitforCSI and b.circle=@circle_id order by b.id desc", con);
            cmd.Parameters.AddWithValue("@circle_id", circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    /****************************************************************
     * Till Here
     * ***************************************************************/
}