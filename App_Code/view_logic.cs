using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

public class view_logic
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /*************************************************
    Author : Amrita Singh
    Date :  18-06-2108
    Purpose : Binding All Administrator(viewadmin.aspx)
    *************************************************/
    public string bind_all_admin()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from administrator order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["first_name"].ToString() + " " + dt.Rows[i]["last_name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["user_name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["email_id"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["contact_no"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updateadmin.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewadmin.aspx)************/
    /*************************************************
   Author : Amrita Singh
   Date :  18-06-2108
   Purpose : Binding All Main Menu(viewmainmenu.aspx)
   *************************************************/
    public string bind_all_main_menu()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["name"].ToString());
                    sb.AppendFormat("</td>");


                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["name_h"].ToString());
                    sb.AppendFormat("</td>");


                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title_h"].ToString());
                    sb.AppendFormat("</td>");

                    //sb.AppendFormat("<td>");
                    //sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    //sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatemainmenu.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewmainmenu.aspx)************/
    /*************************************************
     Author : Amrita Singh
     Date :  18-06-2108
     Purpose : Binding All Main Menu(viewsubmenuone.aspx)
     *************************************************/
    public string bind_all_sub_menu1()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_1 order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["name_h"].ToString());
                    sb.AppendFormat("</td>");


                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title_h"].ToString());
                    sb.AppendFormat("</td>");

                    //sb.AppendFormat("<td>");
                    //sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    //sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatesubmenuone.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewsubmenuone.aspx)************/
    /*************************************************
    Author : Amrita Singh
    Date :  18-06-2108
    Purpose : Binding All Main Menu(viewsubmenutwo.aspx)
    *************************************************/
    public string bind_all_sub_menu2()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_2 order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["name_h"].ToString());
                    sb.AppendFormat("</td>");


                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title_h"].ToString());
                    sb.AppendFormat("</td>");

                    //sb.AppendFormat("<td>");
                    //sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    //sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatesubmenutwo.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewsubmenutwo.aspx)************/
    /*************************************************
    Author : Amrita Singh
    Date :  18-06-2108
    Purpose : Binding All Main Menu(viewsubmenuthree.aspx)
    *************************************************/
    public string bind_all_sub_menu3()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_3 order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["name_h"].ToString());
                    sb.AppendFormat("</td>");


                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title_h"].ToString());
                    sb.AppendFormat("</td>");

                    //sb.AppendFormat("<td>");
                    //sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    //sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatesubmenuthree.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewsubmenuthree.aspx)************/
    /*************************************************
    Author : Amrita Singh
    Date :  18-06-2108
    Purpose : Binding All sub Menu(viewsubmenufour.aspx)
    *************************************************/
    public string bind_all_sub_menu4()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_4 order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["name_h"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title_h"].ToString());
                    sb.AppendFormat("</td>");

                    //sb.AppendFormat("<td>");
                    //sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    //sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatesubmenufour.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewsubmenufour.aspx)************/
    /*************************************************
 Author : Amrita Singh
 Date :  20-06-2108
 Purpose : Binding All Sub Menu(viewsubmenufive.aspx)
 *************************************************/
    public string bind_all_sub_menu5()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_5 order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["name_h"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title_h"].ToString());
                    sb.AppendFormat("</td>");

                    //sb.AppendFormat("<td>");
                    //sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    //sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatesubmenufive.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewsubmenufive.aspx)************/
    /*************************************************
 Author : Amrita Singh
 Date :  20-06-2108
 Purpose : Binding All Sub Menu(viewsubmenusix.aspx)
 *************************************************/
    public string bind_all_sub_menu6()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from category_6 order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["name_h"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title_h"].ToString());
                    sb.AppendFormat("</td>");


                    //sb.AppendFormat("<td>");
                    //sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    //sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatesubmenusix.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewsubmenusix.aspx)************/
    /*************************************************
   Author : Amrita Singh
   Date :  08-07-2108
   Purpose : Binding All recruitment(viewrecruitment.aspx)
   *************************************************/
    public string bind_all_recruitment()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from recruitment order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.SelectCommand = cmd;
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
                    sb.AppendFormat(dt.Rows[i]["description"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updaterecruitment.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("</tr>");
                    j++;
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
        data = sb.ToString();
        return data;
    }
    /*****************End(viewrecruitment.aspx)************/
    /*************************************************
   Author : Amrita Singh
   Date :  18-06-2108
   Purpose : Binding All Administrator(viewadmin.aspx)
   *************************************************/
    public string bind_all_sectiondesignation()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from section_designation order by id desc", con);
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
                    sb.AppendFormat(dt.Rows[i]["branch"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["section_name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["designation"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["full_name"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["contact_no"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updateinsertsectiondesignation.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
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
    /*****************End(viewadmin.aspx)************/
    /*************************************************
 Author : Amrita Singh
 Date :  03-01-2109
 Purpose : Binding All Tender(viewtender.aspx)
 *************************************************/
    public string bind_all_tender()
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from tenders order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.SelectCommand = cmd;
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
                    sb.AppendFormat(dt.Rows[i]["nit_no"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["title"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(dt.Rows[i]["status"].ToString());
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat(Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MMM-yyyy"));
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("<td>");
                    sb.AppendFormat("<a href='updatetender.aspx?id=" + dt.Rows[i]["id"].ToString() + "'><img src='../assets/images/update_icon.png' alt='settings' style='margin-left:auto margin-right:auto;' class='img-circle' title='Update'/></a>");
                    sb.AppendFormat("</td>");

                    sb.AppendFormat("</tr>");
                    j++;
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
        data = sb.ToString();
        return data;
    }
}