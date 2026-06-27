using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


/// <summary>
/// Summary description for CombatCell_Notification
/// </summary>
public class CombatCell_Notification
{

    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cmrs notification all
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cmrs_new_compliance()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from combatcell_review where status='active' and review is not null and (added_by is not null or division_id is not null or district_id is not null) and (viewed='no' or viewed is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cmannoun notification department wise
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cmannoun_new_compliance()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from cmannoun_review where status='active' and review is not null and (added_by is not null or division_id is not null or district_id is not null) and (viewed='no' or viewed is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cdms notification department wise
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cdms_new_compliance()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from cdms_review where status='active' and review is not null and (added_by is not null or division_id is not null or district_id is not null) and (viewed='no' or viewed is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cms notification department wise
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cms_new_compliance()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from eletter_review where status='active' and review is not null and (added_by is not null or division_id is not null or district_id is not null or sp_district is not null) and (viewed='no' or viewed is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Updating CMRS Viewed Data
     * Date    : 07-06-2017
     * **************************************************/
    public int update_cmrs_view(string action_point)
    {
        int number = 1;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        SqlCommand cmd1 = new SqlCommand("update cmrs_review set viewed='yes' WHERE cmrs_ap_title = @id", con);
        cmd1.Parameters.AddWithValue("@id", action_point);
        cmd1.ExecuteNonQuery();
        con.Close();

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Updating CDMS Viewed Data
     * Date    : 07-06-2017
     * **************************************************/
    public int update_cdms_view(string action_point)
    {
        int number = 1;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        SqlCommand cmd1 = new SqlCommand("update cdms_review set viewed='yes' WHERE cdms_ap_title = @id", con);
        cmd1.Parameters.AddWithValue("@id", action_point);
        cmd1.ExecuteNonQuery();
        con.Close();

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Updating CMANNOUN Viewed Data
     * Date    : 07-06-2017
     * **************************************************/
    public int update_cmannoun_view(string action_point)
    {
        int number = 1;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        SqlCommand cmd1 = new SqlCommand("update cmannoun_review set viewed='yes' WHERE cmannoun_ap_title = @id", con);
        cmd1.Parameters.AddWithValue("@id", action_point);
        cmd1.ExecuteNonQuery();
        con.Close();

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Updating CMANNOUN Viewed Data
     * Date    : 07-06-2017
     * **************************************************/
    public int update_eletter_view(string action_point)
    {
        int number = 1;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        SqlCommand cmd1 = new SqlCommand("update eletter_review set viewed='yes' WHERE dairy_no = @id", con);
        cmd1.Parameters.AddWithValue("@id", action_point);
        cmd1.ExecuteNonQuery();
        con.Close();

        return number;
    }


    /**********************************************************
     * Department Data Notification Details
     * ********************************************************/

    /****************************************************
     * Author  : Arnav
     * Purpose : Updating CMRS Viewed Data
     * Date    : 07-06-2017
     * **************************************************/
    public int update_cmrs_view_department(string action_point)
    {
        int number = 1;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        SqlCommand cmd1 = new SqlCommand("update cmrs_review set dept_view='yes' WHERE cmrs_ap_title = @id", con);
        cmd1.Parameters.AddWithValue("@id", action_point);
        cmd1.ExecuteNonQuery();
        con.Close();

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Updating CMANNOUN Viewed Data
     * Date    : 07-06-2017
     * **************************************************/
    public int update_cmannoun_view_department(string action_point)
    {
        int number = 1;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        SqlCommand cmd1 = new SqlCommand("update cmannoun_review set dept_view='yes' WHERE cmannoun_ap_title = @id", con);
        cmd1.Parameters.AddWithValue("@id", action_point);
        cmd1.ExecuteNonQuery();
        con.Close();

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Updating CDMS Viewed Data
     * Date    : 07-06-2017
     * **************************************************/
    public int update_cdms_view_department(string action_point)
    {
        int number = 1;
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        SqlCommand cmd1 = new SqlCommand("update cdms_review set dept_view='yes' WHERE cdms_ap_title = @id", con);
        cmd1.Parameters.AddWithValue("@id", action_point);
        cmd1.ExecuteNonQuery();
        con.Close();

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Count CMRS DATA
     * Date    : 07-06-2017
     * **************************************************/
    public int find_cmrs_new_compliance_dept(string dept_id)
    {
        string number = "";
        int i = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT row_number() over (order by a.id desc) as row_num, a.id as comp_id,a.cmrs_ap_title,a.added_by as dep_id,a.name,a.division_id as div_id,a.district_id as dist_id,a.comment,a.review,a.status,b.id as ap_id,action_point,b.added_by,b.status as ap_status,b.division_id,b.district_id FROM cmrs_review a, cmrs_ap b where a.cmrs_ap_title = b.id and a.comment is not null and (a.designation is not null) and (dept_view='no' or dept_view is null) and a.status='active' and a.comment is not null and b.added_by=@added_by", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@added_by", dept_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {

                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["comp_id"].ToString().Trim();
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

        return i;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Count CDMS DATA
     * Date    : 07-06-2017
     * **************************************************/
    public int find_cdms_new_compliance_dept(string dept_id)
    {
        string number = "";
        int i = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select a.id as comp_id,a.cdms_ap_title,a.added_by as dep_id,a.name,a.division_id as div_id,a.district_id as dist_id,a.viewed,a.comment,a.status,b.id as ap_id,action_point,b.added_by,b.status as ap_status,b.division_id,b.district_id from cdms_review a, cdms_ap b where a.cdms_ap_title = b.id and a.review is not null and (a.designation is not null) and (dept_view='no' or dept_view is null) and a.status='active' and b.added_by=@added_by", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@added_by", dept_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {

                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["comp_id"].ToString().Trim();
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

        return i;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Count CMANNOUNCEMENT DATA
     * Date    : 07-06-2017
     * **************************************************/
    public int find_cmannoun_new_compliance_dept(string dept_id)
    {
        string number = "";
        int i = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select a.id as comp_id,a.cmannoun_ap_title,a.comment,a.added_by as dep_id,a.name,a.division_id as div_id,a.district_id as dist_id,a.viewed,a.dept_view,a.status,b.id as ap_id,action_point,b.added_by,b.status as ap_status,b.division_id,b.district_id FROM cmannoun_review a, cmannoun_ap b where a.cmannoun_ap_title = b.id and a.comment is not null and (a.designation is not null) and (dept_view='no' or dept_view is null) and a.status='active' and b.added_by=@added_by", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@added_by", dept_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {

                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["comp_id"].ToString().Trim();
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

        return i;
    }

    /****************************************************
    * Author  : Arnav
    * Purpose : Count ELETTER DATA
    * Date    : 07-06-2017
    * **************************************************/
    public int find_cms_new_compliance_dept(string dept_id)
    {
        string number = "";
        int i = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select a.id as comp_id,a.dairy_no,a.added_by as dep_id,a.name,a.division_id as div_id,a.district_id as dist_id,a.viewed,a.comment,a.status,b.dairy_no as ap_id,subject,b.added_by,b.status as ap_status,b.division_id,b.district_id,b.letter_no from eletter_review a, letter_title b where a.dairy_no = b.dairy_no and a.review is not null and (a.designation is not null) and (dept_view='no' or dept_view is null) and a.status='active' and b.added_by=@added_by", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@added_by", dept_id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {

                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["comp_id"].ToString().Trim();
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

        return i;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cmrs notification all
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cmrs_new_compliance_review()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from cmrs_review where status='active' and comment is not null and (dept_view='no' or dept_view is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cmannoun notification department wise
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cmannoun_new_compliance_review()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from cmannoun_review where status='active' and comment is not null and (dept_view='no' or dept_view is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cdms notification department wise
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cdms_new_compliance_review()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from cdms_review where status='active' and comment is not null and (dept_view='no' or dept_view is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }

    /****************************************************
     * Author  : Arnav
     * Purpose : Showing cms notification department wise
     * Date    : 07-06-2017
     * **************************************************/
    public string find_cms_new_compliance_review()
    {
        string number = "";

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select count(id) as number from eletter_review where status='active' and comment is not null and (dept_view='no' or dept_view is null)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = dt.Rows[i]["number"].ToString().Trim();
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

        return number;
    }





    /***************************************************************
     * Till Here
     * ***********************************************************/

}