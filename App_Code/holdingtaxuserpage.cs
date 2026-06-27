using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for holdingtaxuserpage
/// </summary>
public class holdingtaxuserpage
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable checkstatus(string title)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select status from property_Details where status='Approved' and id=@title", con);
            cmd.Parameters.AddWithValue("@title", title);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable bind_single_property_detail(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM property_details WHERE id=@pid", con);
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

    public DataSet bind_single_owner_details_new(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataSet ds = new DataSet();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM owner_details WHERE Pro_ID=@pid", con);
            cmd.Parameters.AddWithValue("@pid", p_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
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
        return ds;
    }

    public DataTable bind_single_owner_details(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM owner_details WHERE Pro_ID= @pid", con);
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

    public DataTable bindWardNew()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select len(ward_no),* from tbl_ward_master order by len(ward_no), ward_no", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    public DataTable searchCircelid(string ward_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select Circle from Circle, tbl_ward_master where tbl_ward_master.circle_id=Circle.id and tbl_ward_master.id=@ward_id", con);
            cmd.Parameters.AddWithValue("@ward_id", ward_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public string find_property_type_usage(string property_id)
    {
        string p_type = "";
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("Select UseType from floor_details where Pro_ID= @property_id", con);
            cmd.Parameters.AddWithValue("@property_id", property_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            //Edited by G.C.Verma.
            foreach (DataRow gc in dt.Rows)
            {
                if (gc["UseType"].ToString() == "Residential")
                {
                    p_type = "Residential";
                }
                else if (gc["UseType"].ToString() == null)
                {
                    p_type = "Commercial";
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

	public DataTable bind_property_detail(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM property_details WHERE pid=@pid", con);
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

    public DataTable bind_owner_details(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM owner_details WHERE pid=@pid", con);
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

    public DataSet bind_owner_details_new(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataSet ds = new DataSet();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM owner_details WHERE pid=@pid", con);
            cmd.Parameters.AddWithValue("@pid", p_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
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
        return ds;
    }

    public DataTable holding_payment(string p_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM property_payment WHERE (status='paid' or status='active') and id=@pid", con);
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

public DataTable holding_paymentUsingPaymentCode(string f_id)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM property_payment WHERE (status='paid' or status='active') and payment_code= @pid", con);
            cmd.Parameters.AddWithValue("@pid", f_id);
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

    public DataTable check_payment_status(string p_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM property_payment WHERE (status='paid' or status='active') and pid=@pid ", con);
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

}