using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for Generate_SAS_No
/// </summary>
public class Generate_SAS_No
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    holding_updations hu = new holding_updations();

    public string SAS_No(string PID)
    {
        DataTable dt = hu.bind_available_data_step_one_pid(PID);
        string finyear = "";
        string noofrows = "";
        double incofrows = 0.0;

        double cyear = DateTime.Now.Year;
        string str = cyear.ToString();
        double curyear = Convert.ToDouble(str);
        double prevyear = curyear - 1;
        string pyear = prevyear.ToString();
        double nextyear = curyear + 1;
        string nyear = nextyear.ToString();

        if (DateTime.Now.Month > 3)
        {
            finyear = curyear.ToString() + "-" + nextyear;
        }
        else
        {
            finyear = pyear + "-" + curyear.ToString();
        }

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select COUNT(id) from property_details", con);
            double rows = Convert.ToDouble(cmd.ExecuteScalar());

            if (rows > 0)
            {
                incofrows = rows + 1;
            }
            else
            {
                incofrows = 1;
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

        noofrows = incofrows.ToString("000000");
        string row = "";
        string ward = dt.Rows[0]["WardNo"].ToString();
	//string wardno = ward.Split('-')[1];

        row = "PMC/" + ward + "/" + noofrows + "/" + finyear;

        return row;
    }
}