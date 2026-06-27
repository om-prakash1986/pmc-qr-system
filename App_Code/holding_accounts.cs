using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for holding_accounts
/// </summary>
public class holding_accounts
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Holding User Account
     * Author   ::  Arnav
     * Date     ::  24-03-2017
     * ********************************************************/
    public string holding_user_account(string user_id)
    {
        string data = "";

        return data;
    }

    public int verify_mobile_no(string mobile_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("verify_user_account"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                int auth = Convert.ToInt32(cmd.ExecuteScalar());
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

    /*******************************************************
     * Till Here
     * ******************************************************/
	
}