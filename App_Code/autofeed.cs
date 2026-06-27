using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;
public class autofeed
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    //Added by Amrita Singh
   public int insert_auto_tax_feedback(string name, string email, string phone, string subject, string remarks)
    {
        int num = 0;
        SqlConnection con = new SqlConnection(strcon);
        string query = "insert into autotaxfeedback(name,email_id,phone,subject,remarks,since,time) values(@name,@email_id,@phone,@subject,@remarks,@since,@time)";
        using (SqlCommand cmd = new SqlCommand(query,con))
        {
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email_id", email);
            cmd.Parameters.AddWithValue("@phone",phone);
            cmd.Parameters.AddWithValue("@subject", subject);
            cmd.Parameters.AddWithValue("@remarks", remarks);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("h:mm:ss tt"));
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                num = 1;
            }
            catch(Exception ex)
            {
                
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            con.Close();
            con.Dispose();
        }
            return num;
    }
}