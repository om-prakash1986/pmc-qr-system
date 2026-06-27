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
/// Summary description for account_updation
/// </summary>
public class account_updation
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    /************************************************************
     * Purpose  ::  SP Account updation panel
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string sp_account_details_text(string sp_id)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from sp_districts where id=@id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", sp_id);
            DataTable dt = new DataTable();
            da.Fill(dt);
            
            sb.Append("<input type'hidden' class='hidden' name='p_id' id='p_id' value='" + sp_id.ToString() + "' />");
            sb.Append("<table class='table table-striped table-hover'>");
            
            if (dt.Rows.Count > 0)
            {
                //string img1 = "../handlers/sp_district.ashx?id=" + sp_id.ToString().Trim();
                // department_name
                sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append("<div>");
                        sb.Append("<span id='dept_Nme'><b>District Name :</b>  &nbsp;&nbsp;" + dt.Rows[0]["district_name"] + " </span>");
                        sb.Append("<span id='product_name' class='pull-right'><a href='#modal-department_name' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                    sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // name
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                        sb.Append("<span id='Nme'><b>UserName :</b>  &nbsp;&nbsp;" + dt.Rows[0]["user_name"] + " </span>");
                            //sb.Append("<span id='product_name' class='pull-right'><a href='#modal-product_name' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                //Gender
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Fst_name'><b>District Name Hindi :</b>  &nbsp;&nbsp;" + dt.Rows[0]["district_name_h"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#gender_hindi' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // cmrs
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Lname_q'><b>CMRS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cmrs"] + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#cmrs_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // cdms
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Lname_q_cmrs'><b>CDMS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cdms"] + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#cdms_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // ebid
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Lname_q_e_bid'><b>E-BID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["ebid"] + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#cebid_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                //email id
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='E_id'><b>Email ID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["email_id"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#emil' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // country
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='product_descShow_fax'><b>Fax No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["fax_no"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#ctry' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // state
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='countss'><b>Contact No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["contact_no"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#con_no' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");
                // password
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='product_descShow'><b>Password :</b>  &nbsp;&nbsp; </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#pass' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // user image 
                //sb1.Append("<img src='" + img1 + "' class='img-responsive'/>");
                // till here
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='addressd2'><b>Active From :</b>  &nbsp;&nbsp;" + Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MM-yyyy") + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#2add' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("<br /><br /><br />");
            data = sb.ToString();
        }
        catch(Exception ex)
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
     * Purpose  ::  SP Account Picture updation
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int account_picture_updation(string sp_id,byte[] bytes,string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update sp_districts set district_data=@data,district_data_content_type =@content_type WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@id", sp_id);
            cmd1.Parameters.AddWithValue("@data", bytes);
            cmd1.Parameters.AddWithValue("@content_type", content_type);
            cmd1.ExecuteNonQuery();
            con.Close();
            number = 1;
        }
        catch(Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return number;
    }

    /************************************************************
     * Purpose  ::  Top Letter Timeline
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_letter_timeline(string dairy_no)
    {
        string data = "";

        data = create_letter_title(dairy_no);
        data += create_comment_timeline(dairy_no);

        return data;
    }

    /************************************************************
     * Purpose  ::  Create Letter Title
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_letter_title(string dairy_no)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from letter_title where dairy_no=@id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", dairy_no);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sb.Append("<li>");
                        sb.Append("<div class='timeline-badge'><i class='glyphicon glyphicon-check'></i></div>");
                        sb.Append("<div class='timeline-panel'>");
                            sb.Append("<div class='timeline-heading'>");
                                sb.Append("<h4 class='timeline-title'><b>Letter Subject</b></h4>");
                                sb.Append("<p><small class='text-muted'><i class='glyphicon glyphicon-time'></i> " + Convert.ToDateTime(dt.Rows[i]["since"]).ToString("dd-MM-yyyy") + "</small></p>");
                            sb.Append("</div>");
                            sb.Append("<div class='timeline-body'>");
                                sb.Append("<p>" + dt.Rows[i]["subject"].ToString().Trim() + ".</p>");
                            sb.Append("</div>");
                        sb.Append("</div>");
                    sb.Append("</li>");
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

    /************************************************************
     * Purpose  ::  Create Comment Timeline
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string create_comment_timeline(string dairy_no)
    {
        string data = "";
        StringBuilder list = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            using (SqlCommand cmd = new SqlCommand("show_eletter_timeline"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@title_id", dairy_no.ToString().Trim());
                cmd.Connection = con;
                con.Open();

                using (SqlDataReader articleReader = cmd.ExecuteReader())
                {
                    while (articleReader.Read())
                    {
                        if (articleReader["review"].ToString() != "")
                        {
                            list.Append("<li class='timeline-inverted'>");
                                list.Append("<div class='timeline-badge success'><i class='glyphicon glyphicon-thumbs-up'></i></div>");
                                    list.Append("<div class='timeline-panel'>");
                                    list.Append("<div class='timeline-heading'>");
                                    list.Append("<h4 class='timeline-title'><b>Compliance by :</b> " + articleReader["person_name"].ToString().Trim() + "</h4>");
                                    list.Append("<p><small class='text-muted'><i class='glyphicon glyphicon-time'></i> " + Convert.ToDateTime(articleReader["since"]).ToString("dd-MM-yyyy") + "</small></p>");
                                    list.Append("</div>");
                                    list.Append("<div class='timeline-body'>");
                                    list.Append("<p> " + articleReader["review"].ToString().Trim() + "</p>");
                                    if (articleReader["is_cc"].ToString() == "Yes")
                                    {
                                        if (articleReader["rev_data"].ToString() != "")
                                        {
                                            list.Append("<a href='#myModal' id='" + articleReader["id"].ToString().Trim() + "' class='atch_d' role='button' data-toggle='modal'>   <i class='fa fa-download pull-right'></i></a>");
                                        }
                                    }
                                    else if (articleReader["is_cc"].ToString() == "No")
                                    {
                                        if (articleReader["rev_data"].ToString() != "")
                                        {
                                            list.Append("<a target='_blank' href='downloadcompletter.aspx?id=" + articleReader["id"].ToString().Trim() + "'><i class='fa fa-download pull-right' title='Download Letter'></i>Download :</a>");
                                        }
                                    }
                                    list.Append("</div>");
                                list.Append("</div>");
                            list.Append("</li>");
                        }
                        else if (articleReader["comment"].ToString() != "")
                        {
                            list.Append("<li>");
                                list.Append(" <div class='timeline-badge warning'><i class='glyphicon glyphicon-user'></i></div>");
                                list.Append("<div class='timeline-panel'>");
                                    list.Append("<div class='timeline-heading'>");
                                        list.Append("<h4 class='timeline-title'>" + articleReader["name"].ToString().Trim() + "</h4>");
                                        list.Append("<p><small class='text-muted'><i class='glyphicon glyphicon-time'></i> " + Convert.ToDateTime(articleReader["since"]).ToString("dd-MM-yyyy") + "</small></p>");
                                    list.Append("</div>");
                                    list.Append("<div class='timeline-body'>");
                                        list.Append("<p> " + articleReader["comment"].ToString().Trim() + "</p>");
                                    list.Append("</div>");
                                list.Append("</div>");
                            list.Append("</li>");
                        }
                    }
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
        data = list.ToString();
        return data;
    }

    /************************************************************
     * Purpose  ::  Department Account updation panel
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string department_account_details_text(string sp_id)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from departments where id=@id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", sp_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            sb.Append("<input type'hidden' class='hidden' name='p_id' id='p_id' value='" + sp_id.ToString() + "' />");
            sb.Append("<table class='table table-striped table-hover'>");

            if (dt.Rows.Count > 0)
            {
                // department_name
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='dept_Nme'><b>Department Name :</b>  &nbsp;&nbsp;" + dt.Rows[0]["department_name"] + " </span>");
                            sb.Append("<span id='product_name' class='pull-right'><a href='#modal-department_name' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // name
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Nme'><b>UserName :</b>  &nbsp;&nbsp;" + dt.Rows[0]["user_name"] + " </span>");
                            //sb.Append("<span id='product_name' class='pull-right'><a href='#modal-product_name' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                //Gender
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Fst_name'><b>Department Name Hindi :</b>  &nbsp;&nbsp;" + dt.Rows[0]["department_name_h"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#gender_hindi' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // cmrs
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Lname_q'><b>CMRS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cmrs"] + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#cmrs_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // cdms
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Lname_q_cmrs'><b>CDMS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cdms"] + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#cdms_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // ebid
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Lname_q_e_bid'><b>E-BID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["ebid"] + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#cebid_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                //email id
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='E_id'><b>Email ID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["email_id"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#emil' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // country
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='product_descShow_fax'><b>Fax No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["fax_no"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#ctry' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // state
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='countss'><b>Contact No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["contact_no"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#con_no' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // password
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='product_descShow'><b>Password :</b>  &nbsp;&nbsp; </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#pass' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // user image 
                //sb1.Append("<img src='" + img1 + "' class='img-responsive'/>");
                // till here
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='addressd2'><b>Active From :</b>  &nbsp;&nbsp;" + Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MM-yyyy") + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#2add' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("<br /><br /><br />");
            data = sb.ToString();
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
     * Purpose  ::  Department Account Picture updation
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int department_account_picture_updation(string department_id, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update departments set department_data=@data,department_data_content_type =@content_type WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@id", department_id);
            cmd1.Parameters.AddWithValue("@data", bytes);
            cmd1.Parameters.AddWithValue("@content_type", content_type);
            cmd1.ExecuteNonQuery();
            con.Close();
            number = 1;
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
        return number;
    }

    /************************************************************
     * Purpose  ::  SP Account updation panel
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string district_account_details_text(string district_id)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from districts where id=@id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", district_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            sb.Append("<input type'hidden' class='hidden' name='p_id' id='p_id' value='" + district_id.ToString() + "' />");
            sb.Append("<table class='table table-striped table-hover'>");

            if (dt.Rows.Count > 0)
            {
                //string img1 = "../handlers/sp_district.ashx?id=" + sp_id.ToString().Trim();
                // department_name
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='dept_Nme'><b>District Name :</b>  &nbsp;&nbsp;" + dt.Rows[0]["district_name"] + " </span>");
                            sb.Append("<span id='product_name' class='pull-right'><a href='#modal-department_name' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // name
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Nme'><b>UserName :</b>  &nbsp;&nbsp;" + dt.Rows[0]["user_name"] + " </span>");
                            //sb.Append("<span id='product_name' class='pull-right'><a href='#modal-product_name' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                //Gender
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Fst_name'><b>District Name Hindi :</b>  &nbsp;&nbsp;" + dt.Rows[0]["district_name_h"] + " </span>");
                            sb.Append("<span id='description_name' class='pull-right'><a href='#gender_hindi' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // cmrs
                sb.Append("<tr>");
                    sb.Append("<td>");
                        sb.Append("<div>");
                            sb.Append("<span id='Lname_q'><b>CMRS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cmrs"] + " </span>");
                            //sb.Append("<span id='description_name' class='pull-right'><a href='#cmrs_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                        sb.Append("</div>");
                    sb.Append("<td>");
                sb.Append("</tr>");

                // cdms
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='Lname_q_cmrs'><b>CDMS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cdms"] + " </span>");
                //sb.Append("<span id='description_name' class='pull-right'><a href='#cdms_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // ebid
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='Lname_q_e_bid'><b>E-BID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["ebid"] + " </span>");
                //sb.Append("<span id='description_name' class='pull-right'><a href='#cebid_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                //email id
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='E_id'><b>Email ID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["email_id"] + " </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#emil' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // country
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='product_descShow_fax'><b>Fax No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["fax_no"] + " </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#ctry' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // state
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='countss'><b>Contact No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["contact_no"] + " </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#con_no' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");
                // password
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='product_descShow'><b>Password :</b>  &nbsp;&nbsp; </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#pass' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // user image 
                //sb1.Append("<img src='" + img1 + "' class='img-responsive'/>");
                // till here
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='addressd2'><b>Active From :</b>  &nbsp;&nbsp;" + Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MM-yyyy") + " </span>");
                //sb.Append("<span id='description_name' class='pull-right'><a href='#2add' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("<br /><br /><br />");
            data = sb.ToString();
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
     * Purpose  ::  District Account Picture updation
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int district_account_picture_updation(string district_id, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update districts set district_data=@data,district_data_content_type =@content_type WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@id", district_id);
            cmd1.Parameters.AddWithValue("@data", bytes);
            cmd1.Parameters.AddWithValue("@content_type", content_type);
            cmd1.ExecuteNonQuery();
            con.Close();
            number = 1;
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
        return number;
    }

    /************************************************************
     * Purpose  ::  Division Account updation panel
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string division_account_details_text(string division_id)
    {
        string data = "";
        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from divisions where id=@id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", division_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            sb.Append("<input type'hidden' class='hidden' name='p_id' id='p_id' value='" + division_id.ToString() + "' />");
            sb.Append("<table class='table table-striped table-hover'>");

            if (dt.Rows.Count > 0)
            {
                //string img1 = "../handlers/sp_district.ashx?id=" + sp_id.ToString().Trim();
                // department_name
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='dept_Nme'><b>Division Name :</b>  &nbsp;&nbsp;" + dt.Rows[0]["division_name"] + " </span>");
                sb.Append("<span id='product_name' class='pull-right'><a href='#modal-department_name' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // name
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='Nme'><b>UserName :</b>  &nbsp;&nbsp;" + dt.Rows[0]["user_name"] + " </span>");
                //sb.Append("<span id='product_name' class='pull-right'><a href='#modal-product_name' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                //Gender
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='Fst_name'><b>Division Name Hindi :</b>  &nbsp;&nbsp;" + dt.Rows[0]["division_name_h"] + " </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#gender_hindi' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // cmrs
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='Lname_q'><b>CMRS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cmrs"] + " </span>");
                //sb.Append("<span id='description_name' class='pull-right'><a href='#cmrs_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // cdms
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='Lname_q_cmrs'><b>CDMS :</b>  &nbsp;&nbsp;" + dt.Rows[0]["cdms"] + " </span>");
                //sb.Append("<span id='description_name' class='pull-right'><a href='#cdms_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // ebid
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='Lname_q_e_bid'><b>E-BID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["ebid"] + " </span>");
                //sb.Append("<span id='description_name' class='pull-right'><a href='#cebid_no' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                //email id
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='E_id'><b>Email ID :</b>  &nbsp;&nbsp;" + dt.Rows[0]["email_id"] + " </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#emil' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // country
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='product_descShow_fax'><b>Fax No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["fax_no"] + " </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#ctry' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // state
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='countss'><b>Contact No :</b>  &nbsp;&nbsp;" + dt.Rows[0]["contact_no"] + " </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#con_no' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");
                // password
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='product_descShow'><b>Password :</b>  &nbsp;&nbsp; </span>");
                sb.Append("<span id='description_name' class='pull-right'><a href='#pass' role='button' data-toggle='modal'><img src='../assets/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");

                // user image 
                //sb1.Append("<img src='" + img1 + "' class='img-responsive'/>");
                // till here
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<div>");
                sb.Append("<span id='addressd2'><b>Active From :</b>  &nbsp;&nbsp;" + Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MM-yyyy") + " </span>");
                //sb.Append("<span id='description_name' class='pull-right'><a href='#2add' role='button' data-toggle='modal'><img src='../users/images/edit.png' width='16' height='16' title='edit'/></a></span>");
                sb.Append("</div>");
                sb.Append("<td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("<br /><br /><br />");
            data = sb.ToString();
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
     * Purpose  ::  SP Account Picture updation
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int division_account_picture_updation(string division_id, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update divisions set district_data=@data,district_data_content_type=@content_type WHERE id=@id", con);
            cmd1.Parameters.AddWithValue("@id", division_id);
            cmd1.Parameters.AddWithValue("@data", bytes);
            cmd1.Parameters.AddWithValue("@content_type", content_type);
            cmd1.ExecuteNonQuery();
            con.Close();
            number = 1;
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
        return number;
    }


    /*************************************************************
     * Till Here
     * ***********************************************************/
}