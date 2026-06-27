using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using PMC.DAL;


namespace PMC
{
    /// <summary>
    /// Summary description for TransactionDetailController
    /// </summary>
    public class TransactionDetailController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public TransactionDetailController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public int Insert(long property_id, int ward_id, int ulb_id, string receipt_no, DateTime payment_date, string payment_mode, string ref_no, string ref_date, string bank_name, string branch_name, decimal amount, decimal rebate, decimal penalty, decimal late_penalty, string pos_no, string remarks, int status, DateTime entry_date, string ip_address, int user_id, string receiver_ip, int receiver_id, DateTime receive_datetime, int receive_status, int form_receive_status, int form_receiver_id, DateTime form_receive_datetime) 
        {
            int rowEffected = 0;
            string q = "";
            param=new List<SqlParameter>() ;
            param.Add(new SqlParameter("@property_id", property_id));
            param.Add(new SqlParameter("@ward_id", ward_id));
            param.Add(new SqlParameter("@ulb_id", ulb_id));
            param.Add(new SqlParameter("@receipt_no", receipt_no));
            param.Add(new SqlParameter("@payment_date", payment_date));
            param.Add(new SqlParameter("@payment_mode", payment_mode));
            param.Add(new SqlParameter("@ref_no", ref_no));
            param.Add(new SqlParameter("@ref_date", ref_date));
            param.Add(new SqlParameter("@bank_name", bank_name));
            param.Add(new SqlParameter("@branch_name", branch_name));
            param.Add(new SqlParameter("@amount", amount));
            param.Add(new SqlParameter("@rebate", rebate));
            param.Add(new SqlParameter("@penalty", penalty));
            param.Add(new SqlParameter("@late_penalty", late_penalty));
            param.Add(new SqlParameter("@pos_no", pos_no));
            param.Add(new SqlParameter("@remarks", remarks));
            param.Add(new SqlParameter("@status", status));
            param.Add(new SqlParameter("@entry_date", entry_date));
            param.Add(new SqlParameter("@ip_address", ip_address));
            param.Add(new SqlParameter("@user_id", user_id));
            param.Add(new SqlParameter("@receiver_ip", receiver_ip));
            param.Add(new SqlParameter("@receiver_id", receiver_id));
            param.Add(new SqlParameter("@receive_datetime", receive_datetime));
            param.Add(new SqlParameter("@receive_status", receive_status));
            param.Add(new SqlParameter("@form_receive_status", form_receive_status));
            param.Add(new SqlParameter("@form_receiver_id", form_receiver_id));
            param.Add(new SqlParameter("@form_receive_datetime", form_receive_datetime));


            q = "insert into tbl_transaction_detail(property_id, 	ward_id, 	ulb_id, 	receipt_no, 	payment_date, 	payment_mode, 	ref_no, 	ref_date, 	bank_name, 	branch_name, 	amount, 	rebate, 	penalty, 	late_penalty, 	pos_no, 	remarks, 	status, 	entry_date, 	ip_address, 	user_id, 	receiver_ip, 	receiver_id, 	receive_datetime, 	receive_status, 	form_receive_status, 	form_receiver_id, 	form_receive_datetime) values(@property_id, 	@ward_id, 	@ulb_id, 	@receipt_no, 	@payment_date, 	@payment_mode, 	@ref_no, 	@ref_date, 	@bank_name, 	@branch_name, 	@amount, 	@rebate, 	@penalty, 	@late_penalty, 	@pos_no, 	@remarks, 	@status, 	@entry_date, 	@ip_address, 	@user_id, 	@receiver_ip, 	@receiver_id, 	@receive_datetime, 	@receive_status, 	@form_receive_status, 	@form_receiver_id, 	@form_receive_datetime)";
            rowEffected = dac.update(q, param);
            return rowEffected;
        }
        public DataTable GetTransactionDetails(long property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select * from tbl_transaction_detail where property_id=@property_id and ([status]=1 or [status]=2)";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetTransactionDetailsForApp(long pid)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@pid", pid));
            q = "select id,receipt_no,payment_mode,FORMAT (payment_date, 'dd-MM-yyyy ') as PDate,amount from tbl_transaction_detail where property_id=(select id from tbl_property_detail where pid=@pid and ([status]=1 or [status]=3 or [status]=2)) and ([status]=1 or [status]=2) order by payment_date desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public string GetTransactionID(string refno)
        {
            string tran_id = "";
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ref_no", refno));
            q = "select id from tbl_transaction_detail where ref_no=@ref_no";
            dac = new DataAccessLayer();
            tran_id = Convert.ToString(dac.Scalar(q,param));
            return tran_id;
        }
    }
}
