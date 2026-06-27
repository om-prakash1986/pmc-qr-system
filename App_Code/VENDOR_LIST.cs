using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for VENDOR_LIST
/// </summary>
public class VENDOR_LIST
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable BindAllVendorAreawise()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select AREA_ID, AREA_NAME from STREET_VENDOR_AREA_WISE", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindVendorList(string _A_Filter, string _form_no)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            string query = "select VENDOR_NAME, FATHER_HUSBAND_NAME, WARD_NO, PERMANENT_ADDRESS, FORM_NO from STREET_VENDORS where ";
            string subquery = "";
            if (_A_Filter != "0" && !string.IsNullOrEmpty(_form_no))
            {
                subquery += "VENDING_ADDRESS=@A_Filter and FORM_NO=@form_no";
            }
            else if (_A_Filter != "0" && string.IsNullOrEmpty(_form_no))
            {
                subquery += "VENDING_ADDRESS=@A_Filter";
            }
            else
            {
                subquery += "FORM_NO=@form_no";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery;
            }

            string newquery = query;
            SqlCommand cmd = new SqlCommand(newquery, con);
            cmd.Parameters.AddWithValue("@A_Filter", _A_Filter);
            cmd.Parameters.AddWithValue("@form_no", _form_no);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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

    public DataTable getVendorDetails(string id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from STREET_VENDORS where FORM_NO=@id  ", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable getVendorFaimilyDetails(string id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select FAIMILY_MEMBER from STREET_VENDER_FAMILY where FORM_NO=@id  ", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
}