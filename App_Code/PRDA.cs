using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/// <summary>
/// Created by Gyan Chand Verma
/// Dated 12:02:2021
/// Summary description for PRDA
/// </summary>
public class PRDA
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    #region Admin Side
    public int PRDAAdminLogin(string user_name, string password)
    {
        int number = 0;
        encryption en = new encryption();
        string passw = en.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spAdminLogin]"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@password", passw);
            cmd.Connection = con;
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["PRDALoginId"] = null;
                    HttpContext.Current.Session["PRDAUserName"] = null;
                    HttpContext.Current.Session["PRDAAdminAuthority"] = null;
                    HttpContext.Current.Session["PRDALoginId"] = dt.Rows[0]["pkPMCAL"].ToString();
                    HttpContext.Current.Session["PRDAUserName"] = dt.Rows[0]["UserName"].ToString();
                    HttpContext.Current.Session["PRDAAdminAuthority"] = dt.Rows[0]["fkPMCALA"].ToString();

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
        }
        return number;
    }

    public DataTable bind_Property_Group()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkPGID, PropertyGroupName from [PRDA].[PropertyGroup] where IsActive= 1 order by pkPGID ASC", con);
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

    #region Maurya Block Floor
    public DataTable bind_Maurya_Blocks(string pkPGID)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select [pkPBID],[fkPGID],[PropertyBlockName] from [PropertyBlock] where fkPGID=@pkPGID and IsActive=1 order by pkPBID asc", con);
            cmd.Parameters.AddWithValue("@pkPGID", pkPGID);
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
            SqlCommand cmd = new SqlCommand("select pkOTID, OccupancyType from [OccupancyType] where IsActive=1 order by pkOTID asc", con);
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
    #endregion

    #region Maurya Shop Details
    public DataTable bind_Maurya_floor(string _selected_shop_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkPFID, PropertyFloorName from [PropertyFloor] where fkPBID=@_selected_shop_id and IsActive =1 order by pkPFID ASC", con);
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
            SqlCommand cmd = new SqlCommand("select pkMSTID, StausType from [MutationStatusType] where IsActive=1 order by pkMSTID asc", con);
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

    /*_shops, _selected_shop_id, _selectedShopFloor, _occType, _allotedPRIDs, _noOfRooms, _allotedRooms,_allotedOn, _shopNO, _Agreement, _areaOfShop, _mMonth,
    _mYear, _grRent, _grFinYear, _lease, _mutation, _remarks, _shopHName, _gender, _UID,_address, _pinCode, _mobile, _emailId, _phone, _fax, _status, _created*/
    public int AddPRDAMasterData(string _shops, string _selected_shop_id, string _selectedShopFloor, string _occType, string _allotedPRIDs, int _noOfRooms,
    string _allotedRooms, string _allotedOn, string _shopNO, string _Agreement, string _areaOfShop, string _mMonth, string _mYear, string _grRent,
    string _grFinYear, string _lease, string _mutation, string _remarks, string _shopHName, string _gender, string _UID, string _address, string _pinCode,
    string _mobile, string _emailId, string _phone, string _fax, bool _status, int _created)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spSavePropertyData]"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PGID", _shops);
            cmd.Parameters.AddWithValue("@PBID", _selected_shop_id);
            cmd.Parameters.AddWithValue("@PFID", _selectedShopFloor);
            cmd.Parameters.AddWithValue("@OTID", _occType);
            cmd.Parameters.AddWithValue("@AllotedPRIDs", _allotedPRIDs);
            cmd.Parameters.AddWithValue("@NoOfRooms", _noOfRooms);
            cmd.Parameters.AddWithValue("@AllotedRooms", _allotedRooms);
            if (string.IsNullOrEmpty(_allotedOn) || string.IsNullOrWhiteSpace(_allotedOn))
            {
                cmd.Parameters.AddWithValue("@AllotedOn", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@AllotedOn", DateTime.Parse(_allotedOn.Trim()));
            }
            cmd.Parameters.AddWithValue("@ShopOfficeFlatNo", _shopNO);
            if (string.IsNullOrEmpty(_Agreement) || string.IsNullOrWhiteSpace(_Agreement))
            {
                cmd.Parameters.AddWithValue("@AgreementOn", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@AgreementOn", DateTime.Parse(_Agreement.Trim()));
            }

            cmd.Parameters.AddWithValue("@TotalAreaInSqFeet", _areaOfShop);
            cmd.Parameters.AddWithValue("@MCLastPaidMonth", _mMonth);
            cmd.Parameters.AddWithValue("@MCLastPaidYear", _mYear);
            cmd.Parameters.AddWithValue("@GroundRent", _grRent);
            cmd.Parameters.AddWithValue("@GRLastPaidFinYear", _grFinYear);
            cmd.Parameters.AddWithValue("@LeasePeriodInYear", _lease);
            cmd.Parameters.AddWithValue("@MutationStatus", _mutation);
            cmd.Parameters.AddWithValue("@Remarks", _remarks);
            cmd.Parameters.AddWithValue("@PropertyHolderName", _shopHName);
            cmd.Parameters.AddWithValue("@Gender", _gender);
            cmd.Parameters.AddWithValue("@UID", _UID);
            cmd.Parameters.AddWithValue("@Address", _address);
            cmd.Parameters.AddWithValue("@Pincode", _pinCode);
            cmd.Parameters.AddWithValue("@MobileNumber", _mobile);
            cmd.Parameters.AddWithValue("@EmailID", _emailId);
            cmd.Parameters.AddWithValue("@PhoneNumber", _phone);
            cmd.Parameters.AddWithValue("@FaxNumber", _fax);
            cmd.Parameters.AddWithValue("@IsActive", _status);
            cmd.Parameters.AddWithValue("@CreatedBy", _created);
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

    public DataTable bindRoom(string _shops, string _selected_shop_id, string _selectedShopFloor)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkPRID, RoomNo from [PropertyRoom] where IsActive=1 and IsAlloted=0 and fkPGID=@shops and fkPBID=@selected_shop_id and fkPFID = @selectedShopFloor", con);
            cmd.Parameters.AddWithValue("@shops", _shops);
            cmd.Parameters.AddWithValue("@selected_shop_id", _selected_shop_id);
            cmd.Parameters.AddWithValue("@selectedShopFloor", _selectedShopFloor);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindBankDetails()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [BankDetails] WHERE IsActive=1 ORDER BY pkBKID ASC", con);
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

    public int addBankName(int pkBKID, string BankName, string BankNameAbbr)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spBankDetails]"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BKID", pkBKID);
            cmd.Parameters.AddWithValue("@BankName", BankName);
            cmd.Parameters.AddWithValue("@BankNameAbbr", BankNameAbbr);
            cmd.Parameters.AddWithValue("@IsActive", 1);
            cmd.Parameters.AddWithValue("@CreatedBy", 1);
            cmd.Parameters.AddWithValue("@CreatedOn", System.DateTime.Now);
            cmd.Parameters.AddWithValue("@ModifiedBy", 1);
            cmd.Parameters.AddWithValue("@ModifiedOn", System.DateTime.Now);
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

    public DataTable viewBankNameById(int BKID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [BankDetails] WHERE pkBKID=@BKID ORDER BY pkBKID DESC", con);
            cmd.Parameters.AddWithValue("@BKID", BKID);
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

    public DataTable BindPendingPaymentData(string PaymentMode, string FromDate, string UptoDate, string status)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            string query = "SELECT PT.pkPTID, PT.fkPAID, PT.fkBTID, PT.BankName, PT.BankAddress, PT.DDChequeNo, PT.DDChequeDate, PT.PaymentMode, PT.Narration, PT.PaymentOrderId, PT.TxnAmount, PT.TxnRefNo, PT.PaymentOrderStatus, PT.PaymentStatus, PT.PaymentStatusDesc, PT.IsPaymentClearance, PT.ReceivedBy, PT.ClearanceOn, PT.PaymentOrderOn, PT.fkPAID, PT.AuthCode, PT.ResponceCode, PT.TxnOn, PH.MobileNumber, PG.PropertyGroupName, PB.PropertyBlockName, PF.PropertyFloorName, PA.ShopOfficeFlatNo, OT.OccupancyType FROM PaymentTransaction PT INNER JOIN PropertyAllotment PA ON PA.pkPAID=PT.fkPAID INNER JOIN PropertyHolder PH ON PA.fkPHID=PH.pkPHID INNER JOIN [PropertyGroup] PG ON PA.fkPGID=PG.pkPGID INNER JOIN [PropertyBlock] PB ON PA.fkPBID=PB.pkPBID INNER JOIN [PropertyFloor] PF ON PA.fkPFID=PF.pkPFID INNER JOIN [OccupancyType] OT ON PA.fkOTID=OT.pkOTID WHERE PT.IsPaymentClearance=@status ";
            string subQuery = "";

            if (PaymentMode != "ALL")
            {
                subQuery += " AND PT.PaymentMode=@PaymentMode";
            }
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(UptoDate))
            {
                subQuery += " AND PT.PaymentOrderOn BETWEEN @FromDate AND @UptoDate";
            }

            query += subQuery + " ORDER BY pkPTID DESC";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PaymentMode", PaymentMode);
            cmd.Parameters.AddWithValue("@status", status);
            if (!string.IsNullOrEmpty(FromDate))
            {
		cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(FromDate + " 00:00:00.000"));
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            }
            if (!string.IsNullOrEmpty(FromDate))
            {
		cmd.Parameters.AddWithValue("@UptoDate", Convert.ToDateTime(UptoDate + " 23:59:59.999"));
            }
            else
            {
                cmd.Parameters.AddWithValue("@UptoDate", DBNull.Value);
            }
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

    public DataTable BindLastPaymentBYPAID(string PAID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [MasterDataPRDA] WHERE fkPAID=@PAID ORDER BY pkMID DESC", con);
            cmd.Parameters.AddWithValue("@PAID", PAID);
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

    public int PaymentSettlement(string PAID, string FinancialYear, string Month, string Year)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("spUpdateLastGRAndMCPayment"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PAID", PAID);
            cmd.Parameters.AddWithValue("@GRLastPaidFinYear", FinancialYear);
            cmd.Parameters.AddWithValue("@MCLastPaidMonth", Month);
            cmd.Parameters.AddWithValue("@MCLastPaidYear", Year);
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

    public DataTable AdminBindDataByID(string Group, string Block, string Floor, string _ShopNo)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            string query = "SELECT PropertyHolder.PropertyHolderName, PropertyHolder.MobileNumber, PropertyHolder.EmailID, PropertyAllotment.AllotedOn, PropertyAllotment.AgreementOn, PropertyAllotment.TotalAreaInSqFeet, PropertyAllotment.GroundRent, PropertyAllotment.pkPAID FROM PropertyAllotment INNER JOIN PropertyHolder ON PropertyAllotment.fkPHID = PropertyHolder.pkPHID WHERE ";
            string Subquery = "";
            if (Group != "0" && Block == "0" && Floor == "0" && _ShopNo == "0")
            {
                Subquery = "PropertyAllotment.fkPGID = @Group";
            }
            else if (Group != "0" && Block != "0" && Floor == "0" && _ShopNo == "0")
            {
                Subquery = "(PropertyAllotment.fkPGID = @Group) AND(PropertyAllotment.fkPBID = @Block)";
            }
            else if (Group != "0" && Block != "0" && Floor != "0" && _ShopNo == "0")
            {
                Subquery = "(PropertyAllotment.fkPGID = @Group) AND (PropertyAllotment.fkPBID = @Block) AND (PRDA.PropertyAllotment.fkPFID = @Floor)";
            }
            else if (Group != "0" && Block != "0" && Floor != "0" && _ShopNo != "0")
            {
                Subquery = "(PropertyAllotment.fkPGID = @Group) AND (PropertyAllotment.fkPBID = @Block) AND (PRDA.PropertyAllotment.fkPFID = @Floor) AND (PRDA.PropertyAllotment.pkPAID = @_ShopNo)";
            }

            query = query + Subquery;

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Group", Group);
            cmd.Parameters.AddWithValue("@Block", Block);
            cmd.Parameters.AddWithValue("@Floor", Floor);
            cmd.Parameters.AddWithValue("@_ShopNo", _ShopNo);
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

    public int UpdateDOAgDOAl(DateTime txtDOAL, DateTime txtDOAR, string PAID)
    {
        int number = 0;
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        SqlCommand cmd = new SqlCommand("UPDATE PropertyAllotment SET AllotedOn=@txtDOAL, AgreementOn=@txtDOAR WHERE pkPAID=@PAID", con);
        cmd.Parameters.AddWithValue("@txtDOAL", txtDOAL);
        cmd.Parameters.AddWithValue("@txtDOAR", txtDOAR);
        cmd.Parameters.AddWithValue("@PAID", PAID);
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

    public int addAuthorityType(int id, string AuthorityType)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spPMCAdminLoginAuthority]"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PMCALA", id);
            cmd.Parameters.AddWithValue("@AuthoriseSection", AuthorityType);
            cmd.Parameters.AddWithValue("@IsActive", 1);
            cmd.Parameters.AddWithValue("@CreatedBy", 1);
            cmd.Parameters.AddWithValue("@CreatedOn", System.DateTime.Now);
            cmd.Parameters.AddWithValue("@ModifiedBy", 1);
            cmd.Parameters.AddWithValue("@ModifiedOn", System.DateTime.Now);
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

    public DataTable viewAuthorityTypeById(int PMCALA)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PMCAdminLoginAuthority WHERE pkPMCALA=@PMCALA ORDER BY pkPMCALA DESC", con);
            cmd.Parameters.AddWithValue("@PMCALA", PMCALA);
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

    public DataTable BindAuthorityType()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [PMCAdminLoginAuthority] WHERE IsActive=1 ORDER BY pkPMCALA ASC", con);
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

    public DataTable bind_Authority_Type()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkPMCALA, AuthoriseSection from PMCAdminLoginAuthority where IsActive= 1 order by pkPMCALA ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int ShopPaymentUpdateChequeandOnline(string PTID, string PAID, string BTID, string PaymentMode, string BankName, string DDChequeNo, string DDChequeDate, string Narration, string TxnAmount, string AuthCode, string ResponceCode, string TxnRefNo, string TxnOn, string PaymentStatus, string PaymentStatusDesc, string IsPaymentClearance, string ClearanceOn)
    {
        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "[spPaymentConfirmationForChequeAndOnline]";
        cmd.Connection = con;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@PTID", PTID);
        cmd.Parameters.AddWithValue("@PAID", PAID);
        cmd.Parameters.AddWithValue("@BTID", BTID);
        cmd.Parameters.AddWithValue("@PaymentMode", PaymentMode);
        cmd.Parameters.AddWithValue("@BankName", BankName);
        cmd.Parameters.AddWithValue("@DDChequeNo", DDChequeNo);
        cmd.Parameters.AddWithValue("@DDChequeDate", DBNull.Value);
        cmd.Parameters.AddWithValue("@Narration", Narration);
        if (TxnAmount != "")
        {
            cmd.Parameters.AddWithValue("@TxnAmount", TxnAmount);
        }
        else
        {
            cmd.Parameters.AddWithValue("@TxnAmount", 0);
        }
        cmd.Parameters.AddWithValue("@TxnRefNo", TxnRefNo);
        cmd.Parameters.AddWithValue("@AuthCode", AuthCode);
        cmd.Parameters.AddWithValue("@ResponceCode", ResponceCode);
        if (TxnOn == "")
        {
            cmd.Parameters.AddWithValue("@TxnOn", DBNull.Value);
        }
        else if (TxnOn == "NA")
        {
            cmd.Parameters.AddWithValue("@TxnOn", DBNull.Value);
        }
        else
        {
            cmd.Parameters.AddWithValue("@TxnOn", Convert.ToDateTime(TxnOn));
        }
        cmd.Parameters.AddWithValue("@PaymentStatus", PaymentStatus);
        cmd.Parameters.AddWithValue("@PaymentStatusDesc", PaymentStatusDesc);
        cmd.Parameters.AddWithValue("@IsPaymentClearance", IsPaymentClearance);
        if (ClearanceOn == "")
        {
            cmd.Parameters.AddWithValue("@ClearanceOn", DBNull.Value);
        }
        else if (ClearanceOn == "NA")
        {
            cmd.Parameters.AddWithValue("@ClearanceOn", DBNull.Value);
        }
        else
        {
            cmd.Parameters.AddWithValue("@ClearanceOn", Convert.ToDateTime(ClearanceOn));
        }
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

    public DataTable GetReceiptDetails(string PTID)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spGetPaymentReceipt]", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PTID", PTID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable GetPrdaDemand(string PAIDs)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spGetDemandOrder]", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PAIDs", PAIDs);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable GetDuesList()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spGetDuesList]", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable GetOnlinePendingTransations()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("DECLARE @MODIFIEDDATE DATETIME; " + " DECLARE @STARTDATE DATETIME; " + "SET @MODIFIEDDATE = GETDATE(); "
            + "SET @STARTDATE = DATEADD(D, -5, GETDATE()); " +
	"SELECT T.*,H.PropertyHolderName, H.MobileNumber FROM [PaymentTransaction] AS T INNER JOIN [PropertyAllotment] AS A  ON T.fkpaid=a.pkpaid INNER JOIN [PropertyHolder] AS H ON A.fkPHID= H.pkPHID WHERE PaymentMode = 'ONLINE' AND IsPaymentClearance = 0 and (PaymentStatus = 'PENDING' OR PaymentStatus = 'FAILURE') AND T.PaymentOrderOn BETWEEN @STARTDATE AND GETDATE();", con))
        {
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindPaymentReoprtData(string FromDate, string UptoDate)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spGetPaymentReceivedReport]", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(FromDate + " 00:00:00.000"));
            cmd.Parameters.AddWithValue("@UptoDate", Convert.ToDateTime(UptoDate + " 23:59:59.999"));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
    }
    #endregion
}
