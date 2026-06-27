using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
public class CombatCellForm
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    //Added By Amrita Singh(16-09-2018)
    public int insert_combatcell_form(string full_name,string email_id,string mob_no,string main_cate,string sub_cate,string areaa,string msg,string latitude, string langitude,DataTable dtimage)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_Title_FrontWeb"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", 0);
            cmd.Parameters["@id"].Direction = ParameterDirection.InputOutput;
            cmd.Parameters.AddWithValue("@ComplainNo", "");
            cmd.Parameters.AddWithValue("@added_by", "5");
            cmd.Parameters.AddWithValue("@title", msg);
            cmd.Parameters.AddWithValue("@categoryid", main_cate);
            cmd.Parameters.AddWithValue("@subcategoryId", sub_cate);
            cmd.Parameters.AddWithValue("@Latitude", latitude);
            cmd.Parameters.AddWithValue("@Longitude", langitude);
            cmd.Parameters.AddWithValue("@Area", areaa);
            cmd.Parameters.AddWithValue("@name", full_name);
            cmd.Parameters.AddWithValue("@MobileNo", mob_no);
            cmd.Parameters.AddWithValue("@Emailid", email_id);
            cmd.Parameters.AddWithValue("@Mode", 3);
            cmd.Parameters.AddWithValue("@ComplaintDate", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@status", "true");
            cmd.Parameters.AddWithValue("@iid", "5");
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                int returnid = Convert.ToInt32(cmd.Parameters["@id"].Value);
                SqlConnection con1 = new SqlConnection(strcon);
                foreach (DataRow dr in dtimage.Rows)
                {
                    using (SqlCommand cmd1 = new SqlCommand("Insert_Combat_Cell_Image"))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;

                        cmd1.Parameters.AddWithValue("@combatcelltitleid", returnid);
                        cmd1.Parameters.AddWithValue("@Attachement", dr["Image_data"]);
                        cmd1.Parameters.AddWithValue("@attatchmenttype", dr["content_type"]);
                        cmd1.Parameters.AddWithValue("@status", true);
                        cmd1.Connection = con1;
                        con1.Open();
                        cmd1.ExecuteNonQuery();
                        con1.Close();

                    }
                }

                con.Close();
                number = 1;

                con.Close();
                number = 1;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();

            }
        }
            return number;
    }


    public int insert_combatcell_GetSwachta(string title, string full_name, string email_id, string mob_no, string main_cate, string sub_cate, string areaa, string msg, string latitude, string langitude, string complaintLocation, string complaintLandmark, string GenericId)
    {
        int number = 0;
        if (full_name == "" || full_name == null)
        {
            full_name = "Na";
        }

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_Title_App_Swachta"))
        {
           
            cmd.CommandType = CommandType.StoredProcedure;
           
            cmd.Parameters.AddWithValue("@ComplainNo", "");
            cmd.Parameters.AddWithValue("@added_by", "5");
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@categoryid", 1);
            cmd.Parameters.AddWithValue("@subcategoryId", sub_cate);
            cmd.Parameters.AddWithValue("@Latitude", latitude);
            cmd.Parameters.AddWithValue("@Longitude", langitude);
            cmd.Parameters.AddWithValue("@Area", areaa);
            cmd.Parameters.AddWithValue("@name", full_name);
            cmd.Parameters.AddWithValue("@MobileNo", mob_no);
            cmd.Parameters.AddWithValue("@Emailid", email_id);
            cmd.Parameters.AddWithValue("@complaintLocation", complaintLocation);
            cmd.Parameters.AddWithValue("@complaintLandmark", complaintLandmark);
            cmd.Parameters.AddWithValue("@GenericId", GenericId);
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                number = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();

            }
        }
        return number;
    }


    

}