using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using PMC.DAL;

/// <summary>
/// Summary description for Insertdatafortutor
/// </summary>
public class Insertdatafortutor
{
    DataTable dt;
    List<SqlParameter> param;
    DataAccessLayer dac;
    public Insertdatafortutor()
    {
        //
        // TODO: Add constructor logic here
        //
    }



    public void InsertData(string name, string mobile, string email, string password, int isMobileverified, int isEmailverified, int isStausActive, string RegistrationDate)
    {
        string query = "INSERT INTO YourTableName (Name, Mobile, Email, Password, IsMobileVerified, IsEmailVerified, IsStatusActive, RegistrationDate) " +
                       "VALUES (@name, @mobile, @email, @password, @isMobileverified, @isEmailverified, @isStausActive, @RegistrationDate)";

        param = new List<SqlParameter>();
        param.Add(new SqlParameter("@name", name));
        param.Add(new SqlParameter("@mobile", mobile));
        param.Add(new SqlParameter("@email", email));
        param.Add(new SqlParameter("@password", password));
        param.Add(new SqlParameter("@isMobileverified", isMobileverified));
        param.Add(new SqlParameter("@isEmailverified", isEmailverified));
        param.Add(new SqlParameter("@isStausActive", isStausActive));
        param.Add(new SqlParameter("@RegistrationDate", RegistrationDate));

        dac = new DataAccessLayer(); // Assuming DataAccessLayer is a class in your DAL
      //  dac.ExecuteCommand(query, CommandType.Text, param.ToArray());
    }

}
