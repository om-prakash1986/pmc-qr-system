using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for view_account_fuel
/// </summary>
public class view_account_fuel
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    Maurya_shop ms = new Maurya_shop();
    // check user old password 
    public int check_password(string id,string password,string type)
    {
        int number = 0;
        Maurya_shop ms = new Maurya_shop();
        string pass =password;// ms.encrypt(password);
        if(type == "1")
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strcon);
            try
            {
                SqlCommand cmd = new SqlCommand("select * from fuelHeadquaterLogin where headquaterId = @id and password=@password order by headquaterId desc", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@password", pass);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["password"].ToString() == pass)
                    {
                        number = 1;
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
        }
        else if (type == "2")
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strcon);
            try
            {
                SqlCommand cmd = new SqlCommand("select * from fuelCircleLogin where circleLoginId = @id and password=@password order by circleLoginId desc", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@password", pass);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["password"].ToString() == pass)
                    {
                        number = 1;
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
        }
        else if (type == "3")
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strcon);
            try
            {
                SqlCommand cmd = new SqlCommand("select * from fuelVendorLogin where venderLoginId = @id and password=@password order by venderLoginId desc", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@password", pass);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["password"].ToString() == pass)
                    {
                        number = 1;
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
        }
        return number;
    }

    // update password 
    public int update_password(string id, string password, string type)
    {
        int number = 0;
        string pass = ms.encrypt(password);
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("upadte_fuel_pasword_new"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@password", pass);
            cmd.Parameters.AddWithValue("@type", type);
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
            return number;
        }
    }
    // till here 
}