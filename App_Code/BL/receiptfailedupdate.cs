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
    /// Summary description for receiptfailedupdate
    /// </summary>
    public class receiptfailedupdate
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public receiptfailedupdate()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public void receiptgenerate(long requestId, long property_id, string TransID, DateTime TransDate, decimal Pamount, decimal LateSubmission, string SAS, string mid)
        {
            int msg = 0;

            DateTime frmDate = Convert.ToDateTime("01-" + System.DateTime.Now.ToString("dd-MMM-yyyy").Split('-')[1] + "-" + System.DateTime.Now.Year);
            DateTime toDate = System.DateTime.Now;
            SearchDataController sdc = new SearchDataController();
            decimal ArrearAdvance = sdc.ArrearAdvance(property_id);
            decimal ArrearDue = sdc.ArrearDue(property_id);
            //DataTable dtYearlyTax = new DataTable();
            //dtYearlyTax = new DataTable();
            string q = "";
            //param = new List<SqlParameter>();
            //param.Add(new SqlParameter("@property_id", property_id));
            ////param.Add(new SqlParameter("@fromDate", frmDate));
            ////param.Add(new SqlParameter("@toDate", toDate));
            //q = "select * from tbl_yearly_tax_assessment_tmp where property_id=(select top 1 id from tbl_property_detail_tmp where property_detail_id=@property_id order by id desc)";
            //dac = new DataAccessLayer();
            //dtYearlyTax = dac.GetDataTable(q, param);
            //long iden = 0;
            //for (int i = 0; i < dtYearlyTax.Rows.Count; i++)
            //{

            //    //transfer data from tbl_yearly_tax_assessment_tmp to tbl_yearly_tax_assessment

            //    q = "";
            //    param = new List<SqlParameter>();
            //    param.Add(new SqlParameter("@property_id", property_id));
            //    param.Add(new SqlParameter("@id", dtYearlyTax.Rows[i]["id"].ToString()));

            //    q = "update [dbo].[tbl_adjustment_arrear] set [status]=0,balance_amt=0 where property_id=@property_id;";
            //    q += "update [dbo].[tbl_yearly_tax_assessment] set paid_status=1,balance_amount=0 where property_id=@property_id;INSERT INTO [dbo].[tbl_yearly_tax_assessment] ([property_id],[ulb_id],[fin_year],[total_fy_tax],[vacant_tax],[vacant_rate],[vacant_rate_id]";
            //    q += ",[total_tax],[entry_date],[user_id],[paid_status],[balance_amount]) ";
            //    q += "SELECT @property_id,[ulb_id],[fin_year],[total_fy_tax],[vacant_tax],[vacant_rate],[vacant_rate_id],[total_tax],GETDATE(),2,1,0 ";
            //    q += " FROM [dbo].[tbl_yearly_tax_assessment_tmp] where id=@id; select @@IDENTITY";
            //    iden = Convert.ToInt64(dac.Scalar(q, param));
            //    //transfer data from tbl_yearly_floor_assessment_tmp to tbl_yearly_floor_assessment

            //    q = "";
            //    param = new List<SqlParameter>();
            //    param.Add(new SqlParameter("@id", dtYearlyTax.Rows[i]["id"].ToString()));
            //    q = "INSERT INTO [dbo].[tbl_yearly_floor_assessment]([yearly_taxes_id],[floor_id],[usage_type_id],[occupancy_type_id],[construction_type_id] ";
            //    q += ",[builtup_area],[carpet_area],[use_type],[occupancy_factor],[arv],[total_tax_factor],[total_tax],[arv_rate],[occupancy_type_detail_id]) ";
            //    q += "SELECT " + iden + ", [floor_id],[usage_type_id],[occupancy_type_id],[construction_type_id],[builtup_area],[carpet_area],[use_type] ";
            //    q += ",[occupancy_factor],[arv],[total_tax_factor],[total_tax],[arv_rate],[occupancy_type_detail_id] FROM [dbo].[tbl_yearly_floor_assessment_tmp] where yearly_taxes_id=@id";
            //    dac.update(q, param);
            //    //transfer data from tbl_assessment_rebate_penalty_tmp to tbl_assessment_rebate_penalty

            //    q = "";
            //    param = new List<SqlParameter>();
            //    param.Add(new SqlParameter("@property_id", property_id));
            //    param.Add(new SqlParameter("@id", dtYearlyTax.Rows[i]["id"].ToString()));
            //    q = "INSERT INTO [dbo].[tbl_assesment_rebate_penalty]([yearly_tax_id],[penalty],[tax_with_penalty],[current_tax_rebate],[water_harvt_rebate] ";
            //    q += ",[total_rebate],[final_tax],[property_id]) ";
            //    q += "SELECT " + iden + ", [penalty],[tax_with_penalty],[current_tax_rebate],[water_harvt_rebate],[total_rebate],[final_tax],@property_id FROM [dbo].[tbl_assesment_rebate_penalty_tmp] where yearly_tax_id=@id";
            //    dac.update(q, param);
            //}

            ////transfer data from tbl_yearly_vacant_land_assessment_tmp to tbl_yearly_vacant_land_assessment 


            //q = "";
            //param = new List<SqlParameter>();
            //param.Add(new SqlParameter("@property_id", property_id));
            //param.Add(new SqlParameter("@FromDate", frmDate));
            //param.Add(new SqlParameter("@ToDate", toDate));
            //param.Add(new SqlParameter("@id", property_id));
            //q = "INSERT INTO [dbo].[tbl_yearly_vacant_land_assessment]([property_id],[type],[area_decimal],[area_sqft],[area_sqmtr],[vacant_rate_id],[occupancy_type_id] ";
            //q += ",[occupancy_factor],[tax],[effect_from],[user_id],[entry_date],[status]) ";
            //q += "SELECT @property_id,[type],[area_decimal],[area_sqft],[area_sqmtr],[vacant_rate_id],[occupancy_type_id],[occupancy_factor],[tax] ";
            //q += ",[effect_from],2,GETDATE(),1 FROM [dbo].[tbl_yearly_vacant_land_assessment_tmp] where entry_date>=@FromDate and entry_date<=@ToDate and property_id=@id";
            //dac.update(q, param);


            ////transfer data from tbl_water_tax_detail_tmp to tbl_water_tax_detail
            //q = "";
            //param = new List<SqlParameter>();
            //param.Add(new SqlParameter("@property_id", property_id));
            //param.Add(new SqlParameter("@id", property_id));

            //param.Add(new SqlParameter("@FromDate", frmDate));
            //param.Add(new SqlParameter("@ToDate", toDate));

            //q = "INSERT INTO [dbo].[tbl_water_tax_detail]([property_id],[amount],[entry_date],[user_id],[paid_status],[status]) ";
            //q += "SELECT @property_id,[amount],GETDATE(),2,1,1 FROM [dbo].[tbl_water_tax_detail_tmp] where property_id=@id and entry_date>=@FromDate and entry_date<=@ToDate";
            //dac.update(q, param);

            //Get ward_id
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", property_id));
            q = "select ward_id from tbl_property_detail where id=@id";
            dac = new DataAccessLayer();
            int wardid = Convert.ToInt32(dac.Scalar(q, param));

            //insert into tbl_transaction_detail
            long ApplicationTransID = 0;
            decimal am = 0;
            decimal rebate = 0;
            decimal penalty = 0;
            decimal WaterTaxCharge = 0;

            q = "";

            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", property_id));
            param.Add(new SqlParameter("@FromDate", frmDate));
            param.Add(new SqlParameter("@ToDate", toDate.AddDays(1)));
            q = "";
            q += " select tbl_yearly_tax_assessment.property_id,sum(tbl_yearly_tax_assessment.total_fy_tax) as tax,";
            q += "sum(tbl_assesment_rebate_penalty.penalty) as penalty,sum(tbl_assesment_rebate_penalty.total_rebate) as rebate ";
            q += " from tbl_yearly_tax_assessment,tbl_assesment_rebate_penalty where tbl_yearly_tax_assessment.id=tbl_assesment_rebate_penalty.yearly_tax_id ";
            q += " and tbl_yearly_tax_assessment.property_id=@id and tbl_yearly_tax_assessment.entry_date>=@FromDate and ";
            q += " tbl_yearly_tax_assessment.entry_date<=@ToDate group by tbl_yearly_tax_assessment.property_id";
            DataTable dtTransaction = new DataTable();
            dtTransaction = dac.GetDataTable(q, param);
            if (dtTransaction.Rows.Count > 0)
            {

                rebate = Convert.ToDecimal(dtTransaction.Rows[0]["rebate"].ToString());
                penalty = Convert.ToDecimal(dtTransaction.Rows[0]["penalty"].ToString());
                am = Convert.ToDecimal(dtTransaction.Rows[0]["tax"].ToString()) + penalty - rebate + ArrearDue - ArrearAdvance;

            }
            decimal penaltyLateSubm = LateSubmission;//Condition


            WaterTaxCharge = sdc.WaterTax(property_id, 1);
            am = Math.Round(am + penaltyLateSubm + WaterTaxCharge, 0, MidpointRounding.AwayFromZero);

            //q = "";
            //param = new List<SqlParameter>();
            //param.Add(new SqlParameter("@property_id", property_id));
            //param.Add(new SqlParameter("@pmode", mid));
            //param.Add(new SqlParameter("@ref_no", TransID));
            //param.Add(new SqlParameter("@ref_date", TransDate));
            //param.Add(new SqlParameter("@amount", am));
            //param.Add(new SqlParameter("@rebate", rebate));
            //param.Add(new SqlParameter("@penalty", penalty));
            //param.Add(new SqlParameter("@late_penalty", penaltyLateSubm));
            //q = "INSERT INTO [dbo].[tbl_transaction_detail]([property_id],[ward_id],[ulb_id],[payment_date],[payment_mode],[ref_no],[ref_date] ";
            //q += ",[amount],[rebate],[penalty],[late_penalty],[remarks],[status],[entry_date],[ip_address],[user_id] ";
            //q += ",[receiver_ip],[receiver_id],[receive_datetime],[receive_status]) values(@property_id," + wardid + ",1,GETDATE(),@pmode,@ref_no,@ref_date,@amount,@rebate,";
            //q += "@penalty,@late_penalty,'Online Payment',1,GETDATE(),convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),2,convert(varchar(100),CONNECTIONPROPERTY('local_net_address')),";
            //q += "2,GETDATE(),1); select @@IDENTITY";
            //ApplicationTransID = Convert.ToInt64(dac.Scalar(q, param));

            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", property_id));
            q = "select top 1 * from tbl_transaction_detail where property_id=@id and (receipt_no is null)";
            DataTable dtExistTransactionCheck = dac.GetDataTable(q, param);
            if (dtExistTransactionCheck.Rows.Count > 0)
            {
                ApplicationTransID=Convert.ToInt64(dtExistTransactionCheck.Rows[0]["id"].ToString());
                string Receipt_No = "OLP" + ApplicationTransID;


                //insert into tbl_collection_detail
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@id", property_id));
                param.Add(new SqlParameter("@transaction_id", ApplicationTransID));
                q = "select * from tbl_collection_detail where property_id=@id and transaction_id=@transaction_id";
                DataTable dtCollectionCheck = dac.GetDataTable(q, param);


                if (dtCollectionCheck.Rows.Count <= 0)
                {
                    param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@id", property_id));
                    param.Add(new SqlParameter("@FromDate", frmDate));
                    param.Add(new SqlParameter("@ToDate", toDate.AddDays(1)));

                    q = "INSERT INTO [dbo].[tbl_collection_detail]([property_id],[ward_id],[ulb_id],[transaction_id],[yearly_tax_assesment_id],[fin_year],[total_fy_tax] ";
                    q += ",[vacant_tax],[vacant_rate],[vacant_rate_id],[total_tax],[penalty],[tax_with_penalty],[current_tax_rebate],[water_harvt_rebate],[total_rebate] ";
                    q += ",[paid_amount],[status]) ";

                    q += " select tbl_yearly_tax_assessment.property_id," + wardid + ",1," + ApplicationTransID + ",tbl_yearly_tax_assessment.id,";
                    q += " tbl_yearly_tax_assessment.fin_year,tbl_yearly_tax_assessment.total_fy_tax,tbl_yearly_tax_assessment.vacant_tax,tbl_yearly_tax_assessment.vacant_rate,";
                    q += "tbl_yearly_tax_assessment.vacant_rate_id,tbl_yearly_tax_assessment.total_tax,tbl_assesment_rebate_penalty.penalty,tbl_assesment_rebate_penalty.tax_with_penalty,";
                    q += "tbl_assesment_rebate_penalty.current_tax_rebate,tbl_assesment_rebate_penalty.water_harvt_rebate,tbl_assesment_rebate_penalty.total_rebate,";
                    q += " tbl_assesment_rebate_penalty.final_tax,1 from tbl_yearly_tax_assessment,tbl_assesment_rebate_penalty where tbl_yearly_tax_assessment.id=tbl_assesment_rebate_penalty.yearly_tax_id and tbl_yearly_tax_assessment.property_id=@id and tbl_yearly_tax_assessment.entry_date>=@FromDate and tbl_yearly_tax_assessment.entry_date<=@ToDate";
                    dac.update(q, param);
                    //insert into tbl_payment_receipt
                }
                int monFin = System.DateTime.Now.Month;
                int yrFin = System.DateTime.Now.Year;

                string AssessYear;
                if (monFin < 4)
                {
                    AssessYear = (yrFin - 1) + "-" + yrFin;
                }
                else
                {
                    AssessYear = yrFin + "-" + (yrFin + 1);
                }

                DataTable dtPropertyDetail = new DataTable();
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@id", property_id));
                q = "select circle.circle_name,ward.ward_no,property.PID,RevenueCircle.rev_circle, property.new_holding_no, property.old_holding_no,property.mobile_no," +
                    "property.[address],osm.owner_name,osm.guardian_name " +

                    " from tbl_property_detail as property" +
                    " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                    " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                    " inner join tbl_revenue_circle_master as RevenueCircle on RevenueCircle.id=property.revenue_circle_id" +
                    " inner join (select property_id,STRING_AGG(owner_name,', ') as owner_name,STRING_AGG(guardian_name,', ') as guardian_name from tbl_owner_detail group by property_id) as osm on osm.property_id=property.id" +
                    " where property.id=@id ";
                dtPropertyDetail = dac.GetDataTable(q, param);

                DataTable dtTransactionDetail = new DataTable();
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@transaction_id", ApplicationTransID));
                q = "select * from tbl_collection_detail where transaction_id=@transaction_id";
                dtTransactionDetail = dac.GetDataTable(q, param);
                string fy;
                string minFY = "";
                string maxFY = "";
                decimal arrearPayableAmount = 0;
                decimal currentPayableAmount = 0;
                decimal arrearPenalty = 0;
                decimal currentPenalty = 0;
                decimal arrearTotalPayable = 0;
                decimal currentTotalPayable = 0;
                decimal RebateAmount = 0;
                decimal rainWaterHarvest = 0;
                for (int i = 0; i < dtTransactionDetail.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        minFY = dtTransactionDetail.Rows[i]["fin_year"].ToString();
                        maxFY = dtTransactionDetail.Rows[i]["fin_year"].ToString();
                    }
                    else
                    {
                        fy = dtTransactionDetail.Rows[i]["fin_year"].ToString().Split('-')[0];
                        if (Convert.ToInt32(fy) < Convert.ToInt32(minFY.Split('-')[0]))
                        {
                            minFY = dtTransactionDetail.Rows[i]["fin_year"].ToString();
                        }
                        if (Convert.ToInt32(fy) > Convert.ToInt32(maxFY.Split('-')[0]))
                        {
                            maxFY = dtTransactionDetail.Rows[i]["fin_year"].ToString();
                        }
                    }
                    if (dtTransactionDetail.Rows[i]["fin_year"].ToString() != AssessYear)
                    {
                        arrearPayableAmount += Convert.ToDecimal(dtTransactionDetail.Rows[i]["total_tax"].ToString());
                        arrearPenalty += Convert.ToDecimal(dtTransactionDetail.Rows[i]["penalty"].ToString());
                        arrearTotalPayable += Convert.ToDecimal(dtTransactionDetail.Rows[i]["tax_with_penalty"].ToString());

                    }
                    else
                    {
                        currentPayableAmount += Convert.ToDecimal(dtTransactionDetail.Rows[i]["total_tax"].ToString());
                        currentPenalty += Convert.ToDecimal(dtTransactionDetail.Rows[i]["penalty"].ToString()) + penaltyLateSubm;
                        currentTotalPayable += Convert.ToDecimal(dtTransactionDetail.Rows[i]["tax_with_penalty"].ToString()) + penaltyLateSubm;
                    }
                    RebateAmount += Convert.ToDecimal(dtTransactionDetail.Rows[i]["total_rebate"].ToString());
                    rainWaterHarvest += Convert.ToDecimal(dtTransactionDetail.Rows[i]["water_harvt_rebate"].ToString());
                }

                decimal totalpayableAmount = 0;
                totalpayableAmount = Math.Round(arrearTotalPayable + currentTotalPayable, 0, MidpointRounding.AwayFromZero);
                decimal ActualPaymentAmount = Math.Round(totalpayableAmount - RebateAmount + WaterTaxCharge + ArrearDue - ArrearAdvance, 0, MidpointRounding.AwayFromZero);

                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@transaction_id", ApplicationTransID));
                param.Add(new SqlParameter("@property_id", property_id));
                param.Add(new SqlParameter("@pid", dtPropertyDetail.Rows[0]["PID"].ToString()));
                param.Add(new SqlParameter("@receipt_fy", minFY + " To " + maxFY));
                param.Add(new SqlParameter("@receipt_no", Receipt_No));
                param.Add(new SqlParameter("@receipt_date", Convert.ToDateTime(dtExistTransactionCheck.Rows[0]["payment_date"].ToString()).ToString("dd-MMM-yyyy")));
                param.Add(new SqlParameter("@received_from", dtPropertyDetail.Rows[0]["owner_name"].ToString()));
                string naration = "Circle : " + dtPropertyDetail.Rows[0]["circle_name"].ToString() + ", Ward : " + dtPropertyDetail.Rows[0]["ward_no"].ToString() + ", Property No. " + dtPropertyDetail.Rows[0]["PID"].ToString() + ", Revenue Circle:" + dtPropertyDetail.Rows[0]["rev_circle"].ToString() + ", New Holding: " + dtPropertyDetail.Rows[0]["new_holding_no"].ToString() + ", Old Holding:" + dtPropertyDetail.Rows[0]["old_holding_no"].ToString();
                param.Add(new SqlParameter("@naration", naration));
                param.Add(new SqlParameter("@owner", dtPropertyDetail.Rows[0]["owner_name"].ToString()));
                param.Add(new SqlParameter("@gardiuan_name", dtPropertyDetail.Rows[0]["guardian_name"].ToString()));
                param.Add(new SqlParameter("@mobile_no", dtPropertyDetail.Rows[0]["mobile_no"].ToString()));
                param.Add(new SqlParameter("@address", dtPropertyDetail.Rows[0]["address"].ToString()));
                param.Add(new SqlParameter("@payment_mode", mid));
                param.Add(new SqlParameter("@amount", ActualPaymentAmount));
                param.Add(new SqlParameter("@reference_no", TransID));
                param.Add(new SqlParameter("@reference_date", TransDate));
                param.Add(new SqlParameter("@arrear_payble_amount", arrearPayableAmount));
                param.Add(new SqlParameter("@current_payble_amount", currentPayableAmount));
                param.Add(new SqlParameter("@arrear_received_amount", arrearPayableAmount));
                param.Add(new SqlParameter("@current_received_amount", currentPayableAmount));
                param.Add(new SqlParameter("@arrear_panelty_payble_amount", arrearPenalty));
                param.Add(new SqlParameter("@current_panelty_payble_amount", currentPenalty));
                param.Add(new SqlParameter("@arrear_panelty_received_amount", arrearPenalty));
                param.Add(new SqlParameter("@current_panelty_received_amount", currentPenalty));
                param.Add(new SqlParameter("@arrear_total_payble_amount", arrearTotalPayable));
                param.Add(new SqlParameter("@current_total_payble_amount", currentTotalPayable));
                param.Add(new SqlParameter("@arrear_total_received_amount", arrearTotalPayable));
                param.Add(new SqlParameter("@current_total_received_amount", currentTotalPayable));
                param.Add(new SqlParameter("@water_tax", WaterTaxCharge));
                param.Add(new SqlParameter("@total_payble_amount", totalpayableAmount));
                param.Add(new SqlParameter("@rebate_amount", RebateAmount));
                param.Add(new SqlParameter("@actual_payble_amount", ActualPaymentAmount));
                param.Add(new SqlParameter("@total_received_amount", ActualPaymentAmount));
                param.Add(new SqlParameter("@rain_water_harvt_amount", rainWaterHarvest));
                param.Add(new SqlParameter("@advance_adjusted", ArrearAdvance));
                param.Add(new SqlParameter("@arrear_adjusted", ArrearDue));
                PMC.Common.AmountInWords aw = new PMC.Common.AmountInWords();
                param.Add(new SqlParameter("@amount_in_words", aw.ConvertToWords(ActualPaymentAmount.ToString())));
                param.Add(new SqlParameter("@sas_no", SAS));


                q = "INSERT INTO [dbo].[tbl_payment_receipt]([transaction_id],[property_id],[pid],[receipt_fy],[receipt_no],[receipt_date],[related_to],[cfc_reference] ";
                q += ",[counter_reference],[received_from],[subject],[naration],[owner],[gardiuan_name],[mobile_no],[address],[payment_mode],[amount],[reference_no]";
                q += ",[reference_date],[arrear_payble_amount],[current_payble_amount],[arrear_received_amount],[current_received_amount],[arrear_panelty_payble_amount]";
                q += ",[current_panelty_payble_amount],[arrear_panelty_received_amount],[current_panelty_received_amount],[arrear_total_payble_amount]";
                q += ",[current_total_payble_amount],[arrear_total_received_amount],[current_total_received_amount],[water_tax],[total_payble_amount],[rebate_amount]";
                q += ",[actual_payble_amount],[total_received_amount],[rain_water_harvt_amount],[total_dues],[advance_amount],[advance_adjusted],[arrear_adjusted]";
                q += ",[amount_in_words],[status],[sas_no])";
                q += " Values(@transaction_id,@property_id,@pid,@receipt_fy,@receipt_no,@receipt_date,'Tax and Revenue',994781,967123,@received_from,'Property Tax Self-Assessment And Payment',@naration";
                q += ",@owner,@gardiuan_name,@mobile_no,@address,@payment_mode,@amount,@reference_no,@reference_date,@arrear_payble_amount,@current_payble_amount";
                q += ",@arrear_received_amount,@current_received_amount,@arrear_panelty_payble_amount,@current_panelty_payble_amount,@arrear_panelty_received_amount";
                q += ",@current_panelty_received_amount,@arrear_total_payble_amount,@current_total_payble_amount,@arrear_total_received_amount,@current_total_received_amount";
                q += ",@water_tax,@total_payble_amount,@rebate_amount,@actual_payble_amount,@total_received_amount,@rain_water_harvt_amount,0,0,@advance_adjusted,@arrear_adjusted,@amount_in_words,1,@sas_no);";
                q += " update tbl_transaction_detail set receipt_no=@receipt_no where id=@transaction_id";

                dac.update(q, param);

                //data from Waste master
                DataTable dtConsumerMaster = new DataTable();
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@pid", dtPropertyDetail.Rows[0]["PID"].ToString()));
                q = "select wcd.consumer_mstr_id,wcd.consumer_no,wcd.consumer_name,wcd.gradian_name,wcd.mobile_no,wcm.address from tbl_waste_consumer_master as wcm inner join tbl_waste_consumer_details as wcd on wcd.consumer_mstr_id=wcm.id where wcm.holding_no=@pid and wcm.status=1";
                dtConsumerMaster = dac.GetDataTable(q, param);
                string consumerList = "";
                string[] at;
                if (dtConsumerMaster.Rows.Count > 0)
                {
                    for (int i = 0; i < dtConsumerMaster.Rows.Count; i++)
                    {
                        at = dtConsumerMaster.Rows[i]["consumer_no"].ToString().Split('/');
                        if (at.Length == 4)
                        {
                            consumerList = at[3];
                        }
                    }

                    //select waste demand detail
                    DataTable dtWasteDemandDetail = new DataTable();
                    q = "";
                    param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@consumer_detail_id", consumerList));
                    q = "select * from tbl_waste_demand_details where consumer_detail_id=@consumer_detail_id and payment_status=0 and [status]=1 order by demand_from";
                    dtWasteDemandDetail = dac.GetDataTable(q, param);
                    decimal amountpaid = 0;
                    for (int i = 0; i < dtWasteDemandDetail.Rows.Count; i++)
                    {
                        amountpaid += Convert.ToDecimal(dtWasteDemandDetail.Rows[i]["balance_amount"].ToString());
                    }
                    amountpaid = Math.Round(amountpaid, 0, MidpointRounding.AwayFromZero);



                    //insert into tbl_waste_transaction_detail
                    q = "";
                    param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@consumer_detail_id", consumerList));
                    param.Add(new SqlParameter("@ward_id", wardid));
                    param.Add(new SqlParameter("@receipt_no", Receipt_No));
                    param.Add(new SqlParameter("@payment_date", Convert.ToDateTime(dtExistTransactionCheck.Rows[0]["payment_date"].ToString()).ToString("dd-MMM-yyyy")));
                    param.Add(new SqlParameter("@pmode", mid));
                    param.Add(new SqlParameter("@ref_no", TransID));
                    param.Add(new SqlParameter("@ref_date", TransDate));
                    param.Add(new SqlParameter("@amount", amountpaid));
                    param.Add(new SqlParameter("@entry_date", Convert.ToDateTime(dtExistTransactionCheck.Rows[0]["entry_date"].ToString()).ToString("dd-MMM-yyyy")));
                    param.Add(new SqlParameter("@receive_datetime", Convert.ToDateTime(dtExistTransactionCheck.Rows[0]["entry_date"].ToString()).ToString("dd-MMM-yyyy")));
                    q = "INSERT INTO [dbo].[tbl_waste_transaction_detail]([consumer_detail_id],[ward_id],[ulb_id],[receipt_no],[payment_date],[payment_mode]";
                    q += ",[ref_no],[ref_date],[amount],[remarks],[status],[entry_date],[ip_address],[user_id],[receiver_ip],[receiver_id],[receive_datetime]";
                    q += ",[receive_status])";
                    q += " Values(@consumer_detail_id,@ward_id,1,@receipt_no,@payment_date,@pmode,@ref_no,@ref_date,@amount,'Online Payment',1,@entry_date,convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),2,convert(varchar(100),CONNECTIONPROPERTY('local_net_address')),2,@receive_datetime,1); select @@IDENTITY";
                    long WasteTransactionID = Convert.ToInt64(dac.Scalar(q, param));

                    //insert into tbl_waste_collection_detail
                    string wasteFY = "";
                    for (int i = 0; i < dtWasteDemandDetail.Rows.Count; i++)
                    {
                        q = "";
                        param = new List<SqlParameter>();
                        param.Add(new SqlParameter("@transaction_id", WasteTransactionID));
                        param.Add(new SqlParameter("@consumer_dtl_id", consumerList));
                        param.Add(new SqlParameter("@demand_id", dtWasteDemandDetail.Rows[i]["id"].ToString()));
                        param.Add(new SqlParameter("@net_amount", dtWasteDemandDetail.Rows[i]["balance_amount"].ToString()));
                        param.Add(new SqlParameter("@paid_amount", dtWasteDemandDetail.Rows[i]["balance_amount"].ToString()));
                        param.Add(new SqlParameter("@ward_id", wardid));

                        if (Convert.ToDateTime(dtWasteDemandDetail.Rows[i]["demand_from"].ToString()).Month < 4)
                        {
                            wasteFY = ((Convert.ToDateTime(dtWasteDemandDetail.Rows[i]["demand_from"].ToString()).Year - 1) + "-" + Convert.ToDateTime(dtWasteDemandDetail.Rows[i]["demand_from"].ToString()).Year);
                        }
                        else
                        {
                            wasteFY = (Convert.ToDateTime(dtWasteDemandDetail.Rows[i]["demand_from"].ToString()).Year + "-" + (Convert.ToDateTime(dtWasteDemandDetail.Rows[i]["demand_from"].ToString()).Year + 1));
                        }
                        param.Add(new SqlParameter("@fin_year", wasteFY));
                        q = "INSERT INTO [dbo].[tbl_waste_collection_details]([transaction_id],[consumer_dtl_id],[demand_id],[net_amount],[status]";
                        q += ",[paid_amount],[ulb_id],[ward_id],[fin_year])";
                        q += " Values(@transaction_id,@consumer_dtl_id,@demand_id,@net_amount,1,@paid_amount,1,@ward_id,@fin_year); update tbl_waste_demand_details ";
                        q += "set payment_status=1 where id=@demand_id";
                        dac.update(q, param);
                    }

                    //insert into tbl_waste_receipt
                    if (dtWasteDemandDetail.Rows.Count > 0)
                    {
                        q = "";
                        param = new List<SqlParameter>();
                        param.Add(new SqlParameter("@transaction_id", WasteTransactionID));
                        param.Add(new SqlParameter("@consumer_detail_id", consumerList));
                        param.Add(new SqlParameter("@ref_no", TransID));
                        param.Add(new SqlParameter("@ref_date", TransDate));

                        param.Add(new SqlParameter("@receipt_no", Receipt_No));
                        param.Add(new SqlParameter("@payment_mode", mid));
                        param.Add(new SqlParameter("@receipt_date", Convert.ToDateTime(dtExistTransactionCheck.Rows[0]["payment_date"].ToString()).ToString("dd-MMM-yyyy")));
                        param.Add(new SqlParameter("@fin_year", Convert.ToDateTime(dtWasteDemandDetail.Rows[0]["demand_from"].ToString()).ToString("dd-MM-yyyy") + " To " + Convert.ToDateTime(dtWasteDemandDetail.Rows[dtWasteDemandDetail.Rows.Count - 1]["demand_upto"].ToString()).ToString("dd-MM-yyyy")));
                        param.Add(new SqlParameter("@owner", dtConsumerMaster.Rows[0]["consumer_name"].ToString()));
                        param.Add(new SqlParameter("@gaurdian_name", dtConsumerMaster.Rows[0]["gradian_name"].ToString()));
                        param.Add(new SqlParameter("@mobile_no", dtConsumerMaster.Rows[0]["mobile_no"].ToString()));
                        param.Add(new SqlParameter("@address", dtConsumerMaster.Rows[0]["address"].ToString()));
                        param.Add(new SqlParameter("@current_amount", amountpaid));
                        param.Add(new SqlParameter("@total_recieved_amount", amountpaid));
                        param.Add(new SqlParameter("@total_payable_amount", amountpaid));

                        param.Add(new SqlParameter("@amount_in_words", aw.ConvertToWords(amountpaid.ToString())));
                        param.Add(new SqlParameter("@actual_payble_amount", amountpaid));
                        param.Add(new SqlParameter("@current_received_amount", amountpaid));
                        q = "INSERT INTO [dbo].[tbl_waste_reciept]([transaction_id],[consumer_detail_id],[ref_no],[ref_date],[receipt_no],[payment_mode],[receipt_date]";
                        q += ",[fin_year],[owner],[gaurdian_name],[mobile_no],[address],[current_amount],[total_recieved_amount],[total_payable_amount],[total_dues]";
                        q += " ,[status] ,[amount_in_words],[arrear_adjusted],[advance_adjusted],[actual_payble_amount],[current_received_amount],[arrear_received_amount])";
                        q += " Values(@transaction_id,@consumer_detail_id,@ref_no,@ref_date,@receipt_no,@payment_mode,@receipt_date,@fin_year,@owner,@gaurdian_name,@mobile_no,@address";
                        q += ",@current_amount,@total_recieved_amount,@total_payable_amount,0,1,@amount_in_words,0,0,@actual_payble_amount,@current_received_amount,0)";
                        dac.update(q, param);
                    }

                }
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@application_no", SAS));
                param.Add(new SqlParameter("@last_payment_receipt_no", Receipt_No));
                param.Add(new SqlParameter("@last_payment_amount", ActualPaymentAmount));
                param.Add(new SqlParameter("@assessment_year", maxFY));
                param.Add(new SqlParameter("@pid", property_id));

                q = "update tbl_property_detail set application_no=@application_no,last_payment_receipt_no=@last_payment_receipt_no,last_payment_date=GETDATE(),last_payment_amount=@last_payment_amount,last_payment_quater=4,last_payment_year=@assessment_year,assessment_year=@assessment_year,entry_date=GETDATE() where id=@pid;";
                //q += " update tbl_property_detail_tmp set last_payment_receipt_no=@last_payment_receipt_no,last_payment_date=GETDATE(),last_payment_amount=@last_payment_amount,last_payment_quater=4,last_payment_year=@assessment_year,assessment_year=@assessment_year,entry_date=GETDATE(),[status]=0 where pid=@pid";

                dac.update(q, param);

                q = "";
                DataTable dtOccupancyID = new DataTable();
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@property_id", property_id));

                q = "select id from tbl_occupancy_detail where property_id=@property_id and [status]=1";
                dtOccupancyID = dac.GetDataTable(q, param);
                if (dtOccupancyID.Rows.Count > 0)
                {
                    q = "";
                    param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@property_id", property_id));
                    param.Add(new SqlParameter("@fromYear", minFY));
                    param.Add(new SqlParameter("@uptoYear", maxFY));

                    //string idList = "";
                    //for (int i = 0; i < dtOccupancyID.Rows.Count; i++)
                    //{
                    //    idList += dtOccupancyID.Rows[i]["id"].ToString()+",";
                    //}
                    //idList = idList.Remove(idList.Length - 1, 1);
                    //param.Add(new SqlParameter("@id", idList));
                    q = "insert into tbl_occupancy_detail([property_id],[floor_id],[use_type_id],[usage_type_id],[occupancy_type_id],[construction_type_id]";
                    q += ",[builtup_area],[from_year],[upto_year],[status],[user_id],[entry_date],[effect_from]) ";
                    q += "select property_id,floor_id,use_type_id,usage_type_id,occupancy_type_id,construction_type_id,builtup_area,@fromYear,@uptoYear,";
                    q += "1,2,GETDATE(),@uptoYear from tbl_occupancy_detail where property_id=@property_id and [status]=1; update tbl_occupancy_detail set [status]=0 where property_id=@property_id and entry_date<='" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "'";
                    dac.update(q, param);
                }
                //string message = "Thanks for choosing Property Tax Online Payment Service. Successfully paid Rs. "+ String.Format("{0:0.00}", Pamount) + " on " + TransDate + " for PID No. " + dtPropertyDetail.Rows[0]["PID"].ToString() + ". Receipt download link: https://pmc.bihar.gov.in/payment_receipt.aspx?uid=" + ApplicationTransID;
                //string mobile_no = dtPropertyDetail.Rows[0]["mobile_no"].ToString();
                //smsservice.pmcsendsms ss = new smsservice.pmcsendsms();
                //ss.SendSms(mobile_no, message, "7110EDA4D09E062AA5E4A390B0A572AC0D2C0220");//dtPropertyDetail.Rows[0]["mobile_no"].ToString()
                //send_sms sms = new send_sms();
                //number = sms.sendSingleSMS(mobileNo, message).ToString();
                //return ApplicationTransID;
            }
        }
    }
}