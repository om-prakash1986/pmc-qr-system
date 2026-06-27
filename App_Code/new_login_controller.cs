using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for new_login_controller
/// </summary>
public class new_login_controller
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    // create and send new otp

    public DataSet create_new_otp(string mobile_no)
    {
        mobile_logins ml = new mobile_logins();
        DataSet ds = new DataSet();
        // creating new otp
        string otp = create_otp();
        // insert new otp
        int auth = ml.insert_mobile_login(mobile_no, otp);
        // sending otp
        send_sms ss = new send_sms();
        string msg = "Your OTP for login is " + otp.ToString();
        ss.sendSingleSMS(mobile_no, msg);

        if (auth ==1)
        {
            SqlConnection con = new SqlConnection(strcon);
            con.Open();
            string query = "SELECT top 1 id,mobile_no,otp FROM mobile_logins where mobile_no=@mobile_no and otp=@otp ORDER BY id desc";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@otp", otp);
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                da.Fill(ds, "Partner");
            con.Close();
            //return ds;
        }
        return ds;
    }

    public string create_otp()
    {
        string otp = "";
        try
        {
            otp = null;
            Random random = new Random();
            string combination = "0123456789";
            StringBuilder captcha = new StringBuilder();
            for (int i = 0; i < 6; i++)
                captcha.Append(combination[random.Next(combination.Length)]);
            otp = captcha.ToString();
        }
        catch
        {
            throw;
        }
        return otp;
    }
}