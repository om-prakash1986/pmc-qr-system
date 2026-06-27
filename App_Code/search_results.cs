using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for search_results
/// </summary>
public class search_results
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
    public DataTable show_results_show(string data)
    {
        //or (a.dairy_no LIKE '%' + @hind_Search + '%') or (a.letter_no LIKE '%' + @Eng_search + '%')
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from combatcellTitle where (ComplainNo LIKE '%' + @data + '%') or (title LIKE '%' + @data + '%') or (name LIKE '%' + @data + '%') or (MobileNo LIKE '%' + @data + '%' ) or (EmailId LIKE '%' + @data + '%')", con);
            cmd.Parameters.AddWithValue("@data", data);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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
        return dt;
    }
}