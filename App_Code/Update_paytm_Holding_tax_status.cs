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
/// Summary description for Update_paytm_Holding_tax_status
/// </summary>
[WebService(Namespace = "http://pmc.bihar.gov.in/service")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Update_paytm_Holding_tax_status : System.Web.Services.WebService
{
    ptym_payment_for_holding_tax ptm = new ptym_payment_for_holding_tax();
    Maurya_shop ms = new Maurya_shop();
    public Update_paytm_Holding_tax_status()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void Update_paytm_holding_tax_payment_status(string holding_no,string amount,string tran_id)
    {
        var ReturnValue = "";
        returndetails objreturndetails = new returndetails();
        try
        {
           
            objreturndetails.holiding_no = holding_no;
	    // Added by Gyan Chand Verma
            string assessment_year = ms.Current_Fin_Year();

            if(ptm.getalready_paytm_payment_details(holding_no, tran_id)==true)
            {
                objreturndetails.Holdingstatus = "01";
                objreturndetails.msg = "already Paid";
            }

            else if (ptm.getalready_paytm_payment__byassesment(holding_no, assessment_year, amount) == true)
            {
                objreturndetails.Holdingstatus = "03";
                objreturndetails.msg = "Already success from another gateway/Other Transaction ID";
            }
            else 
            {
                string auth = ptm.insert_paytm_payment_details(holding_no, "Paytm Payment", assessment_year, amount, "Paid", tran_id, "PAYTM","Transaction is Successful");
                if (auth != "0")
                {
                    objreturndetails.holiding_no = holding_no;
                    objreturndetails.Holdingstatus = "00";
                    objreturndetails.msg = "Success";

			//Service Called By Manish
                    //Updated by Gyan Chand Verma
                    sendconformationtosparrow ssp = new sendconformationtosparrow();
                    if (holding_no != "")
                    {
                        DataTable dt = ssp.getPaytmCollectionInformationandSendBacktoSparrow(holding_no);
                        string holding_no1 = dt.Rows[0]["pid"].ToString();
                        string amount1 = dt.Rows[0]["amount"].ToString();
                        string paymentDate = Convert.ToDateTime(dt.Rows[0]["receive_date"]).ToString("yyyy-MM-dd HH:mm:ss");
                        ssp.sendconformationtosparrow_data(holding_no1, amount1, paymentDate);
                    }
                }
                else
                {
                    objreturndetails.holiding_no = holding_no;
                    objreturndetails.Holdingstatus = "02";
                    objreturndetails.msg = "Ohh Something wrong";
                }

            }
           

           
            ReturnValue = JsonConvert.SerializeObject(objreturndetails, Newtonsoft.Json.Formatting.Indented);

        }
        catch (Exception ex)
        {
            objreturndetails.holiding_no = holding_no;
            objreturndetails.Holdingstatus = "02";
            ReturnValue = JsonConvert.SerializeObject(objreturndetails, Newtonsoft.Json.Formatting.Indented);
        }
        Context.Response.Write(ReturnValue);
    }

    public class returndetails
    {
        public string holiding_no { get; set; }
     
        public string Holdingstatus { get; set; }
        public string msg { get; set; }
    }

}
