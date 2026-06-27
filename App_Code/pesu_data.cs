using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
/// <summary>
/// Summary description for pesu_data
/// </summary>
public class pesu_data
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /*************************************************
    Author  : Arnav
    Date    : 18-06-2108
    Purpose : Binding All Administrator(viewadmin.aspx)
    *************************************************/
    public DataTable bind_data(string circle,string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        ;
        try
        {
            if(circle =="1")
            {
                SqlCommand cmd = new SqlCommand("select * from bankipore where id=@id order by id desc", con);
                cmd.Parameters.AddWithValue("@id",id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if(circle =="2")
            {
                SqlCommand cmd = new SqlCommand("select * from kankerbagh where id=@id order by id desc", con);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if(circle =="3")
            {
                SqlCommand cmd = new SqlCommand("select * from PatnaCity where id=@id order by id desc", con);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else if(circle =="4")
            {
                SqlCommand cmd = new SqlCommand("select * from NCCData where id=@id order by id desc", con);
                cmd.Parameters.AddWithValue("@id", id);
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

    /*************************************************
    Author  : Arnav
    Date    : 18-06-2108
    Purpose : Inserting New Editing
    *************************************************/
    public int insert_new_update(string circles,string id,string added_by,string notice_no,string contract_account,string notice_date,string name,string fathers_name,string address,string contact_no,string rate_category,string area,string circle)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_update_data"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@circles", circles);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@added_by", added_by);
            cmd.Parameters.AddWithValue("@notice_no", notice_no);
            cmd.Parameters.AddWithValue("@contract_account", contract_account);
            cmd.Parameters.AddWithValue("@notice_date", notice_date);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@fathers_name", fathers_name);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@rate_category", rate_category);
            cmd.Parameters.AddWithValue("@area", area);
            cmd.Parameters.AddWithValue("@circle", circle);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
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



    /**************************************************
     * Till Here
     * ************************************************/
}