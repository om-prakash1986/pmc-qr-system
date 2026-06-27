using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for calculate_due_payments_for_houses
/// </summary>
public class calculate_due_payments_for_houses
{
    public string calculate_due_payment(string PID, string property_type, string id,string ass_year)
    {
        holding_updations hu = new holding_updations();
        calculate_holding_deu_details chdu = new calculate_holding_deu_details();
        ARV_for_vaccant_land arv_vaccant = new ARV_for_vaccant_land();
        ARV_for_Land_and_building arv_building_land = new ARV_for_Land_and_building();
        ARV_calculation_for_flat arv_flat = new ARV_calculation_for_flat();

        string payment_for_property_tax = "";


        string due = "0";
        double arv_amount = 0;
        double plot_area = 0;
        double constructed_area = 0;
        double calculated_area = 0;

        string tota = "0";
        string builta = "0";
        string rr_t = "";
        if (property_type == "Land + Building")
        {
            DataTable dt = chdu.find_vaccant_land(PID);
            if (dt.Rows.Count > 0)
            {
                plot_area = Convert.ToDouble(dt.Rows[0]["PlotArea"].ToString());
                constructed_area = Convert.ToDouble(dt.Rows[0]["ConstructedArea"].ToString());
                calculated_area = (plot_area - constructed_area);

                DataTable dt1 = hu.is_property_available_flat_and_house_id_wise(PID, id);
                int i = 0;
                if (dt1.Rows.Count > 0)
                {
                    while (i < dt1.Rows.Count)
                    {
                        string total_area = dt.Rows[0]["PlotArea"].ToString();
                        string builtup_area = dt1.Rows[i]["BuiltupArea"].ToString();
                        string use_type = dt1.Rows[i]["UseType"].ToString();
                        string road_type = dt.Rows[0]["StreetType"].ToString();
                        string construction_type = dt1.Rows[i]["ContructionType"].ToString();
                        string occupancy_type = dt1.Rows[i]["OccupancyType"].ToString();
                        string usage_type= dt1.Rows[i]["UsageType"].ToString();
                        string municipality_type = "Municipal Corporation";
                        string floor_no = dt1.Rows[i]["FloorNo"].ToString();
                        
                        if (i == 0)
                        {
                            tota = total_area.ToString();
                            builta = builtup_area.ToString();
                            rr_t = road_type.ToString();
                        }

                        string amut = arv_building_land.calculate_ARV_house_and_building(total_area, builtup_area, use_type, road_type, construction_type, occupancy_type, usage_type, municipality_type, PID, ass_year);
                        arv_amount += Convert.ToDouble(amut);

                        double tot_area = Convert.ToDouble(tota);
                        double bup_area = Convert.ToDouble(builta);
                        double perc = ((bup_area / tot_area) * 100);

                        if (perc < 70)
                        {
                            double a = Convert.ToDouble(tota);
                            double b = Convert.ToDouble(builta);
                            double c = a - (b * 1.43);

                            if (floor_no == "Ground Floor")
                            {

                                ////taxable_vaccant_land = ((Convert.ToDouble(total_area) - (Convert.ToDouble(builtup_area)* 1.43))).ToString();
                                //string taxable_vaccant_land = c.ToString();
                                //ARV_for_vaccant_land vc_land = new ARV_for_vaccant_land();

                                //string amount_2 = vc_land.calculate_arv_for_vaccant_land(taxable_vaccant_land, "Municipal Corporation", rr_t, PID, ass_year);
                                ////amount = (amount_1 + amount_2);

                                //double am1 = Convert.ToDouble(arv_amount);
                                //double am2 = Convert.ToDouble(amount_2);

                                //double total_paid_amount = (am1 + am2);
                                //string amount = total_paid_amount.ToString();
                                //arv_amount = Convert.ToDouble(amount);
                            }
                        }
                        i++;
                    }
                }
            }
            //int round_due = Convert.ToInt32(arv_amount);
            due = arv_amount.ToString();
            //due = round_due.ToString();
        }
        else if (property_type == "Flat")
        {
            DataTable dt = chdu.find_vaccant_land(PID);
            if (dt.Rows.Count > 0)
            {
                DataTable dt1 = hu.is_property_available_flat_and_house_id_wise(PID, id);
                int i = 0;
                if (dt1.Rows.Count > 0)
                {
                    while (i < dt1.Rows.Count)
                    {
                        //string total_area = dt.Rows[0]["PlotArea"].ToString();
                        string builtup_area = dt1.Rows[i]["BuiltupArea"].ToString();
                        string use_type = dt1.Rows[i]["UseType"].ToString();
                        string road_type = dt.Rows[0]["StreetType"].ToString();
                        string construction_type = dt1.Rows[i]["ContructionType"].ToString();
                        string occupancy_type = dt1.Rows[i]["OccupancyType"].ToString();
                        string usage_type = dt1.Rows[i]["UsageType"].ToString();
                        //string municipality_type = "Municipal Corporation";

                        string amut = arv_flat.calculate_ARV_flat(builtup_area, use_type, road_type, construction_type, occupancy_type, usage_type,PID,ass_year);
                        //string amut = arv_building_land.calculate_ARV_house_and_building(total_area, builtup_area, use_type, road_type, construction_type, occupancy_type, usage_type, municipality_type);
                        arv_amount += Convert.ToDouble(amut);
                        i++;
                    }
                }
            }
            due = arv_amount.ToString();
        }
        else if (property_type == "Vacant Land")
        {
            DataTable dt = chdu.find_vaccant_land(PID);
            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    arv_amount += Convert.ToDouble(arv_vaccant.calculate_arv_for_vaccant_land(dt.Rows[i]["PlotArea"].ToString(), "Municipal Corporation", dt.Rows[i]["StreetType"].ToString(),PID,ass_year));
                    i++;
                }
            }
            due = arv_amount.ToString();
        }
        double r_due = Convert.ToDouble(due);
        //int round_due = Convert.ToInt16(r_due);
        //due = arv_amount.ToString();
        //due = round_due.ToString();
        due = r_due.ToString();
        payment_for_property_tax = due.ToString();
        // gcverma
        return payment_for_property_tax;
    }


    public string calculate_due_payment_vaccant_land(string PID, string property_type, string id, string ass_year,string vaccant_area)
    {
        holding_updations hu = new holding_updations();
        calculate_holding_deu_details chdu = new calculate_holding_deu_details();
        ARV_for_vaccant_land arv_vaccant = new ARV_for_vaccant_land();
        ARV_for_Land_and_building arv_building_land = new ARV_for_Land_and_building();
        ARV_calculation_for_flat arv_flat = new ARV_calculation_for_flat();

        string payment_for_property_tax = "";


        string due = "0";
        double arv_amount = 0;
        double plot_area = 0;
        double constructed_area = 0;
        double calculated_area = 0;

        string tota = "0";
        string builta = "0";
        string rr_t = "";
        if (property_type == "Land + Building")
        {
            DataTable dt = chdu.find_vaccant_land(PID);
            if (dt.Rows.Count > 0)
            {
                plot_area = Convert.ToDouble(dt.Rows[0]["PlotArea"].ToString());
                constructed_area = Convert.ToDouble(dt.Rows[0]["ConstructedArea"].ToString());
                calculated_area = (plot_area - constructed_area);

                DataTable dt1 = hu.is_property_available_flat_and_house_id_wise(PID, id);
                int i = 0;
                if (dt1.Rows.Count > 0)
                {
                    while (i < dt1.Rows.Count)
                    {
                        string total_area = dt.Rows[0]["PlotArea"].ToString();
                        string builtup_area = dt1.Rows[i]["BuiltupArea"].ToString();
                        string use_type = dt1.Rows[i]["UseType"].ToString();
                        string road_type = dt.Rows[0]["StreetType"].ToString();
                        string construction_type = dt1.Rows[i]["ContructionType"].ToString();
                        string occupancy_type = dt1.Rows[i]["OccupancyType"].ToString();
                        string usage_type= dt1.Rows[i]["UsageType"].ToString();
                        string municipality_type = "Municipal Corporation";
                        string floor_no = dt1.Rows[i]["FloorNo"].ToString();

                        if (i == 0)
                        {
                            tota = total_area.ToString();
                            builta = builtup_area.ToString();
                            rr_t = road_type.ToString();
                        }

                        string amut = arv_building_land.calculate_ARV_house_and_building(total_area, builtup_area, use_type, road_type, construction_type, occupancy_type, usage_type, municipality_type, PID, ass_year);
                        arv_amount += Convert.ToDouble(amut);

                        double tot_area = Convert.ToDouble(tota);
                        double bup_area = Convert.ToDouble(builta);
                        double perc = ((bup_area / tot_area) * 100);

                        if (perc < 70)
                        {
                            double a = Convert.ToDouble(tota);
                            double b = Convert.ToDouble(builta);
                            double c = a - (b * 1.43);

                            if (floor_no == "Ground Floor")
                            {

                                ////taxable_vaccant_land = ((Convert.ToDouble(total_area) - (Convert.ToDouble(builtup_area)* 1.43))).ToString();
                                //string taxable_vaccant_land = c.ToString();
                                //ARV_for_vaccant_land vc_land = new ARV_for_vaccant_land();

                                //string amount_2 = vc_land.calculate_arv_for_vaccant_land(taxable_vaccant_land, "Municipal Corporation", rr_t, PID, ass_year);
                                ////amount = (amount_1 + amount_2);

                                //double am1 = Convert.ToDouble(arv_amount);
                                //double am2 = Convert.ToDouble(amount_2);

                                //double total_paid_amount = (am1 + am2);
                                //string amount = total_paid_amount.ToString();
                                //arv_amount = Convert.ToDouble(amount);
                            }
                        }
                        i++;
                    }
                }
            }
            //int round_due = Convert.ToInt32(arv_amount);
            due = arv_amount.ToString();
            //due = round_due.ToString();
        }
        else if (property_type == "Flat")
        {
            DataTable dt = chdu.find_vaccant_land(PID);
            if (dt.Rows.Count > 0)
            {
                DataTable dt1 = hu.is_property_available_flat_and_house_id_wise(PID, id);
                int i = 0;
                if (dt1.Rows.Count > 0)
                {
                    while (i < dt1.Rows.Count)
                    {
                        //string total_area = dt.Rows[0]["PlotArea"].ToString();
                        string builtup_area = dt1.Rows[i]["BuiltupArea"].ToString();
                        string use_type = dt1.Rows[i]["UseType"].ToString();
                        string road_type = dt.Rows[0]["StreetType"].ToString();
                        string construction_type = dt1.Rows[i]["ContructionType"].ToString();
                        string occupancy_type = dt1.Rows[i]["OccupancyType"].ToString();
                        string usage_type = dt1.Rows[i]["UsageType"].ToString();
                        //string municipality_type = "Municipal Corporation";

                        string amut = arv_flat.calculate_ARV_flat(builtup_area, use_type, road_type, construction_type, occupancy_type, usage_type, PID, ass_year);
                        //string amut = arv_building_land.calculate_ARV_house_and_building(total_area, builtup_area, use_type, road_type, construction_type, occupancy_type, usage_type, municipality_type);
                        arv_amount += Convert.ToDouble(amut);
                        i++;
                    }
                }
            }
            due = arv_amount.ToString();
        }
        else if (property_type == "Vacant Land")
        {
            DataTable dt = chdu.find_vaccant_land(PID);
            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    arv_amount += Convert.ToDouble(arv_vaccant.calculate_arv_for_vaccant_land(vaccant_area, "Municipal Corporation", dt.Rows[i]["StreetType"].ToString(), PID, ass_year));
                    i++;
                }
            }
            due = arv_amount.ToString();
        }
        double r_due = Convert.ToDouble(due);
        //int round_due = Convert.ToInt32(r_due);
        //due = arv_amount.ToString();
        due = r_due.ToString();
        payment_for_property_tax = due.ToString();
        return payment_for_property_tax;
    }
}