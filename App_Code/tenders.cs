using System;
using System.Collections.Generic;

using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for tenders
/// </summary>
public class tenders
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    /************************************************************
     * Purpose  ::  Find IF NIT Number Exists
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public int check_nit_no(string nit_no)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select id,nit_no from tenders where nit_no=@id", con);
            cmd.Parameters.AddWithValue("@id", nit_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    number = 1;
                    i++;
                }
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

    /************************************************************
     * Purpose  ::  Insert New Tender Details
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public string insert_new_tender_details(string nit_no,string title,string description,string start_date,string time,string end_date,byte[] bytes, string content_type, string end_time,string pre_date,string pre_time,string fin_date,string fin_time)
    {
        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_tender"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());

            cmd.Parameters.AddWithValue("@nit_no",nit_no);
            cmd.Parameters.AddWithValue("@title", title);

            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);

            cmd.Parameters.AddWithValue("@end_time", end_time);

            cmd.Parameters.AddWithValue("@data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);

            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));

            cmd.Parameters.AddWithValue("@pre_date", pre_date);
            cmd.Parameters.AddWithValue("@pre_time", pre_time);

            cmd.Parameters.AddWithValue("@tech_date", fin_date);
            cmd.Parameters.AddWithValue("@tech_time", fin_time);


            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToString(cmd.ExecuteScalar());
                con.Close();
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

    /************************************************************
     * Purpose  ::  Insert New Tender Details
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public int insert_new_tender_details_revised_data(string nit_no, string tender_id, string title, string description, string start_date, string time, string end_date, byte[] bytes, string content_type, string end_time, string pre_date, string pre_time, string fin_date, string fin_time)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_tender_revised_data"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@tender_id", tender_id);
            cmd.Parameters.AddWithValue("@nit_no", nit_no);
            cmd.Parameters.AddWithValue("@title", title);

            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);

            cmd.Parameters.AddWithValue("@end_time", end_time);

            cmd.Parameters.AddWithValue("@data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);

            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));

            cmd.Parameters.AddWithValue("@pre_date", pre_date);
            cmd.Parameters.AddWithValue("@pre_time", pre_time);

            cmd.Parameters.AddWithValue("@tech_date", fin_date);
            cmd.Parameters.AddWithValue("@tech_time", fin_time);

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

    /************************************************************
    * Purpose  ::  Find All Tender Details
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataTable All_tender_details(string value)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            if(value=="3")
            {
                SqlCommand cmd = new SqlCommand("Select id,nit_no,title,description,has_corrigendum,start_date,end_date,has_prebid_meeting,start_time,end_time,fin_date,fin_time,tech_date,tech_time from tenders order by end_date desc", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);     
            }
            else if(value =="2")
            {
                SqlCommand cmd = new SqlCommand("Select id,nit_no,title,description,has_corrigendum,start_date,end_date,has_prebid_meeting,start_time,end_time,fin_date,fin_time,tech_date,tech_time from tenders where end_date<=@date order by end_date desc", con);
                cmd.Parameters.AddWithValue("@date", Convert.ToDateTime(System.DateTime.Now).ToString("dd-MMM-yyyy"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt); 
            }
            else if(value =="1")
            {
                SqlCommand cmd = new SqlCommand("Select id,nit_no,title,description,has_corrigendum,start_date,end_date,has_prebid_meeting,start_time,end_time,fin_date,fin_time,tech_date,tech_time from tenders where end_date>=@date order by end_date desc", con);
                cmd.Parameters.AddWithValue("@date",Convert.ToDateTime(System.DateTime.Now).ToString("dd-MMM-yyyy"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt); 
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
        return dt;
    }

    public DataTable All_tender_details_single(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select id,nit_no,title,description,has_corrigendum,start_date,end_date,has_prebid_meeting,start_time,end_time,fin_date,fin_time,tech_date,tech_time from tenders where id=@id order by end_date desc", con);
            cmd.Parameters.AddWithValue("@id",id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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
        return dt;
    }

    /************************************************************
    * Purpose  ::  Find All Tender Details specific
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataTable All_tender_details_specific(string tender_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select id,nit_no,title,description,has_corrigendum,start_date,end_date,has_prebid_meeting,start_time from tenders where id=@id order by id desc", con);
            cmd.Parameters.AddWithValue("@id",tender_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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

        return dt;
    }

    /************************************************************
     * Purpose  ::  Insert New Tender Details UPDATE
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public int insert_new_tender_details_update(string tender_id,string nit_no, string title, string description, string start_date, string time, string end_date, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_tender_update"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());

            cmd.Parameters.AddWithValue("@nit_no", nit_no);
            cmd.Parameters.AddWithValue("@title", title);

            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);

            cmd.Parameters.AddWithValue("@data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@id", tender_id);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
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
    /************************************************************
   * Purpose  ::  Update New Tender Details 
   * Author   ::  Amrita Singh
   * Date     ::  08-01-2019
   * ********************************************************/
    public int insert_new_tender_details_update_text(string tender_id, string nit_no, string title, string description, string start_date, string time, string end_date)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_tender_update_text"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());

            cmd.Parameters.AddWithValue("@nit_no", nit_no);
            cmd.Parameters.AddWithValue("@title", title);

            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);

            cmd.Parameters.AddWithValue("@id", tender_id);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
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
    /************************************************************
     * Purpose  ::  Insert New Tender Details UPDATE
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public int insert_new_tender_details_revised_data_update(string nit_no, string tender_id, string title, string description, string start_date, string time, string end_date, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_tender_revised_data_update"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@tender_id", tender_id);
            cmd.Parameters.AddWithValue("@nit_no", nit_no);
            cmd.Parameters.AddWithValue("@title", title);

            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);

            cmd.Parameters.AddWithValue("@data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);

            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
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

    /************************************************************
     * Purpose  ::  Insert New Correndrum
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public int insert_new_correndrum(string tender_id,string nit_no, string title, string description, string start_date, string time, string end_date, byte[] bytes, string content_type)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_correndrum"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@tender_id", tender_id);
            cmd.Parameters.AddWithValue("@nit_no", nit_no);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);
            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);
            cmd.Parameters.AddWithValue("@data", bytes);
            cmd.Parameters.AddWithValue("@content_type", content_type);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
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


    /************************************************************
    * Purpose  ::  Find All Tender Details
    * Author   ::  Arnav
    * Date     ::  27-03-2017
    * ********************************************************/
    public DataTable All_correndrum_details(string tender_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select id,tender_id,title,description,start_date,end_date,since from corrinderum order by id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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

        return dt;
    }



    /************************************************************
     * Purpose  ::  Insert New Tender Details
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public string insert_new_tender_details_url(string nit_no, string title, string description, string start_date, string time, string end_date,string end_time, string pre_date, string pre_time, string fin_date, string fin_time,string url)
    {
        string number = "0";
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_tender_url"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());

            cmd.Parameters.AddWithValue("@nit_no", nit_no);
            cmd.Parameters.AddWithValue("@title", title);

            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);

            cmd.Parameters.AddWithValue("@end_time", end_time);

            cmd.Parameters.AddWithValue("@url_data", url);
            //cmd.Parameters.AddWithValue("@content_type", content_type);

            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));

            cmd.Parameters.AddWithValue("@pre_date", pre_date);
            cmd.Parameters.AddWithValue("@pre_time", pre_time);

            cmd.Parameters.AddWithValue("@tech_date", fin_date);
            cmd.Parameters.AddWithValue("@tech_time", fin_time);


            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToString(cmd.ExecuteScalar());
                con.Close();
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

    /************************************************************
     * Purpose  ::  Insert New Tender Details
     * Author   ::  Arnav
     * Date     ::  27-03-2017
     * ********************************************************/
    public int insert_new_tender_details_revised_data_url(string nit_no, string tender_id, string title, string description, string start_date, string time, string end_date, string end_time, string pre_date, string pre_time, string fin_date, string fin_time,string url)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_new_tender_revised_data_url"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@tender_id", tender_id);
            cmd.Parameters.AddWithValue("@nit_no", nit_no);
            cmd.Parameters.AddWithValue("@title", title);

            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@end_date", end_date);

            cmd.Parameters.AddWithValue("@end_time", end_time);

            cmd.Parameters.AddWithValue("@url_data", url);
            //cmd.Parameters.AddWithValue("@content_type", content_type);

            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));

            cmd.Parameters.AddWithValue("@pre_date", pre_date);
            cmd.Parameters.AddWithValue("@pre_time", pre_time);

            cmd.Parameters.AddWithValue("@tech_date", fin_date);
            cmd.Parameters.AddWithValue("@tech_time", fin_time);

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




    public DataTable find_corrundrium(string tender_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id,tender_id,title,description,start_date,end_date from corrinderum where tender_id=@tender_id order by id desc", con);
            cmd.Parameters.AddWithValue("@tender_id", tender_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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

        return dt;
    }

    public DataTable All_tender_details()
    {
        throw new NotImplementedException();
    }






    /***********************************************************
     * till here
     * *********************************************************/
}