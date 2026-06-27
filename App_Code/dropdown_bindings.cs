using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


/// <summary>
/// Summary description for dropdown_bindings
/// </summary>
public class dropdown_bindings
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Binding All section data
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public DataSet bind_available_main_menu()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM category WHERE status='active' ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    /************************************************************
    * Purpose  ::  Binding All sub menu one data
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataSet bind_available_sub_menu(string menu_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM category_1 WHERE status='active' and category_id=@menu_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@menu_id",menu_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    /************************************************************
    * Purpose  ::  Binding All sub menu data Two
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataSet bind_available_sub_menu_two(string menu_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM category_2 WHERE status='active' and category_1_id=@menu_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@menu_id", menu_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    /************************************************************
    * Purpose  ::  Binding All sub menu data Three
    * Author   ::  Amrita
    * Date     ::  20-06-2018
    * ********************************************************/
    public DataSet bind_available_sub_menu_three(string menu_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM category_3 WHERE status='active' and category_2_id=@menu_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@menu_id", menu_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }
    /************************************************************
   * Purpose  ::  Binding All sub menu data four
   * Author   ::  Amrita
   * Date     ::  20-06-2018
   * ********************************************************/
    public DataSet bind_available_sub_menu_four(string menu_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM category_4 WHERE status='active' and category_3_id=@menu_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@menu_id", menu_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }
    /************************************************************
   * Purpose  ::  Binding All sub menu data five
   * Author   ::  Amrita
   * Date     ::  20-06-2018
   * ********************************************************/
    public DataSet bind_available_sub_menu_five(string menu_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM category_5 WHERE status='active' and category_4_id=@menu_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@menu_id", menu_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }
    /************************************************************
 * Purpose  ::  Binding All sub menu data six
 * Author   ::  Amrita
 * Date     ::  20-06-2018
 * ********************************************************/
    public DataSet bind_available_sub_menu_six(string menu_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM category_6 WHERE status='active' and category_5_id=@menu_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@menu_id", menu_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }



    /************************************************************
* Purpose  ::  Binding All cambate cell data of main category data
* Author   ::  Amrita Singh
* Date     ::  16-09-2018
* ********************************************************/
    public DataSet bind_combatcell_main_category()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select id,combatcatagory from CombatCategory where status='active' order by combatcatagory asc";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            da.Fill(ds, "partner");
            con.Close();
        }
        return ds;
    }

    /************************************************************
    * Purpose  ::  Binding All cambate cell data of sub category data
    * Author   ::  Amrita Singh
    * Date     ::  16-09-2018
    * ********************************************************/
    public DataSet bind_combatcell_sub_category(string category_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        string query = "select id,SubcatagoryName from CombatCellSubcategory where status='1' and categoryid=@category_id order by SubcatagoryName asc";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@category_id", category_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            da.Fill(ds, "partner");
            con.Close();
        }
        return ds;
    }

	public DataSet bind_all_states()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,countryid FROM statemaster where countryid=@country_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@country_id", "101");
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_districts_in_states(string state_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,stateid FROM citymaster where stateid=@state_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@state_id", state_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }
   
    public DataSet bind_available_states()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM states where countryid='101' ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_districts_states(string state_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,stateid FROM new_district where stateid=@state_id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@state_id", state_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }
    //added by amrita (19-01-2019)
    public DataSet bind_available_constituency_name()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,c_name FROM constituency_name ORDER BY c_name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    //added by amrita (19-01-2019)
    public DataSet bind_available_constituency_name_hindi()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,c_name FROM constituency_name_h ORDER BY c_name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    // bind all qualification
    public DataSet bind_all_qualification()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select id,qualification,qualification_h,since from qualification where status='active' order by id asc";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    // bind unique occupation
    public DataSet bind_all_occupation()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select id,occupation,occupation_h,since from occupation where status='active' order by id asc";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_districts_in_bihar()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,district_name,district_name_h FROM district ORDER BY district_name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_constituency_in_district(string district_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "select id,district_id,vidhan_sabha_e,vidhan_sabha_h from district_wise_vidhan_sabha_constituency where district_id=@id order by vidhan_sabha_e asc ";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id",district_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_states_new()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,name_h FROM state_list order by name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_states_new_for_volunteer()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,name_h FROM state_list where id=1 order by name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_districts_in_bihar_new(string state_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,district_name,district_name_h FROM district WHERE state_id=@id ORDER BY district_name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", state_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_blocks_in_district(string district_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,name_h FROM block_list WHERE district_id=@id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", district_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_panchyat_in_block(string district_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,name_h FROM panchyat_list WHERE block_id=@id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", district_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_available_ulb_in_district(string district_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,name_h FROM ulb_list WHERE district_id=@id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", district_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    //Added By amrita Singh(12-05-2020)
    public DataSet bind_country_for_advisory()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM CountryMaster order by name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_state_city_for_advisory(string country_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM StateMaster WHERE CountryID=@id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", country_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    public DataSet bind_citymaster_for_advisory(string statee_id)
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name FROM CityMaster WHERE StateID=@id ORDER BY name ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", statee_id);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }
    public DataSet bind_domain_name_for_advisory()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT id,name,name_h FROM domain_name where status='active' order by id ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }

    //added by amrita (23-06-2020)
    public DataSet bind_vidhan_sabha_constituency_name()
    {
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        con.Open();
        string query = "SELECT * FROM district_wise_vidhan_sabha_constituency ORDER BY id ASC";
        SqlCommand cmd = new SqlCommand(query, con);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            da.Fill(ds, "Partner");
        con.Close();
        return ds;
    }
    /**********************************************************
     * Till Here
     * ********************************************************/
}