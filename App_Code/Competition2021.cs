using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Competition2021
/// </summary>
public class Competition2021
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    // check if he is allready registered
    public int if_data_available(string type, string mobile_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select id,type,mobile_no from participants_2021 where type=@type and mobile_no=@mobile_no Order by id DESC", con);
            cmd.Parameters.AddWithValue("@mobile_no", mobile_no);
            cmd.Parameters.AddWithValue("@type", type);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                number = 1;
            }
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
        return number;
    }

    // for jingle competition
    public int insert_new_jingle_competition(string type, string name, string father_name, string address, string contact_no, string alt_contact_no, string email_id, string id_proof_type, string id_no, string id_content_type, byte[] id_data, string user_content_type, byte[] user_data, string mp3_content_type, byte[] mp3_data)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_new_jingle_competition_2021"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@father_name", father_name);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@alt_contact_no", alt_contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@id_proof_type", id_proof_type);
            cmd.Parameters.AddWithValue("@id_no", id_no);
            cmd.Parameters.AddWithValue("@id_content_type", id_content_type);
            cmd.Parameters.AddWithValue("@id_data", id_data);

            cmd.Parameters.AddWithValue("@user_content_type", user_content_type);
            cmd.Parameters.AddWithValue("@user_data", user_data);

            cmd.Parameters.AddWithValue("@mp3_content_type", mp3_content_type);
            cmd.Parameters.AddWithValue("@mp3_data", mp3_data);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now);
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
            return number;
        }
    }

    // for movie making
    public int insert_new_movie_making_competition(string type, string name, string father_name, string address, string contact_no, string alt_contact_no, string email_id, string id_proof_type, string id_no, string id_content_type, byte[] id_data, string user_content_type, byte[] user_data, string youtube_url)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_new_movie_making_competition_2021"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@father_name", father_name);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@alt_contact_no", alt_contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@id_proof_type", id_proof_type);

            cmd.Parameters.AddWithValue("@id_no", id_no);
            cmd.Parameters.AddWithValue("@id_content_type", id_content_type);
            cmd.Parameters.AddWithValue("@id_data", id_data);

            cmd.Parameters.AddWithValue("@user_content_type", user_content_type);
            cmd.Parameters.AddWithValue("@user_data", user_data);

            cmd.Parameters.AddWithValue("@youtube_url", youtube_url);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now);
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
            return number;
        }
    }

    // for poster making
    public int insert_new_poster_making_competition(string type, string name, string father_name, string address, string contact_no, string alt_contact_no, string email_id, string id_proof_type, string id_no, string id_content_type, byte[] id_data, string user_content_type, byte[] user_data, string poster_content_type, byte[] poster_data, string writeup_content_type, byte[] writeup_data)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_new_poster_making_competition_2021"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@father_name", father_name);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@alt_contact_no", alt_contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@id_proof_type", id_proof_type);

            cmd.Parameters.AddWithValue("@id_no", id_no);
            cmd.Parameters.AddWithValue("@id_content_type", id_content_type);
            cmd.Parameters.AddWithValue("@id_data", id_data);

            cmd.Parameters.AddWithValue("@user_content_type", user_content_type);
            cmd.Parameters.AddWithValue("@user_data", user_data);

            cmd.Parameters.AddWithValue("@poster_content_type", poster_content_type);
            cmd.Parameters.AddWithValue("@poster_data", poster_data);
            cmd.Parameters.AddWithValue("@writeup_content_type", writeup_content_type);
            cmd.Parameters.AddWithValue("@writeup_data", writeup_data);


            cmd.Parameters.AddWithValue("@since", System.DateTime.Now);
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
            return number;
        }
    }

    // for street art 
    public int insert_new_street_art_making_competition(string type, string name, string father_name, string address, string contact_no, string alt_contact_no, string email_id, string id_proof_type, string id_no, string id_content_type, byte[] id_data, string user_content_type, byte[] user_data, string art_content_type, byte[] art_data, string member_1, string member_2, string member_3)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_new_street_art_making_competition_2021"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@father_name", father_name);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@alt_contact_no", alt_contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@id_proof_type", id_proof_type);

            cmd.Parameters.AddWithValue("@id_no", id_no);
            cmd.Parameters.AddWithValue("@id_content_type", id_content_type);
            cmd.Parameters.AddWithValue("@id_data", id_data);

            cmd.Parameters.AddWithValue("@user_content_type", user_content_type);
            cmd.Parameters.AddWithValue("@user_data", user_data);

            cmd.Parameters.AddWithValue("@art_content_type", art_content_type);
            cmd.Parameters.AddWithValue("@art_data", art_data);
            //cmd.Parameters.AddWithValue("@writeup_content_type", writeup_content_type);
            //cmd.Parameters.AddWithValue("@writeup_data", writeup_data);


            cmd.Parameters.AddWithValue("@member_1", member_1);
            cmd.Parameters.AddWithValue("@member_2", member_2);
            cmd.Parameters.AddWithValue("@member_3", member_3);

            cmd.Parameters.AddWithValue("@since", System.DateTime.Now);
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
            return number;
        }
    }

    // for street play
    // for street art 
    public int insert_new_street_play_competition(string type, string name, string team_name, string address, string contact_no, string alt_contact_no, string email_id, string id_proof_type, string id_no, string id_content_type, byte[] id_data, string user_content_type, byte[] user_data, string member_1, string member_2, string member_3, string member_4, string member_5)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_new_street_play_competition_2021"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@team_name", team_name);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@contact_no", contact_no);
            cmd.Parameters.AddWithValue("@alt_contact_no", alt_contact_no);
            cmd.Parameters.AddWithValue("@email_id", email_id);
            cmd.Parameters.AddWithValue("@id_proof_type", id_proof_type);

            cmd.Parameters.AddWithValue("@id_no", id_no);
            cmd.Parameters.AddWithValue("@id_content_type", id_content_type);
            cmd.Parameters.AddWithValue("@id_data", id_data);

            cmd.Parameters.AddWithValue("@user_content_type", user_content_type);
            cmd.Parameters.AddWithValue("@user_data", user_data);


            cmd.Parameters.AddWithValue("@member_1", member_1);
            cmd.Parameters.AddWithValue("@member_2", member_2);
            cmd.Parameters.AddWithValue("@member_3", member_3);

            cmd.Parameters.AddWithValue("@member_4", member_4);
            cmd.Parameters.AddWithValue("@member_5", member_5);

            cmd.Parameters.AddWithValue("@since", System.DateTime.Now);
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
            return number;
        }
    }
	 // for Common for others Participants
    public int insert_into_Common(string TypeId, string OrgName, string PostalAddr, string EmailID, string NodalName, string ContactNo)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_new_Common_competition_2021"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TypeId", TypeId);
            cmd.Parameters.AddWithValue("@OrgName", OrgName);
            cmd.Parameters.AddWithValue("@PostalAddr", PostalAddr);
            cmd.Parameters.AddWithValue("@EmailId", EmailID);
            cmd.Parameters.AddWithValue("@NodalName", NodalName);
            cmd.Parameters.AddWithValue("@ContactNo", ContactNo);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now);
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
            return number;
        }
    }
}