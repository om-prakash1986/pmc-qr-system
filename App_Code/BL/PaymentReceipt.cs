using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QRCoder;
using System.IO;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using PMC.DAL;

/// <summary>
/// Summary description for receipt
/// </summary>
namespace PMC
{
    public class PaymentReceipt
    {
        public PaymentReceipt()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DataTable getData(long id)
        {
            DataTable dt = new DataTable();
            List<SqlParameter> param;
            DataAccessLayer dac;

            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", id));
            q = "select * from tbl_payment_receipt where transaction_id=@id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);

            return dt;

        }
        public string getSoliWasteCharge(string receipt_no)
        {
            List<SqlParameter> param;
            DataAccessLayer dac;

            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@receipt_no", receipt_no));

            q = "select concat(isnull(total_recieved_amount,0),'-',isnull(total_payable_amount,0)) from tbl_waste_reciept where receipt_no=@receipt_no";
            dac = new DataAccessLayer();
            string waste_charge = Convert.ToString(dac.Scalar(q, param));

            return waste_charge;
        }
        public string getUser(string receiptno)
        {
            List<SqlParameter> param;
            DataAccessLayer dac;

            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@receipt_no", receiptno));

            q = "  select concat([name],',',CONVERT(VARCHAR(10), payment_date,105),',',CONVERT(VARCHAR(10), payment_date,108)) from tbl_transaction_detail,tbl_user_master where receipt_no=@receipt_no and tbl_transaction_detail.user_id=tbl_user_master.id";
            dac = new DataAccessLayer();
            string username = Convert.ToString(dac.Scalar(q, param));

            return username;
        }
    }
}