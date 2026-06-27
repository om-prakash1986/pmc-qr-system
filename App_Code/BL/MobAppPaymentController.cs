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
    /// Summary description for MobAppPaymentController
    /// </summary>
    public class MobAppPaymentController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public MobAppPaymentController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public long chkPayment(long pid)
        {
            long msg = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@pid", pid));
            q = "select top 1 tbl_mobapp_payment.request_id,pid,paid_amount,payment_status,transaction_date from tbl_mobapp_payment ,tbl_mobapp_request where tbl_mobapp_payment.request_id=tbl_mobapp_request.request_id and pid=@pid and payment_status='"+"Success"+"'";
            dac = new DataAccessLayer();
            dt = new DataTable();
            dt = dac.GetDataTable(q, param);

            if (dt.Rows.Count > 0)
            {
                if (Convert.ToDateTime(dt.Rows[0]["transaction_date"].ToString()).Month == System.DateTime.Now.Month & Convert.ToDateTime(dt.Rows[0]["transaction_date"].ToString()).Year == System.DateTime.Now.Year)
                {
                    msg = Convert.ToInt64(dt.Rows[0]["request_id"].ToString());
                }
                else
                {
                    msg = 0;
                }
            }
            else
            {
                msg = 0;
            }


            return msg;
        }
        public long chkPayment(string propid)
        {
            long msg = 0;
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", propid));
            q = "select top 1 tbl_mobapp_payment.request_id,pid,paid_amount,payment_status,transaction_date from tbl_mobapp_payment ,tbl_mobapp_request where tbl_mobapp_payment.request_id=tbl_mobapp_request.request_id and property_id=@property_id and payment_status='" + "Success" + "'";
            dac = new DataAccessLayer();
            dt = new DataTable();
            dt = dac.GetDataTable(q, param);

            if (dt.Rows.Count > 0)
            {
                if (Convert.ToDateTime(dt.Rows[0]["transaction_date"].ToString()).Month == System.DateTime.Now.Month & Convert.ToDateTime(dt.Rows[0]["transaction_date"].ToString()).Year == System.DateTime.Now.Year)
                {
                    msg = Convert.ToInt64(dt.Rows[0]["request_id"].ToString());
                }
                else
                {
                    msg = 0;
                }
            }
            else
            {
                msg = 0;
            }


            return msg;
        }
        public DataTable chkPaymentForOrder(string propid)
        {
            
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", propid));
            q = "select top 1 tbl_mobapp_payment.request_id,pid,paid_amount,payment_status,transaction_date,transaction_id from tbl_mobapp_payment ,tbl_mobapp_request where tbl_mobapp_payment.request_id=tbl_mobapp_request.request_id and property_id=@property_id and payment_status='" + "Success" + "'";
            dac = new DataAccessLayer();
            dt = new DataTable();
            dt = dac.GetDataTable(q, param);

            return dt;
        }
        public int Insert(long request_id, string transaction_id, DateTime transaction_date, decimal paid_amount, string payment_status, string remarks, int status, DateTime entry_date, string ip_address) 
        {
            int rowEffected = 0;
            string q = "";
            param=new List<SqlParameter>();
            param.Add(new SqlParameter("@request_id", request_id));
            param.Add(new SqlParameter("@transaction_id", transaction_id));
            param.Add(new SqlParameter("@transaction_date", transaction_date));
            param.Add(new SqlParameter("@paid_amount", paid_amount));
            param.Add(new SqlParameter("@payment_status", payment_status));
            param.Add(new SqlParameter("@remarks", remarks));
            param.Add(new SqlParameter("@status", status));
            param.Add(new SqlParameter("@entry_date", entry_date));
            param.Add(new SqlParameter("@ip_address", ip_address));

            q = "insert into tbl_mobapp_payment	(request_id, transaction_id, transaction_date, paid_amount, payment_status, remarks, status, entry_date, ip_address) Values	(@request_id, 	@transaction_id, 	@transaction_date, 	@paid_amount, 	@payment_status, 	@remarks, 	@status, 	@entry_date, 	@ip_address)";
            dac = new DataAccessLayer();
            rowEffected = dac.update(q, param);
            return rowEffected;
        }
        public DataTable GetMobAppPaymentDetail(long request_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@request_id", request_id));
            q = "select * from tbl_mobapp_payment where request_id=@request_id and status=1";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }

        public DataTable GetMobAppPaymentByTransaction(long transaction_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@rtransaction_id", transaction_id));
            q = "select * from tbl_mobapp_payment where transaction_id=@transaction_id and status=1";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable chkPaymentStatusByRID(long rid)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@request_id", rid));
            q = "select * from tbl_mobapp_payment where request_id=@request_id and payment_status='Success' and status=1";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}
