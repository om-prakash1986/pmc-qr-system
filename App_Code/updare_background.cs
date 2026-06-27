using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for updare_background
/// </summary>
public class updare_background
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    /*************************************************
   Author : Amrita Singh
   Date :  09-09-2108
   Purpose : Binding All mainmenu(updatemainmenu.aspx)
   *************************************************/
    public int update_mainmenu_background_image(string menu_id, string path)
    {
        int number = 1;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update category set back_image =@product_name WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@product_name", HttpUtility.HtmlEncode(path));
            cmd1.Parameters.AddWithValue("@id", HttpUtility.HtmlEncode(menu_id));
            cmd1.ExecuteNonQuery();
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
    /*************************************************
  Author : Amrita Singh
  Date :  09-09-2108
  Purpose : Binding All updatemainmenu(updatemainmenu.aspx)
  *************************************************/
    public int update_mainmenu_bg_image_path(string menu_id, string path)
    {
        int number = 1;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update category set bg_image_path =@product_name WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@product_name", HttpUtility.HtmlEncode(path));
            cmd1.Parameters.AddWithValue("@id", HttpUtility.HtmlEncode(menu_id));
            cmd1.ExecuteNonQuery();
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
    /*************************************************
    Author : Amrita Singh
    Date :  06-09-2108
    Purpose : Binding All updatesubmenuone(updatesubmenuone.aspx)
    *************************************************/
    public int update_submenuone_background_image(string menu_id,string path)
    {
        int number = 1;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update category_1 set back_image =@product_name WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@product_name", HttpUtility.HtmlEncode(path));
            cmd1.Parameters.AddWithValue("@id", HttpUtility.HtmlEncode(menu_id));
            cmd1.ExecuteNonQuery();
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
    /*************************************************
  Author : Amrita Singh
  Date :  06-09-2108
  Purpose : Binding All updatesubmenuone(updatesubmenuone.aspx)
  *************************************************/
    public int update_submenuone_bg_image_path(string menu_id, string path)
    {
        int number = 1;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update category_1 set bg_image_path =@product_name WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@product_name", HttpUtility.HtmlEncode(path));
            cmd1.Parameters.AddWithValue("@id", HttpUtility.HtmlEncode(menu_id));
            cmd1.ExecuteNonQuery();
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
    /*************************************************
   Author : Amrita Singh
   Date :  06-09-2108
   Purpose : Binding All updatesubmenutwo(updatesubmenutwo.aspx)
   *************************************************/
    public int update_submenutwo_background_image(string menu_id, string path)
    {
        int number = 1;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update category_2 set back_image =@product_name WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@product_name", HttpUtility.HtmlEncode(path));
            cmd1.Parameters.AddWithValue("@id", HttpUtility.HtmlEncode(menu_id));
            cmd1.ExecuteNonQuery();
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
    /*************************************************
  Author : Amrita Singh
  Date :  06-09-2108
  Purpose : Binding All updatesubmenutwo(updatesubmenutwo.aspx)
  *************************************************/
    public int update_submenutwo_bg_image_path(string menu_id, string path)
    {
        int number = 1;

        SqlConnection con = new SqlConnection(strcon);
        try
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("update category_2 set bg_image_path =@product_name WHERE id = @id", con);
            cmd1.Parameters.AddWithValue("@product_name", HttpUtility.HtmlEncode(path));
            cmd1.Parameters.AddWithValue("@id", HttpUtility.HtmlEncode(menu_id));
            cmd1.ExecuteNonQuery();
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