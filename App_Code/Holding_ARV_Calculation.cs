using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Holding_ARV_Calculation
/// </summary>
public class Holding_ARV_Calculation
{
    // PRIME ARV CALCULATION
    public string calculate_ARV_flat(string floor_area,string use_type,string road_type,string construction_type,string occupancy_type,string usage_type)
    { 
        string amount = "0";

        string occupancy_factor = calculate_occupancy_factor(occupancy_type);
        string carpet_area = calculate_carpet_area(floor_area, occupancy_factor);
        string rental_value = calculate_rental_value(road_type, use_type, construction_type);
        string multiplying_factor = calculate_multiplying_factor(usage_type);

        double CA = Convert.ToDouble(carpet_area);
        double RV = Convert.ToDouble(rental_value);
        double OF = Convert.ToDouble(occupancy_factor);
        double MF = Convert.ToDouble(multiplying_factor);

        double ARV = (CA * RV * OF * MF);
        double Property_tax = (ARV * 9) / 100;
        amount = Property_tax.ToString();
        return amount;
    }

    // FLOOR WISE CARPET AREA CALCULATION
    public string calculate_carpet_area(string floor_area,string occupancy_factor)
    {
        string calculated_value = "";
        if (occupancy_factor == "1")
        {
            double f_a = Convert.ToDouble(floor_area);
            double c_value = (f_a * 70) / 100;
            calculated_value = c_value.ToString();
        }
        else
        {
            double f_a = Convert.ToDouble(floor_area);
            double c_value = (f_a * 80) / 100;
            calculated_value = c_value.ToString();
        }
        return calculated_value;
    }

    // FLOOR WISE RENTAL VALUE CALCULATION
    public string calculate_rental_value(string Road_type,string residential_type,string construction_type)
    {
        string rental_value = "";
            if(Road_type =="Principal Main Road")
            {
                if (residential_type == "Residential" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
                {
                    rental_value = "12";
                }
                else if (residential_type == "Residential" && construction_type == "Pucca with RCC Roof (RCC)")
                {
                    rental_value = "18";
                }
                else if (residential_type == "Residential" && construction_type == "Kuttcha with Clay Roof (Other))")
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
                else if (residential_type == "Commercial" && construction_type == "Kuttcha with Clay Roof (Other))")
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
                else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Kuttcha with Clay Roof (Other))")
                {
                    rental_value = "12";
                }
            }

            else if(Road_type =="Main Road")
            {
                if (residential_type == "Residential" && construction_type == "Pucca with Asbestos/Corrugated Sheet (ACC)")
                {
                    rental_value = "8";
                }
                else if (residential_type == "Residential" && construction_type == "Pucca with RCC Roof (RCC)")
                {
                    rental_value = "12";
                }
                else if (residential_type == "Residential" && construction_type == "Kuttcha with Clay Roof (Other))")
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
                else if (residential_type == "Commercial" && construction_type == "Kuttcha with Clay Roof (Other))")
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
                else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Kuttcha with Clay Roof (Other))")
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
                else if (residential_type == "Residential" && construction_type == "Kuttcha with Clay Roof (Other))")
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
                else if (residential_type == "Commercial" && construction_type == "Kuttcha with Clay Roof (Other))")
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
                else if ((residential_type == "Other" || residential_type == "Others") && construction_type == "Kuttcha with Clay Roof (Other))")
                {
                    rental_value = "4";
                }
            }
        return rental_value;
    }

    // FLOOR WISE OCCUPANCY FACTOR
    public string calculate_occupancy_factor(string occupancy_type)
    {
        string occupancy_factor = "0";
            if (occupancy_type == "Self-Occupied")
            {
                occupancy_factor = "1";
            }
            else if (occupancy_type == "Tenanted")
            {
                occupancy_factor = "1.5";
            }
        return occupancy_factor;
    }
    // FLOOR WISE MULTIPLYING FACTOR

    public string calculate_multiplying_factor(string usage_type)
    {
        string multiplying_factor = "1";
         if(usage_type =="Residential")
         {
           multiplying_factor="1";  
         }	

        if(usage_type =="Commercial Offices")
        {
           multiplying_factor="3";  
        }	

        if(usage_type =="Shops")
        {
           multiplying_factor="1";  
        }
        if(usage_type =="Govt. Establishment")
        {
            multiplying_factor="1";  
        }
        if(usage_type =="Govt. Offices")
        {
            multiplying_factor="1";  
        }
        if(usage_type =="Financial Institutes")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Schools")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Storage")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Coaching Classes")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Workshop")
        {
            multiplying_factor="2";  
        }
        if(usage_type =="Hotels")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Parking")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Trust")
        {
            multiplying_factor="1";  
        }
        if(usage_type =="Colleges")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Clubs")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Petrol Pump")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Research Institute")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Guest Houses")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Hostels")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Godowns")
        {
            multiplying_factor="2";  
        }
        if(usage_type =="Dispensaries")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Hospitals")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Warehouses")
        {
            multiplying_factor="2";  
        }
        if(usage_type =="Insurance Offices")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Laboratories")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Nursing Home")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Religious and Spritual Places")
        {
            multiplying_factor="0";  
        }
        if(usage_type =="Industries")
        {
            multiplying_factor="2";  
        }
        if(usage_type =="Other")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Show room")
        {
            multiplying_factor="1";  
        }
        if(usage_type =="Health Club")
        {
            multiplying_factor="3";  
        }
        if(usage_type =="Educational Institutes")
        {
            multiplying_factor="1";  
        }
        if(usage_type =="Restaurants")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Commercial")
        {
            multiplying_factor="1.5";  
        }
        if(usage_type =="Marriage Hall")
        {
            multiplying_factor="3";  
        }
        return multiplying_factor;
    }

    // Till HERE
}