using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for claculate_the_interest
/// </summary>
public class claculate_the_interest
{
    public DataTable create_all_financial_year(double last_year, double current_year)
    {
        DataTable dt = new DataTable();
        double i = last_year;
        double j = current_year;
        dt.Clear();
        dt.Columns.Add("Financial_Year");
        while (i <= j)
        {
            string fin_year = create_fnancial_year(i);
            dt.Rows.Add(fin_year);
            i++;
        }
        return dt;
    }

    public string create_fnancial_year(double year)
    {
        double CurrentYear = year;
        double PreviousYear = year - 1;
        double NextYear = year + 1;
        string PreYear = PreviousYear.ToString();
        string NexYear = NextYear.ToString();
        string CurYear = CurrentYear.ToString();
        string FinYear = null;

        if (DateTime.Today.Month > 3)
            FinYear = CurYear + "-" + NexYear;
        else
            FinYear = PreYear + "-" + CurYear;
        return FinYear.Trim();
    }

    public double calculate_interest(string last_fin_year, string current_date)
    {
        double amount = 0;

        return amount;
    }

    public string fnGetCurrFiscalYear(string upto_paid_year, string date, double arv)
    {
        try
        {
            Int32 day = Convert.ToInt32(date.Split('.')[0]);
            Int32 month = Convert.ToInt32(date.Split('.')[1]);
            Int32 yr = Convert.ToInt32(date.Split('.')[2]);

            Int32 day1 = Convert.ToInt32(System.DateTime.Now.Day);
            Int32 mn1 = Convert.ToInt32(System.DateTime.Now.Month);
            Int32 yr1 = Convert.ToInt32(System.DateTime.Now.Year);

            string amount = arv.ToString();
            double new_amount = 0.0;
            // double arv_tax = 0.0;

            string Ass_year = null;

            string dbyear = upto_paid_year;
            string before = dbyear.Split('-')[0];
            string after = dbyear.Split('-')[1];
            double cr_year = Convert.ToDouble(dbyear.Split('-')[1]);

            // Get Current Financial Year
            string Total_Property_Tax = null;

            double CurrentYear = Convert.ToDouble(DateTime.Today.Year);
            double PreviousYear = Convert.ToDouble(DateTime.Today.Year) - 1;
            double NextYear = Convert.ToDouble(DateTime.Today.Year) + 1;

            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurrYear = CurrentYear.ToString();

            if (DateTime.Today.Month > 3)
            {
                Ass_year = CurrYear + "-" + NexYear;
            }
            else
            {
                Ass_year = PreYear + "-" + CurrYear;
            }

            //int currentMonth = DateTime.Now.Month;
            double currentMonth = Convert.ToDouble(DateTime.Now.Month);
            DateTime start = new DateTime(yr, month, day);
            //string n_date = Convert.ToDateTime("03/31/2013").ToString("dd-MMM-yyyy");
            DateTime middend = new DateTime(2013, 4, 1);
            DateTime middend1 = new DateTime(yr, 10, 1);
            DateTime end = new DateTime(yr1, mn1, day1);

            if (cr_year < 2010)
            {
                //Total_Property_Tax = "0";
                double fixmonth = 38;
                double amou = Convert.ToDouble(arv * fixmonth * (2.0 / 100));
                double diffMonths = (end.Month + end.Year * 12) - (middend.Month + middend.Year * 12) + 1;
                double amoun = arv * diffMonths * (1.5 / 100);
                Total_Property_Tax = (amou + amoun).ToString("#,##0.00");
            }
            else if (upto_paid_year == "2009-2010")
            {
                double fixmonth = 38;
                double amou = Convert.ToDouble(arv * fixmonth * (2.0 / 100));
                double diffMonths = (end.Month + end.Year * 12) - (middend.Month + middend.Year * 12) + 1;
                double amoun = arv * diffMonths * (1.5 / 100);
                Total_Property_Tax = (amou + amoun).ToString("#,##0.00");
            }
            else if (upto_paid_year == "2010-2011")
            {
                double fixmonth = 24;
                double amou = Convert.ToDouble(arv * fixmonth * (2.0 / 100));
                double diffMonths = (end.Month + end.Year * 12) - (middend.Month + middend.Year * 12) + 1;
                double amoun = arv * diffMonths * (1.5 / 100);
                Total_Property_Tax = (amou + amoun).ToString("#,##0.00");
            }
            else if (upto_paid_year == "2011-2012")
            {
                double fixmonth = 12;
                double amou = Convert.ToDouble(arv * fixmonth * (2.0 / 100));
                double diffMonths = (end.Month + end.Year * 12) - (middend.Month + middend.Year * 12) + 1;
                double amoun = arv * diffMonths * (1.5 / 100);
                Total_Property_Tax = (amou + amoun).ToString("#,##0.00");
            }
            else if (upto_paid_year == "2012-2013")
            {
                double fixmonth = 1.5;
                double amou = Convert.ToDouble(arv * fixmonth * (2.0 / 100));
                double diffMonths = (end.Month + end.Year * 12) - (middend.Month + middend.Year * 12) + 1;
                double amoun = arv * diffMonths * (1.5 / 100);
                Total_Property_Tax = (amou + amoun).ToString("#,##0.00");
            }
// Updated by Gyan Chand Verma
            else if (upto_paid_year == Ass_year)
            {
                if (currentMonth > 3 && currentMonth < 7)
                {
                    // Give discount of 5% on after calculation of ARV from month 1st april to 30th june
                    //new_amount = Convert.ToDouble(amount);
                    //double after_discount = (new_amount - (new_amount * 5) / 100);
                    //Total_Property_Tax = after_discount.ToString();
                    int xx = 0;
                    Total_Property_Tax = xx.ToString("#,##0.00");
                }
                else if (currentMonth > 6 && currentMonth < 10)
                {
                    // Don't Give discount on after calculation of ARV from 1st july to 30th Sept
                    //Total_Property_Tax = amount;
                    int xx = 0;
                    Total_Property_Tax = xx.ToString("#,##0.00");
                }
                else
                {
                    double diffMonths = (end.Month + end.Year * 12) - (middend1.Month + middend1.Year * 12) + 1;
                    double amoun = arv * diffMonths * (1.5 / 100);
                    Total_Property_Tax = amoun.ToString("#,##0.00");
                }
            }
            else
            {
                //double diffMonths = (end.Month + end.Year * 12) - (middend1.Month + middend1.Year * 12) + 1;
                //double amoun = arv * diffMonths * (1.5 / 100);
                //Total_Property_Tax = amoun.ToString();

                // added by arnav
                //if (end.Year.ToString() != middend1.Year.ToString())
                {
                    double diffMonths = (end.Month + end.Year * 12) - (middend1.Month + middend1.Year * 12) + 1;
                    double amoun = arv * diffMonths * (1.5 / 100);
                    Total_Property_Tax = amoun.ToString("#,##0.00");
                }
                //else
                //{
                //    double amoun = 0;
                //    Total_Property_Tax = amoun.ToString();
                //}
            }
            return Total_Property_Tax;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public claculate_the_interest()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}