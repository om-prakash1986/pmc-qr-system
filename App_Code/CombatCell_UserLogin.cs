using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;


/// <summary>
/// Summary description for CombatCell_UserLogin
/// </summary>
public class CombatCell_UserLogin
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public string user_login_auth(string user_name, string password)
    {
        string number = "0";

        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("user_login_authentication"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Connection = con;
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["RA_user"] = null;
                    HttpContext.Current.Session["RA_user_ID"] = null;
                    HttpContext.Current.Session["RA_user_Typeid"] = null;
                    HttpContext.Current.Session["RA_user"] = dt.Rows[0]["user_name"].ToString();
                    HttpContext.Current.Session["RA_user_ID"] = dt.Rows[0]["id"].ToString();
                    HttpContext.Current.Session["RA_user_Typeid"] = dt.Rows[0]["UserTypeid"].ToString();
                    string numbers = HttpContext.Current.Session["RA_user_Typeid"].ToString();
                    number = numbers;
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

    /************************************************************
    * Purpose  ::  Update Password of user
    * Author   ::  Arnav
    * Date     ::  24-03-2017
    * ********************************************************/
    public string check_user_name(string user_name)
    {
        string auth = "";
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("forgot_password_authentication"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Connection = con;
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["user_id"].ToString() != "0")
                    {
                        auth = create_password(user_name, dt.Rows[0]["contact_no"].ToString(), 1);
                        number = 1;
                    }
                    else if (dt.Rows[0]["department_id"].ToString() != "0")
                    {
                        auth = create_password(user_name, dt.Rows[0]["contact_no"].ToString(), 2);
                        number = 2;
                    }
                    else if (dt.Rows[0]["division_id"].ToString() != "0")
                    {
                        auth = create_password(user_name, dt.Rows[0]["contact_no"].ToString(), 3);
                        number = 3;
                    }
                    else if (dt.Rows[0]["district_id"].ToString() != "0")
                    {
                        auth = create_password(user_name, dt.Rows[0]["contact_no"].ToString(), 4);
                        number = 4;
                    }
                    else if (dt.Rows[0]["sp_id"].ToString() != "0")
                    {
                        auth = create_password(user_name, dt.Rows[0]["contact_no"].ToString(), 5);
                        number = 5;
                    }
                    else if (dt.Rows[0]["corporation_id"].ToString() != "0")
                    {
                        auth = create_password(user_name, dt.Rows[0]["contact_no"].ToString(), 6);
                        number = 6;
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
        return auth;
    }

    public string create_password(string user_name, string contact_no, int number)
    {
        string allow = "";
        string password = "";
        try
        {
            Random random = new Random();
            string combination = "0123456789ABCDEFGHKMNOPQRSTUVWXYZabcdefghkmnopqrstuvwxyz";
            StringBuilder captcha = new StringBuilder();
            for (int i = 0; i < 8; i++)
                captcha.Append(combination[random.Next(combination.Length)]);
            password = captcha.ToString();

            int auth = 0;
            //reset_password(user_name, password, contact_no, number);

            if (auth == 1)
            {
                allow = "1";
            }
        }
        catch
        {
            throw;
        }
        return allow;
    }


    //public int reset_password(string user_name, string password, string contact_no, int number)
    //{
    //    encryption en = new encryption();
    //    //send_mail sm = new send_mail();

    //    string message = "Your new password for e-Dashboard login is : " + password.ToString() + "";
    //    password = en.GetMD5(password);

    //    int auth = 0;

    //    if (number == 1)
    //    {
    //        SqlConnection con = new SqlConnection(strcon);
    //        try
    //        {
    //            con.Open();
    //            SqlCommand cmd1 = new SqlCommand("update users set password=@password WHERE user_name=@user_name", con);
    //            cmd1.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
    //            cmd1.Parameters.AddWithValue("@user_name", HttpUtility.HtmlEncode(user_name));
    //            cmd1.ExecuteNonQuery();
    //            con.Close();
    //            sm.sendSingleSMS(contact_no, message);
    //            auth = 1;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //    else if (number == 2)
    //    {
    //        SqlConnection con = new SqlConnection(strcon);
    //        try
    //        {
    //            con.Open();
    //            SqlCommand cmd1 = new SqlCommand("update departments set password=@password WHERE user_name=@user_name", con);
    //            cmd1.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
    //            cmd1.Parameters.AddWithValue("@user_name", HttpUtility.HtmlEncode(user_name));
    //            cmd1.ExecuteNonQuery();
    //            con.Close();
    //            sm.sendSingleSMS(contact_no, message);
    //            auth = 1;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //    else if (number == 3)
    //    {
    //        SqlConnection con = new SqlConnection(strcon);
    //        try
    //        {
    //            con.Open();
    //            SqlCommand cmd1 = new SqlCommand("update divisions set password=@password WHERE user_name=@user_name", con);
    //            cmd1.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
    //            cmd1.Parameters.AddWithValue("@user_name", HttpUtility.HtmlEncode(user_name));
    //            cmd1.ExecuteNonQuery();
    //            con.Close();
    //            sm.sendSingleSMS(contact_no, message);
    //            auth = 1;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //    else if (number == 4)
    //    {
    //        SqlConnection con = new SqlConnection(strcon);
    //        try
    //        {
    //            con.Open();
    //            SqlCommand cmd1 = new SqlCommand("update districts set password=@password WHERE user_name=@user_name", con);
    //            cmd1.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
    //            cmd1.Parameters.AddWithValue("@user_name", HttpUtility.HtmlEncode(user_name));
    //            cmd1.ExecuteNonQuery();
    //            con.Close();
    //            sm.sendSingleSMS(contact_no, message);
    //            auth = 1;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //    else if (number == 5)
    //    {
    //        SqlConnection con = new SqlConnection(strcon);
    //        try
    //        {
    //            con.Open();
    //            SqlCommand cmd1 = new SqlCommand("update sp_districts set password=@password WHERE user_name=@user_name", con);
    //            cmd1.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
    //            cmd1.Parameters.AddWithValue("@user_name", HttpUtility.HtmlEncode(user_name));
    //            cmd1.ExecuteNonQuery();
    //            con.Close();
    //            sm.sendSingleSMS(contact_no, message);
    //            auth = 1;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //    else if (number == 6)
    //    {
    //        SqlConnection con = new SqlConnection(strcon);
    //        try
    //        {
    //            con.Open();
    //            SqlCommand cmd1 = new SqlCommand("update corporations set password=@password WHERE user_name=@user_name", con);
    //            cmd1.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
    //            cmd1.Parameters.AddWithValue("@user_name", HttpUtility.HtmlEncode(user_name));
    //            cmd1.ExecuteNonQuery();
    //            con.Close();
    //            sm.sendSingleSMS(contact_no, message);
    //            auth = 1;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //    return auth;
    //}

}