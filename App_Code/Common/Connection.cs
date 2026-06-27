using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PMC.Common
{
    class Connection
    {
        private static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PTax"].ConnectionString);
        public static SqlTransaction tr;

        public static SqlConnection String()
        {
            return con;
        }
        public static SqlConnection OpenConnection()
        {
try
{
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            
}
catch(Exception ex)
{
	throw ex;
}
return con;
        }
        public static void CloseConnection()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        public static void BeginTransaction()
        {
            OpenConnection();
            tr = con.BeginTransaction();
        }
        public static void Commit()
        {
            tr.Commit();
            CloseConnection();
        }
        public static void Rollback()
        {
            tr.Rollback();
            CloseConnection();
        }
    }
}
