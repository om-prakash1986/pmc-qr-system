using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using PMC;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using PMC.DAL;

/// <summary>
/// Summary description for demanbysms
/// </summary>
namespace PMC
{
    public class demandbysms
    {
        TaxCalculationController tcc;
        SearchDataController sdc;
        WasteRateChartController wrcc;
        OccupancyTypeDetailController otdc;
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
        decimal Water_Harvest = 0;
        int fromAY = 0;
        int toAY = 0;
        string CurrentAY;
        DataRow mrow;
        DataTable dtYearlyAssessment;
        DataRow yrow;
        DataTable dtYearlyVacantAssessment;
        DataTable dtYearlyFloorAssessment;
        DataRow vrow;
        DataRow frow;
        decimal waterharvest = 0;
        int clearStatus = 1;

        List<SqlParameter> param;
        DataAccessLayer dac;
        public demandbysms()
        {
            //
            // TODO: Add constructor logic here
            //
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
            dtArrearData = new DataTable();
            dtArrearData.Columns.Add("Years", typeof(string));
            dtArrearData.Columns.Add("AnnualTax", typeof(string));
            dtArrearData.Columns.Add("VacantLandTax", typeof(string));
            dtArrearData.Columns.Add("Interest_Penalty", typeof(string));


            dtYearlyAssessment = new DataTable();

            dtYearlyAssessment.Columns.Add("fin_year", typeof(string));
            dtYearlyAssessment.Columns.Add("total_fy_tax", typeof(string));
            dtYearlyAssessment.Columns.Add("vacant_tax", typeof(string));
            dtYearlyAssessment.Columns.Add("vacant_rate", typeof(string));
            dtYearlyAssessment.Columns.Add("vacant_rate_id", typeof(string));
            dtYearlyAssessment.Columns.Add("penalty", typeof(string));
            dtYearlyAssessment.Columns.Add("tax_with_penalty", typeof(string));
            dtYearlyAssessment.Columns.Add("current_tax_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("water_harvest_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("total_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("final_tax", typeof(string));

            dtYearlyVacantAssessment = new DataTable();
            dtYearlyVacantAssessment.Columns.Add("area_decimal", typeof(string));
            dtYearlyVacantAssessment.Columns.Add("area_sqft", typeof(string));
            dtYearlyVacantAssessment.Columns.Add("area_sqmtr", typeof(string));
            dtYearlyVacantAssessment.Columns.Add("vacant_rate_id", typeof(string));

            dtYearlyVacantAssessment.Columns.Add("tax", typeof(string));
            dtYearlyVacantAssessment.Columns.Add("effect_from", typeof(string));


            dtYearlyFloorAssessment = new DataTable();
            dtYearlyFloorAssessment.Columns.Add("fin_year", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("floor_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("usage_type_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("occupancy_type_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("construct_type_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("built_area", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("carpet_area", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("use_type", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("occupancy_factor", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("arv", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("total_tax_factor", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("total_tax", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("arv_rate", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("occupancy_type_detail_id", typeof(string));
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
        public decimal TaxCalculationForLand(decimal TPArea, decimal BuiltArea, int STypeId, int OType, int PTYpe, string Last_AY, int WH)
        {
            string[] CalYear = FromAndToYear(Last_AY);
            CAnnualTax = 0;
            decimal intPenal = 0;
            decimal intRebate = 0;
            decimal[] intRebatePenal;

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
                    intRebatePenal = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + AY), TTax);
                    intPenal = intRebatePenal[1];
                    intRebate = intRebatePenal[0];
                    if (WH == 0)
                    {
                        waterharvest = 0;
                    }
                    else
                    {
                        waterharvest = Math.Round(TTax * 5 / 100, 2, MidpointRounding.AwayFromZero);
                    }
                }
                /*Check FY and add with same FY*/
                if (AY != CalYear[2])
                {
                    intRebate = 0;
                    mrow = dtArrearData.NewRow();
                    mrow[0] = AY;
                    mrow[1] = 0;
                    mrow[2] = Math.Round(TTax, 2, MidpointRounding.AwayFromZero);
                    //Calculate Penaly/Interest
                    intPenal = tcc.InterestOrPanelOrRebate(Convert.ToDateTime("01-Apr-" + i), System.DateTime.Now, TTax);
                    mrow[3] = Math.Round(intPenal, 2, MidpointRounding.AwayFromZero);
                    dtArrearData.Rows.Add(mrow);
                    waterharvest = 0;

                }

                yrow = dtYearlyAssessment.NewRow();
                yrow[0] = AY;
                yrow[1] = TTax;
                yrow[2] = TTax;
                yrow[3] = dtTaxCalculate.Rows[0]["TaxRate"];
                yrow[4] = dtTaxCalculate.Rows[0]["TaxRateID"];
                /*dtYearlyAssessment.Columns.Add("penalty", typeof(string));
            dtYearlyAssessment.Columns.Add("tax_with_penalty", typeof(string));
            dtYearlyAssessment.Columns.Add("current_tax_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("water_harvest_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("total_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("final_tax", typeof(string));*/
                yrow[5] = intPenal;
                yrow[6] = intPenal + TTax;
                yrow[7] = intRebate;
                yrow[8] = waterharvest;
                yrow[9] = intRebate + waterharvest;
                yrow[10] = intPenal + TTax - intRebate - waterharvest;

                dtYearlyAssessment.Rows.Add(yrow);

                /*
                 dtYearlyVacantAssessment.Columns.Add("area_decimal", typeof(string));
                dtYearlyVacantAssessment.Columns.Add("area_sqft",typeof(string));
                dtYearlyVacantAssessment.Columns.Add("area_sqmtr", typeof(string));
                dtYearlyVacantAssessment.Columns.Add("vacant_rate_id", typeof(string));
            
                dtYearlyVacantAssessment.Columns.Add("tax", typeof(string));
                dtYearlyVacantAssessment.Columns.Add("effect_from", typeof(string));
                 */

                vrow = dtYearlyVacantAssessment.NewRow();
                vrow[0] = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["TaxableArea"].ToString()) * Convert.ToDecimal(0.002295894), 2, MidpointRounding.AwayFromZero).ToString();
                vrow[1] = dtTaxCalculate.Rows[0]["TaxableArea"].ToString();
                vrow[2] = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["TaxableArea"].ToString()) * Convert.ToDecimal(10.764), 2, MidpointRounding.AwayFromZero).ToString();
                vrow[3] = dtTaxCalculate.Rows[0]["TaxRateID"];

                vrow[4] = TTax;
                vrow[5] = AY;

                dtYearlyVacantAssessment.Rows.Add(vrow);
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
        public decimal TaxCalculationForStructure(string Last_AY, DataTable dtFloor, int RTID, int OType, int WH)
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
            int Occup_Type_Detail_ID = 0;
            decimal intPenal = 0;
            decimal intRebate = 0;
            decimal[] intRebatePenal;
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
                        intRebatePenal = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + AY), TTax);
                        intPenal = intRebatePenal[1];
                        intRebate = intRebatePenal[0];
                        if (WH == 0)
                        {
                            waterharvest = 0;
                        }
                        else
                        {
                            waterharvest = Math.Round(TTax * 5 / 100, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    if (AY != CalYear[2])
                    {
                        intRebate = 0;
                        mrow = dtArrearData.NewRow();
                        mrow[0] = AY;
                        mrow[1] = Math.Round(TTax, 2, MidpointRounding.AwayFromZero);
                        mrow[2] = 0;
                        //Calculate Penaly/Interest
                        intPenal = tcc.InterestOrPanelOrRebate(Convert.ToDateTime("01-Apr-" + i), System.DateTime.Now, TTax);
                        mrow[3] = Math.Round(intPenal, 2, MidpointRounding.AwayFromZero);
                        dtArrearData.Rows.Add(mrow);
                        waterharvest = 0;

                    }

                    yrow = dtYearlyAssessment.NewRow();
                    yrow[0] = AY;
                    yrow[1] = TTax;
                    yrow[2] = 0;
                    yrow[3] = 0;
                    yrow[4] = 0;
                    yrow[5] = intPenal;
                    yrow[6] = intPenal + TTax;
                    yrow[7] = intRebate;
                    yrow[8] = waterharvest;
                    yrow[9] = intRebate + waterharvest;
                    yrow[10] = intPenal + TTax - intRebate - waterharvest;
                    dtYearlyAssessment.Rows.Add(yrow);


                    frow = dtYearlyFloorAssessment.NewRow();
                    /*
                     dtYearlyFloorAssessment.Columns.Add("floor_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("usage_type_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("occupancy_type_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("construct_type_id", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("built_area", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("carpet_area", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("use_type", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("occupancy_factor", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("arv", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("total_tax_factor", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("total_tax", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("arv_rate", typeof(string));
            dtYearlyFloorAssessment.Columns.Add("occupancy_type_detail_id", typeof(string));
                     */
                    frow[0] = AY;
                    frow[1] = dtFloor.Rows[j]["floor_id"].ToString();
                    frow[2] = useTID.ToString();
                    frow[3] = OcuupID;
                    frow[4] = ConstTID;
                    frow[5] = BuiltUpArea.ToString();
                    frow[6] = RatableArea.ToString();
                    frow[7] = dtFloor.Rows[j]["Type_Of_Use"].ToString();
                    frow[8] = OccupFactor.ToString();
                    frow[9] = ARV.ToString();
                    frow[10] = PropTaxRate.ToString();
                    frow[11] = TTax.ToString();
                    frow[12] = ARVFactor.ToString();

                    otdc = new OccupancyTypeDetailController();
                    Occup_Type_Detail_ID = otdc.GetOccupancyTypeDetailID(OcuupID, AY);

                    frow[13] = Occup_Type_Detail_ID.ToString();

                    dtYearlyFloorAssessment.Rows.Add(frow);
                }
            }
            decimal txConstruction = 0;

            txConstruction = CAnnualTax + CurVacTax;
            return Math.Round(txConstruction, 2);

        }

        public void yearlyTax(long property_id)
        {
            YearlyTaxAssessmentTmpController ytatc;
            ytatc = new YearlyTaxAssessmentTmpController();
            long YIden = 0;
            // ytatc.DeleteBeforeInsert(property_id);
            if (dtYearlyAssessment.Rows.Count > 0)
            {
                DataTable dtFinalYearlyTax = dtYearlyAssessment.AsEnumerable()
                .GroupBy(r => r["fin_year"])
                .Select(x =>
                {
                    var row = dtYearlyAssessment.NewRow();
                    row["fin_year"] = x.Key;
                    row["total_fy_tax"] = x.Sum(r => Convert.ToDecimal(r["total_fy_tax"]));
                    row["vacant_tax"] = x.Sum(r => Convert.ToDecimal(r["vacant_tax"]));
                    row["vacant_rate"] = x.Sum(r => Convert.ToDecimal(r["vacant_rate"]));
                    row["vacant_rate_id"] = x.Sum(r => Convert.ToInt32(r["vacant_rate_id"]));
                    /*dtYearlyAssessment.Columns.Add("penalty", typeof(string));
            dtYearlyAssessment.Columns.Add("tax_with_penalty", typeof(string));
            dtYearlyAssessment.Columns.Add("current_tax_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("water_harvest_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("total_rebate", typeof(string));
            dtYearlyAssessment.Columns.Add("final_tax", typeof(string));*/
                    row["penalty"] = x.Sum(r => Convert.ToDecimal(r["penalty"]));
                    row["tax_with_penalty"] = x.Sum(r => Convert.ToDecimal(r["tax_with_penalty"]));
                    row["current_tax_rebate"] = x.Sum(r => Convert.ToDecimal(r["current_tax_rebate"]));
                    row["water_harvest_rebate"] = x.Sum(r => Convert.ToDecimal(r["water_harvest_rebate"]));
                    row["total_rebate"] = x.Sum(r => Convert.ToDecimal(r["total_rebate"]));
                    row["final_tax"] = x.Sum(r => Math.Round(Convert.ToDecimal(r["final_tax"]), 2, MidpointRounding.AwayFromZero));
                    return row;
                }).CopyToDataTable();


                for (int i = 0; i < dtFinalYearlyTax.Rows.Count; i++)
                {

                    YIden = ytatc.InsertYearlyAssessment(property_id, dtFinalYearlyTax.Rows[i]["fin_year"].ToString(), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["total_fy_tax"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["vacant_tax"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["vacant_rate"].ToString()), Convert.ToInt32(dtFinalYearlyTax.Rows[i]["vacant_rate_id"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["total_fy_tax"].ToString()));
                    /*dtYearlyAssessment.Columns.Add("penalty", typeof(string));
                    dtYearlyAssessment.Columns.Add("tax_with_penalty", typeof(string));
                    dtYearlyAssessment.Columns.Add("current_tax_rebate", typeof(string));
                    dtYearlyAssessment.Columns.Add("water_harvest_rebate", typeof(string));
                    dtYearlyAssessment.Columns.Add("total_rebate", typeof(string));
                    dtYearlyAssessment.Columns.Add("final_tax", typeof(string));*/
                    ytatc.InsertYearlyRebatePenaltyAssessment(YIden, Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["penalty"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["tax_with_penalty"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["current_tax_rebate"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["water_harvest_rebate"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["total_rebate"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["final_tax"].ToString()), property_id);

                    if (dtYearlyFloorAssessment.Rows.Count > 0)
                    {
                        for (int j = 0; j < dtYearlyFloorAssessment.Rows.Count; j++)
                        {
                            if (dtYearlyFloorAssessment.Rows[j]["fin_year"].ToString() == dtFinalYearlyTax.Rows[i]["fin_year"].ToString())
                            {
                                /*
                    dtYearlyFloorAssessment.Columns.Add("floor_id", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("usage_type_id", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("occupancy_type_id", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("construct_type_id", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("built_area", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("carpet_area", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("use_type", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("occupancy_factor", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("arv", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("total_tax_factor", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("total_tax", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("arv_rate", typeof(string));
           dtYearlyFloorAssessment.Columns.Add("occupancy_type_detail_id", typeof(string));
                    */
                                // ytatc = new YearlyTaxAssessmentTmpController();
                                ytatc.InsertYearlyFloorAssessment(YIden, Convert.ToInt32(dtYearlyFloorAssessment.Rows[j]["floor_id"].ToString()), Convert.ToInt32(dtYearlyFloorAssessment.Rows[j]["usage_type_id"].ToString()), Convert.ToInt32(dtYearlyFloorAssessment.Rows[j]["occupancy_type_id"].ToString()), Convert.ToInt32(dtYearlyFloorAssessment.Rows[j]["construct_type_id"].ToString()), Convert.ToDecimal(dtYearlyFloorAssessment.Rows[j]["built_area"].ToString()), Convert.ToDecimal(dtYearlyFloorAssessment.Rows[j]["carpet_area"].ToString()), dtYearlyFloorAssessment.Rows[j]["use_type"].ToString(), Convert.ToDecimal(dtYearlyFloorAssessment.Rows[j]["occupancy_factor"].ToString()), Convert.ToDecimal(dtYearlyFloorAssessment.Rows[j]["arv"].ToString()), Convert.ToDecimal(dtYearlyFloorAssessment.Rows[j]["total_tax_factor"].ToString()), Convert.ToDecimal(dtYearlyFloorAssessment.Rows[j]["total_tax"].ToString()), Convert.ToDecimal(dtYearlyFloorAssessment.Rows[j]["arv_rate"].ToString()), Convert.ToInt32(dtYearlyFloorAssessment.Rows[j]["occupancy_type_detail_id"].ToString()));
                            }
                        }
                    }
                }
            }
            if (dtYearlyVacantAssessment.Rows.Count > 0)
            {
                for (int i = 0; i < dtYearlyVacantAssessment.Rows.Count; i++)
                {
                    /*
                 dtYearlyVacantAssessment.Columns.Add("area_decimal", typeof(string));
                dtYearlyVacantAssessment.Columns.Add("area_sqft",typeof(string));
                dtYearlyVacantAssessment.Columns.Add("area_sqmtr", typeof(string));
                dtYearlyVacantAssessment.Columns.Add("vacant_rate_id", typeof(string));
            
                dtYearlyVacantAssessment.Columns.Add("tax", typeof(string));
                dtYearlyVacantAssessment.Columns.Add("effect_from", typeof(string));
                 */
                    ytatc = new YearlyTaxAssessmentTmpController();
                    ytatc.InsertYearlyVacantAssessment(property_id, Convert.ToDecimal(dtYearlyVacantAssessment.Rows[i]["area_decimal"].ToString()), Convert.ToDecimal(dtYearlyVacantAssessment.Rows[i]["area_sqft"].ToString()), Convert.ToDecimal(dtYearlyVacantAssessment.Rows[i]["area_sqmtr"].ToString()), Convert.ToInt32(dtYearlyVacantAssessment.Rows[i]["vacant_rate_id"].ToString()), Convert.ToDecimal(dtYearlyVacantAssessment.Rows[i]["tax"].ToString()), dtYearlyVacantAssessment.Rows[i]["effect_from"].ToString());
                }
            }
        }
        public decimal FinalCalculation(decimal Tax, decimal CRebate, decimal CPenal, decimal Arr, decimal LateSub, decimal due, decimal adv, decimal WH, decimal WaterCharge)
        {
            decimal FTax = 0;
            FTax = Tax - Math.Round(CRebate, 0, MidpointRounding.AwayFromZero) + Math.Round(CPenal, 0, MidpointRounding.AwayFromZero) + Arr + LateSub + due - adv - WH + WaterCharge;
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
                CTRebate = CTRebateOrPenalty * (-1);
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
        public void getDemandBySMS(string mobile,string Pid)
        {
            //List<final> liFinal = new List<final>();
            long PropertyID = Convert.ToInt64(Pid);//2000011;

            bool isAuthenticate = false;
            decimal Tax_Holding = 0;
            decimal Tot_Pay = 0;

            String username = "BIHAREDISTRICT-pmc";
            String password = "pmcpat@#*2018";
            String senderid = "BRGOVT";
            String key = "f2475cb7-0406-4ca8-8e93-ebb16800d159";

            // generating hash
            new_single_sms ss;

            if (System.DateTime.Now.Month < 4)
            {
                CurrentAY = (System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year;
            }
            else
            {
                CurrentAY = System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1);
            }
            encryptdecrypt ed = new encryptdecrypt();
            MobAppRequestController mrc = new MobAppRequestController();
            isAuthenticate = true;

            if (isAuthenticate)
            {
                try
                {
                    MobAppPaymentController mapc = new MobAppPaymentController();
                    long test = mapc.chkPayment(Convert.ToInt64(Pid));
                    string Own_Name = "";
                    string Guard_Name = "";
                    sdc = new SearchDataController();
                    dt = sdc.SearchProperty(PropertyID);
                    if (dt.Rows.Count > 0)
                    {
                        Owner = sdc.SearchOwnerDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                        if (dt.Rows[0]["ownership_type"].ToString() == "Single Owner")
                        {
                            Own_Name = Owner.Rows[0]["owner_name"].ToString();
                            Guard_Name = Owner.Rows[0]["guardian_name"].ToString();
                        }
                        else if (dt.Rows[0]["ownership_type"].ToString() == "Joint Owner")
                        {
                            for (int i = 0; i < Owner.Rows.Count; i++)
                            {
                                if (i == (Owner.Rows.Count - 1))
                                {
                                    Own_Name += Owner.Rows[i]["owner_name"].ToString();
                                    Guard_Name += Owner.Rows[i]["guardian_name"].ToString();
                                }
                                else
                                {
                                    Own_Name += Owner.Rows[i]["owner_name"].ToString() + ", ";
                                    Guard_Name += Owner.Rows[i]["guardian_name"].ToString() + ", ";
                                }
                            }
                        }
                        else
                        {
                            Own_Name = Owner.Rows[0]["owner_name"].ToString() + " (" + dt.Rows[0]["ownership_type"].ToString() + ")";
                            Guard_Name = "NA";
                        }
                        if (test == 0)
                        {

                            if (dt.Rows.Count > 0)
                            {
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

                                dtAdvance = sdc.SearchAdvancePayment(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                                if (dtAdvance.Rows.Count <= 0)
                                {
                                    if (dt.Rows[0]["last_payment_year"].ToString() != "")
                                    {
                                        if (dt.Rows[0]["last_payment_receipt_no"].ToString() != "")
                                        {
                                            last_FY = dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + 1);
                                        }
                                        else
                                        {
                                            last_FY = dt.Rows[0]["last_assessment_year"].ToString();
                                        }
                                        dispAY = last_FY;
                                    }
                                    else
                                    {
                                        last_FY = Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Month < 4 ? ((Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year - 1) + "-" + (Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year)) : (Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year) + "-" + (Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year + 1);
                                        dispAY = "NA";
                                    }
                                    PdueAmount = 0;
                                    pAdvance = 0;

                                    if (dt.Rows[0]["last_payment_year"].ToString() != "")
                                    {
                                        LStatus = 1;
                                    }
                                    else
                                    {
                                        LStatus = 0;
                                    }


                                }
                                else
                                {
                                    LStatus = 1;
                                    //last_FY = dtAdvance.Rows[0]["fin_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dtAdvance.Rows[0]["fin_year"].ToString().Split('-')[1]) + 1);
                                    last_FY = dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + 1);
                                    //dispAY = dtAdvance.Rows[0]["fin_year"].ToString();
                                    dispAY = dt.Rows[0]["last_assessment_year"].ToString();

                                    if (dtAdvance.Rows[0]["total_dues"].ToString() == "" || dtAdvance.Rows[0]["total_dues"].ToString() == "0.00")
                                    {
                                        PdueAmount = 0;
                                    }
                                    else
                                    {
                                        decimal CurrDiffPenal = 0;
                                        decimal CurDiff = Convert.ToDecimal(dtAdvance.Rows[0]["current_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["current_received_amount"].ToString());
                                        //if (Convert.ToDecimal(dtAdvance.Rows[0]["current_panelty_payble_amount"].ToString()) > 0)
                                        //{
                                        CurrDiffPenal = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + Convert.ToInt32(dispAY.Split('-')[0])), CurDiff)[1];
                                        //}
                                        decimal CurrPenaltyDiff = Convert.ToDecimal(dtAdvance.Rows[0]["current_panelty_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["current_panelty_received_amount"].ToString());
                                        decimal ArrDiff = Convert.ToDecimal(dtAdvance.Rows[0]["arrear_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["arrear_received_amount"].ToString());
                                        decimal ArrDiffPenal = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + (Convert.ToInt32(dispAY.Split('-')[0]) - 1)), ArrDiff)[1];
                                        //PdueAmount = Convert.ToDecimal(dtAdvance.Rows[0]["total_dues"].ToString());

                                        decimal ArrPenaltyDiff = Convert.ToDecimal(dtAdvance.Rows[0]["arrear_panelty_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["arrear_panelty_received_amount"].ToString());

                                        PdueAmount = CurDiff + CurrDiffPenal + ArrDiff + ArrDiffPenal + CurrPenaltyDiff + ArrPenaltyDiff;

                                        //last_FY = Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + "-" + (Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + 1);
                                    }
                                    pAdvance = sdc.ArrearAdvance(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                                }
                                if (Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()) == 1)
                                {
                                    //TaxCalculationForLand(decimal TPArea,decimal BuiltArea,int STypeId,int OType,int PTYpe,string Last_AY)
                                    /*Current Tax*/
                                    Tax_Holding = TaxCalculationForLand(Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString()), Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString()), Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), last_FY, Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));//water_harvesting
                                    /*Current Rebate or Penalty*/
                                    CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                                    Arrear = ArrearCalculation(dtArrearData, toAY);

                                }
                                else if (Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()) == 2)
                                {
                                    decimal TPlotArea = Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString());
                                    decimal BuiltArea = Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString());
                                    DataTable dtFloorForBuilding = new DataTable();
                                    dtFloorForBuilding = sdc.SearchOccupancyDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                                    if (BuiltArea >= (TPlotArea * 70 / 100))
                                    {
                                        Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForBuilding, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                                        CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                                        Arrear = ArrearCalculation(dtArrearData, toAY);

                                    }
                                    else
                                    {
                                        Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForBuilding, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                                        //Arrear = ArrearCalculation(dtArrearData, toAY);
                                        Tax_Holding += TaxCalculationForLand(Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString()), Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString()), Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), last_FY, Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                                        Arrear += ArrearCalculation(dtArrearData, toAY);
                                        CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);

                                    }


                                }
                                else
                                {
                                    DataTable dtFloorForFlat = new DataTable();
                                    dtFloorForFlat = sdc.SearchOccupancyDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                                    Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForFlat, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                                    CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                                    Arrear = ArrearCalculation(dtArrearData, toAY);

                                }
                                wrcc = new WasteRateChartController();
                                Waste_Charge = wrcc.WasteChargeCalculate(PropertyID);
                                LateSubmission = LateSubmissionCal(Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year < 2013 ? Convert.ToDateTime("01-Apr-2013") : Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), LStatus, chkCommercial);
                                WaterTaxCharge = sdc.WaterTax(Convert.ToInt64(dt.Rows[0]["id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_tax_id"].ToString()));

                                if (WaterTaxCharge > 0)
                                {
                                    YearlyTaxAssessmentTmpController yyyy = new YearlyTaxAssessmentTmpController();
                                    //yyyy.InsertWaterTax(Convert.ToInt64(dt.Rows[0]["id"].ToString()), WaterTaxCharge);
                                }


                                Tax_Holding = FinalCalculation(Tax_Holding, CRP[0], CRP[1], Arrear, LateSubmission, PdueAmount, pAdvance, waterharvest, WaterTaxCharge);
                                if (Tax_Holding < 0)
                                {
                                    Tax_Holding = 0;
                                }


                                decimal ArrDue = sdc.ArrearDue(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                                Tax_Holding += ArrDue;

                                Tax_Holding = Math.Round(Tax_Holding, 0, MidpointRounding.AwayFromZero);
                                Tot_Pay = Tax_Holding + Waste_Charge;

                                string clientIp = HttpContext.Current.Request.UserHostAddress;
                                
                                string rid = mrc.Insert("SMS", Convert.ToInt64(dt.Rows[0]["PID"].ToString()), "", "1", 1, System.DateTime.Now, clientIp);
                                MobAppResponseController marc = new MobAppResponseController();
                                PropertyDetailTmpController pdtc = new PropertyDetailTmpController();

                                //long tempid = pdtc.Insert(Convert.ToInt64(dt.Rows[0]["PID"].ToString()), dt.Rows[0]["ward_id"].ToString(), rid);
                                int ms = marc.Insert(Convert.ToInt64(rid), Tax_Holding, Waste_Charge, LateSubmission, Tot_Pay, Convert.ToInt64(dt.Rows[0]["PID"].ToString()), 1, System.DateTime.Now);//tempid--Convert.ToInt64(dt.Rows[0]["PID"].ToString())
                                //yearlyTax(tempid);
                                clearStatus = sdc.ClearPayStatus(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                                if (clearStatus == 0)
                                {
                                    //Request_No = rid,
                                    //Property_No = dt.Rows[0]["PID"].ToString(),
                                    //Circle = dt.Rows[0]["circle_name"].ToString(),
                                    //Ward = dt.Rows[0]["ward_no"].ToString(),
                                    //Owner_Name = Own_Name,
                                    //Guardian_Name = Guard_Name,
                                    //Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                                    //Property_Type = dt.Rows[0]["property_type"].ToString(),
                                    //Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                                    //Last_Assessment_Year = dispAY,
                                    //Current_Assessment_Year = CurrentAY,
                                    //Total_Holding_Dues = String.Format("{0:0.00}", Tax_Holding),//String.Fo
                                    //Solid_Waste_Usage_Charge = String.Format("{0:0.00}", Math.Round(Waste_Charge, 0, MidpointRounding.AwayFromZero)),
                                    //Payable_Amt = String.Format("{0:0.00}", Tot_Pay),
                                    //Status = "Previous transaction pending due to Cheque clearance",
                                    //Status_Code = "600"
                                    ss = new new_single_sms();
                                    ss.sendSingleSMSNew(username, password, senderid, mobile, "Dear Satish, this message is to confirm that we received a payment of INR " + String.Format("{0:0.00}", Tot_Pay) + " against PRDA Shop/Office on 14-06-2021. Thank you! - Patna Municipal Corporation", key, "1307162308439202847");
                                }
                                else
                                {
                                    //Request_No = rid,
                                    //Property_No = dt.Rows[0]["PID"].ToString(),
                                    //Circle = dt.Rows[0]["circle_name"].ToString(),
                                    //Ward = dt.Rows[0]["ward_no"].ToString(),
                                    //Owner_Name = Own_Name,
                                    //Guardian_Name = Guard_Name,
                                    //Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                                    //Property_Type = dt.Rows[0]["property_type"].ToString(),
                                    //Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                                    //Last_Assessment_Year = dispAY,
                                    //Current_Assessment_Year = CurrentAY,
                                    //Total_Holding_Dues = String.Format("{0:0.00}", Tax_Holding),//String.Fo
                                    //Solid_Waste_Usage_Charge = String.Format("{0:0.00}", Math.Round(Waste_Charge, 0, MidpointRounding.AwayFromZero)),
                                    //Payable_Amt = String.Format("{0:0.00}", Tot_Pay),
                                    //Status = "Successful",
                                    //Status_Code = "200"

                                    ss = new new_single_sms();
                                    ss.sendSingleSMSNew(username, password, senderid, mobile, "Dear Satish, this message is to confirm that we received a payment of INR " + String.Format("{0:0.00}", Tot_Pay) + " against PRDA Shop/Office on 14-06-2021. Thank you! - Patna Municipal Corporation", key, "1307162308439202847");
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            //Request_No = test.ToString(),
                            //Property_No = Pid,
                            //Circle = dt.Rows[0]["circle_name"].ToString(),
                            //Ward = dt.Rows[0]["ward_no"].ToString(),
                            //Owner_Name = Own_Name,
                            //Guardian_Name = Guard_Name,
                            //Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                            //Property_Type = dt.Rows[0]["property_type"].ToString(),
                            //Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                            //Last_Assessment_Year = System.DateTime.Now.Month < 4 ? ((System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year) : (System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1)),
                            //Current_Assessment_Year = System.DateTime.Now.Month < 4 ? ((System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year) : (System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1)),
                            //Total_Holding_Dues = "0.00",//String.Fo
                            //Solid_Waste_Usage_Charge = "0.00",
                            //Payable_Amt = "0.00",
                            //Status = "Successful",
                            //Status_Code = "200"

                            ss = new new_single_sms();
                            ss.sendSingleSMSNew(username, password, senderid, mobile, "Dear Satish, this message is to confirm that we received a payment of INR 0.00 against PRDA Shop/Office on 14-06-2021. Thank you! - Patna Municipal Corporation", key, "1307162308439202847");
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }

        }


        public decimal getDemandForMutation(string Pid)
        {
            //List<final> liFinal = new List<final>();
            long PropertyID = 1443162; // static fallback
            if (!string.IsNullOrEmpty(Pid))
            {
                long.TryParse(Pid, out PropertyID);
            }


            decimal Tax_Holding = 0;
            decimal Tot_Pay = 0;
            if (System.DateTime.Now.Month < 4)
            {
                CurrentAY = (System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year;
            }
            else
            {
                CurrentAY = System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1);
            }

            MobAppRequestController mrc = new MobAppRequestController();

            MobAppPaymentController mapc = new MobAppPaymentController();
            long test = mapc.chkPayment(PropertyID);
            if (test == 0)
            {
                try
                {
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        string q = "SELECT COUNT(*) FROM tbl_QR_Citizen_Transaction WHERE PID = @PID AND Status = 'SUCCESS' AND MONTH(PaymentDate) = MONTH(GETDATE()) AND YEAR(PaymentDate) = YEAR(GETDATE())";
                        using (SqlCommand cmd = new SqlCommand(q, conn))
                        {
                            cmd.Parameters.AddWithValue("@PID", PropertyID.ToString());
                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                            if (count > 0)
                            {
                                test = 1; // Mark as paid to bypass calculation
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore and proceed
                }
            }
            string Own_Name = "";
            string Guard_Name = "";
            sdc = new SearchDataController();
            dt = sdc.SearchProperty(PropertyID);
            if (dt.Rows.Count > 0)
            {
                Owner = sdc.SearchOwnerDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                if (dt.Rows[0]["ownership_type"].ToString() == "Single Owner")
                {
                    Own_Name = Owner.Rows[0]["owner_name"].ToString();
                    Guard_Name = Owner.Rows[0]["guardian_name"].ToString();
                }
                else if (dt.Rows[0]["ownership_type"].ToString() == "Joint Owner")
                {
                    for (int i = 0; i < Owner.Rows.Count; i++)
                    {
                        if (i == (Owner.Rows.Count - 1))
                        {
                            Own_Name += Owner.Rows[i]["owner_name"].ToString();
                            Guard_Name += Owner.Rows[i]["guardian_name"].ToString();
                        }
                        else
                        {
                            Own_Name += Owner.Rows[i]["owner_name"].ToString() + ", ";
                            Guard_Name += Owner.Rows[i]["guardian_name"].ToString() + ", ";
                        }
                    }
                }
                else
                {
                    Own_Name = Owner.Rows[0]["owner_name"].ToString() + " (" + dt.Rows[0]["ownership_type"].ToString() + ")";
                    Guard_Name = "NA";
                }
                if (test == 0)
                {

                    if (dt.Rows.Count > 0)
                    {
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

                        dtAdvance = sdc.SearchAdvancePayment(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                        if (dtAdvance.Rows.Count <= 0)
                        {
                            if (dt.Rows[0]["last_payment_year"].ToString() != "")
                            {
                                if (dt.Rows[0]["last_payment_receipt_no"].ToString() != "")
                                {
                                    last_FY = dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + 1);
                                }
                                else
                                {
                                    last_FY = dt.Rows[0]["last_assessment_year"].ToString();
                                }
                                dispAY = last_FY;
                            }
                            else
                            {
                                last_FY = Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Month < 4 ? ((Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year - 1) + "-" + (Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year)) : (Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year) + "-" + (Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year + 1);
                                dispAY = "NA";
                            }
                            PdueAmount = 0;
                            pAdvance = 0;

                            if (dt.Rows[0]["last_payment_year"].ToString() != "")
                            {
                                LStatus = 1;
                            }
                            else
                            {
                                LStatus = 0;
                            }


                        }
                        else
                        {
                            LStatus = 1;
                            //last_FY = dtAdvance.Rows[0]["fin_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dtAdvance.Rows[0]["fin_year"].ToString().Split('-')[1]) + 1);
                            last_FY = dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1] + "-" + (Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + 1);
                            //dispAY = dtAdvance.Rows[0]["fin_year"].ToString();
                            dispAY = dt.Rows[0]["last_assessment_year"].ToString();

                            if (dtAdvance.Rows[0]["total_dues"].ToString() == "" || dtAdvance.Rows[0]["total_dues"].ToString() == "0.00")
                            {
                                PdueAmount = 0;
                            }
                            else
                            {
                                decimal CurrDiffPenal = 0;
                                decimal CurDiff = Convert.ToDecimal(dtAdvance.Rows[0]["current_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["current_received_amount"].ToString());
                                //if (Convert.ToDecimal(dtAdvance.Rows[0]["current_panelty_payble_amount"].ToString()) > 0)
                                //{
                                CurrDiffPenal = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + Convert.ToInt32(dispAY.Split('-')[0])), CurDiff)[1];
                                //}
                                decimal CurrPenaltyDiff = Convert.ToDecimal(dtAdvance.Rows[0]["current_panelty_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["current_panelty_received_amount"].ToString());
                                decimal ArrDiff = Convert.ToDecimal(dtAdvance.Rows[0]["arrear_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["arrear_received_amount"].ToString());
                                decimal ArrDiffPenal = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + (Convert.ToInt32(dispAY.Split('-')[0]) - 1)), ArrDiff)[1];
                                //PdueAmount = Convert.ToDecimal(dtAdvance.Rows[0]["total_dues"].ToString());

                                decimal ArrPenaltyDiff = Convert.ToDecimal(dtAdvance.Rows[0]["arrear_panelty_payble_amount"].ToString()) - Convert.ToDecimal(dtAdvance.Rows[0]["arrear_panelty_received_amount"].ToString());

                                PdueAmount = CurDiff + CurrDiffPenal + ArrDiff + ArrDiffPenal + CurrPenaltyDiff + ArrPenaltyDiff;

                                //last_FY = Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + "-" + (Convert.ToInt32(dt.Rows[0]["last_assessment_year"].ToString().Split('-')[1]) + 1);
                            }
                            pAdvance = sdc.ArrearAdvance(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                        }
                        if (Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()) == 1)
                        {
                            //TaxCalculationForLand(decimal TPArea,decimal BuiltArea,int STypeId,int OType,int PTYpe,string Last_AY)
                            /*Current Tax*/
                            Tax_Holding = TaxCalculationForLand(Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString()), Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString()), Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), last_FY, Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));//water_harvesting
                            /*Current Rebate or Penalty*/
                            CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                            Arrear = ArrearCalculation(dtArrearData, toAY);

                        }
                        else if (Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()) == 2)
                        {
                            if(dt.Rows[0]["water_harvesting"].ToString()=="")
                            {
                                dt.Rows[0]["water_harvesting"] = "0";
                            }
                            decimal TPlotArea = Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString());
                            decimal BuiltArea = Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString());
                            DataTable dtFloorForBuilding = new DataTable();
                            dtFloorForBuilding = sdc.SearchOccupancyDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));

                            if (BuiltArea >= (TPlotArea * 70 / 100))
                            {
                                Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForBuilding, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                                CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                                Arrear = ArrearCalculation(dtArrearData, toAY);

                            }
                            else
                            {
                                Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForBuilding, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                                //Arrear = ArrearCalculation(dtArrearData, toAY);
                                Tax_Holding += TaxCalculationForLand(Convert.ToDecimal(dt.Rows[0]["plot_area"].ToString()), Convert.ToDecimal(dt.Rows[0]["constructed_area"].ToString()), Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), last_FY, Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                                Arrear += ArrearCalculation(dtArrearData, toAY);
                                CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);

                            }


                        }
                        else
                        {
                            DataTable dtFloorForFlat = new DataTable();
                            dtFloorForFlat = sdc.SearchOccupancyDetails(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                            if (dt.Rows[0]["water_harvesting"].ToString() == "")
                            {
                                dt.Rows[0]["water_harvesting"]= "0";
                            }
                            Tax_Holding = TaxCalculationForStructure(last_FY, dtFloorForFlat, Convert.ToInt32(dt.Rows[0]["street_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["ownership_type_id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_harvesting"].ToString()));
                            CRP = CurrentRebatePenalty(Convert.ToDateTime("01-Apr-" + toAY), Tax_Holding);
                            Arrear = ArrearCalculation(dtArrearData, toAY);

                        }
                        wrcc = new WasteRateChartController();
                        Waste_Charge = wrcc.WasteChargeCalculate(PropertyID);
                        LateSubmission = LateSubmissionCal(Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year < 2013 ? Convert.ToDateTime("01-Apr-2013") : Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), LStatus, chkCommercial);
                        WaterTaxCharge = sdc.WaterTax(Convert.ToInt64(dt.Rows[0]["id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_tax_id"].ToString()));

                        
                        Tax_Holding = FinalCalculation(Tax_Holding, CRP[0], CRP[1], Arrear, LateSubmission, PdueAmount, pAdvance, waterharvest, WaterTaxCharge);
                        if (Tax_Holding < 0)
                        {
                            Tax_Holding = 0;
                        }

                        decimal ArrDue = sdc.ArrearDue(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                        Tax_Holding += ArrDue;

                        Tax_Holding = Math.Round(Tax_Holding, 0, MidpointRounding.AwayFromZero);
                        Tot_Pay = Tax_Holding + Waste_Charge;
                    }
                }
            }
            return Tot_Pay;
        }
    }
}