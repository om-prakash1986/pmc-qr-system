using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for calculate_rate_interest
/// </summary>
public class calculate_rate_interest
{
    // Calculate Financial Year
    //public string fnGetCurrFiscalYear(string upto_paid_year, string date, double arv)
    //{
    //    try
    //    {
    //        string amount = arv.ToString();
    //        double new_amount = 0.0;
    //        double arv_tax = 0.0;

    //        string Ass_year = null;

    //        string dbyear = upto_paid_year;
    //        //string before = dbyear.Split('-')[0];
    //        //string after = dbyear.Split('-')[1];
    //        //int getbefyear = Convert.ToInt16(before);
    //        //int getaftyear = Convert.ToInt16(after);

    //        // Get Current Financial Year
    //        string Total_Property_Tax = null;


    //        //int CurrentYear = DateTime.Today.Year;
    //        //int PreviousYear = DateTime.Today.Year - 1;
    //        //int NextYear = DateTime.Today.Year + 1;
    //        double CurrentYear = Convert.ToDouble(DateTime.Today.Year);
    //        double PreviousYear = Convert.ToDouble(DateTime.Today.Year) - 1;
    //        double NextYear = Convert.ToDouble(DateTime.Today.Year) + 1;

    //        string PreYear = PreviousYear.ToString();
    //        string NexYear = NextYear.ToString();
    //        string CurrYear = CurrentYear.ToString();

    //        if (DateTime.Today.Month > 3)
    //        {
    //            Ass_year = CurrYear + "-" + NexYear;
    //        }
    //        else
    //        {
    //            Ass_year = PreYear + "-" + CurrYear;
    //        }

    //        //int currentMonth = DateTime.Now.Month;
    //        double currentMonth = Convert.ToDouble(DateTime.Now.Month);
    //        #region 2.0%
    //        if (dbyear == "2009-2010" || dbyear == "2010-2011" || dbyear == "2011-2012" || dbyear == "2012-2013")
    //        {
    //            if (dbyear == Ass_year)
    //            {
    //                //Console.WriteLine("Already paid");
    //                //Console.ReadLine();
    //            }
    //            else
    //            {
    //                if (currentMonth >= 4 && currentMonth <= 12)
    //                {
    //                    // In these Assessment year Penality will be calculated from the month of January 2010
    //                    Total_Property_Tax = amount;
    //                }
    //                else
    //                {
    //                    // Take 2 % interest / month from 1st oct of 2010 to 31st march 2013
    //                    //if ()
    //                    {
    //                        DateTime start = DateTime.ParseExact(date, "dd-MM-yyyy", null);
    //                        DateTime end = new DateTime(2013, 3, 31);
    //                        double diffMonths = (end.Month + end.Year * 12) - (start.Month + start.Year * 12) + 1;
    //                        double occupiedInterest = (diffMonths * 2) / 100;
    //                        new_amount = Convert.ToDouble(amount);
    //                        arv_tax = new_amount * occupiedInterest;
    //                        Total_Property_Tax = arv_tax.ToString("#,##0.00");
    //                    }
    //                    //DateTime Date1 = DateTime.Now.Date;
    //                    //DateTime Date2 = DateTime.ParseExact(date, "dd-MM-yyyy", null);
    //                    //double months = Date2.Month - Date1.Month + (Date2.Year - Date1.Year) * 12 - 1;
    //                    //double monthcountpositive = months * (-1);
    //                    //double occupied_Interest = (monthcountpositive * 2) / 100;
    //                    //new_amount = Convert.ToDouble(amount);
    //                    //arv_tax = new_amount * occupied_Interest;
    //                    //Total_Property_Tax = arv_tax.ToString();
    //                }
    //            }
    //        }
    //        else
    //        #endregion
    //        {
    //            // Comparison of Assessment Year and Financial Year After 2013
    //            if (dbyear == Ass_year)
    //            {
    //                Console.WriteLine("Already paid");
    //                Console.ReadLine();
    //            }
    //            else
    //            {
    //                if (currentMonth <= 6 && currentMonth >= 4)
    //                {
    //                    // Give discount of 5% on after calculation of ARV from month 1st april to 30th june
    //                    new_amount = Convert.ToDouble(amount);
    //                    double after_discount = new_amount - (5 / 100);
    //                    Total_Property_Tax = after_discount.ToString("#,##0.00");
    //                }
    //                else if (currentMonth > 6 && currentMonth <= 9)
    //                {
    //                    // Don't Give discount on after calculation of ARV from 1st july to 30th Sept
    //                    Total_Property_Tax = amount;
    //                }
    //                else
    //                {
    //                    // Take 1.5 % interest / month from 1st oct

    //                    DateTime Date1 = DateTime.Now.Date;
    //                    //DateTime Date2 = DateTime.ParseExact(date, "dd-MM-yyyy", null);
    //                    DateTime Date2 = Convert.ToDateTime(date);

    //                    double months = Date2.Month - Date1.Month + (Date2.Year - Date1.Year) * 12 - 1;
    //                    double monthcountpositive = months * (-1);
    //                    double occupied_Interest = (monthcountpositive * 1.5) / 100;
    //                    new_amount = Convert.ToDouble(amount);
    //                    arv_tax = new_amount * occupied_Interest;
    //                    Total_Property_Tax = arv_tax.ToString("#,##0.00");
    //                }
    //            }
    //        }
    //        return Total_Property_Tax;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    public static DataTable create_all_financial_year(double last_year, double current_year)
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

    public static string create_fnancial_year(double year)
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
}