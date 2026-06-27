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

/// <summary>
/// Summary description for ExistingPropertyCalculation
/// </summary>
namespace PMC
{
    public class ExistingPropertyCalculation
    {
        TaxCalculationController tcc;
    SearchDataController sdc;
    WasteRateChartController wrcc;
    DataTable dt;
    DataTable Owner;
    DataTable dtTaxCalculate;
    DataTable dtArrearData;
    DataTable dtAdvance;
    decimal TTax;
    string AY;
    decimal ArvTotal = 0;
    decimal CAnnualTax = 0;
    decimal CurVacTax = 0;
    int chkCommercial = 0;
    decimal TotalAnnualTax = 0;
    decimal TotalVacantTax = 0;
    decimal TotalInterest = 0;
    decimal TotalArrearsInit = 0;
    decimal TotalArrears = 0;
    decimal Waste_Charge = 0;
    int fromAY=0;
    int toAY=0;
    string CurrentAY;
    DataRow mrow;
	    public ExistingPropertyCalculation()
	    {
		    //
		    // TODO: Add constructor logic here
		    //
            dtArrearData = new DataTable();
            dtArrearData.Columns.Add("Years", typeof(string));
            dtArrearData.Columns.Add("AnnualTax", typeof(string));
            dtArrearData.Columns.Add("VacantLandTax", typeof(string));
            dtArrearData.Columns.Add("Interest_Penalty", typeof(string));
	    }
        public string[] FromAndToYear(string LAY)
        {
            string[] FTYear;
            fromAY = Convert.ToInt32(LAY.Split('-')[0]);

            int monFin = System.DateTime.Now.Month;
            int yrFin = System.DateTime.Now.Year;

            string AssessYear;
            if (monFin < 4)
            {
                toAY = yrFin - 1;
                AssessYear = (yrFin - 1) + "-" + yrFin;
            }
            else
            {
                toAY = yrFin;
                AssessYear = yrFin + "-" + (yrFin + 1);
            }
            FTYear = new string[] { fromAY.ToString(), toAY.ToString(), AssessYear };

            return FTYear;
        }
        public decimal TaxCalculationForLand(decimal TPArea, decimal BuiltArea, int STypeId, int OType, int PTYpe, string Last_AY)
        {
            string[] CalYear = FromAndToYear(Last_AY);
            CAnnualTax = 0;

            for (int i = Convert.ToInt32(CalYear[1]); i >= (Convert.ToInt32(CalYear[0]) < 2013 ? 2013 : Convert.ToInt32(CalYear[0])); i--)
            {
                AY = (i + "-" + (i + 1));
                tcc = new TaxCalculationController();
                dtTaxCalculate = new DataTable();
                if (PTYpe == 1)
                {
                    dtTaxCalculate = tcc.VaccantAreaTax(TPArea, STypeId, AY, OType);
                }
                else
                {
                    dtTaxCalculate = tcc.VaccantAreaTaxForBuilding(TPArea, BuiltArea, STypeId, AY, OType);
                }
                TTax = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["TaxValue"].ToString()), 0);
                if (i == toAY)
                {
                    CurVacTax = TTax;
                }
                /*Check FY and add with same FY*/
                if (AY != CalYear[2])
                {
                    mrow = dtArrearData.NewRow();
                    mrow[0] = AY;
                    mrow[1] = 0;
                    mrow[2] = TTax;
                    //Calculate Penaly/Interest
                    mrow[3] = tcc.InterestOrPanelOrRebate(Convert.ToDateTime("01-Apr-" + i), System.DateTime.Now, TTax);
                    dtArrearData.Rows.Add(mrow);


                }
            }

            decimal txLand = 0;

            txLand = CAnnualTax + CurVacTax;
            return Math.Round(txLand, 0);
        }
        public decimal ArrearCalculation(DataTable dtArr, int yr)
        {
            decimal ArrValue = 0;
            if (dtArr.Rows.Count > 0)
            {
                DataTable dtFinalArrear = dtArr.AsEnumerable()
                .GroupBy(r => r["Years"])
                .Select(x =>
                {
                    var row = dtArr.NewRow();
                    row["Years"] = x.Key;
                    row["AnnualTax"] = x.Sum(r => Convert.ToDecimal(r["AnnualTax"]));
                    row["VacantLandTax"] = x.Sum(r => Convert.ToDecimal(r["VacantLandTax"]));
                    row["Interest_Penalty"] = x.Sum(r => Convert.ToDecimal(r["Interest_Penalty"]));

                    return row;
                }).CopyToDataTable();


                for (int at = 0; at < dtFinalArrear.Rows.Count; at++)
                {
                    TotalArrearsInit = Convert.ToDecimal(dtFinalArrear.Rows[at]["AnnualTax"].ToString()) + Convert.ToDecimal(dtFinalArrear.Rows[at]["VacantLandTax"].ToString()) + Convert.ToDecimal(dtFinalArrear.Rows[at]["Interest_Penalty"].ToString());

                    TotalAnnualTax += Convert.ToDecimal(dtFinalArrear.Rows[at]["AnnualTax"].ToString());
                    TotalVacantTax += Convert.ToDecimal(dtFinalArrear.Rows[at]["VacantLandTax"].ToString());
                    TotalInterest += Convert.ToDecimal(dtFinalArrear.Rows[at]["Interest_Penalty"].ToString());
                    TotalArrears += TotalArrearsInit;
                }

                ArrValue = TotalArrears;
            }
            return ArrValue;

        }
        public decimal TaxCalculationForStructure(string Last_AY, DataTable dtFloor, int RTID, int OType)
        {
            decimal BuiltUpArea = 0;
            int OcuupID;
            int useTID;
            int ConstTID;
            string ARVFactor;
            string usesTypeforCal;
            string RatableArea;

            string OccupFactor;
            string PropTaxRate;
            CurVacTax = 0;
            string[] CalYear = FromAndToYear(Last_AY);
            for (int j = 0; j < dtFloor.Rows.Count; j++)
            {
                for (int i = Convert.ToInt32(CalYear[1]); i >= Convert.ToInt32(CalYear[0]); i--)
                {
                    AY = (i + "-" + (i + 1));
                    BuiltUpArea = Convert.ToDecimal(dtFloor.Rows[j]["Built_Up_Area"].ToString());
                    OcuupID = Convert.ToInt32(dtFloor.Rows[j]["Occupancy_Type_ID"].ToString());

                    useTID = Convert.ToInt32(dtFloor.Rows[j]["Type_Of_Use_ID"].ToString());
                    if (useTID != 1)
                    {
                        chkCommercial = 1;
                    }
                    ConstTID = Convert.ToInt32(dtFloor.Rows[j]["Construction_Type_ID"].ToString());
                    usesTypeforCal = dtFloor.Rows[j]["Usage_Factor"].ToString();
                    dtTaxCalculate = new DataTable();
                    tcc = new TaxCalculationController();
                    dtTaxCalculate = tcc.BuildupAreaTax(BuiltUpArea, OcuupID, RTID, useTID, ConstTID, usesTypeforCal, AY, OType);
                    RatableArea = dtTaxCalculate.Rows[0]["TaxableArea"].ToString();
                    ARVFactor = dtTaxCalculate.Rows[0]["ARVFactor"].ToString();
                    OccupFactor = dtTaxCalculate.Rows[0]["OccupancyFactor"].ToString();
                    PropTaxRate = dtTaxCalculate.Rows[0]["PropertyTaxRate"].ToString();
                    TTax = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["AnnualTax"].ToString()), 0);

                    //ArrTax = ArrTax + Convert.ToDecimal(TTax);

                    decimal ARV = Convert.ToDecimal(RatableArea) * Convert.ToDecimal(ARVFactor) * Convert.ToDecimal(OccupFactor);

                    if (i == toAY)
                    {
                        ArvTotal = ArvTotal + ARV;
                        CAnnualTax = CAnnualTax + Convert.ToDecimal(TTax);
                    }
                    if (AY != CalYear[2])
                    {
                        mrow = dtArrearData.NewRow();
                        mrow[0] = AY;
                        mrow[1] = TTax;
                        mrow[2] = 0;
                        //Calculate Penaly/Interest
                        mrow[3] = tcc.InterestOrPanelOrRebate(Convert.ToDateTime("01-Apr-" + i), System.DateTime.Now, TTax);
                        dtArrearData.Rows.Add(mrow);


                    }
                }
            }
            decimal txConstruction = 0;

            txConstruction = CAnnualTax + CurVacTax;
            return Math.Round(txConstruction, 2);

        }
        public decimal FinalCalculation(decimal Tax, decimal CRebate, decimal CPenal, decimal Arr, decimal LateSub, decimal due, decimal adv)
        {
            decimal FTax = 0;
            FTax = Tax - CRebate + CPenal + Arr + LateSub + due - adv;
            return FTax;
        }
        public decimal[] CurrentRebatePenalty(DateTime frmDate, decimal amnt)
        {
            decimal[] RP;
            tcc = new TaxCalculationController();
            decimal CTRebateOrPenalty = Convert.ToDecimal(tcc.InterestOrPanelOrRebate(frmDate, System.DateTime.Now, amnt));
            decimal CTRebate;
            decimal CTPenalty;
            if (CTRebateOrPenalty == 0)
            {
                CTRebate = 0;
                CTPenalty = 0;
            }
            else if ((CTRebateOrPenalty > 0))
            {
                CTRebate = 0;
                CTPenalty = CTRebateOrPenalty;
            }
            else
            {
                CTRebate = CTRebateOrPenalty;
                CTPenalty = 0;
            }
            RP = new decimal[] { CTRebate, CTPenalty };
            return RP;
        }
        public decimal LateSubmissionCal(DateTime frmDate, int PType, int status, int CommStatus)
        {
            decimal penaltyLateSubm = 0;//Condition
            if (frmDate.AddDays(90) < System.DateTime.Now & status == 0)
            {
                if (PType == 1)
                {
                    penaltyLateSubm = 5000;
                }
                else
                {
                    if (CommStatus == 0)
                    {
                        penaltyLateSubm = 2000;
                    }
                    else
                    {
                        penaltyLateSubm = 5000;
                    }
                }

            }
            return penaltyLateSubm;
        }
        public void fincalculation(long pid)
        {
            //List<final> liFinal = new List<final>();
            long PropertyID = pid;//2000011;

            decimal Tax_Holding = 0;
            decimal Tot_Pay = 0;
            string CurrentAY;
            if (System.DateTime.Now.Month < 4)
            {
                CurrentAY = (System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year;
            }
            else
            {
                CurrentAY = System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1);
            }
            string last_FY;
            decimal PdueAmount = 0;
            decimal pAdvance = 0;
            decimal WaterTaxCharge = 0;
            int LStatus = 0;
            decimal LateSubmission = 0;
            decimal Arrear = 0;
            decimal[] CRP;
            string dispAY;
            Tax_Holding = 0;
            DataTable dtAdvance;
            decimal waste_charge = 0;
            dt = new DataTable();
            sdc = new SearchDataController();
            dt = sdc.SearchProperty(pid);
            dtAdvance = sdc.SearchAdvancePayment(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
            if (dtAdvance.Rows.Count <= 0)
            {
                last_FY = dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + 1);
                PdueAmount = 0;
                pAdvance = 0;

                if (dt.Rows[0]["last_assessment_year"].ToString() != "")
                {
                    LStatus = 1;
                }
                else
                {
                    LStatus = 0;
                }

                dispAY = dt.Rows[0]["last_assessment_year"].ToString();
            }
            else
            {
                LStatus = 1;
                dispAY = dtAdvance.Rows[0]["fin_year"].ToString();
                last_FY = dtAdvance.Rows[0]["fin_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dtAdvance.Rows[0]["fin_year"].ToString().Split('-')[1]) + 1);
                if (dtAdvance.Rows[0]["total_dues"].ToString() == "")
                {
                    PdueAmount = 0;
                }
                else
                {
                    PdueAmount = Convert.ToDecimal(dtAdvance.Rows[0]["total_dues"].ToString());
                }
                if (dtAdvance.Rows[0]["advance_amount"].ToString() == "")
                {
                    pAdvance = 0;
                }
                else
                {
                    pAdvance = Convert.ToDecimal(dtAdvance.Rows[0]["advance_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["advance_adjusted"].ToString());
                }
            }
            
            if (Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()) == 1)
            {
                //TaxCalculationForLand(decimal TPArea,decimal BuiltArea,int STypeId,int OType,int PTYpe,string Last_AY)
                /*Current Tax*/
                Tax_Holding = TaxCalculationForLand(Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString()), Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString()), Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), last_FY);
                /*Current Rebate or Penalty*/
                CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                Arrear = ArrearCalculation(dtArrearData, toAY);
                waste_charge = 0;
            }
            else if (Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()) == 2)
            {
                decimal TPlotArea = Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString());
                decimal BuiltArea = Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString());
                DataTable dtFloorForBuilding = new DataTable();
                dtFloorForBuilding = sdc.SearchOccupancyDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                if (BuiltArea >= (TPlotArea * 70 / 100))
                {
                    Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForBuilding, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()));
                    CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                    Arrear = ArrearCalculation(dtArrearData, toAY);

                }
                else
                {
                    Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForBuilding, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()));
                    Arrear = ArrearCalculation(dtArrearData, toAY);
                    Tax_Holding += TaxCalculationForLand(Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString()), Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString()), Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), last_FY);
                    Arrear += ArrearCalculation(dtArrearData, toAY);
                    CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);

                }
                WasteRateChartController wrcc = new WasteRateChartController();
                waste_charge = wrcc.WasteCharge(PropertyID.ToString());

            }
            else
            {
                DataTable dtFloorForFlat = new DataTable();
                dtFloorForFlat = sdc.SearchOccupancyDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForFlat, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()));
                CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                Arrear = ArrearCalculation(dtArrearData, toAY);
                WasteRateChartController wrcc = new WasteRateChartController();
                waste_charge = wrcc.WasteCharge(PropertyID.ToString());
            }
            LateSubmission = LateSubmissionCal(Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year < 2013 ? Convert.ToDateTime("01-Apr-2013") : Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), LStatus, chkCommercial);
            Tax_Holding = FinalCalculation(Tax_Holding, CRP[0], CRP[1], Arrear, LateSubmission, PdueAmount, pAdvance);
            if (Tax_Holding < 0)
            {
                Tax_Holding = 0;
            }
            Tot_Pay = Tax_Holding + waste_charge;
        }
    }
}