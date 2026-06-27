using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

/// <summary>
/// Summary description for getGccTransactionstatus
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class getGccTransactionstatus : System.Web.Services.WebService
{
    garbage_tax_calculation gtc = new garbage_tax_calculation();
    garbage_collection_data garbage_data = new garbage_collection_data();
    public getGccTransactionstatus()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void getGCCTransactiondetails(string holiding_no, string propertytype, string transactionid, string amount, string transactiondate, string paymentmode, string agency)
    {
             returndetails objreturndetails = new returndetails();
        try
        {
            if (holiding_no != null && propertytype != null)
            {
                DataTable dt = new DataTable();
                string name = string.Empty;
                string paidstatus = string.Empty;
                string email = string.Empty;
                string property_type = string.Empty;
                string address = string.Empty;
                string monthlyamount = string.Empty;
                string Yearlyamount = string.Empty;

                if (holiding_no.ToLower().Contains("gcc"))
                {
                    dt = gtc.find_if_gccid_available(holiding_no);
                }
                else
                {
                    dt = gtc.find_if_property_available(holiding_no);
                }
                if (dt.Rows.Count > 0)
                {

                    string paid = "no";
                    double paid_for_month = 0;
                    double paid_to_month = 0;
                    double from_year = 0;

                    DataTable dtcheck = new DataTable();
                    dtcheck = gtc.find_if_allready_paidby_paytmTransaction(holiding_no, transactionid);
                    if (dtcheck.Rows.Count > 0)
                    {

                        paidstatus = "01";

                    }
                    else if (dtcheck.Rows.Count == 0)
                    {

                        //double current_year = Convert.ToDouble(System.DateTime.Now.Year);
                        //double current_month = Convert.ToDouble(System.DateTime.Now.Month);
                        //paid_for_month = Convert.ToDouble(dtcheck.Rows[0]["from_month"]);
                        //paid_to_month = Convert.ToDouble(dtcheck.Rows[0]["to_month"]);
                        //from_year = Convert.ToDouble(dtcheck.Rows[0]["from_year"]);
                        //double to_year = Convert.ToDouble(dtcheck.Rows[0]["to_year"]);

                        //if (to_year >= current_year)
                        //{
                        //    if (paid_to_month >= current_month)
                        //    {
                        //        paid = "yes";
                        //    }
                        //    else if (paid_to_month < current_month)
                        //    {
                        //        paid = "no";
                        //    }
                        //}
                        //else if (to_year < current_year)
                        //{
                        //    paid = "no";
                        //}

                        string amount1 = gtc.find_amount_from_property_type(propertytype);
                        DataTable paid_till = gtc.amount_paid_till_date(holiding_no);
                        if (paid_till.Rows.Count > 0)
                        {
                            double year = Convert.ToDouble(paid_till.Rows[0]["to_year"]);
                            double month = Convert.ToDouble(paid_till.Rows[0]["to_month"]);

                            monthlyamount = bind_grid(amount1, year, month, propertytype);

                        }
                        else
                        {
                            monthlyamount = bind_grid(amount1, 2019, 1, propertytype);
                        }

                        Yearlyamount = GetduePaidamontYearly(System.DateTime.Now.Year.ToString(), propertytype);
                        if (paymentmode == "2" && Yearlyamount == amount)
                        {

                            DataTable mn = find_from_date("12");
                            string year = mn.Rows[0]["year"].ToString();
                            string current_year = mn.Rows[0]["from_year"].ToString();
                            string to_year = mn.Rows[0]["to_year"].ToString();
                            string from_month = mn.Rows[0]["to_year"].ToString();
                            string to_month = mn.Rows[0]["to_month"].ToString();
                            string scheme = "12";
                            string id = garbage_data.find_current_scheme(scheme, propertytype);

                            int a = gtc.Insert_garbage_collection_payment_paytm(transactionid, holiding_no, amount, "Paid", "Active", year, current_year, to_year, from_month, to_month, "PaytmApp");
                            paidstatus = "00";
                        }
                        else if (paymentmode == "1" && monthlyamount == amount)
                        {
                            DataTable mn = find_from_date("1");
                            string year = mn.Rows[0]["year"].ToString();
                            string current_year = mn.Rows[0]["from_year"].ToString();
                            string to_year = mn.Rows[0]["to_year"].ToString();
                            string from_month = mn.Rows[0]["from_month"].ToString();
                            string to_month = mn.Rows[0]["to_month"].ToString();
                            int a = gtc.Insert_garbage_collection_payment_paytm(transactionid, holiding_no, amount, "Paid", "Active", year, current_year, to_year, from_month, to_month, "PaytmApp");
                            paidstatus = "00";
                        }
                        else
                        {
                            paidstatus = "03";
                        }


                    }

                    else {
                        paidstatus = "02";
                    }
                   
                    
                    objreturndetails.status = paidstatus;
                }
                else
                {
                    objreturndetails.status = "Holding No not available";
                   
                }
            }
        }
        catch (Exception ex)
        {

        }
        Context.Response.Write(JsonConvert.SerializeObject(objreturndetails, Newtonsoft.Json.Formatting.Indented));
    }
    public DataTable find_from_date(string months)
    {
        DataTable ds = new DataTable();
        ds.Columns.Add("Year");
        ds.Columns.Add("from_year");
        ds.Columns.Add("to_year");
        ds.Columns.Add("from_month");
        ds.Columns.Add("to_month");

        double current_year = System.DateTime.Now.Year;
        double current_month = System.DateTime.Now.Month;
        double from_year = System.DateTime.Now.Year;
        string to_months = months;

        if (to_months == "12")
        {
            int month_add = Convert.ToInt32(to_months);
            DateTime curr_date = Convert.ToDateTime(System.DateTime.Now.ToString("dd-MMM-yyyy"));
            DateTime newDate = curr_date.AddMonths(month_add);

            String dy = newDate.Day.ToString();
            String mn = newDate.Month.ToString();
            String yy = newDate.Year.ToString();

            ds.Rows.Add(current_year, current_year, yy.ToString(), current_month, mn.ToString());
        }
        else
        {
            ds.Rows.Add(current_year, current_year, current_year, current_month, current_month);
        }

        return ds;
    }
    private string GetduePaidamontYearly(string year, string property_type)
    {
        string Data = string.Empty;
        DataTable dt = garbage_data.select_all_schemes(year, property_type);
        Data = dt.Rows[0]["amount"].ToString();
        return Data;
    }
    public string bind_grid(string amount, double till_year, double till_month, string property_type)
    {

        string Data = string.Empty;

        DataTable dt = new DataTable();
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("PropertyType");
        dt.Columns.Add("Amount");

        double calculation_from_year = 2019;
        double calculation_from_month = 1;

        double current_year = Convert.ToDouble(System.DateTime.Now.Year);
        double current_month = Convert.ToDouble(System.DateTime.Now.Month);

        int current_date_month = Convert.ToInt32(System.DateTime.Now.Day);
        int current_year_month = Convert.ToInt32(current_month);
        int current_year_year = Convert.ToInt32(current_year);
        int j = 1;
        if (current_year == till_year)
        {
            if (current_month > till_month)
            {
                double difference = (current_month - till_month);
                for (int i = 0; i <= difference; i++)
                {
                    dt.Rows.Add(current_year, j, property_type, amount);
                    j++;
                }
            }
            else if (current_month <= till_month)
            {
                Data = "0";
            }
        }
        else if (current_year > till_year)
        {
            DateTime start = new DateTime(2019, 1, 1);
            DateTime end = new DateTime(current_year_year, current_year_month, current_date_month);
            var diffMonths = (end.Month + end.Year * 12) - (start.Month + start.Year * 12);
            int m = 1;
            for (int i = 0; i < diffMonths; i++)
            {
                dt.Rows.Add(current_year, m, property_type, amount);
                m++;
            }
        }
        else if (current_year < till_year)
        {
            Data = "0";
        }

        int k = 0;
        double pay_amount = 0;
        if (dt.Rows.Count > 0)
        {
            while (k < dt.Rows.Count)
            {
                pay_amount += Convert.ToDouble(dt.Rows[k]["Amount"]);
                k++;
            }
            Data = pay_amount.ToString();
        }
        else
        {
            Data = "0";
        }

        return Data;
    }


    public class returndetails
    {
        //public string propertytype { get; set; }
        //public string holiding_no { get; set; }
        //public string amount { get; set; }
        //public string transectionid { get; set; }
        public string status { get; set; }

    }

}
