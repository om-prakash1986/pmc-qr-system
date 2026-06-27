using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ARV_calculation_for_flat
/// </summary>
public class ARV_calculation_for_flat
{
    ARV_for_vaccant_land ar = new ARV_for_vaccant_land();
    // PRIME ARV CALCULATION
    public string calculate_ARV_flat(string floor_area, string use_type, string road_type, string construction_type, string occupancy_type, string usage_type, string pid, string ass_year)
    {
        string amount_1 = "0";

        string carpet_area = calculate_carpet_area_new(floor_area, use_type);
        string occupancy_factor = calculate_occupancy_factor(occupancy_type, ass_year);
        string rental_value = calculate_rental_value(road_type, use_type, construction_type);
        string multiplying_factor = calculate_multiplying_factor(usage_type);

        double CA = Convert.ToDouble(carpet_area);
        double RV = Convert.ToDouble(rental_value);
        double OF = Convert.ToDouble(occupancy_factor);
        double MF = Convert.ToDouble(multiplying_factor);

        double ARV = (CA * RV * OF * MF);

        int is_govt = ar.find_if_property_is_govt(pid);
        if (is_govt == 1)
        {
            double real_amount = Convert.ToDouble(ARV);
            double payable_tax = ((real_amount * 75) / 100);

            ARV = payable_tax;
        }

        string finyear = "";
        double cyear = DateTime.Now.Year;
        double curyear = Convert.ToDouble(cyear);
        double prevyear = curyear - 1;
        string pyear = prevyear.ToString();
        double nextyear = curyear + 1;
        string nyear = nextyear.ToString();
        double currentMonth = Convert.ToDouble(DateTime.Now.Month);

        if (DateTime.Now.Month > 3)
        {
            finyear = curyear.ToString() + "-" + nextyear;
        }
        else
        {
            finyear = pyear + "-" + curyear.ToString();
        }

        if (ass_year == finyear)
        {
            if (currentMonth > 3 && currentMonth < 7)
            {
                // Give discount of 5% on after calculation of ARV from month 1st april to 30th june
                double Property_tax = (ARV * 9) / 100;

                //double after_discount = (Property_tax - (Property_tax * 5) / 100);
                amount_1 = Property_tax.ToString("0.00");
            }
            else if (currentMonth > 6 && currentMonth < 10)
            {
                // Don't Give discount on after calculation of ARV from 1st july to 30th Sept
                double Property_tax = (ARV * 9) / 100;
                amount_1 = Property_tax.ToString("0.00");
            }
            else
            {
                double Property_tax = (ARV * 9) / 100;
                amount_1 = Property_tax.ToString("0.00");
            }
        }
        else
        {
            double Property_tax = (ARV * 9) / 100;
            amount_1 = Property_tax.ToString("0.00");
        }
        return amount_1;
    }

    // FLOOR WISE CARPET AREA CALCULATION
    public string calculate_carpet_area(string floor_area, string occupancy_factor)
    {
        string calculated_value = "";
        // residential any case
        if (occupancy_factor == "1")
        {
            double f_a = Convert.ToDouble(floor_area);
            double c_value = (f_a * 70) / 100;
            calculated_value = c_value.ToString();
        }
        // commercial any case
        else
        {
            double f_a = Convert.ToDouble(floor_area);
            double c_value = (f_a * 80) / 100;
            calculated_value = c_value.ToString();
        }
        return calculated_value;
    }

    // FLOOR WISE RENTAL VALUE CALCULATION
    public string calculate_rental_value(string Road_type, string residential_type, string construction_type)
    {
        string rental_value = "";
        if (Road_type == "Principal Main Road")
        {
            if (residential_type == "Residential" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "12";
            }
            else if (residential_type == "Residential" && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "18";
            }
            else if (residential_type == "Residential" && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "6";
            }

            else if (residential_type == "Commercial" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "36";
            }
            else if (residential_type == "Commercial" && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "54";
            }
            else if (residential_type == "Commercial" && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "18";
            }

            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "24";
            }
            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "36";
            }
            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "12";
            }
        }

        else if (Road_type == "Main Road")
        {
            if (residential_type == "Residential" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "8";
            }
            else if (residential_type == "Residential" && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "12";
            }
            else if (residential_type == "Residential" && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "4";
            }

            else if (residential_type == "Commercial" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "24";
            }
            else if (residential_type == "Commercial" && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "36";
            }
            else if (residential_type == "Commercial" && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "12";
            }

            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "16";
            }
            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "24";
            }
            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "8";
            }
        }
        else if (Road_type == "Other Road")
        {
            if (residential_type == "Residential" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "4";
            }
            else if (residential_type == "Residential" && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "6";
            }
            else if (residential_type == "Residential" && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "2";
            }

            else if (residential_type == "Commercial" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "12";
            }
            else if (residential_type == "Commercial" && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "18";
            }
            else if (residential_type == "Commercial" && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "6";
            }

            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
            {
                rental_value = "8";
            }
            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Pucca with RCC Roof (RCC)")
            {
                rental_value = "12";
            }
            else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Kuttcha with Clay Roof (Other)")
            {
                rental_value = "4";
            }
        }
        return rental_value;
    }

    // FLOOR WISE OCCUPANCY FACTOR
    public string calculate_occupancy_factor(string occupancy_type, string ass_year)
    {
        string occupancy_factor = "0";
        string dbyear = ass_year;
        string before = dbyear.Split('-')[0];
        string after = dbyear.Split('-')[1];

        double frm_year = Convert.ToDouble(before);
        double to_year = Convert.ToDouble(after);

        if (to_year > 2013)
        {
            if (occupancy_type == "Self-Occupied")
            {
                occupancy_factor = "1";
            }
            else if (occupancy_type == "Tenanted" || occupancy_type == "Rented")
            {
                occupancy_factor = "1.5";
            }
        }
        else
        {
            if (occupancy_type == "Self-Occupied")
            {
                occupancy_factor = "1";
            }
            else if (occupancy_type == "Tenanted" || occupancy_type == "Rented")
            {
                occupancy_factor = "1";
            }
        }
        return occupancy_factor;
    }
    // FLOOR WISE MULTIPLYING FACTOR

    public string calculate_multiplying_factor(string usage_type)
    {
        string multiplying_factor = "1";
        multiplying_factor = "1";
        if (usage_type == "Religious and Spritual Places")
        {
            multiplying_factor = "0";
        }
        else
        {
            multiplying_factor = "1";
        }
        return multiplying_factor;
    }

    // FLOOR WISE CARPET AREA CALCULATION
    public string calculate_carpet_area_new(string floor_area, string usage_type)
    {
        string calculated_value = "";
        // residential any case
        if (usage_type == "Residential")
        {
            double f_a = Convert.ToDouble(floor_area);
            double c_value = (f_a * 70) / 100;
            calculated_value = c_value.ToString();
        }
        // commercial any case
        else
        {
            double f_a = Convert.ToDouble(floor_area);
            double c_value = (f_a * 80) / 100;
            calculated_value = c_value.ToString();
        }
        return calculated_value;
    }

    // Till HERE
}