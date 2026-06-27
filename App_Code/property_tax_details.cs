using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for property_tax_details
/// </summary>
public class property_tax_details
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Bind all circles
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataSet bind_available_circles()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select distinct(CircleName) from property_details where circlename !='' ORDER BY CircleName ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    /************************************************************
    * Purpose  ::  Bind all Wards
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public DataSet bind_available_wards()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select distinct(WardNo) from property_details where wardno !='' ORDER BY WardNo ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }


    public string find_property_type_usage(string property_id)
    {
        string p_type = "";
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("Select usagetype from floor_details where PID=@property_id", con);
            cmd.Parameters.AddWithValue("@property_id", property_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            //Edited by G.C.Verma.
            foreach (DataRow gc in dt.Rows)
            {
                if (gc["usagetype"].ToString() == "Residential")
                {
                    p_type = "Residential";
                }
                else
                {
                    p_type = "Commercial";
                    break;
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
        return p_type;
    }




    /************************************************************
    * Purpose  ::  Bind all Wards In cirlce
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public DataSet bind_available_wards_in_circle(string circle_name)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select distinct(WardNo) from property_details where CircleName=@circle_name and wardno !='' ORDER BY WardNo ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@circle_name",circle_name);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    /*******************************************************
     * Bind all Property details
     * *****************************************************/
    public DataTable bind_all_property(string circle,string ward,string property_no,string holding_no,string owner_name)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            //string Query = "SELECT a.PID,a.CircleName,a.WardNo,a.OldHoldingNo,a.OldPID,a.NewHoldingNo,a.Address,a.LastAssessmentYear,b.PID,b.OwnerName FROM property_details a, owner_details b WHERE a.PID = b.PID and a.PID is not null and b.PID is not null and a.PID !='NULL' and b.PID !='NULL'";
		string Query = "Select a.PID,a.CircleName,a.WardNo,a.OldHoldingNo,a.OldPID,a.NewHoldingNo,a.Address, b.OwnerName from property_details a,vw_GetOwnerData b where a.PID = b.PID and a.PID is not null and b.PID is not null and a.PID != 'NULL' and b.PID != 'NULL'";
       
            string sub_query ="";
            if(circle !="0")
            {
                sub_query += " AND a.CircleName=@circle_name";
            }
            if (ward !="0")
            {
                sub_query += " AND a.WardNo=@ward_no";
            }
            if(property_no !="")
            {
                sub_query += " AND (a.OldPID=@property_no OR a.PID=@property_no)";
            }
            if(holding_no !="")
            {
                sub_query += " AND (a.OldHoldingNo=@holding_no OR a.NewHoldingNo=@holding_no)";
            }
            if(owner_name !="")
            {
                sub_query += " AND (b.OwnerName like '%' + @owner_name + '%')";
            }

            if(sub_query !="")
            {
                Query = Query + " " + sub_query;
            }
            
            string new_query = Query;
            SqlCommand cmd = new SqlCommand(new_query, con);
            //SqlCommand cmd = new SqlCommand("select a.*,b.* from property_details a, owner_details b where (a.CircleName=@circle_name or a.wardno=@ward_name or a.OldPID=@holding_no or a.NewHoldingNo=@holding_no or b.OwnerName=@owner_name) and a.PID=b.PID", con);
            cmd.Parameters.AddWithValue("@circle_name", circle);
            cmd.Parameters.AddWithValue("@ward_no", ward);
            cmd.Parameters.AddWithValue("@property_no", property_no);
            cmd.Parameters.AddWithValue("@holding_no", holding_no);
            cmd.Parameters.AddWithValue("@owner_name",owner_name);
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

    public DataTable find_owner_name(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT PID,OwnerName from owner_details WHERE PID=@pid", con);
            cmd.Parameters.AddWithValue("@pid",p_id);
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

    public DataTable bind_single_property_detail(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM property_details WHERE PID=@pid", con);
            cmd.Parameters.AddWithValue("@pid", p_id);
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

    public DataTable bind_single_owner_details(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM owner_details WHERE PID=@pid", con);
            cmd.Parameters.AddWithValue("@pid", p_id);
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

    

    /**********************************************************8
     * Till Here
     * *****************************************************/
}