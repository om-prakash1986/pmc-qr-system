using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for combat_reports
/// </summary>
public class combat_reports
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  All Wards Wise Report
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable all_wards_comparisation(string circle)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        try
        {
            if (circle == "0")
            {
                SqlCommand cmd = new SqlCommand("select id,Circleid,WardNo,status,[dbo].[find_circle_name](circleid) as circle_name,[dbo].[find_all_complaints_in_ward](id) as all_complaints,[dbo].[find_all_closed_complaints_in_ward](id) as closed_count,[dbo].[find_AvgTime_closed_complaints_in_ward_Si] (id) as avgtimeinsi,[dbo].[find_AvgTime_closed_complaints_in_ward_CSi] (id) as avgtimeinCsi from tblWard where status='1' order by WardNo Asc", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if (circle != "0")
            {
                SqlCommand cmd = new SqlCommand("select id,Circleid,WardNo,status,[dbo].[find_circle_name](circleid) as circle_name,[dbo].[find_all_complaints_in_ward](id) as all_complaints,[dbo].[find_all_closed_complaints_in_ward](id) as closed_count,[dbo].[find_AvgTime_closed_complaints_in_ward_Si] (id) as avgtimeinsi,[dbo].[find_AvgTime_closed_complaints_in_ward_CSi] (id) as avgtimeinCsi from tblWard where status='1' and Circleid=@circle_id order by WardNo Asc", con);
                cmd.Parameters.AddWithValue("@circle_id", circle);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
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
        return dt;
    }

    public DataTable bindall_modes()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,mode from CombatCellMode order by mode ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // pending at ward si
    public DataTable BindAllComplains_received_all_pending_combat_si_pending_ward_wise(string ward, string mode, string catagory, string subcactagory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            if (mode == "0" && catagory == "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only mode is selected
            else if (mode != "0" && catagory == "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.mode=@mode order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only catagory is selected
            else if (mode == "0" && catagory != "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.categoryid=@catagory order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", catagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only sub catagory is selected
            else if (mode == "0" && catagory != "0" && subcactagory != "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.subcategoryId=@catagory order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", subcactagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only mode and category is selected
            else if (mode != "0" && catagory != "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.categoryid=@catagory and b.mode=@mode order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", catagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // mode and subcatagory is selected
            else if (mode != "0" && catagory != "0" && subcactagory != "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.subcategoryId=@catagory and b.mode=@mode order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", subcactagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
        }
        return dt;
    }

    // pending at ward csi
    public DataTable BindAllComplains_received_all_pending_combat_csi_pending_ward_wise(string ward, string mode, string catagory, string subcactagory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            //SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id order by b.id desc", con);
            //cmd.Parameters.AddWithValue("@ward_id", ward);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //da.Fill(ds);
            //dt = ds.Tables[0];

            if (mode == "0" && catagory == "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only mode is selected
            else if (mode != "0" && catagory == "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.mode=@mode order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only catagory is selected
            else if (mode == "0" && catagory != "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.categoryid=@catagory order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", catagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only sub catagory is selected
            else if (mode == "0" && catagory != "0" && subcactagory != "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.subcategoryId=@catagory order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", subcactagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // only mode and category is selected
            else if (mode != "0" && catagory != "0" && subcactagory == "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.categoryid=@catagory and b.mode=@mode order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", catagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // mode and subcatagory is selected
            else if (mode != "0" && catagory != "0" && subcactagory != "0")
            {
                SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.mode,b.id,b.ComplainNo,b.categoryid,[dbo].[find_category_name](b.categoryid) as m_category,[dbo].[find_subcategory_name](b.subcategoryId) as m_sub_category,b.subcategoryId,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.subcategoryId=@catagory and b.mode=@mode order by b.id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", subcactagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
        }
        return dt;
    }

    // all complaints in ward
    public DataTable all_complaints_in_ward(string ward, string mode, string catagory, string subcatagory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            // if none available
            if (mode == "0" && catagory == "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if only mode is available
            else if (mode != "0" && catagory == "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and mode=@mode order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if only catagory is available
            else if (mode == "0" && catagory != "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and categoryId=@category order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@category", catagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if catagory and subcatagory is available
            else if (mode == "0" && catagory != "0" && subcatagory != "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and subcategoryId=@category order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@category", subcatagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if mode and catagory is available
            else if (mode != "0" && catagory != "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and categoryId=@category and mode=@mode order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@category", catagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // every thing is available
            else if (mode != "0" && catagory != "0" && subcatagory != "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and subcategoryId=@category and mode=@mode order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@category", catagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
        }
        return dt;
    }

    // closed complaints
    public DataTable all_closed_complaints_in_ward(string ward, string mode, string catagory, string subcatagory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            if (mode == "0" && catagory == "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and ComplainStatus='closed' order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if only mode is available
            else if (mode != "0" && catagory == "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and ComplainStatus='closed' and mode=@mode and mode=@mode order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if only catagory is available
            else if (mode == "0" && catagory != "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and ComplainStatus='closed' and categoryid=@catagory order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", catagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if catagory and subcatagory is available
            else if (mode == "0" && catagory != "0" && subcatagory != "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and ComplainStatus='closed' and subcategoryId=@catagory order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", subcatagory);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // if mode and catagory is available
            else if (mode != "0" && catagory != "0" && subcatagory == "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and ComplainStatus='closed' and mode=@mode and categoryid=@catagory order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", catagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            // every thing is available
            else if (mode != "0" && catagory != "0" && subcatagory != "0")
            {
                SqlCommand cmd = new SqlCommand("Select *,[dbo].[find_category_name](categoryid) as category,[dbo].[find_subcategory_name](subcategoryId) as sub_category from combatcelltitle where status='true' and ward=@ward_id and ComplainStatus='closed' and mode=@mode and subcategoryId=@catagory order by id desc", con);
                cmd.Parameters.AddWithValue("@ward_id", ward);
                cmd.Parameters.AddWithValue("@catagory", catagory);
                cmd.Parameters.AddWithValue("@mode", mode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
        }
        return dt;
    }

    // find ward name
    public string fins_ward_name(string ward_id)
    {
        string name = "";
        SqlConnection con = new SqlConnection(strcon);

        try
        {
            SqlCommand cmd = new SqlCommand("select id,WardNo from tblWard where id=@id", con);
            cmd.Parameters.AddWithValue("@id", ward_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["WardNo"].ToString();
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
        return name;
    }



    // find current status
    public DataTable find_forwarded_to_si_CSI(string id)
    {
        DataTable data = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 a.*,b.UserTypeid,b.first_name+' '+b.last_name as Name,c.authority_name from CombatCell_forward_list a left join users b on a.fw_officer_Id=b.id left join  authority_details c on b.UserTypeid=c.id where  ComplainNo=@id order by fw_DateTime desc", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                data = dt;
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


    public string find_image_path(string image_id)
    {
        string path = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id,combatcelltitleid,image_id,imagepath,contenttype from CombatCellAttatchment where image_id=@id", con);
            cmd.Parameters.AddWithValue("@id", image_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                path = dt.Rows[0]["imagepath"].ToString();
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
        return path;
    }




    /************************************************************
    * Purpose  ::  All Circle Wise Report
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public DataTable all_Circle_comparisation()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id,Circle as circle_name,status,[dbo].[find_all_complaints_in_Circle](id) as all_complaints,[dbo].[find_all_closed_complaints_in_Circle](id) as closed_count,[dbo].[find_AvgTime_closed_complaints_in_circle](id) as AvgTime from Circle where status='1' order by Circle Asc", con);
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

    #region Filter Report Circle
    // add new filter sarfraz
    public DataTable all_Circle_comparisation_filter()
    {




        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id,Circle as circle_name,status from Circle where status='1' order by Circle Asc", con);
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


    public DataTable find_close_complain(string Circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle where Circle=@Circle_id and ComplainStatus='Closed'", con);
            cmd.Parameters.AddWithValue("@Circle_id", Circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }

    public DataTable find_close_complain_filter(string circle_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto  and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode=@Mode and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }
            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode<>@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }


    public DataTable findall_complaints(string Circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and status='true'", con);
            cmd.Parameters.AddWithValue("@Circle_id", Circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }

    public DataTable findall_complaints_filter(string circle_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode=@Mode and status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }
            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where Circle=@Circle_id and DOE between @datefrom and @dateto and Mode<>@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }


    public DataTable pending_at_si_filter(string circle_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(distinct(b.id)) as nummber from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(distinct(b.id)) as nummber from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(distinct(b.id)) as nummber from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select count(distinct(b.id)) as nummber from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }
            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select count(distinct(b.id)) as nummber from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode<>@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }


    public DataTable pending_at_csi_filter(string circle_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(distinct b.id) as number from users a,combatcellTitle b, CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(distinct b.id) as number from users a,combatcellTitle b, CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(distinct b.id) as number from users a,combatcellTitle b, CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select count(distinct b.id) as number from users a,combatcellTitle b, CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }
            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select count(distinct b.id) as number from users a,combatcellTitle b, CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id and b.DOE between @datefrom and @dateto and b.Mode<>@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@Circle_id", circle_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }

    // ward Count Complain
    public DataTable find_close_complain_ward(string ward_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle where ward=@ward_id and ComplainStatus='Closed'", con);
            cmd.Parameters.AddWithValue("@ward_id", ward_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }

    public DataTable find_close_complain_filter_ward(string ward_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle where ward=@ward_id and DOE between @datefrom and @dateto  and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle where ward=@ward_id and DOE between @datefrom and @dateto and Mode=@Mode and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle where ward=@ward_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle where ward=@ward_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }

            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle where ward=@ward_id and DOE between @datefrom and @dateto and Mode<>@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and ComplainStatus='Closed'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }


    public DataTable findall_complaints_ward(string ward_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where ward=@ward_id and status='true'", con);
            cmd.Parameters.AddWithValue("@ward_id", ward_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }

    public DataTable findall_complaints_filter_ward(string ward_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where ward=@ward_id and DOE between @datefrom and @dateto and status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);

            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where ward=@ward_id and DOE between @datefrom and @dateto and Mode=@Mode and status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);

            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where ward=@ward_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);

            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where ward=@ward_id and DOE between @datefrom and @dateto and Mode=@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);

            }
            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("SELECT count(id) as name from combatcellTitle  where ward=@ward_id and DOE between @datefrom and @dateto and Mode<>@Mode and categoryid=@categoryid and subcategoryId=@subcategoryId and status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);

            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }


    public DataTable pending_at_si_filter_ward(string ward_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select  count( a.id) as number from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select  count( a.id) as number from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select  count( a.id) as number from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select  count( a.id) as number from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }
            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select  count( a.id) as number from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode<>@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }


    public DataTable pending_at_csi_filter_ward(string ward_id, string datefrom, string dateto, string mode, string catagory, string subcategory)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();

            if (mode == "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(a.id)  as numer from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
            }
            else if (mode != "0" && catagory == "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(a.id)  as numer from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
            }
            else if (mode != "0" && catagory != "0" && subcategory == "0")
            {
                cmd = new SqlCommand("select count(a.id)  as numer from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
            }
            else if (mode != "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select count(a.id)  as numer from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode=@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }
            else if (mode == "0" && catagory != "0" && subcategory != "0")
            {
                cmd = new SqlCommand("select count(a.id)  as numer from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ward=@ward_id and b.DOE between @datefrom and @dateto and b.Mode<>@Mode and b.categoryid=@categoryid and b.subcategoryId=@subcategoryId and b.status='true'", con);
                cmd.Parameters.AddWithValue("@ward_id", ward_id);
                cmd.Parameters.AddWithValue("@datefrom", datefrom);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@Mode", mode);
                cmd.Parameters.AddWithValue("@categoryid", catagory);
                cmd.Parameters.AddWithValue("@subcategoryId", subcategory);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }





    // end
    #endregion

    // pending at ward si
    public DataTable BindAllComplains_received_all_pending_combat_si_pending_Circle_wise(string Circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("select count(distinct(b.id)) as nummber from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle_id", con);
            cmd.Parameters.AddWithValue("@Circle_id", Circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];

        }
        return dt;
    }

    // pending at ward csi
    public DataTable BindAllComplains_received_all_pending_combat_csi_pending_Circle_wise(string Circle)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select count(distinct b.id) as number from users a,combatcellTitle b, CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.Circle=@Circle", con);
            cmd.Parameters.AddWithValue("@Circle", Circle);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    //// pending at ward csi
    //public DataTable BindAllComplains_received_all_Closed_AVG_Time_Circle_wise(string Circle)
    //{
    //    DataTable dt = new DataTable();
    //    DataSet ds = new DataSet();
    //    using (SqlConnection con = new SqlConnection(strcon))
    //    {
    //        con.Open();
    //        SqlCommand cmd = new SqlCommand("select [dbo].[find_AvgTime_closed_complaints_in_circle](@Circle) as Count", con);
    //        cmd.Parameters.AddWithValue("@Circle", Circle);
    //        SqlDataAdapter da = new SqlDataAdapter(cmd);
    //        da.Fill(ds);
    //        dt = ds.Tables[0];
    //    }
    //    return dt;
    //}


    /*********************************************************
     * Till Here
     * ********************************************************/
}