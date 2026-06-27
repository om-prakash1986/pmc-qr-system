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
/// Summary description for FinalDetail
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [ScriptService]
public class FinalDetail : System.Web.Services.WebService {
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
    int fromAY=0;
    int toAY=0;
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
    public FinalDetail () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
        dtArrearData = new DataTable();
        dtArrearData.Columns.Add("Years",typeof(string));
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
        dtYearlyVacantAssessment.Columns.Add("area_sqft",typeof(string));
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
        FTYear = new string[] {fromAY.ToString(),toAY.ToString(),AssessYear};
        
        return FTYear;
    }
    public decimal TaxCalculationForLand(decimal TPArea,decimal BuiltArea,int STypeId,int OType,int PTYpe,string Last_AY,int WH)
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
            if (OType == 9 || OType == 10)
            {
                OType = 10;
            }
            if (PTYpe == 1)
            {
                dtTaxCalculate = tcc.VaccantAreaTax(TPArea, STypeId, AY, OType);
            }
            else
            {
                dtTaxCalculate = tcc.VaccantAreaTaxForBuilding(TPArea,BuiltArea, STypeId, AY, OType);
            }
            TTax = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["TaxValue"].ToString()),0);
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
                mrow[2] = Math.Round(TTax,2,MidpointRounding.AwayFromZero);
                //Calculate Penaly/Interest
                intPenal = tcc.InterestOrPanelOrRebate(Convert.ToDateTime("01-Apr-" + i), System.DateTime.Now, TTax);
                mrow[3] = Math.Round(intPenal,2,MidpointRounding.AwayFromZero);
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
            yrow[6] = intPenal+TTax;
            yrow[7] = intRebate;
            yrow[8] = waterharvest;
            yrow[9] = intRebate+waterharvest;
            yrow[10] = intPenal+TTax-intRebate-waterharvest;

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
            vrow[0] = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["TaxableArea"].ToString()) * Convert.ToDecimal(0.002295894), 2,MidpointRounding.AwayFromZero).ToString();
            vrow[1] = dtTaxCalculate.Rows[0]["TaxableArea"].ToString();
            vrow[2] = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["TaxableArea"].ToString()) * Convert.ToDecimal(10.764), 2,MidpointRounding.AwayFromZero).ToString();
            vrow[3] = dtTaxCalculate.Rows[0]["TaxRateID"];

            vrow[4] = TTax;
            vrow[5] = AY;

            dtYearlyVacantAssessment.Rows.Add(vrow);
        }
        
        
        decimal txLand = 0;

        txLand = CAnnualTax + CurVacTax;
        return Math.Round(txLand,0);
    }
    public decimal ArrearCalculation(DataTable dtArr,int yr)
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
        decimal BuiltUpArea=0;
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

                if (OType == 9 || OType == 10)
                {
                    if (chkCommercial == 1)
                    {
                        OType = 10;
                    }
                    else
                    {
                        OType = 9;
                    }
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
                TTax = Math.Round(Convert.ToDecimal(dtTaxCalculate.Rows[0]["AnnualTax"].ToString()),0);

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
                    mrow[1] = Math.Round(TTax,2,MidpointRounding.AwayFromZero);
                    mrow[2] = 0;
                    //Calculate Penaly/Interest
                    intPenal = tcc.InterestOrPanelOrRebate(Convert.ToDateTime("01-Apr-" + i), System.DateTime.Now, TTax);
                    mrow[3] = Math.Round(intPenal,2,MidpointRounding.AwayFromZero);
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
                Occup_Type_Detail_ID = otdc.GetOccupancyTypeDetailID(OcuupID,AY);

                frow[13] = Occup_Type_Detail_ID.ToString();

                dtYearlyFloorAssessment.Rows.Add(frow);
            }
        }
        decimal txConstruction = 0;

        txConstruction = CAnnualTax + CurVacTax;
        return Math.Round(txConstruction,2);
        
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
                row["final_tax"] = x.Sum(r => Math.Round(Convert.ToDecimal(r["final_tax"]),2,MidpointRounding.AwayFromZero));
                return row;
            }).CopyToDataTable();

            
            for (int i = 0; i < dtFinalYearlyTax.Rows.Count; i++)
            {
                
                YIden=ytatc.InsertYearlyAssessment(property_id, dtFinalYearlyTax.Rows[i]["fin_year"].ToString(), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["total_fy_tax"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["vacant_tax"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["vacant_rate"].ToString()), Convert.ToInt32(dtFinalYearlyTax.Rows[i]["vacant_rate_id"].ToString()), Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["total_fy_tax"].ToString()));
                /*dtYearlyAssessment.Columns.Add("penalty", typeof(string));
                dtYearlyAssessment.Columns.Add("tax_with_penalty", typeof(string));
                dtYearlyAssessment.Columns.Add("current_tax_rebate", typeof(string));
                dtYearlyAssessment.Columns.Add("water_harvest_rebate", typeof(string));
                dtYearlyAssessment.Columns.Add("total_rebate", typeof(string));
                dtYearlyAssessment.Columns.Add("final_tax", typeof(string));*/
                ytatc.InsertYearlyRebatePenaltyAssessment(YIden, Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["penalty"].ToString()),Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["tax_with_penalty"].ToString()),Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["current_tax_rebate"].ToString()),Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["water_harvest_rebate"].ToString()),Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["total_rebate"].ToString()),Convert.ToDecimal(dtFinalYearlyTax.Rows[i]["final_tax"].ToString()),property_id);
                
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



    public decimal FinalCalculation(decimal Tax,decimal CRebate,decimal CPenal,decimal Arr,decimal LateSub,decimal due,decimal adv,decimal WH,decimal WaterCharge)
    {
        decimal FTax = 0;
        FTax = Tax - Math.Round(CRebate, 0, MidpointRounding.AwayFromZero) + Math.Round(CPenal, 0, MidpointRounding.AwayFromZero) + Arr + LateSub + due - adv - WH + WaterCharge;
        return FTax;
    }
    public decimal[] CurrentRebatePenalty(DateTime frmDate,decimal amnt)
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
            CTRebate = CTRebateOrPenalty*(-1);
            CTPenalty = 0;
        }
        RP = new decimal[] {CTRebate,CTPenalty};
        return RP;
    }
    public decimal LateSubmissionCal(DateTime frmDate,int PType,int status,int CommStatus)
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
    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }
    JavaScriptSerializer js = new JavaScriptSerializer();


    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void getDemand(string Mid,string authoKey, string Pid)
    {
        //List<final> liFinal = new List<final>();
        long PropertyID = Convert.ToInt64(Pid);//2000011;
        final objFinal = null;
        bool isAuthenticate=false;
        decimal Tax_Holding = 0;
        decimal Tot_Pay = 0;
        if (System.DateTime.Now.Month < 4)
        {
            CurrentAY = (System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year;
        }
        else
        {
            CurrentAY = System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year+1);
        }
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();
        if (mrc.verifyUser(Mid,ed.EncodePasswordToBase64(authoKey))==true)
        {
            isAuthenticate = true;
        }
        if (isAuthenticate)
        {
            try
            { 
                MobAppPaymentController mapc=new MobAppPaymentController();
                long test=mapc.chkPayment(Convert.ToInt64(Pid));
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
                                
                                if (dtAdvance.Rows[0]["total_dues"].ToString() == "" || dtAdvance.Rows[0]["total_dues"].ToString()=="0.00")
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
                                pAdvance=sdc.ArrearAdvance(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
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
                                yyyy.InsertWaterTax(Convert.ToInt64(dt.Rows[0]["id"].ToString()), WaterTaxCharge);
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
                            string rid = mrc.Insert(Mid, Convert.ToInt64(dt.Rows[0]["PID"].ToString()), "", "1", 1, System.DateTime.Now, clientIp);
                            MobAppResponseController marc = new MobAppResponseController();
                            PropertyDetailTmpController pdtc = new PropertyDetailTmpController();

                           long tempid = pdtc.Insert(Convert.ToInt64(dt.Rows[0]["PID"].ToString()), dt.Rows[0]["ward_id"].ToString(),rid);
                           int ms = marc.Insert(Convert.ToInt64(rid), Tax_Holding, Waste_Charge, LateSubmission, Tot_Pay, tempid, 1, System.DateTime.Now);//tempid--Convert.ToInt64(dt.Rows[0]["PID"].ToString())
                           yearlyTax(tempid);
                            clearStatus=sdc.ClearPayStatus(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                            if (clearStatus==0)
                            {
                                objFinal = new final()
                                {
                                    Request_No = rid,
                                    Property_No = dt.Rows[0]["PID"].ToString(),
                                    Circle = dt.Rows[0]["circle_name"].ToString(),
                                    Ward = dt.Rows[0]["ward_no"].ToString(),
                                    Owner_Name = Own_Name,
                                    Guardian_Name = Guard_Name,
                                    Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                                    Property_Type = dt.Rows[0]["property_type"].ToString(),
                                    Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                                    Last_Assessment_Year = dispAY,
                                    Current_Assessment_Year = CurrentAY,
                                    Total_Holding_Dues = String.Format("{0:0.00}", Tax_Holding),//String.Fo
                                    Solid_Waste_Usage_Charge = String.Format("{0:0.00}", Math.Round(Waste_Charge, 0, MidpointRounding.AwayFromZero)),
                                    Payable_Amt = String.Format("{0:0.00}", Tot_Pay),
                                    Status = "Previous transaction pending due to Cheque clearance",
                                    Status_Code = "600"
                                };
                            }
                            else
                            {
                                objFinal = new final()
                                {
                                    Request_No = rid,
                                    Property_No = dt.Rows[0]["PID"].ToString(),
                                    Circle = dt.Rows[0]["circle_name"].ToString(),
                                    Ward = dt.Rows[0]["ward_no"].ToString(),
                                    Owner_Name = Own_Name,
                                    Guardian_Name = Guard_Name,
                                    Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                                    Property_Type = dt.Rows[0]["property_type"].ToString(),
                                    Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                                    Last_Assessment_Year = dispAY,
                                    Current_Assessment_Year = CurrentAY,
                                    Total_Holding_Dues = String.Format("{0:0.00}", Tax_Holding),//String.Fo
                                    Solid_Waste_Usage_Charge = String.Format("{0:0.00}", Math.Round(Waste_Charge, 0, MidpointRounding.AwayFromZero)),
                                    Payable_Amt = String.Format("{0:0.00}", Tot_Pay),
                                    Status = "Successful",
                                    Status_Code = "200"
                                };
                                
                            }
                        }
                        else
                        {
                            objFinal = new final()
                            {
                                Request_No = "NA",
                                Property_No = "NA",
                                Circle = "NA",
                                Ward = "NA",
                                Owner_Name = "NA",
                                Guardian_Name = "NA",
                                Mobile_No = "NA",
                                Property_Type = "NA",
                                Total_Plot_Area = "NA",
                                Last_Assessment_Year = "NA",
                                Current_Assessment_Year = "NA",
                                Total_Holding_Dues = "NA",//String.Fo
                                Solid_Waste_Usage_Charge = "NA",
                                Payable_Amt = "NA",
                                Status = "Invalid Property No.",
                                Status_Code="300"
                            };
                        }
                    }
                    else
                    {
                        objFinal = new final()
                        {
                            Request_No = test.ToString(),
                            Property_No = Pid,
                            Circle = dt.Rows[0]["circle_name"].ToString(),
                            Ward = dt.Rows[0]["ward_no"].ToString(),
                            Owner_Name = Own_Name,
                            Guardian_Name = Guard_Name,
                            Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                            Property_Type = dt.Rows[0]["property_type"].ToString(),
                            Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                            Last_Assessment_Year = System.DateTime.Now.Month < 4 ? ((System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year) : (System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1)),
                            Current_Assessment_Year = System.DateTime.Now.Month < 4 ? ((System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year) : (System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1)),
                            Total_Holding_Dues = "0.00",//String.Fo
                            Solid_Waste_Usage_Charge = "0.00",
                            Payable_Amt = "0.00",
                            Status = "Successful",
                            Status_Code = "200"
                        };
                        
                    }
                }
                else
                {
                    objFinal = new final()
                    {
                        Request_No = "NA",
                        Property_No = "NA",
                        Circle = "NA",
                        Ward = "NA",
                        Owner_Name = "NA",
                        Guardian_Name = "NA",
                        Mobile_No = "NA",
                        Property_Type = "NA",
                        Total_Plot_Area = "NA",
                        Last_Assessment_Year = "NA",
                        Current_Assessment_Year = "NA",
                        Total_Holding_Dues = "NA",//String.Fo
                        Solid_Waste_Usage_Charge = "NA",
                        Payable_Amt = "NA",
                        Status = "Invalid Property No.",
                        Status_Code = "300"
                    };
                }
            }
            catch (Exception ex)
            {
                objFinal = new final()
                {
                    Request_No = "NA",
                    Property_No = "NA",
                    Circle = "NA",
                    Ward = "NA",
                    Owner_Name = "NA",
                    Guardian_Name = "NA",
                    Mobile_No = "NA",
                    Property_Type = "NA",
                    Total_Plot_Area = "NA",
                    Last_Assessment_Year = "NA",
                    Current_Assessment_Year = "NA",
                    Total_Holding_Dues = "NA",//String.Fo
                    Solid_Waste_Usage_Charge = "NA",
                    Payable_Amt = "NA",
                    Status = "Error!!",
                    Status_Code = "400"
                };
            }
        }
        else
        {
            objFinal = new final()
            {
                            Request_No = "NA",
                            Property_No = "NA",
                            Circle = "NA",
                            Ward = "NA",
                            Owner_Name = "NA",
                            Guardian_Name = "NA",
                            Mobile_No = "NA",
                            Property_Type = "NA",
                            Total_Plot_Area = "NA",
                            Last_Assessment_Year = "NA",
                            Current_Assessment_Year = "NA",
                            Total_Holding_Dues = "NA",//String.Fo
                            Solid_Waste_Usage_Charge = "NA",
                            Payable_Amt = "NA",
                            Status = "Invalid Access",
                            Status_Code = "500"
            };
        }
        //string objectF = JsonConvert.SerializeObject(objFinal, Formatting.Indented).ToString();
        Context.Response.Write(JsonConvert.SerializeObject(objFinal, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
        //return js.Serialize(objFinal
    }

    

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void pushTransaction(test t1)
    {
        /*
         public string Request_No { get; set; }
    public string Transaction_ID { get; set; }
    public string Transaction_Date { get; set; }
    public string Paid_Amount { get; set; }
    public string Payment_Status { get; set; }
    public
         */
        bool isAuthenticate = false;
        string Mid = t1.Mid;
        string authoKey = t1.authoKey;
        string Request_No;
        string Transaction_ID;
        string Transaction_Date;
        string Paid_Amount;
        string Payment_Status;
        string Payment_Remarks;
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();
        returnStatus RStatus = null;
        if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
        {
            isAuthenticate = true;
        }
        try
        {
            if (isAuthenticate)
            {

                Request_No = t1.Request_No;
                Transaction_ID = t1.Transaction_ID;
                Transaction_Date = t1.Transaction_Date;
                Paid_Amount = t1.Paid_Amount;
                Payment_Status = t1.Payment_Status;
                Payment_Remarks = t1.Payment_Remarks;

            }
            else
            {
                Request_No = t1.Request_No;
                Transaction_ID = t1.Transaction_ID;
                Transaction_Date = t1.Transaction_Date;
                Paid_Amount = t1.Paid_Amount;
                Payment_Status = "Failed";
                Payment_Remarks = "Invalid Access";
            }
            string clientIp = HttpContext.Current.Request.UserHostAddress;
            MobAppPaymentController mapc = new MobAppPaymentController();
            int msg = mapc.Insert(Convert.ToInt64(Request_No), Transaction_ID, Convert.ToDateTime(Transaction_Date), Convert.ToDecimal(Paid_Amount), Payment_Status, Payment_Remarks, 1, System.DateTime.Now, clientIp);

            RStatus = new returnStatus();
            if (isAuthenticate)
            {
                
                if (Payment_Status == "Success")
                {
                    YearlyTaxAssessmentController ytac = new YearlyTaxAssessmentController();
                    DataTable dtRequestDetail= mrc.GetMobAppRequestDetailOnRequestID(Convert.ToInt64(Request_No));
                    DataTable PDetail=new DataTable();
                    sdc=new SearchDataController();
                    if (dtRequestDetail.Rows[0]["pid"].ToString() == "")
                    {
                        PDetail = sdc.SearchProperty(Convert.ToInt64(dtRequestDetail.Rows[0]["property_id"].ToString()));
                    }
                    else
                    {
                        PDetail = sdc.SearchProperty(Convert.ToInt64(dtRequestDetail.Rows[0]["pid"].ToString()));
                    }
                    decimal lateSub = 0;
                    if(dtRequestDetail.Rows[0]["late_submission_charge"].ToString()=="")
                    {
                        lateSub=0;
                    }
                    else
                    {
                        lateSub=Convert.ToDecimal(dtRequestDetail.Rows[0]["late_submission_charge"].ToString());
                    }
                    long ApplicationTranID = ytac.insertOnSuccessPayment(Convert.ToInt64(Request_No), Convert.ToInt64(PDetail.Rows[0]["id"].ToString()), Transaction_ID, Convert.ToDateTime(Transaction_Date), Convert.ToDecimal(Paid_Amount), lateSub, dtRequestDetail.Rows[0]["sas_no"].ToString(), Mid);

                    RStatus.Transaction = "Transaction ID: " + ApplicationTranID;
                    RStatus.Status = "Transaction Successfull";
                    RStatus.Receipt_Url = "Receipt URL: https://ptax.misspl.co.in/payment_receipt.aspx?uid=" + ApplicationTranID;
                    

                }
                else
                {
                    RStatus.Transaction = "Transaction ID: " + Transaction_ID;
                    RStatus.Status = "Transaction Failed";
                    RStatus.Receipt_Url = "NA";
                }
            }
            else
            {
                
                RStatus.Transaction = "NA";
                RStatus.Status = "Invalid Access";
                RStatus.Receipt_Url = "NA";
            }
            
        }
        catch (Exception ex)
        {
            
            RStatus.Transaction = "NA";
            RStatus.Status = "Error";
            RStatus.Receipt_Url = "NA";
            
        }
        Context.Response.Write(JsonConvert.SerializeObject(RStatus, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void pushTransactionChequeDD(testChequeDD t1)
    {
        /*
         public string Mid { get; set; }
    public string authoKey { get; set; }
    public string Request_No { get; set; }
    public string Payment_Mode { get; set; }//CHEQUE/DEMAND DRAFT
    public string Cheque_DD_No { get; set; }
    public string Cheque_DD_Date { get; set; }
    public string Bank_Name { get; set; }
    public string Bank_Branch_Name { get; set; }
    public string Paid_Amount { get; set; }
    public string Transaction_Date { get; set; }
    public string Payment_Status { get; set; }
    public string Payment_Remarks { get; set; }
         */
        bool isAuthenticate = false;
        string Mid = t1.Mid;
        string authoKey = t1.authoKey;
        string Request_No;
        string Payment_Mode;
        string Cheque_DD_No;
        string Cheque_DD_Date;
        string Bank_Name;
        string Bank_Branch_Name;
        
        string Paid_Amount;
        string Transaction_Date;
        string Payment_Status;
        string Payment_Remarks;
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();
        returnStatus RStatus = null;
        string clientIp = HttpContext.Current.Request.UserHostAddress;
        MobAppPaymentController mapc = new MobAppPaymentController();
        int msg = 0;
        if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
        {
            isAuthenticate = true;
        }
        try
        {
            if (isAuthenticate)
            {

                Request_No = t1.Request_No;
                Payment_Mode = t1.Payment_Mode;
                Cheque_DD_No = t1.Cheque_DD_No;                
                Cheque_DD_Date = t1.Cheque_DD_Date;
                Bank_Name = t1.Bank_Name;
                Bank_Branch_Name = t1.Bank_Branch_Name;
                Paid_Amount = t1.Paid_Amount;
                Transaction_Date = t1.Transaction_Date;
                Payment_Status = t1.Payment_Status;
                Payment_Remarks = t1.Payment_Remarks;
                string Transaction_ID = "";
                if (Payment_Mode == "CHEQUE")
                {
                    Transaction_ID += "CH";
                }
                else if (Payment_Mode == "DEMAND DRAFT")
                {
                    Transaction_ID += "DD";
                }
                else
                {

                }
                Transaction_ID += Cheque_DD_No + Convert.ToDateTime(Cheque_DD_Date).Day + Convert.ToDateTime(Cheque_DD_Date).Month + Convert.ToDateTime(Cheque_DD_Date).Year;
                msg = mapc.Insert(Convert.ToInt64(Request_No), Transaction_ID, Convert.ToDateTime(Transaction_Date), Convert.ToDecimal(Paid_Amount), Payment_Status, Payment_Remarks, 1, System.DateTime.Now, clientIp);
            }
            else
            {
                Request_No = t1.Request_No;
                Payment_Mode = t1.Payment_Mode;
                Cheque_DD_No = t1.Cheque_DD_No;
                Cheque_DD_Date = t1.Cheque_DD_Date;
                Bank_Name = t1.Bank_Name;
                Bank_Branch_Name = t1.Bank_Branch_Name;
                Paid_Amount = t1.Paid_Amount;
                Transaction_Date = t1.Transaction_Date;
                Payment_Status = "Failed";
                Payment_Remarks = "Invalid Access";
            }
            
            RStatus = new returnStatus();
            if (isAuthenticate)
            {

                if (Payment_Status == "Success")
                {
                    YearlyTaxAssessmentController ytac = new YearlyTaxAssessmentController();
                    DataTable dtRequestDetail = mrc.GetMobAppRequestDetailOnRequestID(Convert.ToInt64(Request_No));
                    DataTable PDetail = new DataTable();
                    sdc = new SearchDataController();
                    if (dtRequestDetail.Rows[0]["pid"].ToString() == "")
                    {
                        PDetail = sdc.SearchProperty(Convert.ToInt64(dtRequestDetail.Rows[0]["property_id"].ToString()));
                    }
                    else
                    {
                        PDetail = sdc.SearchProperty(Convert.ToInt64(dtRequestDetail.Rows[0]["pid"].ToString()));
                    }
                    decimal lateSub = 0;
                    if (dtRequestDetail.Rows[0]["late_submission_charge"].ToString() == "")
                    {
                        lateSub = 0;
                    }
                    else
                    {
                        lateSub = Convert.ToDecimal(dtRequestDetail.Rows[0]["late_submission_charge"].ToString());
                    }
                    long ApplicationTranID = ytac.insertOnSuccessPaymentChequeDD(Convert.ToInt64(Request_No), Convert.ToInt64(PDetail.Rows[0]["id"].ToString()), Payment_Mode, Cheque_DD_No, Convert.ToDateTime(Cheque_DD_Date), Bank_Name, Bank_Branch_Name, Convert.ToDecimal(Paid_Amount), Convert.ToDateTime(Transaction_Date), lateSub, dtRequestDetail.Rows[0]["sas_no"].ToString(), Mid);

                    RStatus.Transaction = "Transaction ID: " + ApplicationTranID;
                    RStatus.Status = "Transaction Successfull";
                    RStatus.Receipt_Url = "Receipt URL: https://ptax.misspl.co.in/payment_receipt.aspx?uid=" + ApplicationTranID;


                }
                else
                {
                    RStatus.Transaction = "NA";
                    RStatus.Status = "Transaction Failed";
                    RStatus.Receipt_Url = "NA";
                }
            }
            else
            {

                RStatus.Transaction = "NA";
                RStatus.Status = "Invalid Access";
                RStatus.Receipt_Url = "NA";
            }

        }
        catch (Exception ex)
        {

            RStatus.Transaction = "NA";
            RStatus.Status = "Error";
            RStatus.Receipt_Url = "NA";

        }
        Context.Response.Write(JsonConvert.SerializeObject(RStatus, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void getStatus(string Mid, string authoKey, string rid)
    {
        checkStatus objFinal = null;
        bool isAuthenticate = false;
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();
        if (mrc.verifyUser(Mid,ed.EncodePasswordToBase64(authoKey))==true)
        {
            isAuthenticate = true;
        }
        if (isAuthenticate)
        {
            try
            {
                MobAppPaymentController mapc = new MobAppPaymentController();
                DataTable test = mapc.GetMobAppPaymentDetail(Convert.ToInt64(rid));
                if (test.Rows.Count > 0)
                {
                    if (test.Rows[0]["payment_status"].ToString() == "Success")
                    {
                        objFinal = new checkStatus()
                        {
                            Status_Code="1",
                            Status = "Success"
                        };
                    }
                    else
                    {
                        objFinal = new checkStatus()
                        {
                            Status_Code = "0",
                            Status = "Failed"
                        };
                    }
                }
                else
                {
                    objFinal = new checkStatus()
                    {
                        Status_Code = "2",
                        Status = "Transaction not found"
                    };
                }
            }
            catch (Exception ex)
            {
                objFinal = new checkStatus()
                {
                    Status_Code = "3",
                    Status = "Technical Issue"
                };
            }
        }
        else
        {
            objFinal = new checkStatus()
            {
                Status_Code = "4",
                Status = "Invalid Access"
            };
        }
        //string objectF = JsonConvert.SerializeObject(objFinal, Formatting.Indented).ToString();
        Context.Response.Write(JsonConvert.SerializeObject(objFinal, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
        //return js.Serialize(objFinal
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void getTransaction(string Mid, string authoKey, string Pid)
    {
        transaction t;
        List<transaction> tl;
        transactionDetails objTransaction = null;
        try
        {
            bool isAuthenticate = false;
            encryptdecrypt ed = new encryptdecrypt();
            MobAppRequestController mrc = new MobAppRequestController();
            if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
            {
                isAuthenticate = true;
            }
            if (isAuthenticate)
            {
                long PropertyID = Convert.ToInt64(Pid);
                SearchDataController sdc;
                sdc = new SearchDataController();
                dt = sdc.SearchProperty(PropertyID);
                if (dt.Rows.Count > 0)
                {
                    TransactionDetailController tdc = new TransactionDetailController();
                    DataTable dtTransactionForApp = new DataTable();
                    dtTransactionForApp = tdc.GetTransactionDetailsForApp(PropertyID);
                    
                    if (dtTransactionForApp.Rows.Count > 0)
                    {
                        tl = new List<transaction>();
                        for (int i = 0; i < dtTransactionForApp.Rows.Count; i++)
                        {
                            t = new transaction()
                            {
                                Receipt_No = dtTransactionForApp.Rows[i]["receipt_no"].ToString(),
                                Payment_Mode = dtTransactionForApp.Rows[i]["payment_mode"].ToString(),
                                Payment_Date = dtTransactionForApp.Rows[i]["PDate"].ToString(),
                                Paid_Amount = dtTransactionForApp.Rows[i]["amount"].ToString(),
                                Receipt_Url="https://pmc.bihar.gov.in/payment_receipt.aspx?uid="+dtTransactionForApp.Rows[i]["id"].ToString()
                            };

                            tl.Add(t);
                        }
                        objTransaction = new transactionDetails()
                        {
                            transactionList = tl,
                            Status = "Success",
                            Status_Code = "100"
                        };
                    }
                    else
                    {
                        t = new transaction()
                        {
                            Receipt_No = "NA",
                            Payment_Mode = "NA",
                            Payment_Date = "NA",
                            Paid_Amount = "NA",
                            Receipt_Url="NA"
                        };
                        tl = new List<transaction>();
                        tl.Add(t);
                        objTransaction = new transactionDetails()
                        {
                            transactionList = tl,
                            Status = "No transaction found",
                            Status_Code = "200"
                        };
                    }
                }
                else
                {
                    t = new transaction()
                    {
                        Receipt_No = "NA",
                        Payment_Mode = "NA",
                        Payment_Date = "NA",
                        Paid_Amount = "NA",
                        Receipt_Url="NA"
                    };
                    tl = new List<transaction>();
                    tl.Add(t);
                    objTransaction = new transactionDetails()
                    {
                        transactionList = tl,
                        Status = "Invalid Property No.",
                        Status_Code = "300"
                    };
                }
            }
            else
            {
                t = new transaction()
                {
                    Receipt_No = "NA",
                    Payment_Mode = "NA",
                    Payment_Date = "NA",
                    Paid_Amount = "NA",
                    Receipt_Url="NA"
                };
                tl = new List<transaction>();
                tl.Add(t);
                objTransaction = new transactionDetails()
                {
                    transactionList = tl,
                    Status = "Invalid Access",
                    Status_Code = "500"
                };
            }
        }
        catch (Exception ex)
        {
            t = new transaction()
            {
                Receipt_No = "NA",
                Payment_Mode = "NA",
                Payment_Date = "NA",
                Paid_Amount = "NA",
                Receipt_Url="NA"
            };
            tl = new List<transaction>();
            tl.Add(t);
            objTransaction = new transactionDetails()
            {
                transactionList=tl,
                Status = "Error!!",
                Status_Code = "400"
            };
        }
        //string objectF = JsonConvert.SerializeObject(objFinal, Formatting.Indented).ToString();
        Context.Response.Write(JsonConvert.SerializeObject(objTransaction, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
        //return js.Serialize(objFinal
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void getData(string Mid, string authoKey, string Pid,string Sas,string Mobile)
    {
        data d;
        List<data> dl;
        dataDetails objData = null;
        try
        {
            bool isAuthenticate = false;
            encryptdecrypt ed = new encryptdecrypt();
            MobAppRequestController mrc = new MobAppRequestController();
            if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
            {
                isAuthenticate = true;
            }
            if (isAuthenticate)
            {
                string q = "";
                
                if (Pid != null)
                {
                    dt = new DataTable();
                    q = "";
                    param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@id", Convert.ToInt64(Pid)));
                    q = "select property.id, property.application_no as sas_no, circle.circle_name,ward.ward_no, " +
                        "property.assessment_year as last_assessment_year,property.PID as property_no, property.old_pid, property.new_holding_no, property.old_holding_no," +
                        "property.mobile_no,property.[address],osm.owner_name " +

                        " from tbl_property_detail as property" +
                        " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                        " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                        " inner join (select property_id,STRING_AGG(owner_name,', ') as owner_name from tbl_owner_detail group by property_id) as osm on osm.property_id=property.id" +
                        " where property.pid=@id and (property.status=1 or property.status=3 or property.status=2)";
                    dac = new DataAccessLayer();
                    dt = dac.GetDataTable(q, param);
                             
                }
                else if(Sas!=null)
                {
                    dt = new DataTable();
                    q = "";
                    param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@id", Sas));
                    q = "select property.id, property.application_no as sas_no, circle.circle_name,ward.ward_no, " +
                        "property.assessment_year as last_assessment_year,property.PID as property_no, property.old_pid, property.new_holding_no, property.old_holding_no," +
                        "property.mobile_no,property.[address],osm.owner_name " +

                        " from tbl_property_detail as property" +
                        " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                        " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                        " inner join (select property_id,STRING_AGG(owner_name,', ') as owner_name from tbl_owner_detail group by property_id) as osm on osm.property_id=property.id" +
                        " where property.application_no=@id and (property.status=1 or property.status=3 or property.status=2)";
                    dac = new DataAccessLayer();
                    dt = dac.GetDataTable(q, param);
                }
                else if (Mobile != null)
                {
                    dt = new DataTable();
                    q = "";
                    param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@id", Mobile));
                    q = "select property.id, property.application_no as sas_no, circle.circle_name,ward.ward_no, " +
                        "property.assessment_year as last_assessment_year,property.PID as property_no, property.old_pid, property.new_holding_no, property.old_holding_no," +
                        "property.mobile_no,property.[address],osm.owner_name " +

                        " from tbl_property_detail as property" +
                        " inner join tbl_ward_master as ward on ward.id=property.ward_id" +
                        " inner join tbl_circle_master as circle on circle.id=ward.circle_id" +
                        " inner join (select property_id,STRING_AGG(owner_name,', ') as owner_name from tbl_owner_detail group by property_id) as osm on osm.property_id=property.id" +
                        " where property.mobile_no=@id and (property.status=1 or property.status=3 or property.status=2)";
                    dac = new DataAccessLayer();
                    dt = dac.GetDataTable(q, param);
                }
                else
                {
                    dt = new DataTable();
                }
                if (dt.Rows.Count > 0)
                {
                    dl = new List<data>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        d = new data()
                        {
                            id = dt.Rows[i]["id"].ToString(),
                            application_no = dt.Rows[i]["sas_no"].ToString(),
                            property_no = dt.Rows[i]["property_no"].ToString(),
                            circle = dt.Rows[i]["circle_name"].ToString(),
                            ward = dt.Rows[i]["ward_no"].ToString(),
                            owner_name = dt.Rows[i]["owner_name"].ToString(),

                            mobile_no = dt.Rows[i]["mobile_no"].ToString(),
                            address = dt.Rows[i]["address"].ToString(),
                            last_assessment_year = dt.Rows[i]["last_assessment_year"].ToString()
                        };

                        dl.Add(d);
                    }
                    objData = new dataDetails()
                    {
                         dataList= dl,
                        Status = "Success",
                        Status_Code = "100"
                    };
                }
                else
                {
                    d = new data()
                    {
                        id = "NA",
                        application_no = "NA",
                        property_no = "NA",
                        circle = "NA",
                        ward = "NA",
                        owner_name = "NA",

                        mobile_no = "NA",
                        address = "NA",
                        last_assessment_year = "NA"
                    };
                    dl = new List<data>();
                    dl.Add(d);
                    objData = new dataDetails()
                    {
                        dataList = dl,
                        Status = "Invalid input data",
                        Status_Code = "200"
                    };   
                }
            }
            else
            {
                d = new data()
                {
                    id="NA",
                    application_no="NA",
                    property_no="NA",
                    circle="NA",
                    ward="NA",
                    owner_name="NA",
    
                    mobile_no="NA",
                    address="NA",
                    last_assessment_year="NA"
                };
                dl = new List<data>();
                dl.Add(d);
                objData = new dataDetails()
                {
                    dataList = dl,
                    Status = "Invalid Access",
                    Status_Code = "300"
                };
            }
        }
        catch (Exception ex)
        {
            d = new data()
            {
                id = "NA",
                application_no = "NA",
                property_no = "NA",
                circle = "NA",
                ward = "NA",
                owner_name = "NA",

                mobile_no = "NA",
                address = "NA",
                last_assessment_year = "NA"
            };
            dl = new List<data>();
            dl.Add(d);
            objData = new dataDetails()
            {
                dataList = dl,
                Status = "Error!!",
                Status_Code = "400"
            };
        }
        //string objectF = JsonConvert.SerializeObject(objFinal, Formatting.Indented).ToString();
        Context.Response.Write(JsonConvert.SerializeObject(objData, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
        //return js.Serialize(objFinal
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void getFinalDemand(string Mid, string authoKey, string id)
    {
        //List<final> liFinal = new List<final>();
        long PropertyID = Convert.ToInt64(id);//2000011;
        final_sas objFinal = null;
        bool isAuthenticate = false;
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
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();
        if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
        {
            isAuthenticate = true;
        }
        if (isAuthenticate)
        {
            try
            {
                MobAppPaymentController mapc = new MobAppPaymentController();
                long test = mapc.chkPayment(id);
                string Own_Name = "";
                string Guard_Name = "";
                string Sas_No = "";
                string PID = "";
                sdc = new SearchDataController();
                dt = sdc.SearchProperty(id);
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
                            if (dt.Rows[0]["PID"].ToString() == "")
                            {
                                Waste_Charge = 0;
                            }
                            else
                            {
                                Waste_Charge = wrcc.WasteChargeCalculate(Convert.ToInt64(dt.Rows[0]["PID"].ToString()));
                            }
                            LateSubmission = LateSubmissionCal(Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()).Year < 2013 ? Convert.ToDateTime("01-Apr-2013") : Convert.ToDateTime(dt.Rows[0]["acquisition"].ToString()), Convert.ToInt32(dt.Rows[0]["property_type_id"].ToString()), LStatus, chkCommercial);
                            WaterTaxCharge = sdc.WaterTax(Convert.ToInt64(dt.Rows[0]["id"].ToString()), Convert.ToInt32(dt.Rows[0]["water_tax_id"].ToString()));

                            if (WaterTaxCharge > 0)
                            {
                                YearlyTaxAssessmentTmpController yyyy = new YearlyTaxAssessmentTmpController();
                                yyyy.InsertWaterTax(Convert.ToInt64(dt.Rows[0]["id"].ToString()), WaterTaxCharge);
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
                            string rid = mrc.InsertWithPid(Mid,dt.Rows[0]["id"].ToString(), dt.Rows[0]["PID"].ToString(), "", "1", 1, clientIp);
                            MobAppResponseController marc = new MobAppResponseController();
                            PropertyDetailTmpController pdtc = new PropertyDetailTmpController();

                            long tempid = pdtc.InsertSas(dt.Rows[0]["id"].ToString(), dt.Rows[0]["ward_id"].ToString(), rid);
                            int ms = marc.Insert(Convert.ToInt64(rid), Tax_Holding, Waste_Charge, LateSubmission, Tot_Pay, tempid, 1, System.DateTime.Now);//tempid--Convert.ToInt64(dt.Rows[0]["PID"].ToString())
                            yearlyTax(tempid);
                            clearStatus = sdc.ClearPayStatus(Convert.ToInt64(dt.Rows[0]["id"].ToString()));
                            if (clearStatus == 0)
                            {
                                objFinal = new final_sas()
                                {
                                    Request_No = rid,
                                    Property_No = dt.Rows[0]["PID"].ToString(),
                                    Sas_No = dt.Rows[0]["application_no"].ToString(),
                                    Circle = dt.Rows[0]["circle_name"].ToString(),
                                    Ward = dt.Rows[0]["ward_no"].ToString(),
                                    Owner_Name = Own_Name,
                                    Guardian_Name = Guard_Name,
                                    Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                                    Property_Type = dt.Rows[0]["property_type"].ToString(),
                                    Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                                    Last_Assessment_Year = dispAY,
                                    Current_Assessment_Year = CurrentAY,
                                    Total_Holding_Dues = String.Format("{0:0.00}", Tax_Holding),//String.Fo
                                    Solid_Waste_Usage_Charge = String.Format("{0:0.00}", Math.Round(Waste_Charge, 0, MidpointRounding.AwayFromZero)),
                                    Payable_Amt = String.Format("{0:0.00}", Tot_Pay),
                                    Status = "Previous transaction pending due to Cheque clearance",
                                    Status_Code = "600"
                                };
                            }
                            else
                            {
                                objFinal = new final_sas()
                                {
                                    Request_No = rid,
                                    Property_No = dt.Rows[0]["PID"].ToString(),
                                    Sas_No = dt.Rows[0]["application_no"].ToString(),
                                    Circle = dt.Rows[0]["circle_name"].ToString(),
                                    Ward = dt.Rows[0]["ward_no"].ToString(),
                                    Owner_Name = Own_Name,
                                    Guardian_Name = Guard_Name,
                                    Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                                    Property_Type = dt.Rows[0]["property_type"].ToString(),
                                    Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                                    Last_Assessment_Year = dispAY,
                                    Current_Assessment_Year = CurrentAY,
                                    Total_Holding_Dues = String.Format("{0:0.00}", Tax_Holding),//String.Fo
                                    Solid_Waste_Usage_Charge = String.Format("{0:0.00}", Math.Round(Waste_Charge, 0, MidpointRounding.AwayFromZero)),
                                    Payable_Amt = String.Format("{0:0.00}", Tot_Pay),
                                    Status = "Successful",
                                    Status_Code = "200"
                                };

                            }
                        }
                        else
                        {
                            objFinal = new final_sas()
                            {
                                Request_No = "NA",
                                Property_No = "NA",
                                Sas_No = "NA",
                                Circle = "NA",
                                Ward = "NA",
                                Owner_Name = "NA",
                                Guardian_Name = "NA",
                                Mobile_No = "NA",
                                Property_Type = "NA",
                                Total_Plot_Area = "NA",
                                Last_Assessment_Year = "NA",
                                Current_Assessment_Year = "NA",
                                Total_Holding_Dues = "NA",//String.Fo
                                Solid_Waste_Usage_Charge = "NA",
                                Payable_Amt = "NA",
                                Status = "Invalid Property No.",
                                Status_Code = "300"
                            };
                        }
                    }
                    else
                    {
                        objFinal = new final_sas()
                        {
                            Request_No = test.ToString(),
                            Property_No = dt.Rows[0]["PID"].ToString(),
                            Sas_No = dt.Rows[0]["application_no"].ToString(),
                            Circle = dt.Rows[0]["circle_name"].ToString(),
                            Ward = dt.Rows[0]["ward_no"].ToString(),
                            Owner_Name = Own_Name,
                            Guardian_Name = Guard_Name,
                            Mobile_No = dt.Rows[0]["mobile_no"].ToString(),
                            Property_Type = dt.Rows[0]["property_type"].ToString(),
                            Total_Plot_Area = dt.Rows[0]["plot_area"].ToString(),
                            Last_Assessment_Year = System.DateTime.Now.Month < 4 ? ((System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year) : (System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1)),
                            Current_Assessment_Year = System.DateTime.Now.Month < 4 ? ((System.DateTime.Now.Year - 1) + "-" + System.DateTime.Now.Year) : (System.DateTime.Now.Year + "-" + (System.DateTime.Now.Year + 1)),
                            Total_Holding_Dues = "0.00",//String.Fo
                            Solid_Waste_Usage_Charge = "0.00",
                            Payable_Amt = "0.00",
                            Status = "Successful",
                            Status_Code = "200"
                        };

                    }
                }
                else
                {
                    objFinal = new final_sas()
                    {
                        Request_No = "NA",
                        Property_No = "NA",
                        Sas_No = "NA",
                        Circle = "NA",
                        Ward = "NA",
                        Owner_Name = "NA",
                        Guardian_Name = "NA",
                        Mobile_No = "NA",
                        Property_Type = "NA",
                        Total_Plot_Area = "NA",
                        Last_Assessment_Year = "NA",
                        Current_Assessment_Year = "NA",
                        Total_Holding_Dues = "NA",//String.Fo
                        Solid_Waste_Usage_Charge = "NA",
                        Payable_Amt = "NA",
                        Status = "Invalid Property No.",
                        Status_Code = "300"
                    };
                }
            }
            catch (Exception ex)
            {
                objFinal = new final_sas()
                {
                    Request_No = "NA",
                    Property_No = "NA",
                    Sas_No = "NA",
                    Circle = "NA",
                    Ward = "NA",
                    Owner_Name = "NA",
                    Guardian_Name = "NA",
                    Mobile_No = "NA",
                    Property_Type = "NA",
                    Total_Plot_Area = "NA",
                    Last_Assessment_Year = "NA",
                    Current_Assessment_Year = "NA",
                    Total_Holding_Dues = "NA",//String.Fo
                    Solid_Waste_Usage_Charge = "NA",
                    Payable_Amt = "NA",
                    Status = "Error!!",
                    Status_Code = "400"
                };
            }
        }
        else
        {
            objFinal = new final_sas()
            {
                Request_No = "NA",
                Property_No = "NA",
                Sas_No = "NA",
                Circle = "NA",
                Ward = "NA",
                Owner_Name = "NA",
                Guardian_Name = "NA",
                Mobile_No = "NA",
                Property_Type = "NA",
                Total_Plot_Area = "NA",
                Last_Assessment_Year = "NA",
                Current_Assessment_Year = "NA",
                Total_Holding_Dues = "NA",//String.Fo
                Solid_Waste_Usage_Charge = "NA",
                Payable_Amt = "NA",
                Status = "Invalid Access",
                Status_Code = "500"
            };
        }
        //string objectF = JsonConvert.SerializeObject(objFinal, Formatting.Indented).ToString();
        Context.Response.Write(JsonConvert.SerializeObject(objFinal, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
        //return js.Serialize(objFinal
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void getReceipt(string Mid, string authoKey, string TransactionId)
    {
        receipt objFinal = null;
        bool isAuthenticate = false;
        encryptdecrypt ed = new encryptdecrypt();
        MobAppRequestController mrc = new MobAppRequestController();
        if (mrc.verifyUser(Mid, ed.EncodePasswordToBase64(authoKey)) == true)
        {
            isAuthenticate = true;
        }
        if (isAuthenticate)
        {
            try
            {
                PaymentReceipt pr = new PaymentReceipt();
                DataTable ReceiptData = pr.getData(Convert.ToInt64(TransactionId));
                
                if (ReceiptData.Rows.Count > 0)
                {
                    string R_Fy = "";
                    string[] fy = ReceiptData.Rows[0]["receipt_fy"].ToString().Replace(" To ", ",").Trim().Split(',');
                    if (fy[0] == fy[1])
                    {
                        R_Fy = fy[0].ToString();
                    }
                    else
                    {
                        R_Fy = ReceiptData.Rows[0]["receipt_fy"].ToString();
                    }

                    string[] swc = pr.getSoliWasteCharge(ReceiptData.Rows[0]["receipt_no"].ToString()).Split('-');
                    decimal swc1 = 0;
                    decimal swc2 = 0;
                    if (swc[0] != "")
                    {
                        swc1 = Convert.ToDecimal(swc[0]);
                    }
                    if (swc.Length == 2)
                    {
                        if (swc[1] != "")
                        {
                            swc2 = Convert.ToDecimal(swc[1]);
                        }
                    }
                    objFinal = new receipt()
                    {
                        Receipt_FY = R_Fy,
                        PID = ReceiptData.Rows[0]["pid"].ToString(),
                        Receipt_No = ReceiptData.Rows[0]["receipt_no"].ToString(),
                        Payment_Date = ReceiptData.Rows[0]["receipt_date"].ToString(),
                        Related_To = ReceiptData.Rows[0]["related_to"].ToString(),
                        Received_From = ReceiptData.Rows[0]["received_from"].ToString(),
                        Subject = ReceiptData.Rows[0]["subject"].ToString(),
                        Narration = ReceiptData.Rows[0]["naration"].ToString(),
                        Owner = ReceiptData.Rows[0]["owner"].ToString(),
                        Gardiuan_Name = ReceiptData.Rows[0]["gardiuan_name"].ToString(),
                        Mobile_No = ReceiptData.Rows[0]["mobile_no"].ToString(),
                        Address = ReceiptData.Rows[0]["address"].ToString(),
                        Payment_Mode = ReceiptData.Rows[0]["payment_mode"].ToString(),
                        Reference_No = ReceiptData.Rows[0]["reference_no"].ToString(),
                        Reference_Date = ReceiptData.Rows[0]["reference_date"].ToString(),
                        Bank_Name = ReceiptData.Rows[0]["bank_name"].ToString(),
                        PTax_Amount = ReceiptData.Rows[0]["amount"].ToString(),
                        Arrear_Amount = ReceiptData.Rows[0]["arrear_payble_amount"].ToString(),
                        Current_Amount = ReceiptData.Rows[0]["current_payble_amount"].ToString(),
                        Arrear_Penalty_Amount = ReceiptData.Rows[0]["arrear_panelty_payble_amount"].ToString(),
                        Current_Penalty_Amount = ReceiptData.Rows[0]["current_panelty_payble_amount"].ToString(),
                        Total_Payble_Amount = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["total_payble_amount"].ToString()), 0)),
                        Rebate_Amount = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["rebate_amount"].ToString()), 0)),
                        RWH_amount = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["rain_water_harvt_amount"].ToString()), 0)),
                        Water_Tax = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["water_tax"].ToString()), 0)),
                        Advance_Adjusted = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["advance_adjusted"].ToString()), 0)),
                        Arrear_Adjusted = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["arrear_adjusted"].ToString()), 0)),
                        Waste_Usage_Charge = String.Format("{0:0.00}", swc2),
                        Actual_Payable_Amount = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["total_payble_amount"].ToString()), 0) - Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["rebate_amount"].ToString()), 0) - Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["rain_water_harvt_amount"].ToString()), 0) + Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["water_tax"].ToString()), 0) - Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["advance_adjusted"].ToString()), 0) + Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["arrear_adjusted"].ToString()), 0)+swc2),
                        Received_Amount = String.Format("{0:0.00}", (Convert.ToDecimal(ReceiptData.Rows[0]["total_received_amount"].ToString()) + Convert.ToDecimal(swc1))),
                        Outstanding_Amount = String.Format("{0:0.00}", (Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["total_payble_amount"].ToString()), 0) - Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["rebate_amount"].ToString()), 0) - Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["rain_water_harvt_amount"].ToString()), 0) + Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["water_tax"].ToString()), 0) - Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["advance_adjusted"].ToString()), 0) + Math.Round(Convert.ToDecimal(ReceiptData.Rows[0]["arrear_adjusted"].ToString()), 0) + swc2) - (Convert.ToDecimal(ReceiptData.Rows[0]["total_received_amount"].ToString()) + Convert.ToDecimal(swc1))),
                        Receipt_URL = "https://ptax.misspl.co.in/payment_receipt.aspx?uid=" + TransactionId,
                        Status_Code = "1",
                        Status = "Success"
                    };
                }
                else
                {
                    objFinal = new receipt()
                    {
                        Receipt_FY = "NA",
                        PID = "NA",
                        Receipt_No = "NA",
                        Payment_Date = "NA",
                        Related_To = "NA",
                        Received_From = "NA",
                        Subject = "NA",
                        Narration = "NA",
                        Owner = "NA",
                        Gardiuan_Name = "NA",
                        Mobile_No = "NA",
                        Address = "NA",
                        Payment_Mode = "NA",
                        Reference_No = "NA",
                        Reference_Date = "NA",
                        Bank_Name = "NA",
                        PTax_Amount = "NA",
                        Arrear_Amount = "NA",
                        Current_Amount = "NA",
                        Arrear_Penalty_Amount = "NA",
                        Current_Penalty_Amount = "NA",
                        Total_Payble_Amount = "NA",
                        Rebate_Amount = "NA",
                        RWH_amount = "NA",
                        Water_Tax = "NA",
                        Advance_Adjusted = "NA",
                        Arrear_Adjusted = "NA",
                        Waste_Usage_Charge = "NA",
                        Actual_Payable_Amount = "NA",
                        Received_Amount = "NA",
                        Outstanding_Amount = "NA",
                        Receipt_URL = "NA",
                        Status_Code = "2",
                        Status = "Record not found"
                    };
                }
            }
            catch (Exception ex)
            {
                objFinal = new receipt()
                {
                    Receipt_FY="NA",
                    PID="NA",
                    Receipt_No="NA",
                    Payment_Date="NA",
                    Related_To="NA",
                    Received_From="NA",
                    Subject="NA",
                    Narration="NA",
                    Owner="NA",
                    Gardiuan_Name="NA",
                    Mobile_No="NA",
                    Address="NA",
                    Payment_Mode="NA",
                    Reference_No="NA",
                    Reference_Date="NA",
                    Bank_Name="NA",
                    PTax_Amount="NA",
                    Arrear_Amount="NA",
                    Current_Amount="NA",
                    Arrear_Penalty_Amount="NA",
                    Current_Penalty_Amount="NA",
                    Total_Payble_Amount="NA",
                    Rebate_Amount="NA",
                    RWH_amount="NA",
                    Water_Tax="NA",
                    Advance_Adjusted="NA",
                    Arrear_Adjusted="NA",
                    Waste_Usage_Charge="NA",
                    Actual_Payable_Amount="NA",
                    Received_Amount="NA",
                    Outstanding_Amount="NA",
                    Receipt_URL = "NA",
                    Status_Code = "3",
                    Status = "Technical Issue"
                };
            }
        }
        else
        {
            objFinal = new receipt()
            {
                Receipt_FY = "NA",
                PID = "NA",
                Receipt_No = "NA",
                Payment_Date = "NA",
                Related_To = "NA",
                Received_From = "NA",
                Subject = "NA",
                Narration = "NA",
                Owner = "NA",
                Gardiuan_Name = "NA",
                Mobile_No = "NA",
                Address = "NA",
                Payment_Mode = "NA",
                Reference_No = "NA",
                Reference_Date = "NA",
                Bank_Name = "NA",
                PTax_Amount = "NA",
                Arrear_Amount = "NA",
                Current_Amount = "NA",
                Arrear_Penalty_Amount = "NA",
                Current_Penalty_Amount = "NA",
                Total_Payble_Amount = "NA",
                Rebate_Amount = "NA",
                RWH_amount = "NA",
                Water_Tax = "NA",
                Advance_Adjusted = "NA",
                Arrear_Adjusted = "NA",
                Waste_Usage_Charge = "NA",
                Actual_Payable_Amount = "NA",
                Received_Amount = "NA",
                Outstanding_Amount = "NA",
                Receipt_URL="NA",
                Status_Code = "4",
                Status = "Invalid Access"
            };
        }
        //string objectF = JsonConvert.SerializeObject(objFinal, Formatting.Indented).ToString();
        Context.Response.Write(JsonConvert.SerializeObject(objFinal, Formatting.Indented));
        Context.Response.Flush();
        Context.Response.End();
        //return js.Serialize(objFinal
    }
}
public class final
{
    public string Request_No { get; set; }
    public string Property_No { get; set; }
    public string Circle { get; set; }
    public string Ward { get; set; }
    public string Owner_Name { get; set; }
    public string Guardian_Name { get; set; }
    public string Mobile_No { get; set; }
    public string Property_Type { get; set; }
    public string Total_Plot_Area { get; set; }
    public string Last_Assessment_Year { get; set; }
    public string Current_Assessment_Year { get; set; }
    public string Total_Holding_Dues { get; set; }
    public string Solid_Waste_Usage_Charge { get; set; }
    public string Payable_Amt { get; set; }
    public string Status { get; set; }
    public string Status_Code { get; set; }
}
public class final_sas
{
    public string Request_No { get; set; }
    public string Property_No { get; set; }
    public string Sas_No { get; set; }
    public string Circle { get; set; }
    public string Ward { get; set; }
    public string Owner_Name { get; set; }
    public string Guardian_Name { get; set; }
    public string Mobile_No { get; set; }
    public string Property_Type { get; set; }
    public string Total_Plot_Area { get; set; }
    public string Last_Assessment_Year { get; set; }
    public string Current_Assessment_Year { get; set; }
    public string Total_Holding_Dues { get; set; }
    public string Solid_Waste_Usage_Charge { get; set; }
    public string Payable_Amt { get; set; }
    public string Status { get; set; }
    public string Status_Code { get; set; }
}
public class test {
    public string Mid { get; set; }
    public string authoKey { get; set; }
    public string Request_No { get; set; }
    public string Transaction_ID { get; set; }
    public string Transaction_Date { get; set; }
    public string Paid_Amount { get; set; }
    public string Payment_Status { get; set; }
    public string Payment_Remarks { get; set; }
}
public class testChequeDD
{
    public string Mid { get; set; }
    public string authoKey { get; set; }
    public string Request_No { get; set; }
    public string Payment_Mode { get; set; }//CHEQUE/DEMAND DRAFT
    public string Cheque_DD_No { get; set; }
    public string Cheque_DD_Date { get; set; }
    public string Bank_Name { get; set; }
    public string Bank_Branch_Name { get; set; }
    public string Paid_Amount { get; set; }
    public string Transaction_Date { get; set; }
    public string Payment_Status { get; set; }
    public string Payment_Remarks { get; set; }
}
public class returnStatus
{
    public string Transaction { get; set; }
    public string Status { get; set; }
    public string Receipt_Url { get; set; }
}
public class checkStatus
{
    public string Status { get; set; }
    public string Status_Code { get; set; }
}
public class transaction
{
    public string Receipt_No { get; set; }
    public string Payment_Mode { get; set; }
    public string Payment_Date { get; set; }
    public string Paid_Amount { get; set; }
    public string Receipt_Url { get; set; }
}
public class transactionDetails
{
    public List<transaction> transactionList
    {
        get;
        set;
    }
    public string Status { get; set; }
    public string Status_Code { get; set; }
}
public class data
{
    public string id { get; set; }
    public string application_no { get; set; }
    public string property_no { get; set; }
    public string circle { get; set; }
    public string ward { get; set; }
    public string owner_name { get; set; }
    
    public string mobile_no { get; set; }
    public string address { get; set; }
    public string last_assessment_year { get; set; }
}
public class dataDetails
{
    public List<data> dataList
    {
        get;
        set;
    }
    public string Status { get; set; }
    public string Status_Code { get; set; }
}
public class receipt
{
    public string Receipt_FY { get; set; }
    public string PID { get; set; }
    public string Receipt_No { get; set; }
    public string Payment_Date { get; set; }
    public string Related_To { get; set; }
    public string Received_From { get; set; }
    public string Subject { get; set; }
    public string Narration { get; set; }
    public string Owner { get; set; }
    public string Gardiuan_Name { get; set; }
    public string Mobile_No { get; set; }
    public string Address { get; set; }
    public string Payment_Mode { get; set; }
    public string Reference_No { get; set; }
    public string Reference_Date { get; set; }
    public string Bank_Name { get; set; }
    public string PTax_Amount { get; set; }
    public string Arrear_Amount { get; set; }
    public string Current_Amount { get; set; }
    public string Arrear_Penalty_Amount { get; set; }
    public string Current_Penalty_Amount { get; set; }
    public string Total_Payble_Amount { get; set; }
    public string Rebate_Amount { get; set; }
    public string RWH_amount { get; set; }
    public string Water_Tax { get; set; }
    public string Advance_Adjusted { get; set; }
    public string Arrear_Adjusted { get; set; }
    public string Waste_Usage_Charge { get; set; }
    public string Actual_Payable_Amount { get; set; }
    public string Received_Amount { get; set; }
    public string Outstanding_Amount { get; set; }
    public string Receipt_URL { get; set; }
    public string Status_Code { get; set; }
    public string Status { get; set; }
}
