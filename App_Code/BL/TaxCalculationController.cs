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
using System.IO;
using PMC.DAL;

namespace PMC
{
    /// <summary>
    /// Summary description for TaxCalculationController
    /// </summary>
    public class TaxCalculationController
    {
        public TaxCalculationController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public DataTable BuildupAreaTax(decimal builupArea, int OccupencyType, int streetTypeID, int useTypeID, int constTypeID, string usageType, string AY,int govtFact)
        {
            decimal _unitAreaRate, holdingtax, taxableArea, TaxValue = 0;
            decimal factor = 0;
            int arvMasterID = 0;

            ARVRateMasterController arvmaster = new ARVRateMasterController();
            OccupancyTypeDetailController otd = new OccupancyTypeDetailController();
            TaxRateMasterController trm = new TaxRateMasterController();
            UsageTypeMasterController utm = new UsageTypeMasterController();
            
            arvMasterID = arvmaster.GetARVRate(AY);
            factor = otd.GetOccupancyTypeDetail(OccupencyType, AY);
            _unitAreaRate = UnitAreaRate(streetTypeID, useTypeID, constTypeID, arvMasterID);
            
            
            DataTable dtTaxRate = trm.GetTaxRate(arvMasterID);
            holdingtax = Convert.ToDecimal(dtTaxRate.Rows[0]["holding_tax"]); //***value is in percent mode

            if (useTypeID == 1)
            {
                taxableArea = builupArea * 70 / 100;
            }
            else
            {
                taxableArea = builupArea * 80 / 100;
            }

            TaxValue = Math.Round((taxableArea * _unitAreaRate * factor ) * holdingtax / 100,0);
            if (govtFact == 9)
            {
                TaxValue = TaxValue * 75 / 100;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("TaxableArea", typeof(string));
            dt.Columns.Add("ARVFactor", typeof(string));
            dt.Columns.Add("OccupancyFactor", typeof(string));
            dt.Columns.Add("PropertyTaxRate", typeof(string));
            dt.Columns.Add("AnnualTax", typeof(string));
            DataRow myrow = dt.NewRow();
            myrow[0] = taxableArea.ToString();
            myrow[1] = _unitAreaRate.ToString();
            myrow[2] = factor.ToString();
            myrow[3] = holdingtax.ToString();
            myrow[4] = TaxValue.ToString();
            dt.Rows.Add(myrow);
            return dt;
        }
        private decimal UnitAreaRate(int streetTypeID, int useTypeID, int constTypeID, int arvMasterID)
        {
            decimal uar = 0;
            
            ARVRateDetailsController arvrate = new ARVRateDetailsController();
            uar = arvrate.GetARVRateDetail(arvMasterID, streetTypeID, useTypeID, constTypeID);
            return uar;
        }
        public DataTable VaccantAreaTaxForBuilding(decimal TotalArea, decimal BuildupArea, int streetTypeID, string AssessmentYear, int govtFact)
        {
            decimal taxRate,  taxableVaccantland, taxValue = 0;
            int taxRateid = 0;

            VaccandLandTaxController vlt = new VaccandLandTaxController();
            taxableVaccantland = TotalArea - (BuildupArea * Convert.ToDecimal(1.42855));
            taxRate = vlt.GetVaccandLandTax(streetTypeID, AssessmentYear);
            taxRateid = vlt.GetVaccandLandTaxRateID(streetTypeID, AssessmentYear);
            taxValue = Math.Round((taxableVaccantland * taxRate),0);
            
            if (govtFact == 9)
            {
                taxValue = taxValue * 75 / 100;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("TaxableArea", typeof(string));
            dt.Columns.Add("TaxRate", typeof(string));
            dt.Columns.Add("TaxRateID", typeof(string));
            dt.Columns.Add("TaxValue", typeof(string));

            DataRow myrow = dt.NewRow();
            myrow[0] = taxableVaccantland.ToString();
            myrow[1] = taxRate.ToString();
            myrow[2] = taxRateid.ToString();
            myrow[3] = taxValue.ToString();

            dt.Rows.Add(myrow);
            return dt;
        }
        public DataTable VaccantAreaTax(decimal TotalArea, int streetTypeID, string AssessmentYear, int govtFact)
        {
            decimal taxRate, taxableVaccantland, taxValue = 0;
            int taxRateid = 0;

            VaccandLandTaxController vlt = new VaccandLandTaxController();
            taxableVaccantland = TotalArea;
            taxRate = vlt.GetVaccandLandTax(streetTypeID, AssessmentYear);
            taxRateid = vlt.GetVaccandLandTaxRateID(streetTypeID, AssessmentYear);
            taxValue = Math.Round((taxableVaccantland * taxRate),0);
            if (govtFact == 9)
            {
                taxValue = taxValue * 75 / 100;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("TaxableArea", typeof(string));
            dt.Columns.Add("TaxRate", typeof(string));
            dt.Columns.Add("TaxRateID", typeof(string));
            dt.Columns.Add("TaxValue", typeof(string));
            
            DataRow myrow = dt.NewRow();
            myrow[0] = taxableVaccantland.ToString();
            myrow[1] = taxRate.ToString();
            myrow[2] = taxRateid.ToString();
            myrow[3] = taxValue.ToString();
            
            dt.Rows.Add(myrow);
            return dt;
        }
        public decimal InterestOrPanelOrRebate(DateTime fromDate,DateTime toDate,decimal amount)
        {
            decimal IntOrRebate = 0;
            int monthDiff = GetMonthsBetween(fromDate,toDate);
            int FinDiff = 0;
            int FinDiffOther = 0;
            if (monthDiff > 6)
            {
                FinDiff = monthDiff-6;
            }
           
            if (monthDiff <= 3)
            {
                IntOrRebate = -(amount * 5 / 100);
            }
            else if (monthDiff >= 4 & monthDiff <= 6)
            {
                IntOrRebate = 0;
            }
            else
            {
                FinDiffOther = GetMonthsBetween(Convert.ToDateTime("01-Apr-2013"), System.DateTime.Now);
                if (fromDate >= Convert.ToDateTime("01-Apr-2013"))
                {
                    IntOrRebate = amount * FinDiff * Convert.ToDecimal(1.5) / 100;
                }
                if (fromDate >= Convert.ToDateTime("01-Apr-2012") & fromDate <= Convert.ToDateTime("31-Mar-2013"))
                {
                    IntOrRebate = (amount * Convert.ToDecimal(3) / 100)+(amount*FinDiffOther*Convert.ToDecimal(1.5)/100);
                }
                if (fromDate >= Convert.ToDateTime("01-Apr-2011") & fromDate <= Convert.ToDateTime("31-Mar-2012"))
                {
                    IntOrRebate = (amount * Convert.ToDecimal(24) / 100) + (amount * FinDiffOther * Convert.ToDecimal(1.5) / 100); ;
                }
                if (fromDate >= Convert.ToDateTime("01-Apr-2010") & fromDate <= Convert.ToDateTime("31-Mar-2011"))
                {
                    IntOrRebate = (amount * Convert.ToDecimal(48) / 100) + (amount * FinDiffOther * Convert.ToDecimal(1.5) / 100); ;
                }
                if (fromDate <= Convert.ToDateTime("31-Mar-2010"))
                {
                    IntOrRebate = (amount * Convert.ToDecimal(76) / 100) + (amount * FinDiffOther * Convert.ToDecimal(1.5) / 100); ;
                }
            }
            return IntOrRebate;
        }
        public decimal WaterHarvestingRebate(decimal amount)
        {
            decimal WHRebate;
            WHRebate = amount * 5 / 100;
            return WHRebate;
        }
        
        public int GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to) return GetMonthsBetween(to, from);
            int monthDiff = 1;
            int yr = to.Year;
            if (to.Month >= from.Month)
            {
                monthDiff = (to.Month - from.Month) + ((to.Year - from.Year) * 12) + 1;
            }
            else
            {
                monthDiff = ((to.Month + 12 - from.Month) + ((to.Year - 1) - from.Year) * 12) + 1;
            }
            return monthDiff;
            
        }
    }
}
