using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for admin
/// </summary>
public class admin
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    //logError_Handler le_h = new logError_Handler();
    public DataTable BindAllCircle()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, Circle from adminCircle where status=1 order by Circle ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindCircle()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, Circle from Circle where status=1 order by Circle ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindWardCircleWise(string circleid)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from tblWard where circleid=@circleid order by wardno ASC", con);
            cmd.Parameters.AddWithValue("@circleid", circleid);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable find_circle_by_name(string circle_name)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("Select id, circle from circle where circle=@pid", con);
            cmd.Parameters.AddWithValue("@pid", circle_name);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public int add_admin(int id, string sel_id, string sel_circle, string f_name, string l_name, string cont_no, string e_id, string pass)
    {
        int number = 0;

        encryption en = new encryption();
        string passw = en.GetMD5(pass);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_into_holding_admin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["holding_U_ID_admin"].ToString());
            //cmd.Parameters.AddWithValue("@user_type", HttpContext.Current.Session["holding_U_ID_admin"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@sel_id", sel_id);
            cmd.Parameters.AddWithValue("@sel_circle", sel_circle);
            cmd.Parameters.AddWithValue("@first_name", f_name);
            cmd.Parameters.AddWithValue("@last_name", l_name);
            //cmd.Parameters.AddWithValue("@user_name", u_name);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Parameters.AddWithValue("@contact_no", cont_no);
            cmd.Parameters.AddWithValue("@email_id", e_id);
            cmd.Parameters.AddWithValue("@status", "active");
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.Date);
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
                //this.le_h.LogError(ex);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        return number;
    }

    public int addBankName(int id, string _bankName, string _bankAbbr)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_into_Bank_Name"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@bankName", _bankName);
            cmd.Parameters.AddWithValue("@bankAbbr", _bankAbbr);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.Date);
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

    public DataTable viewBankName()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from bank_Name where status=1 order by id desc", con);
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

    public DataTable BindAllBank()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from bank_Name where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable getBankDetails(string _AppNO)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Bank_Details where payment_Code=@AppNo", con);
            cmd.Parameters.AddWithValue("@AppNo", _AppNO);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable getBankName(string _BankID)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select bankName, bankNamrAbbr from Bank_Name where id=@BankID and status=1", con);
            cmd.Parameters.AddWithValue("@BankID", _BankID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable viewAll()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from holding_admin order by id desc", con);
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

    public DataTable viewAllPropertyAcctoCircle(string circle)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from Property_details where CircleName=@circleName order by id desc", con);
            cmd.Parameters.AddWithValue("@circleName", circle);
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

    public DataTable viewDetailsById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from holding_admin where id = @id order by id desc", con);
            cmd.Parameters.AddWithValue("@id", id);
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

    public DataTable viewBankNameById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from Bank_Name where id = @id order by id desc", con);
            cmd.Parameters.AddWithValue("@id", id);
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

    public int admin_check_contactNo(string cont_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from holding_admin where contact_no= @cont_no order by id desc", con);
            cmd.Parameters.AddWithValue("@cont_no", cont_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = 1;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return number;
    }

    public int admin_check_emailId(string ema_id)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from holding_admin where email_id= @emailid order by id desc", con);
            cmd.Parameters.AddWithValue("@emailid", ema_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                number = 1;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        return number;
    }

    public DataTable searchPropertyDetails(string circleName, string wardno, string _pid, string _sas, string _holding)
    {
        DataTable ds = new DataTable();

        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select * from property_details where (CircleName=@circle and WardNo=@wardno) or PID=@pid or SASNo=@sas or NewHoldingNo=@holding order by SlNo ASC", con);
            cmd.Parameters.AddWithValue("@circle", circleName);
            cmd.Parameters.AddWithValue("@wardno", wardno);
            cmd.Parameters.AddWithValue("@pid", _pid);
            cmd.Parameters.AddWithValue("@sas", _sas);
            cmd.Parameters.AddWithValue("@holding", _holding);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(ds);
        }
        return ds;
    }

    // Updated by Gyan Chand Verma
    public DataTable searchPropertyDetailsbysasno(string _sas)
    {
        DataTable ds = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select * from property_details where SASNo=@sas order by SlNo ASC", con);
            cmd.Parameters.AddWithValue("@sas", _sas);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(ds);
        }
        return ds;
    }

    public int rejectMesssage(string title, string txtMessage)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("insert into admin_holding_reject_message values(@title,@message,'pending',@date)", con);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@message", txtMessage);
            cmd.Parameters.AddWithValue("@date", System.DateTime.Now.Date);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

    public int updateStatusafterRejection(string title)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update property_details set status='Reject' where id = @title", con);
            cmd.Parameters.AddWithValue("@title", title);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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

    public DataTable CkeckPIDisavailable(string title)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select PID,added_by from property_details where id = @title and PID is not null", con);
            cmd.Parameters.AddWithValue("@title", title);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable FindRejectMessage(string title)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select message from admin_holding_reject_message where Pro_ID=@title", con);
            cmd.Parameters.AddWithValue("@title", title);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable FindContactno(string title)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select contact_no from holding_user where id=@title and status='active'", con);
            cmd.Parameters.AddWithValue("@title", title);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable GetAddedBy(string title)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select PID,added_by from property_details where id = @title", con);
            cmd.Parameters.AddWithValue("@title", title);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable GetMessage(string title)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select message from admin_holding_reject_message where Pro_ID = @title", con);
            cmd.Parameters.AddWithValue("@title", title);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable FindProperty(string _pid, string _paymentcode)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            string query = "select id, pid, payment_code, assessment_year, tran_id, amount, status, CONVERT(varchar(20), since, 103) as Since, receive_date from property_payment where";
            string subquery = "";
            if (!string.IsNullOrEmpty(_pid) && !string.IsNullOrEmpty(_paymentcode))
            {
                subquery += " pid=@_pid and payment_code=@_paymentcode";
            }

            else if (!string.IsNullOrEmpty(_pid))
            {
                subquery += " pid=@_pid";
            }

            else if (!string.IsNullOrEmpty(_paymentcode))
            {
                subquery += " payment_code=@_paymentcode";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery + " order by id desc";
            }

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@_pid", _pid);
            cmd.Parameters.AddWithValue("@_paymentcode", _paymentcode);
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

    public DataTable FindShopPayment(string _pid, string _paymentcode)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select id, pid,  payment_code, assessment_year, amount, status, CONVERT(varchar(20), since, 103) as Since, receive_date from shops_payment where pid=@_pid or payment_code=@_paymentcode", con);
            cmd.Parameters.AddWithValue("@_pid", _pid);
            cmd.Parameters.AddWithValue("@_paymentcode", _paymentcode);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public int updatePayment(int _id)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update property_payment set status='active', payment='updated', payment_status_description='Transaction is Successful',payment_status='S' where id=@_id", con);
            cmd.Parameters.AddWithValue("@_id", _id);
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

    public int updatepropertyPayment(int _id)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update property_payment set status='active', payment='updated', payment_status_description='Transaction is Successful',payment_status='S' where id=@_id", con);
            cmd.Parameters.AddWithValue("@_id", _id);
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

    public DataTable FindShopIdandPaymentCode(string _pid)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select pid, payment_code, since from shops_payment where id=@_id and (status='inactive' or status='pending')", con);
            cmd.Parameters.AddWithValue("@_id", _pid);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable FindShopIdandPaymentCodeforReceipt(string _pid)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select pid, payment_code, since from shops_payment where id=@_id and status='active'", con);
            cmd.Parameters.AddWithValue("@_id", _pid);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }

    public int updateShopPayment(int _id)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update shops_payment set status='active', shop_status='updated', payment_status_description='Transaction is Successful' where id=@_id", con);
            cmd.Parameters.AddWithValue("@_id", _id);
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

    public int updatePropertyPayment(int _id)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update property_payment set @pid=pid, payment_code=@payment_code,assessment_year=@assessment_year where id=@_id", con);
            cmd.Parameters.AddWithValue("@_id", _id);
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

    public int UpdatePendingPayment(int _id, string _transactionRef, DateTime _lastUpdate, string _statusDescription)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("spPropertyPaymentUpdate"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", _id);
            cmd.Parameters.AddWithValue("@transactionRef", _transactionRef);
            cmd.Parameters.AddWithValue("@lastUpdate", _lastUpdate);
            cmd.Parameters.AddWithValue("@statusDescription", _statusDescription);
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
                //this.le_h.LogError(ex);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        return number;
    }
}