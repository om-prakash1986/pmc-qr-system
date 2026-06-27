using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for combat_forwarding
/// </summary>
public class combat_forwarding
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Update and forward Complaints
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string update_forwarding_to_sanitization_officer(string complian_no,string officer_id,string comment,string circle,string ward)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_forward_sp"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@ComplainNo", complian_no);
            cmd.Parameters.AddWithValue("@fw_officer_Id", officer_id);
            cmd.Parameters.AddWithValue("@authority_type", HttpContext.Current.Session["RA_user_Typeid"].ToString());
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@fw_DateTime", System.DateTime.Now.ToString("dd-MMM-yyyy HH:MM:ss"));
            cmd.Parameters.AddWithValue("@circle", circle);
            cmd.Parameters.AddWithValue("@ward", ward);

            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                number = "1";
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
        }
        return number;
    }

    /************************************************************
     * Purpose  ::  Insert New Compliance
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string insert_new_compliance(string complian_no,string officer_id,string comment)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_cell_compliance"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@ComplainNo", complian_no);
            cmd.Parameters.AddWithValue("@fw_officer_Id", officer_id);
            cmd.Parameters.AddWithValue("@authority_type", HttpContext.Current.Session["RA_user_Typeid"].ToString());
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@fw_DateTime", System.DateTime.Now.ToString("dd-MMM-yyyy HH:MM:ss"));
            cmd.Connection = con;
            try
            {
                con.Open();
                number =Convert.ToString(cmd.ExecuteScalar());
                con.Close();
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
        }
        return number;
    }

    /************************************************************
     * Purpose  ::  Insert New Compliance Images
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int insert_compliance_images(string complain_no,string compliance_id,byte[] bytes,string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_cell_compliance_images"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@compliance_id", compliance_id);
            cmd.Parameters.AddWithValue("@ComplainNo",complain_no);
            cmd.Parameters.AddWithValue("@image_data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy HH:MM:ss"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
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
        }
        return number;
    }
    
    /************************************************************
    * Purpose  ::  Insert New Compliance for closure
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public int insert_new_closure_compliance(string complain_no,string remarks)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_cell_close_complain"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@ComplainNo", complain_no);
            cmd.Parameters.AddWithValue("@comment", remarks);
            cmd.Parameters.AddWithValue("@authority_type", HttpContext.Current.Session["RA_user_Typeid"].ToString());
            cmd.Parameters.AddWithValue("@fw_DateTime", System.DateTime.Now.ToString("dd-MMM-yyyy HH:MM:ss"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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
        }
        return number;
    }

    /************************************************************
    * Purpose  ::  Find Last Forwarding ID
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public string find_last_forward_id(string complain_no)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from CombatCell_forward_list where ComplainNo=@complain_no and added_by !=@user_id order by id desc", con);
            cmd.Parameters.AddWithValue("@complain_no", complain_no);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                number = dt.Rows[0]["added_by"].ToString();
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
        return number;
    }

    public string insert_new_compliance_forward(string complian_no, string officer_id, string comment)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_cell_compliance_resend"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@ComplainNo", complian_no);
            cmd.Parameters.AddWithValue("@fw_officer_Id", officer_id);
            cmd.Parameters.AddWithValue("@authority_type", HttpContext.Current.Session["RA_user_Typeid"].ToString());
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@fw_DateTime", System.DateTime.Now.ToString("dd-MMM-yyyy HH:MM:ss"));
            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToString(cmd.ExecuteScalar());
                con.Close();
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
        }
        return number;
    }

    public string update_forwarding_to_sanitization_officer_create_no(string complian_no, string officer_id, string comment, string circle, string ward,string compppp_no)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_forward_sp_create_no"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@ComplainNo", complian_no);
            cmd.Parameters.AddWithValue("@fw_officer_Id", officer_id);
            cmd.Parameters.AddWithValue("@authority_type", HttpContext.Current.Session["RA_user_Typeid"].ToString());
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@fw_DateTime", System.DateTime.Now.ToString("dd-MMM-yyyy HH:MM:ss"));
            cmd.Parameters.AddWithValue("@circle", circle);
            cmd.Parameters.AddWithValue("@ward", ward);
            cmd.Parameters.AddWithValue("@ccc_no", compppp_no);

            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                number = "1";
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
        }
        return number;
    }

    // find chief sanitization officer
    public string find_chief_sanitization_officer(string circle_id)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id,UserTypeid,Circle from users where circle=@circle_id and UserTypeid='3' order by id desc", con);
            cmd.Parameters.AddWithValue("@circle_id", circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                number = dt.Rows[0]["id"].ToString();
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
        return number;
    }


    public string insert_new_compliance_company(string complian_no, string officer_id, string comment)
    {
        string number = "";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Combat_cell_compliance_company"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@ComplainNo", complian_no);
            cmd.Parameters.AddWithValue("@fw_officer_Id", officer_id);
            cmd.Parameters.AddWithValue("@authority_type", HttpContext.Current.Session["RA_user_Typeid"].ToString());
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@fw_DateTime", System.DateTime.Now.ToString("dd-MMM-yyyy HH:MM:ss"));
            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToString(cmd.ExecuteScalar());
                con.Close();
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
        }
        return number;
    }
    /**********************************************************
     * Till Here
     * *********************************************************/
}