using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

/// <summary>
/// Summary description for AxisBankPOS
/// </summary>
public class AxisBankPOS
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public DataTable BindPOSByCircleId(string CircleID)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("select DISTinct CWP.DummyUserId, CWP.pkCWPOSId from [AXISBANK].[CircleWisePOS] AS CWP INNER JOIN [AXISBANK].[MasterTable] MT ON CWP.DummyUserId=MT.username where MT.IsActive=1 AND CWP.Status=1 and CWP.CircleId=@CircleID", con))
        {
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@CircleID", CircleID);
            sda.Fill(dt);
        }
        return dt;
    }

    public DataTable BindGrid(string CircleId, string POSId, string FromDate, string UptoDate)
    {
        DataTable dt = new DataTable();
        string query = "SELECT MT.pkId, MT.circleName, MT.wardNo, MT.createdOn, MT.customerMobile, MT.citizenName, MT.username, MT.paymentMode, MT.amount, MT.deviceSerial, MT.states, MT.subFine, MT.fineType, MT.subFine, MT.txnId, MT.receiptUrl FROM [AXISBANK].[MasterTable] AS MT INNER JOIN [AXISBANK].[CircleWisePOS] AS CW ON CW.DummyUserId=MT.username where IsActive =1 ";
        string subquery = string.Empty;

        if (CircleId != "0" && POSId != "Select" && FromDate != "" && UptoDate != "")
        {
            subquery += "and CW.CircleId=@CircleId and CW.DummyUserId=@POSId and MT.EnteredDate between @fromdate and @uptodate";
        }
        if (CircleId != "0" && POSId == "Select" && FromDate != "" && UptoDate != "")
        {
            subquery += "and CW.CircleId=@CircleId and MT.EnteredDate between @fromdate and @uptodate";
        }
        if (CircleId == "0" && FromDate != "" && UptoDate != "")
        {
            subquery += "and MT.EnteredDate between @fromdate and @uptodate";
        }
        if (CircleId != "0" && POSId != "Select" && FromDate == "" && UptoDate == "")
        {
            subquery += "and CW.CircleId=@CircleId and CW.DummyUserId=@POSId";
        }
        if (CircleId != "0" && POSId == "Select" && FromDate == "" && UptoDate == "")
        {
            subquery += "and CW.CircleId=@CircleId";
        }
        if (!string.IsNullOrEmpty(subquery))
        {
            query = query + " " + subquery + " order by MT.pkId desc";
        }
        string newquery = query;
        //try
        //{
        SqlConnection con = new SqlConnection(strcon);
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@CircleId", CircleId);
        cmd.Parameters.AddWithValue("@POSId", POSId);
        if (FromDate != "")
        {
            cmd.Parameters.AddWithValue("@fromdate", Convert.ToDateTime(FromDate));
        }
        if (UptoDate != "")
        {
            cmd.Parameters.AddWithValue("@uptodate", Convert.ToDateTime(UptoDate));
        }
        SqlDataAdapter sda = new SqlDataAdapter(cmd);
        sda.Fill(dt);
        //}
        //catch (Exception ex)
        //{

        //}
        return dt;
    }
}