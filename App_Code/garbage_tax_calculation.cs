using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for garbage_tax_calculation
/// </summary>
public class garbage_tax_calculation
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable find_if_property_available(string holding_no)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_owner_name](PID) as owner_name,[dbo].[find_circle_id](CircleName) as circle_id from property_details where (pid=@pid or NewHoldingNo=@pid or oldHoldingNo=@pid or oldPID=@pid) ", con);
            cmd.Parameters.AddWithValue("@pid",holding_no);
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

    public DataTable find_if_gccid_available(string holding_no)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_property_type](property_type) as my_p_type from garbage_collection_user_details where gcc_id=@pid ", con);
            cmd.Parameters.AddWithValue("@pid", holding_no);
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

    public string find_property_type(string PID)
    {
        string type = "Residential";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from floor_details where pid=@pid and usetype!='Residential'", con);
            cmd.Parameters.AddWithValue("@pid", PID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                type = "Commercial";
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
        return type;
    }

    public int insert_new_garbage_collection_user(string owner_name, string contact_no, string email_id, string property_type, string address, string pin, string GCCID,string circle)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_collection_user_online"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GCCID", GCCID);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["gar_User_ID"]);
            cmd.Parameters.AddWithValue("@owner_name", owner_name);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@property_type", property_type);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@pin", pin);
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

    public int insert_new_garbage_collection_user_app(string owner_name, string gar_User_ID, string contact_no, string email_id, string property_type, string address, string pin, string GCCID, string circle)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_collection_user_online"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GCCID", GCCID);
            cmd.Parameters.AddWithValue("@user_id", gar_User_ID);
            cmd.Parameters.AddWithValue("@owner_name", owner_name);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@property_type", property_type);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@pin", pin);
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

    public DataTable find_if_allready_paidby_paytmTransaction(string gcc_id,string transactionid)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select transacton_no,property_id,amount_paid,status,gcc_user_id,from_month,to_month,from_year,to_year from garbage_collection_user_details where (gcc_id=@pid or property_id=@pid) and transacton_no=@transacton_no  order by id desc", con);
            cmd.Parameters.AddWithValue("@transacton_no", transactionid);
            cmd.Parameters.AddWithValue("@pid", gcc_id);
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
    public DataTable find_if_allready_paid_Paytm(string gcc_id)
    {
        DataTable dt = new DataTable();
        double current_year = Convert.ToDouble(System.DateTime.Now.Year);
        double current_month = Convert.ToDouble(System.DateTime.Now.Month);

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from garbage_collection_user_details where (gcc_id=@pid or property_id=@pid) and status='active' order by id desc", con);
            cmd.Parameters.AddWithValue("@pid", gcc_id);
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


    public string find_if_allready_paid(string gcc_id)
    {
        string paid = "no";
        double current_year = Convert.ToDouble(System.DateTime.Now.Year);
        double current_month = Convert.ToDouble(System.DateTime.Now.Month);

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from garbage_collection_user_details where (gcc_id=@pid or property_id=@pid) and status='active' order by id desc", con);
            cmd.Parameters.AddWithValue("@pid", gcc_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                double paid_for_month = Convert.ToDouble(dt.Rows[0]["from_month"]);
                double paid_to_month = Convert.ToDouble(dt.Rows[0]["to_month"]);
                double from_year = Convert.ToDouble(dt.Rows[0]["from_year"]);
                double to_year = Convert.ToDouble(dt.Rows[0]["to_year"]);

                if (to_year >= current_year)
                {
                    if(paid_to_month >=current_month)
                    {
                        paid = "yes";
                    }
                    else if(paid_to_month < current_month)
                    {
                        paid = "no";
                    }
                }
                else if (to_year < current_year)
                {
                    paid = "no";


                }
            }
            else
            {
                paid = "no";
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
        return paid;
    }

    public DataTable amount_paid_till_date(string gcc_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from garbage_collection_user_details where (gcc_id=@pid or property_id=@pid) and status='active' order by id desc", con);
            cmd.Parameters.AddWithValue("@pid", gcc_id);
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

    public string find_new_property_type(string property_type)
    {
        string name = "";
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from garbage_collection_house_types where id=@pid ", con);
            cmd.Parameters.AddWithValue("@pid", property_type);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if(dt.Rows.Count >0)
            {
                name = dt.Rows[0]["property_type"].ToString();
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
        return name;
    }

    public string find_amount_from_property_type(string property_type)
    {
        string amount = "0";
        string year = System.DateTime.Now.Year.ToString();
         SqlConnection con = new SqlConnection(strcon);
         try
         {
             SqlCommand cmd = new SqlCommand("select amount from garbage_collection_schemes where property_type=@property_type and no_of_months='1' and from_year=@year order by id desc", con);
             cmd.Parameters.AddWithValue("@property_type", property_type);
             cmd.Parameters.AddWithValue("@year",year);
             SqlDataAdapter da = new SqlDataAdapter(cmd);
             DataTable dt = new DataTable();
             da.Fill(dt);
             if (dt.Rows.Count > 0)
             {
                 amount = dt.Rows[0]["amount"].ToString();
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
        return amount;
    }

    public DataTable find_property_type_new(string PID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 *,[dbo].[find_house_type](property_type) as p_type from garbage_collection_user_details where gcc_id=@pid order by id desc", con);
            cmd.Parameters.AddWithValue("@pid", PID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            
            da.Fill(dt);
            //if (dt.Rows.Count > 0)
            //{
            //    type = dt.Rows[0]["property_type"].ToString();
            //}
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

    public int insert_garbage_collection_Payment(string Txnid, string GCId, string Amount, string Paymentstatus, string status, string year, string from_year, string to_year, string from_month, string to_month, string Payment_gateway)
    {
        string id = find_last_id(GCId);
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_collection_payment"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Txnid", Txnid);
            cmd.Parameters.AddWithValue("@GCId", GCId);
            cmd.Parameters.AddWithValue("@Amount", Amount);
            cmd.Parameters.AddWithValue("@Paymentstatus", Paymentstatus);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@from_year", from_year);
            cmd.Parameters.AddWithValue("@to_year", to_year);
            cmd.Parameters.AddWithValue("@from_month", from_month);
            cmd.Parameters.AddWithValue("@to_month", to_month);
            cmd.Parameters.AddWithValue("@Paymentgateway", Payment_gateway);

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

    public int Insert_garbage_collection_payment_paytm(string Txnid, string GCId, string Amount, string Paymentstatus, string status, string year, string from_year, string to_year, string from_month, string to_month, string Payment_gateway)
    {
        string id = find_last_id(GCId);
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_collection_payment_paytm"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Txnid", Txnid);
            cmd.Parameters.AddWithValue("@GCId", GCId);
            cmd.Parameters.AddWithValue("@Amount", Amount);
            cmd.Parameters.AddWithValue("@Paymentstatus", Paymentstatus);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@from_year", from_year);
            cmd.Parameters.AddWithValue("@to_year", to_year);
            cmd.Parameters.AddWithValue("@from_month", from_month);
            cmd.Parameters.AddWithValue("@to_month", to_month);
            cmd.Parameters.AddWithValue("@Paymentgateway", Payment_gateway);

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

    public string find_last_id(string gcc_id)
    {
        string id = "0";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 * from garbage_collection_user_details where gcc_id=@gcc_id order by id desc", con);
            cmd.Parameters.AddWithValue("@gcc_id", gcc_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
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

    public int update_garbage_collection_Payment(string Txnid, string GCId, string Paymentstatus)
    {
        int number = 0;
        string id = find_last_id(GCId);
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Update_garbage_collection_payment"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Txnid", Txnid);
            cmd.Parameters.AddWithValue("@GCId", GCId);
            cmd.Parameters.AddWithValue("@Paymentstatus", Paymentstatus);
            cmd.Parameters.AddWithValue("@id",id);
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

    public int insert_garbage_collection_Payment_failed(string Txnid, string GCId, string Amount, string Paymentstatus, string status)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_garbage_collection_payment_failed"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Txnid", Txnid);
            cmd.Parameters.AddWithValue("@GCId", GCId);
            cmd.Parameters.AddWithValue("@Amount", Amount);
            cmd.Parameters.AddWithValue("@Paymentstatus", Paymentstatus);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@status", status);
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




    //******************************************************/
    // TILL HERE
    //******************************************************/

}