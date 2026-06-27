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
/// Summary description for check_paytm_holding_tax
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class check_paytm_holding_tax : System.Web.Services.WebService
{

    ptym_payment_for_holding_tax ptm = new ptym_payment_for_holding_tax();

    public check_paytm_holding_tax()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void check_paytm_holding_no(string holding_no)
    {
        var ReturnValue = "";
        try
        {
            returndetails objreturndetails = new returndetails();

            string auth = ptm.ptym_payment_for_holding_tax_check(holding_no);

            if (auth != "")
            {
                string getDetails = ptm.new_request(auth);
                string _proCharge = getDetails.Split(',')[0];
                string _waste = getDetails.Split(',')[1];
                string _payable = getDetails.Split(',')[2];
                objreturndetails.PropertyTax = _proCharge;
                objreturndetails.WasteCharge = _waste;
                objreturndetails.Amount = _payable;

                if (objreturndetails.Amount == "0")
                {
                    objreturndetails.Holdingstatus = "01";
                    objreturndetails.msg = "Already Paid";
                }
                else
                {
                    // removed for testing of new api 
                    DataTable dt = ptm.getDetails_for_paytm_payment(holding_no);
                    objreturndetails.holiding_no = holding_no;
                    objreturndetails.Propertytype = dt.Rows[0]["PropertyType"].ToString();
                    objreturndetails.Name = dt.Rows[0]["OwnerName"].ToString();
                    objreturndetails.Address = dt.Rows[0]["Address"].ToString();
                    objreturndetails.mobile_no = dt.Rows[0]["MobileNo"].ToString();
                    objreturndetails.Holdingstatus = "00";
                    objreturndetails.msg = "Success";


                    //objreturndetails.Holdingstatus = "02";
                    //objreturndetails.msg = "Unavailable due to testing going on.";
                }
            }
            else
            {
                objreturndetails.Holdingstatus = "02";
                objreturndetails.msg = "Holding No. Not Available";
            }
            ReturnValue = JsonConvert.SerializeObject(objreturndetails, Newtonsoft.Json.Formatting.Indented);

        }
        catch (Exception ex)
        {

        }
        Context.Response.Write(ReturnValue);
    }

    public class returndetails
    {
        public string holiding_no { get; set; }
        public string Propertytype { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string mobile_no { get; set; }
        public string PropertyTax { get; set; }
        public string WasteCharge { get; set; }
        public string Amount { get; set; }
        public string Holdingstatus { get; set; }
        public string msg { get; set; }
    }
}
