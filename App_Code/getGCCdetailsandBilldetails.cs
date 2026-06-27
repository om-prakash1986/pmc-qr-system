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
/// Summary description for getGCCdetailsandBilldetails
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class getGCCdetailsandBilldetails : System.Web.Services.WebService
{
    garbage_tax_calculation gtc = new garbage_tax_calculation();
    garbage_collection_data garbage_data = new garbage_collection_data();
    public getGCCdetailsandBilldetails()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void getGCCdetailandbilldetail(string holiding_no, string propertytype)
    {
        var ReturnValue = "";
        try
        {
            if (holiding_no != null && propertytype != null)
            {
                DataTable dt = new DataTable();
                string name = string.Empty;
                string mobile_no = string.Empty;
                string email = string.Empty;
                string property_type = string.Empty;
                string address = string.Empty;
                string pin = string.Empty;
                string circle = string.Empty;
                string monthlyamount = string.Empty;
                string Yearlyamount = string.Empty;
                string has_paid = "";

                if (holiding_no.ToLower().Contains("gcc"))
                {
                    dt = gtc.find_if_gccid_available(holiding_no);
                    if (dt != null)
                    {
                        name = dt.Rows[0]["name"].ToString();
                        mobile_no = dt.Rows[0]["contact_no"].ToString().Trim();
                        email = dt.Rows[0]["email_id"].ToString().Trim();
                        property_type = propertytype;
                        address = dt.Rows[0]["address"].ToString();
                        pin = dt.Rows[0]["pincode"].ToString();
                        circle = dt.Rows[0]["circle"].ToString();
                        has_paid = gtc.find_if_allready_paid(holiding_no);
                        if (has_paid == "yes")
                        {
                            monthlyamount = "0";
                            Yearlyamount = "0";
                            has_paid = "Paid";

                        }
                        else
                        {
                            string amount = gtc.find_amount_from_property_type(propertytype);
                            DataTable paid_till = gtc.amount_paid_till_date(holiding_no);
                            if (paid_till.Rows.Count > 0)
                            {
                                double year = Convert.ToDouble(paid_till.Rows[0]["to_year"]);
                                double month = Convert.ToDouble(paid_till.Rows[0]["to_month"]);

                              monthlyamount=  bind_grid(amount, year, month, propertytype);
                               
                            }
                            else
                            {
                              monthlyamount=  bind_grid(amount, 2019, 1, propertytype);
                            }

                            Yearlyamount = GetduePaidamontYearly(System.DateTime.Now.Year.ToString(), propertytype);

                        }
                        returndetails objreturndetails = new returndetails();
                        objreturndetails.holiding_no = holiding_no;
                        objreturndetails.mobile_no = mobile_no;
                        objreturndetails.monthlyamount = monthlyamount;
                        objreturndetails.Yearlyamount = Yearlyamount;
                        objreturndetails.Paymentstatus = has_paid;
                        objreturndetails.Name = name;
                        objreturndetails.Address = address;
                        objreturndetails.Holdingstatus = "";

                        ReturnValue = JsonConvert.SerializeObject(objreturndetails, Newtonsoft.Json.Formatting.Indented);
                    }
                    else
                    {
                        ReturnValue = "holding no not available";
                    }
                }
                else
                {
                    ReturnValue = "Please enter right holding no";
                }
            }
        }
        catch (Exception ex)
        {

        }
        Context.Response.Write(ReturnValue);
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
                Data = "Already Paid";
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
            Data = "Already Paid";
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
            Data = "Already Paid";
        }

        return Data;
    }

    public class returndetails
    {
        public string mobile_no { get; set; }
        public string holiding_no { get; set; }
        public string monthlyamount { get; set; }
        public string Yearlyamount { get; set; }
        public string Paymentstatus { get; set; }
        public string Holdingstatus { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

    }

}
