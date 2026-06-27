using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Data.SqlClient;


/// <summary>
/// Summary description for DataAccessLayer
/// </summary>

namespace PMC.DAL
{
    public class DataAccessLayer
    {
        //216.67.234.112
        //p2sonline
        SqlConnection con;
        SqlCommand cmd;
        string connstring = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
        //SqlConnection con = new SqlConnection(connstring);
        public int update(string query, List<SqlParameter> paramters)
        {
            con = new SqlConnection(connstring);
            cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (paramters != null)
            {
                foreach (object param in paramters)
                {
                    cmd.Parameters.Add(param);
                }
            }
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }
        public object Scalar(String query, List<SqlParameter> parameters)
        {
            con = new SqlConnection(connstring);
            cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (parameters != null)
            {
                foreach (object param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }
            con.Open();
            Object obj = cmd.ExecuteScalar();
            con.Close();
            return obj;
        }
        public DataTable GetDataTable(String query, List<SqlParameter> parameters)
        {

            con = new SqlConnection(connstring);
            cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (parameters != null)
            {
                foreach (object param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
             sda.Fill(dt);
             return dt;

        }
    }
}
