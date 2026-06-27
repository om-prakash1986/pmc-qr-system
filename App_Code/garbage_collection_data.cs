using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for garbage_collection_data
/// </summary>
public class garbage_collection_data
{
    //hinet6814038
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Insert New Scheme Name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int add_new_scheme_name(string name,string year,string month,string amount,string description,string property_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_scheme_names"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID_admin"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@property_type", property_type);
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

    /************************************************************
     * Purpose  ::  List of all Schemes
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable select_all_schemes(string year,string property_type)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_collection_schemes where from_year=@year and property_type=@property_type and no_of_months='12'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@property_type", property_type);
            da.Fill(dt);
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
        return dt;
    }

    /************************************************************
     * Purpose  ::  User Selected garbage collection scheme
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int insert_new_garbage_collection_for_user(string year,string user_id,string scheme_id,string pid)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_user_selected_schemes"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID"].ToString());
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@scheme_id", scheme_id);
            cmd.Parameters.AddWithValue("@pid", pid);
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

    public DataTable select_scheme_of_user(string user_id,string property_id,string year)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from user_selected_schemes where user_id=@user_id and PID=@pid and Year=@year", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            cmd.Parameters.AddWithValue("@pid", property_id);
            da.Fill(dt);
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
        return dt;
    }

    /************************************************************
     * Purpose  ::  All Garbage Collection Payment By User
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public DataTable bind_all_payments(string pid,string year)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_collection_user_details where property_id=@pid and Year=@year", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@pid", pid);
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

    /************************************************************
     * Purpose  ::  Public void Show and Hide Div
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string show_hide_div(string PID)
    {
        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from garbage_collection_user_details where property_id=@property_id order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@property_id", PID);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                double current_year = Convert.ToDouble(System.DateTime.Now.Year);
                double used_year = Convert.ToDouble(dt.Rows[0]["Year"].ToString());

                if (current_year == used_year)
                {
                    string f_month = dt.Rows[0]["from_month"].ToString();
                    string t_month = dt.Rows[0]["to_month"].ToString();
                    string since = Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MMM-yyyy");
                    double current_month = Convert.ToDouble(System.DateTime.Now.Month);

                    if (current_month <= Convert.ToDouble(t_month))
                    {
                        // no need to pay tax allready paid
                        number = "0";
                    }
                    else
                    {
                        // Need to pay tax
                        number = "1";
                    }
                }
                else
                {
                    string f_month = dt.Rows[0]["from_month"].ToString();
                    string t_month = dt.Rows[0]["to_month"].ToString();
                    string since = Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MMM-yyyy");
                    double current_month = Convert.ToDouble(System.DateTime.Now.Month);

                    if (current_month <= Convert.ToDouble(t_month))
                    {
                        // allready paid
                        number = "0";
                    }
                    else
                    {
                        // need to pay tax
                        number = "1";
                    }
                }
            }
            else
            {
                SqlCommand cmd1 = new SqlCommand("select top 1 * from user_selected_schemes where property_id=@property_id order by id desc", con);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                cmd1.Parameters.AddWithValue("@property_id", PID);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    // if any scheme selected
                    number = "selected scheme";
                }
                else
                {
                    // if no scheme is selected
                    number = "1";
                }
            }
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
     * Purpose  ::  Update Scheme Name
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public int add_update_scheme_name(string name, string year, string month, string amount, string description)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Update_garbage_scheme_names"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID_admin"].ToString());
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@description", description);
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

    public int insert_new_garbage_collection_for_user_online(string year, string user_id, string scheme_id, string pid,string gcc_id)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_user_selected_schemes_online"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", gcc_id.ToString());
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@scheme_id", scheme_id);
            cmd.Parameters.AddWithValue("@pid", pid);
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

    public string find_selected_scheme_type(string pid)
    {
        string id = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from user_selected_schemes where (pid=@pid or gcc_id=@pid) order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@pid", pid);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                id = dt.Rows[0]["scheme_type"].ToString();
            }
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
        return id;
    }


    public DataTable find_selected_scheme_name(string scheme_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from garbage_collection_schemes where id=@id order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@id", scheme_id);
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

    public DataTable select_all_available_property_type()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_collection_house_types where status='active' order by property_type ASC", con);
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

    public string find_current_scheme(string month,string property_type)
    {
        string id = "";
        string year = System.DateTime.Now.Year.ToString();
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from garbage_collection_schemes where from_year=@year and property_type=@property_type and no_of_months=@month order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@property_type", property_type);
            cmd.Parameters.AddWithValue("@month", month);
            da.Fill(dt);
            if(dt.Rows.Count >0)
            {
                id = dt.Rows[0]["id"].ToString();
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
        return id;
    }
    //Till Here
}