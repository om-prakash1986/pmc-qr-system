using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for ARV_for_vaccant_land
/// </summary>
public class ARV_for_vaccant_land
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public string calculate_arv_for_vaccant_land(string area, string municipality_type, string road_type, string pid, string ass_year)
    {
        string land_tax = "0";

        string dbyear = ass_year;
        string before = dbyear.Split('-')[0];
        string after = dbyear.Split('-')[1];

        double cal_year = Convert.ToDouble(after);


        //double vaccant_area = Convert.ToDouble(area);
        //double munaciplity = find_municipality_value(municipality_type, road_type);

        //land_tax = (vaccant_area * munaciplity).ToString();

        //int is_govt = find_if_property_is_govt(pid);
        //if (is_govt == 1)
        //{
        //    double real_amount = Convert.ToDouble(land_tax);
        //    double payable_tax = ((real_amount * 75) / 100);
        //    land_tax = payable_tax.ToString();
        //}

        if (cal_year >= 2014)
        {
            double vaccant_area = Convert.ToDouble(area);
            double munaciplity = find_municipality_value(municipality_type, road_type);
            double vc_land = vaccant_area * munaciplity;
            //int vcx_land = Convert.ToInt32(vc_land);
            land_tax = vc_land.ToString();

            int is_govt = find_if_property_is_govt(pid);
            if (is_govt == 1)
            {
                double real_amount = Convert.ToDouble(land_tax);
                double payable_tax = ((real_amount * 75) / 100);
                land_tax = payable_tax.ToString();
            }

// Updated by Gyan Chand Verma
       string finyear = "";
            double cyear = DateTime.Now.Year;
            double curyear = Convert.ToDouble(cyear);
            double prevyear = curyear - 1;
            string pyear = prevyear.ToString();
            double nextyear = curyear + 1;
            string nyear = nextyear.ToString();
            double currentMonth = Convert.ToDouble(DateTime.Now.Month);

            if (DateTime.Now.Month > 3)
            {
                finyear = curyear.ToString() + "-" + nextyear;
            }
            else
            {
                finyear = pyear + "-" + curyear.ToString();
            }

            if (ass_year == finyear)
            {
                if (currentMonth > 3 && currentMonth < 7)
                {
                    // Give discount of 5% on after calculation of ARV from month 1st april to 30th june
                    double real_amount = Convert.ToDouble(land_tax);

                    //double after_discount = (real_amount - (real_amount * 5) / 100);
                    land_tax = real_amount .ToString("#,##0.00");
                }
                else if (currentMonth > 6 && currentMonth < 10)
                {
                    // Don't Give discount on after calculation of ARV from 1st july to 30th Sept
                    double real_amount = Convert.ToDouble(land_tax);
                    land_tax = real_amount.ToString("#,##0.00");
                }
                else
                {
                    double real_amount = Convert.ToDouble(land_tax);
                    land_tax = real_amount.ToString("#,##0.00");
                }
            }
            else
            {
                double real_amount = Convert.ToDouble(land_tax);
                land_tax = real_amount.ToString("#,##0.00");
            }
        }
        return land_tax;
    }

    public double find_municipality_value(string municipality_type, string road_type)
    {
        double value = 0;
        if (municipality_type == "Municipal Corporation" && road_type == "Principal Main Road")
        {
            value = 0.46;
        }
        else if (municipality_type == "Municipal Corporation" && road_type == "Main Road")
        {
            value = 0.37;
        }
        else if (municipality_type == "Municipal Corporation" && road_type == "Other Road")
        {
            value = 0.28;
        }


        if (municipality_type == "Municipal Council" && road_type == "Principal Main Road")
        {
            value = 0.36;
        }
        else if (municipality_type == "Municipal Council" && road_type == "Main Road")
        {
            value = 0.28;
        }
        else if (municipality_type == "Municipal Council" && road_type == "Other Road")
        {
            value = 0.19;
        }

        if (municipality_type == "Nagar Panchayat" && road_type == "Principal Main Road")
        {
            value = 0.28;
        }
        else if (municipality_type == "Nagar Panchayat" && road_type == "Main Road")
        {
            value = 0.19;
        }
        else if (municipality_type == "Nagar Panchayat" && road_type == "Other Road")
        {
            value = 0.11;
        }
        return value;
    }

    public int find_if_property_is_govt(string pid)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT ownershiptype from property_details where pid=@pid", con);
            cmd.Parameters.AddWithValue("@pid", pid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["ownershiptype"].ToString() == "Government Body" || dt.Rows[0]["ownershiptype"].ToString() == "Government Entity")
                {
                    number = 1;
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
        return number;
    }
}