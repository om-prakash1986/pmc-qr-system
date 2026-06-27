using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/// <summary>
/// Created by Gyan Chand Verma
/// </summary>
public class Maurya_shop
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    #region Encrypt and Decrypt
    public string encrypt(string encryptString)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encryptString;
    }

    public string Decrypt(string cipherText)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            encryptor.Padding = PaddingMode.None;
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    #endregion

    #region Admin Side
    #region Maurya Block
    public int add_block(int id, string b_name, int _status)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_into_Maurya_block"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@b_name", b_name);
            cmd.Parameters.AddWithValue("@_status", _status);
            cmd.Parameters.AddWithValue("@since", DateTime.UtcNow);
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

    public DataTable fillMauryaBlock()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from MAURYA_BLOCK order by BLOCK_ID asc", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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

    public DataTable viewMauryaDetailsById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from MAURYA_BLOCK where BLOCK_ID = @id order by BLOCK_ID asc", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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

    public DataTable bind_status()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from shop_status order by id asc", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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
    #endregion

    #region Maurya Block Floor
    public DataTable bind_Maurya_Blocks()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select BLOCK_ID, BLOCK_NO from MAURYA_BLOCK where status=1 order by BLOCK_ID ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindMonth()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select Month_No, Month_Name from Default_Month where Status=1 order by id asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    public DataTable bindRoom()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkPRID, RoomNo from [PRDA].[PropertyRoom] where IsActive=1 and IsAlloted=0 and fkPGID=1 and fkPBID=1 and fkPFID = 1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindOfficeType()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Office_Type where Status=1 order by Office_id asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int add_floor(int id, string _floor, int b_id, int _status)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_into_Maurya_block_floor"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@_floor", _floor);
            cmd.Parameters.AddWithValue("@b_id", b_id);
            cmd.Parameters.AddWithValue("@_status", _status);
            cmd.Parameters.AddWithValue("@since", DateTime.UtcNow);
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

    public DataTable fillMauryaBlockfloor()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select mb.BLOCK_ID,BF.FLOOR_ID,BF.FLOOR_NO,MB.BLOCK_NO, BF.ADDED_BY, BF.STATUS, BF.SINCE from BLOCK_FLOOR BF, MAURYA_BLOCK MB WHERE BF.BLOCK_ID=MB.BLOCK_ID order by FLOOR_ID asc", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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

    public DataTable viewMauryaFloorDetailsById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from BLOCK_FLOOR where FLOOR_ID = @id order by FLOOR_ID asc", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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

    public DataTable find_id_by_Block(string block_id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("Select BLOCK_ID, BLOCK_NO from MAURYA_BLOCK where BLOCK_ID=@block_id", con);
            cmd.Parameters.AddWithValue("@block_id", block_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }
    #endregion

    #region Maurya Shop Details
    public DataTable bind_Maurya_floor(string _selected_shop_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select FLOOR_ID, FLOOR_NO from BLOCK_FLOOR where BLOCK_ID=@_selected_shop_id and status =1 order by FLOOR_ID ASC", con);
            cmd.Parameters.AddWithValue("@_selected_shop_id", _selected_shop_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bind_shop_mutation()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from SHOP_MUTATION order by MU_SHOP_ID asc", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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

    public int add_Shop_Details(int id, int s_block, int s_floor, string s_holder, string s_shopno, int n_of_shop, int s_lease, string d_avantan, string d_agreement, double s_area, int s_mutation, string s_remarks, int s_status, int _m_month, int _m_year, double _g_rent, string _f_year, int _occ_type)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insert_Into_Maurya_Shop"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@s_block", s_block);
            cmd.Parameters.AddWithValue("@s_floor", s_floor);
            cmd.Parameters.AddWithValue("@s_holder", s_holder);
            cmd.Parameters.AddWithValue("@s_shopno", s_shopno);
            cmd.Parameters.AddWithValue("@n_of_shop", n_of_shop);
            cmd.Parameters.AddWithValue("@s_lease", s_lease);
            cmd.Parameters.AddWithValue("@d_avantan", d_avantan);
            cmd.Parameters.AddWithValue("@d_agreement", d_agreement);
            cmd.Parameters.AddWithValue("@s_area", s_area);
            cmd.Parameters.AddWithValue("@s_mutation", s_mutation);
            cmd.Parameters.AddWithValue("@s_remarks", s_remarks);
            cmd.Parameters.AddWithValue("@s_status", s_status);
            cmd.Parameters.AddWithValue("@_m_month", _m_month);
            cmd.Parameters.AddWithValue("@_m_year", _m_year);
            cmd.Parameters.AddWithValue("@_g_rent", _g_rent);
            cmd.Parameters.AddWithValue("@_f_year", _f_year);
            cmd.Parameters.AddWithValue("@_occ_type", _occ_type);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["User_ID"].ToString());
            cmd.Parameters.AddWithValue("@since", DateTime.UtcNow);
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

    public DataTable bind_Maurya_Shops()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select sd.Shop_Id, sd.Shop_Holder_Name,sd.M_LastPaidYear, sd.Ground_raint, sd.Area_of_Shop, bf.FLOOR_NO, ot.Office_Details, sd.G_R_fin_year, sd.status, sd.since from Shop_Details sd, BLOCK_FLOOR bf, Office_Type ot where bf.FLOOR_ID=sd.Block_Floor and ot.Office_id=sd.Occupany_type order by sd.Shop_Id asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable viewShopDetailsById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from Shop_Details where Shop_Id = @id order by Shop_Id asc", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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

    public DataTable find_id_by_Floor(string floor_id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("Select FLOOR_ID, FLOOR_NO from BLOCK_FLOOR where FLOOR_ID= @floor_id", con);
            cmd.Parameters.AddWithValue("@floor_id", floor_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable find_id_by_shop_mutation(string mutation_id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("Select MU_SHOP_ID, CURRENT_STATUS from SHOP_MUTATION where MU_SHOP_ID= @mutation_id", con);
            cmd.Parameters.AddWithValue("@mutation_id", mutation_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable find_id_by_Status(string status_id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("Select status_id, status from shop_status where status_id= @status_id", con);
            cmd.Parameters.AddWithValue("@status_id", status_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable find_id_by_Month(string id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select Month_No,Month_Name from Default_Month where Month_No= @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable find_Month_by_Id(string id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select Month_No,Month_Name from Default_Month where Month_Name= @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable find_id_by_Shop_Type(string id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select * from Office_Type where Office_id= @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable find_id_by_rate(int id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("select * from Shop_Office_Rate where road_id= @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }
    #endregion
    #endregion

    #region User Side
    public DataTable bind_Maurya_Shop_No(string _selected_shop_id, string _selected_floor_id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("Select Shop_Id, Shop_No from Shop_Details where Maurya_Block=@_selected_shop_id and Block_Floor=@_selected_floor_id order by Shop_Id ASC", con);
            cmd.Parameters.AddWithValue("@_selected_shop_id", _selected_shop_id);
            cmd.Parameters.AddWithValue("@_selected_floor_id", _selected_floor_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable bind_Maurya_Shops_by_Id(string _U_Shop_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Shop_Details where Shop_Id=@_U_Shop_Id and status=1 order by Shop_Id desc", con);
            cmd.Parameters.AddWithValue("@_U_Shop_Id", _U_Shop_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    //Added by Gyan Chand Verma
    public DataTable bindShopCalculationDeatilsby_Id(string _U_Shop_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select top 1* from ShopCalculationDeatils where Shop_Id=@_U_Shop_Id and status='active' order by id desc", con);
            cmd.Parameters.AddWithValue("@_U_Shop_Id", _U_Shop_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // date 25-09-2020
    public DataTable bindShopCalculationDeatils(string _U_Shop_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select top 1(id) from ShopCalculationDeatils where Shop_Id=@_U_Shop_Id order by id desc", con);
            cmd.Parameters.AddWithValue("@_U_Shop_Id", _U_Shop_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindShopCalculationDeatilsusingPaymentcode(string paymentCode)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id from ShopCalculationDeatils where payment_Code= @_U_Shop_Id ", con);
            cmd.Parameters.AddWithValue("@_U_Shop_Id", paymentCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    #region Current Financial year
    public string Current_Fin_Year()
    {
        string curr_fin_year = "";
        if (DateTime.Now.Month > 3)
        {
            curr_fin_year = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Year + 1).ToString();
        }
        else
        {
            curr_fin_year = (DateTime.Now.Year - 1).ToString() + "-" + DateTime.Now.Year.ToString();
        }
        return curr_fin_year;
    }

    public DataTable Calculate_fin_year(string L_F_Y)
    {
        string L_fin_year = L_F_Y.Split('-')[0];
        int I_L_fin_year = Convert.ToInt32(L_fin_year);
        DataTable dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("Financial_Year");
        string gen_fin_year = "";
        for (int i = I_L_fin_year + 1; i <= DateTime.Now.Year; i++)
        {
            if (DateTime.Now.Month > 3)
            {
                gen_fin_year = i + "-" + (i + 1);
            }
            else
            {
                //Updated by Gyan Chand Verma dated 12-01-2021
                //gen_fin_year = (i - 1) + "-" + i;
                gen_fin_year = i + "-" + (i + 1);
            }
            dt.Rows.Add(gen_fin_year);
        }
        return dt;
    }
    #endregion

    #region Ground Rent Calculation and Maintenance
    public double Calculate_Interest_ground_Rent(double gr, double count)
    {
        double interest = 0.0;
        double rate = 10;
        interest = (gr * count * rate) / 100;
        return interest;
    }

    public double Calculate_Interest_Maintenance(double grMain, double count, int m, int y)
    {
        double interest = 0.0;
        double rate = 10;
        if (DateTime.Now.Month == m && DateTime.Now.Year == y && DateTime.Now.Day <= 7)
        {
            interest = 0.0;
        }
        else
        {
            interest = (grMain * count * rate) / 1200;// interest =(P*R*10)/(100*12)
        }
        return interest;
    }
    #endregion

    #region Maintainence
    public DataTable getMonthandYear(int lastpaidmonth, int lastpaidyear)
    {
        int month = DateTime.Now.Month;
        int year = DateTime.Now.Year;
        DataTable dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("month");
        dt.Columns.Add("year");
        int yearcount = year - lastpaidyear;

        for (int i = 0; i <= yearcount; i++)
        {
            for (int j = lastpaidmonth + 1; j <= 12; j++)
            {
                if (month == j && year == lastpaidyear)
                {
                    dt.Rows.Add(j, lastpaidyear);
                    break;
                }
                else
                {
                    dt.Rows.Add(j, lastpaidyear);
                }
            }
            lastpaidmonth = 0;
            lastpaidyear += 1;
        }
        return dt;
    }

    public double GroundRentCharge(int month, int year, double shop_area, double oldRate, double newRate)
    {
        double calcu = 0.0;
        double rate = 0.0;
        if (month >= 11 && year >= 2017)
        {
            rate = newRate;
            calcu = shop_area * rate;
        }
        else if (year > 2017)
        {
            rate = newRate;
            calcu = shop_area * rate;
        }
        else
        {
            rate = oldRate;
            calcu = shop_area * rate;
        }
        return calcu;
    }
    #endregion

    public int updateStatus(string _id, string _contact_No, string _checked)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update Shop_Details set Contact_No=@_contact_No, Disclaimer=@_checked where Shop_Id=@_id ", con);
            cmd.Parameters.AddWithValue("@_id", _id);
            cmd.Parameters.AddWithValue("@_contact_No", _contact_No);
            cmd.Parameters.AddWithValue("@_checked", _checked);
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
    #endregion

    //New addition by Gyan Chand Verma
    public int updateShopPayment(string _id)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update shops_payment set status='active',payment='OFFLINE',receive_date=@date Where payment_code=@application_no", con);
            cmd.Parameters.AddWithValue("@date", System.DateTime.Now);
            cmd.Parameters.AddWithValue("@application_no", _id);
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

    public int insertIntoBankDeatils(string _Shop_id, string _AppNO, string _drop1, string _drop2, string _number, string _drop3, string _drop4, string _address)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("insert into Bank_Details (fk_Uid,shop_Id, payment_code,payment_mode,payment_option,check_number,bank_name,branch_Address,collected_by, since) values(@session,@Shop_id,@AppNO,@drop1,@drop2,@number,@drop3,@address,@drop4,@date)", con);
            cmd.Parameters.AddWithValue("@Shop_id", _Shop_id);
            cmd.Parameters.AddWithValue("@AppNO", _AppNO);
            cmd.Parameters.AddWithValue("@drop1", _drop1);
            cmd.Parameters.AddWithValue("@drop2", _drop2);
            cmd.Parameters.AddWithValue("@number", _number);
            cmd.Parameters.AddWithValue("@drop3", _drop3);
            cmd.Parameters.AddWithValue("@drop4", _drop4);
            cmd.Parameters.AddWithValue("@address", _address);
            cmd.Parameters.AddWithValue("@session", HttpContext.Current.Session["holding_U_ID_admin"]);
            cmd.Parameters.AddWithValue("@date", System.DateTime.Now);
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

    public DataTable getShopPaymentData(string _AppNO)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, payment_code, receive_date, payment, amount from shops_payment where payment_code=@AppNO", con);
            cmd.Parameters.AddWithValue("@AppNO", _AppNO);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable getShopDetailsData(string _ShopId)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select bf.FLOOR_NO, mb.BLOCK_NO, sd.Shop_No, sd.Shop_Holder_Name, ot.Office_Details, sd.Contact_No, sd.M_LastPaidMonth, sd.M_LastPaidYear, sd.G_R_fin_year, sd.Occupany_type from Shop_Details sd, MAURYA_BLOCK mb, BLOCK_FLOOR bf, Office_Type ot where sd.Maurya_Block = mb.BLOCK_ID and sd.Block_Floor = bf.FLOOR_ID and sd.Occupany_type = ot.Office_id and sd.Shop_Id = @ShopId", con);
            cmd.Parameters.AddWithValue("@ShopId", _ShopId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable getShopCalculationDeatils(string _AppNO)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select L_M_Month, L_M_Year, L_P_GroundRent, C_M_Month, C_M_Year, C_P_GroundRent, A_GroundRent, A_I_GroundRent, C_GroundRent, C_Interest_GroundRent,T_GroundRent, A_Maintenance, A_I_Maintenance, C_Maintenance, C_Interest_Maintenance, T_Maintenance, Total_GR_M, CONVERT(VARCHAR(10), since, 103) AS ssince from ShopCalculationDeatils where payment_code = @AppNO and status = 'active'", con);
            cmd.Parameters.AddWithValue("@AppNO", _AppNO);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int InsertShopCalculationDeatils(string _id, string app_no, string LastMonth, string LastYear, string LastFin_Year, string A, string B, string C, string D, string E, string F, string G, string H, string I, string J, string K)
    {
        int number = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("insert into ShopCalculationDeatils(shop_Id, payment_code, L_M_Month, L_M_Year, L_P_GroundRent, A_GroundRent, A_I_GroundRent, C_GroundRent, C_Interest_GroundRent, T_GroundRent,A_Maintenance, A_I_Maintenance, C_Maintenance, C_Interest_Maintenance, T_Maintenance, Total_GR_M, status,date_Time, since) values(@Shop_id,@AppNO,@LastMonth,@LastYear,@LastFin_Year,@A,@B,@C,@D,@E,@F,@G,@H,@I,@J,@K,@status,@date_time,@date)", con);
            cmd.Parameters.AddWithValue("@Shop_id", _id);
            cmd.Parameters.AddWithValue("@AppNO", app_no);
            cmd.Parameters.AddWithValue("@LastMonth", LastMonth);
            cmd.Parameters.AddWithValue("@LastYear", LastYear);
            cmd.Parameters.AddWithValue("@LastFin_Year", LastFin_Year);
            cmd.Parameters.AddWithValue("@A", A);
            cmd.Parameters.AddWithValue("@B", B);
            cmd.Parameters.AddWithValue("@C", C);
            cmd.Parameters.AddWithValue("@D", D);
            cmd.Parameters.AddWithValue("@E", E);
            cmd.Parameters.AddWithValue("@F", F);
            cmd.Parameters.AddWithValue("@G", G);
            cmd.Parameters.AddWithValue("@H", H);
            cmd.Parameters.AddWithValue("@I", I);
            cmd.Parameters.AddWithValue("@J", J);
            cmd.Parameters.AddWithValue("@K", K);
            cmd.Parameters.AddWithValue("@status", "Inactive");
            cmd.Parameters.AddWithValue("@date_time", System.DateTime.Now);
            cmd.Parameters.AddWithValue("@date", System.DateTime.Now.ToString("dd-MMM-yyyy"));
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

    public DataTable BindMauryaLokShopData()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select sd.Shop_Id, sd.Shop_Holder_Name, sd.Area_of_Shop, mb.BLOCK_NO, bf.FLOOR_NO, sd.Shop_No, ot.Office_Details from Shop_Details sd, BLOCK_FLOOR bf, MAURYA_BLOCK mb, Office_Type ot where bf.FLOOR_ID = sd.Block_Floor and ot.Office_id = sd.Occupany_type and mb.block_id = bf.block_id", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    public DataTable BindShopLastPaymentDetailsById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select L_M_Month, L_M_year, L_P_GroundRent, C_M_Month, C_M_year, C_P_GroundRent, date_Time from ShopCalculationDeatils where Shop_Id = @id and status='active'", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
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
}
