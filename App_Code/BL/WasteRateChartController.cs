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
    /// Summary description for WasteRateChartController
    /// </summary>
    public class WasteRateChartController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public WasteRateChartController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //*******************************FOR Waste CALCULATION********************************************//
        //****************************WATE PROCESSING FUNCTION***********************************//
        public DataTable SearchWasteLastPayment(string property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select top 1 wr.id, wcm.holding_no as PID, wr.transaction_id, wr.receipt_no, wr.receipt_date, wr.fin_year from tbl_waste_consumer_master as wcm" +
                " inner join tbl_waste_consumer_details as wcd on wcd.consumer_mstr_id=wcm.id" +
                " inner join tbl_waste_reciept as wr on wr.consumer_detail_id=wcd.id" +
                " where wcm.holding_no=@property_id order by wr.id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable SearchAdvanceDue(string property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select top 1 wr.id, wcm.holding_no as PID,wr.receipt_no, wr.advance_amount, wr.advance_adjusted, wr.arrear_amount, wr.arrear_adjusted" +
                " from tbl_waste_consumer_master as wcm" +
                " inner join tbl_waste_consumer_details as wcd on wcd.consumer_mstr_id=wcm.id" +
                " inner join tbl_waste_reciept as wr on wr.consumer_detail_id=wcd.id" +
                " where wcm.holding_no=@property_id order by wr.id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable WasteRate(string property_id)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", property_id));
            q = "select wrc.id as waste_rate_chart_id, wcm.holding_no, wrc.amount,wcd.consumer_no, wcrd.fee_effectdate from tbl_waste_consumer_master as wcm" +
                " inner join tbl_waste_consumer_details as wcd on wcd.consumer_mstr_id=wcm.id" +
                " inner join tbl_waste_consumer_rate_detail as wcrd on wcrd.consumer_detail_id=wcd.id" +
                " inner join tbl_waste_rate_chart as wrc on wrc.range_mstr_id=wcrd.range_mstr_id" +
                " where wcm.holding_no=@property_id order by wrc.id desc";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public decimal WasteCharge(string property_id)
        {
            decimal waste_charge = 0;
            DataTable dtLastpayment = new DataTable();
            dtLastpayment = SearchWasteLastPayment(property_id);
            DataTable dtAdvanceDue = new DataTable();
            dtAdvanceDue = SearchAdvanceDue(property_id);
            DataTable dtwaste = new DataTable();
            dtwaste = WasteRate(property_id);
            DateTime frmDate;
            DateTime toDate;
            decimal adv=0;
            decimal predue = 0;
            decimal rate = 0;
            if (dtLastpayment.Rows.Count > 0)
            {
                //17----7
                frmDate = Convert.ToDateTime(dtLastpayment.Rows[0]["fin_year"].ToString().Substring(14,10)).AddDays(1);
            }
            else
            {
                frmDate = Convert.ToDateTime(dtwaste.Rows[0]["fee_effectdate"].ToString());
            }
            if (dtAdvanceDue.Rows.Count > 0)
            {
                if (dtAdvanceDue.Rows[0]["advance_amount"].ToString() == "")
                {
                    dtAdvanceDue.Rows[0]["advance_amount"] = "0";
                }
                if (dtAdvanceDue.Rows[0]["advance_adjusted"].ToString() == "")
                {
                    dtAdvanceDue.Rows[0]["advance_adjusted"] = "0";
                }
                if (dtAdvanceDue.Rows[0]["arrear_amount"].ToString() == "")
                {
                    dtAdvanceDue.Rows[0]["arrear_amount"] = "0";
                }
                if (dtAdvanceDue.Rows[0]["arrear_adjusted"].ToString() == "")
                {
                    dtAdvanceDue.Rows[0]["arrear_adjusted"] = "0";
                }
                adv = Convert.ToDecimal(dtAdvanceDue.Rows[0]["advance_amount"].ToString()) - Convert.ToDecimal(dtAdvanceDue.Rows[0]["advance_adjusted"].ToString());
                predue = Convert.ToDecimal(dtAdvanceDue.Rows[0]["arrear_amount"].ToString()) - Convert.ToDecimal(dtAdvanceDue.Rows[0]["arrear_adjusted"].ToString());
            }

            
            toDate = System.DateTime.Now.Month < 4 ? Convert.ToDateTime("31-Mar-" + System.DateTime.Now.Year) : Convert.ToDateTime("31-Mar-" + System.DateTime.Now.Year);
            TaxCalculationController tcc=new TaxCalculationController();
            
            int diffMonth=0;
            if (frmDate <= toDate)
            {
                diffMonth = tcc.GetMonthsBetween(frmDate, toDate);
            }
            if (dtwaste.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] at=dtwaste.Rows[i]["consumer_no"].ToString().Split('/');
                    if (at.Length == 4)
                    {
                        rate += Convert.ToDecimal(dtwaste.Rows[i]["amount"].ToString());
                    }
                }
                 
                waste_charge = (rate * diffMonth) - adv + predue;
            }
            return waste_charge;
        }
        public decimal WasteChargeCalculate(long pid)
        {
            decimal wasteCharge = 0;
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@pid", pid.ToString()));
            q = "select wcd.consumer_no from tbl_waste_consumer_master as wcm inner join tbl_waste_consumer_details as wcd on wcd.consumer_mstr_id=wcm.id where wcm.holding_no=@pid and wcm.status=1 ";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                string consumerList = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] at = dt.Rows[i]["consumer_no"].ToString().Split('/');
                    if (at.Length == 4)
                    {
                        consumerList = at[3] + ",";
                    }
                }
                consumerList = consumerList.Substring(0, consumerList.Length - 1);

                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@consumer", consumerList));
                q = "select isnull(sum(balance_amount),0) from tbl_waste_demand_details where payment_status=0 and [status]=1 and consumer_detail_id in (@consumer) ";
                //dac = new DataAccessLayer();
                wasteCharge = Convert.ToDecimal(dac.Scalar(q, param));
            }
            return wasteCharge;
        }
        
    }
}
