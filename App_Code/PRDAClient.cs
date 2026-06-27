using System;
using System.Collections.Generic;

using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using com.awl.MerchantToolKit;

/// <summary>
/// Created by Gyan Chand Verma
/// Dated :- 18-03-2021
/// PRDA Clent Data Binding
/// </summary>
public class PRDAClient
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable bind_Property_Group()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkPGID, PropertyGroupName from [PropertyGroup] where IsActive= 1 order by pkPGID ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

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

    public DataTable bindRoom(string _Group, string _selected_Block_id, string _selectedFloorID)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkPAID, ShopOfficeFlatNo from [PropertyAllotment] where IsActive=1 and fkPGID=@Group and fkPBID=@selected_Block_id and fkPFID = @selectedFloorID", con);
            cmd.Parameters.AddWithValue("@Group", _Group);
            cmd.Parameters.AddWithValue("@selected_Block_id", _selected_Block_id);
            cmd.Parameters.AddWithValue("@selectedFloorID", _selectedFloorID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bind_Occupancy_Type()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select pkOTID, OccupancyType from OccupancyType where IsActive= 1 order by pkOTID ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindDataByID(string Group, string Block, string Floor, string _ShopNo, string OTID, string PropertyHolderName)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("[spPropertySearchForPayment]"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PGID", Group);
            cmd.Parameters.AddWithValue("@PBID", Block);
            cmd.Parameters.AddWithValue("@PFID", Floor);
            cmd.Parameters.AddWithValue("@OTID", OTID);
            cmd.Parameters.AddWithValue("@PAID", _ShopNo);
            if (PropertyHolderName != "")
            {
                cmd.Parameters.AddWithValue("@PropertyHolderName", PropertyHolderName);
            }
            else
            {
                cmd.Parameters.AddWithValue("@PropertyHolderName", DBNull.Value);
            }
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public string SaveDataBeforeSendToOnlineProcess(string _PAID, string FinYear, string BTID, string PaymentMode, string BankName, string DDChequeNo, string DDChequeDate,
        string Narration, string PaymentOrderOn, string TxnAmount, string PaymentOrderStatus, string TxnRefNo, string TxnOn, string PaymentStatus,
        string IsPaymentClearance, string ClearanceOn, string MobileNumber, string EmailID, string BankAddress, string ReceivedBy)
    {
        string PaymentOrderId = "";
        Int64 PTID = 0;
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "[spPaymentReceive]";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PTID", SqlDbType.BigInt, 0);
            cmd.Parameters["@PTID"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@PaymentOrderId", SqlDbType.NVarChar, 30);
            cmd.Parameters["@PaymentOrderId"].Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@PAID", _PAID);
            cmd.Parameters.AddWithValue("@FinYear", FinYear);
            cmd.Parameters.AddWithValue("@BTID", BTID);
            cmd.Parameters.AddWithValue("@PaymentMode", PaymentMode);
            cmd.Parameters.AddWithValue("@BankName", BankName);
            cmd.Parameters.AddWithValue("@DDChequeNo", DDChequeNo);

            if (!string.IsNullOrEmpty(DDChequeDate))
            {
                cmd.Parameters.AddWithValue("@DDChequeDate", Convert.ToDateTime(DDChequeDate));
            }
            else
            {
                cmd.Parameters.AddWithValue("@DDChequeDate", DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@Narration", Narration);
            //cmd.Parameters.AddWithValue("@PaymentOrderId", PaymentOrderId);
            if (!string.IsNullOrEmpty(PaymentOrderOn))
            {
                cmd.Parameters.AddWithValue("@PaymentOrderOn", Convert.ToDateTime(PaymentOrderOn));
            }
            else
            {
                cmd.Parameters.AddWithValue("@PaymentOrderOn", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@TxnAmount", TxnAmount);
            cmd.Parameters.AddWithValue("@PaymentOrderStatus", PaymentOrderStatus);
            cmd.Parameters.AddWithValue("@TxnRefNo", TxnRefNo);
            cmd.Parameters.AddWithValue("@TxnType", "S");
            if (!string.IsNullOrEmpty(TxnOn))
            {
                cmd.Parameters.AddWithValue("@TxnOn", Convert.ToDateTime(TxnOn));
            }
            else
            {
                cmd.Parameters.AddWithValue("@TxnOn", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@PaymentStatus", PaymentStatus);
            if (!string.IsNullOrEmpty(IsPaymentClearance))
            {
                cmd.Parameters.AddWithValue("@IsPaymentClearance", IsPaymentClearance);
            }
            else
            {
                cmd.Parameters.AddWithValue("@IsPaymentClearance", DBNull.Value);
            }
            if (!string.IsNullOrEmpty(ClearanceOn))
            {
                cmd.Parameters.AddWithValue("@ClearanceOn", Convert.ToDateTime(ClearanceOn));
            }
            else
            {
                cmd.Parameters.AddWithValue("@ClearanceOn", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@MobileNumber", MobileNumber);
            cmd.Parameters.AddWithValue("@EmailID", EmailID);
            cmd.Parameters.AddWithValue("@BankAddress", BankAddress);
            cmd.Parameters.AddWithValue("@ReceivedBy", ReceivedBy);

            con.Open();
            cmd.ExecuteNonQuery();
            PaymentOrderId = cmd.Parameters["@PaymentOrderId"].Value.ToString();
            PTID = Convert.ToInt64(cmd.Parameters["@PTID"].Value.ToString());
            con.Close();

            ReqMsgDTO objReqMsgDTO = new ReqMsgDTO();
            //step 2

            objReqMsgDTO.OrderId = "" + PaymentOrderId.ToString().Trim() + "";
            objReqMsgDTO.Mid = "WL0000000007384";
            objReqMsgDTO.Enckey = "7f2e93c63179a5ae9006f24dc6547ab4";
            objReqMsgDTO.MeTransReqType = "S";
            objReqMsgDTO.TrnAmt = Convert.ToString(Convert.ToDouble(TxnAmount) * 100);
            objReqMsgDTO.ResponseUrl = "https://www.pmc.bihar.gov.in/thankyoupayment.aspx";
            //objReqMsgDTO.ResponseUrl = "http://localhost:49700/thankyoupayment.aspx";
            objReqMsgDTO.TrnRemarks = "Shops monthly payment";
            objReqMsgDTO.TrnCurrency = "INR";
            objReqMsgDTO.AddField1 = _PAID;
            objReqMsgDTO.AddField2 = "SP";
            objReqMsgDTO.AddField3 = PTID.ToString();
            objReqMsgDTO.AddField4 = BTID.ToString();
            //Step 3: Call API to generate the message
            AWLMEAPI objawlmerchantkit = new AWLMEAPI();
            objReqMsgDTO = objawlmerchantkit.generateTrnReqMsg(objReqMsgDTO);
            if (objReqMsgDTO != null)
            {
                string Message;
                if (objReqMsgDTO.StatusDesc == "Success")
                {
                    Message = objReqMsgDTO.ReqMsg;
                    HttpContext.Current.Session["PaymentOrderId"] = PaymentOrderId;
                    HttpContext.Current.Session["MID"] = objReqMsgDTO.Mid;
                    HttpContext.Current.Session["Msg"] = Message;
                    HttpContext.Current.Response.Redirect("myTransaction.aspx", false);
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
        return PaymentOrderId;
    }

    public string SaveDataBeforeSendToOfflineProcess(string _PAID, string FinYear, string BTID, string PaymentMode, string BankName, string DDChequeNo, string DDChequeDate,
       string Narration, string PaymentOrderOn, string TxnAmount, string PaymentOrderStatus, string TxnRefNo, string TxnOn, string PaymentStatus,
       string IsPaymentClearance, string ClearanceOn, string MobileNumber, string EmailID, string BankAddress, string ReceivedBy)
     {
        string PaymentOrderId = "";
        string PTID = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "[spPaymentReceive]";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PTID", SqlDbType.BigInt, 0);
            cmd.Parameters["@PTID"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@PaymentOrderId", SqlDbType.NVarChar, 30);
            cmd.Parameters["@PaymentOrderId"].Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@PAID", _PAID);
            cmd.Parameters.AddWithValue("@FinYear", FinYear);
            cmd.Parameters.AddWithValue("@BTID", BTID);
            cmd.Parameters.AddWithValue("@PaymentMode", PaymentMode);
            cmd.Parameters.AddWithValue("@BankName", BankName);
            cmd.Parameters.AddWithValue("@DDChequeNo", DDChequeNo);

            if (!string.IsNullOrEmpty(DDChequeDate))
            {
                cmd.Parameters.AddWithValue("@DDChequeDate", Convert.ToDateTime(DDChequeDate));
            }
            else
            {
                cmd.Parameters.AddWithValue("@DDChequeDate", DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@Narration", Narration);
            //cmd.Parameters.AddWithValue("@PaymentOrderId", PaymentOrderId);
            if (!string.IsNullOrEmpty(PaymentOrderOn))
            {
                cmd.Parameters.AddWithValue("@PaymentOrderOn", Convert.ToDateTime(PaymentOrderOn));
            }
            else
            {
                cmd.Parameters.AddWithValue("@PaymentOrderOn", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@TxnAmount", TxnAmount);
            cmd.Parameters.AddWithValue("@PaymentOrderStatus", PaymentOrderStatus);
            cmd.Parameters.AddWithValue("@TxnRefNo", TxnRefNo);
            cmd.Parameters.AddWithValue("@TxnType", "S");
            if (!string.IsNullOrEmpty(TxnOn))
            {
                cmd.Parameters.AddWithValue("@TxnOn", Convert.ToDateTime(TxnOn));
            }
            else
            {
                cmd.Parameters.AddWithValue("@TxnOn", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@PaymentStatus", PaymentStatus);
            if (!string.IsNullOrEmpty(IsPaymentClearance))
            {
                cmd.Parameters.AddWithValue("@IsPaymentClearance", IsPaymentClearance);
            }
            else
            {
                cmd.Parameters.AddWithValue("@IsPaymentClearance", DBNull.Value);
            }

            if (!string.IsNullOrEmpty(ClearanceOn))
            {
                cmd.Parameters.AddWithValue("@ClearanceOn", Convert.ToDateTime(ClearanceOn));
            }
            else
            {
                cmd.Parameters.AddWithValue("@ClearanceOn", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@MobileNumber", MobileNumber);
            cmd.Parameters.AddWithValue("@EmailID", EmailID);
            cmd.Parameters.AddWithValue("@BankAddress", BankAddress);
            cmd.Parameters.AddWithValue("@ReceivedBy", ReceivedBy);

            con.Open();
            cmd.ExecuteNonQuery();

            PaymentOrderId = cmd.Parameters["@PaymentOrderId"].Value.ToString();
            PTID = cmd.Parameters["@PTID"].Value.ToString();
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
        return PTID;
    }

    public DataTable bindGridGRent(string PAID, string BTID)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DuesBills.FinYear, DuesBills.GroundRent, DuesBills.GroundRentInterest, DuesBills.TotalAmount FROM DuesBills INNER JOIN Bills ON DuesBills.fkBID = Bills.pkBID WHERE (DuesBills.fkPAID = @PAID) AND (DuesBills.fkBTID = @BTID) AND (Bills.IsPaid = 0)", con);
            cmd.Parameters.AddWithValue("@PAID", PAID);
            cmd.Parameters.AddWithValue("@BTID", BTID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public DataTable bindGridMCharge(string PAID, string BTID)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DuesBills.BillForMonth, DuesBills.BillForYear, DuesBills.MantainanceCharge, DuesBills.MantainanceChargeInterest, DuesBills.TotalAmount FROM DuesBills INNER JOIN Bills ON DuesBills.fkBID = Bills.pkBID WHERE (DuesBills.fkPAID = @PAID) AND (DuesBills.fkBTID = @BTID) AND (Bills.IsPaid = 0)", con);
            cmd.Parameters.AddWithValue("@PAID", PAID);
            cmd.Parameters.AddWithValue("@BTID", BTID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }


    public DataTable GetPaymentHistory(Int64 PAID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [PaymentTransaction] WHERE fkPAID=@PAID AND IsPaymentClearance=1 ORDER BY pkPTID DESC", con);
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
}