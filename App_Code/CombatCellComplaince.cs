using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for CombatCellComplaince
/// </summary>
public class CombatCellComplaince
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    public Int32 AddCombatCellComplaince(string added_by, string AutherityTypeid, string title, string categoryid, string subcategoryId, string Area, string Circle, string Ward, string name, string MobileNo, string Emailid, string Mode, DataTable dtImage)
    {
        string mo = "";
        if (Mode == "1")
        {
            mo = "MP";
        }
        if (Mode == "2")
        {
            mo = "TF";
        }
        if (Mode == "3")
        {
            mo = "Web";
        }
        else if (Mode == "4")
        {
            mo = "SM";
        }
        Int64 id = 0;
        string Complainno = string.Empty;

        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("Insert_Combat_Cell_Title"))
        {

            Complainno = GenerateComplainNo(categoryid, subcategoryId, Circle, Ward, mo);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters["@id"].Direction = ParameterDirection.InputOutput;
            cmd.Parameters.AddWithValue("@added_by", added_by);
            cmd.Parameters.AddWithValue("@ComplainNo", Complainno);
            cmd.Parameters.AddWithValue("@AutherityTypeid", AutherityTypeid);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@categoryid", categoryid);
            cmd.Parameters.AddWithValue("@subcategoryId", subcategoryId);
            cmd.Parameters.AddWithValue("@Latitude", "");
            cmd.Parameters.AddWithValue("@Longitude", "");
            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@Circle", Circle);
            cmd.Parameters.AddWithValue("@Ward", Ward);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@MobileNo", MobileNo);
            cmd.Parameters.AddWithValue("@Emailid", Emailid);
            cmd.Parameters.AddWithValue("@Mode", Mode);
            cmd.Parameters.AddWithValue("@ComplaintDate", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Parameters.AddWithValue("@status", "true");
            cmd.Connection = con;
            try
            {

                con.Open();
                cmd.ExecuteNonQuery();
                if (MobileNo != "")
                {
                    send_sms ss = new send_sms();
                    int auth = ss.sendSingleSMSNew(MobileNo, "Thank you " + name.ToString().Trim() + ", your complain has been registered successfully. Your complain no is " + Complainno.ToString() + ".-Patna Municipal Corporation.", "1307162308455904331");
                    // adding data to swashta app
                    Swashta_Data_new sd = new Swashta_Data_new();
                    string Status = "";
                    string status1 = "";
                    string ResponseId = "";
                    Status = sd.register_new_user("", MobileNo, "", "");
                    string[] st = new string[5];
                    st = Status.Split(',');
                    status1 = st[2].Split(':')[1].Replace('"', ' ');
                    status1 = status1.TrimEnd();
                    status1 = status1.TrimStart();
                    ResponseId = st[3].Split(':')[2];
                    if (status1 == "You have been successfully registred")
                    {
                        sd.update_sent_details(MobileNo, status1, ResponseId, "1");
                    }
                    else
                    {
                        sd.update_sent_details(MobileNo, status1, ResponseId, "0");
                    }
                }
                int returnid = Convert.ToInt32(cmd.Parameters["@id"].Value);
                SqlConnection con1 = new SqlConnection(strcon);
                foreach (DataRow dr in dtImage.Rows)
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

    public string GenerateComplainNo(string catagory, string Subcatagory, string cluster, string ward, string mode)
    {
        string complainno = string.Empty;
        
            complainno = mode + "-" + findCatsubcatclusterwardcode(catagory, "0", "0", "0") + "-" + findCatsubcatclusterwardcode("0", Subcatagory, "0", "0") + "-" + findCatsubcatclusterwardcode("0", "0", cluster, "0") + "-" + findCatsubcatclusterwardcode("0", "0", "0", ward) + "-" + findlastComplain();
       
        return complainno;
    }

    public string findlastComplain()
    {
        string id = "";

        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select COUNT(id)+1 as counter from combatcellTitle ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            id = ds.Tables[0].Rows[0]["counter"].ToString();

        }
        return id;
    }

    public string findCatsubcatclusterwardcode(string catagory, string Subcatagory, string cluster, string ward)
    {
        string code = string.Empty;
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();
            if (catagory != "0")
            {
                cmd.CommandText = "select CatCode as Code from  CombatCategory where id=@id ";
                cmd.Parameters.AddWithValue("@id", catagory);
            }
            else if (Subcatagory != "0")
            {
                cmd.CommandText = "select SubCatCode as Code from  CombatCellSubcategory where id=@id";
                cmd.Parameters.AddWithValue("@id", Subcatagory);
            }
            else if (cluster != "0")
            {
                cmd.CommandText = "select Code as Code from  Circle where id=@id ";
                cmd.Parameters.AddWithValue("@id", cluster);
            }
            else if (ward != "0")
            {
                cmd.CommandText = "select WardCode as Code from  tblWard where id=@id";
                cmd.Parameters.AddWithValue("@id", ward);
            }

            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            code = ds.Tables[0].Rows[0]["Code"].ToString();

        }

        return code;
    }

    public DataTable bindCategory()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,combatcatagory from CombatCategory where status='Active' order by combatcatagory ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindsubcategory(string categoryid)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,SubcatagoryName from CombatCellSubcategory where status=1 and categoryid=@catagoryid order by SubcatagoryName ASC", con);
            cmd.Parameters.Add("@catagoryid", categoryid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindMode()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,Mode from CombatCellMode where status=1 order by Mode ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindCircle()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,Circle from Circle where status=1 order by Circle ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable searchCircelid(string ward_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select Circle from Circle, tblWard where tblWard.Circleid=Circle.id and tblWard.id=@ward_id", con);
            cmd.Parameters.AddWithValue("@ward_id", ward_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindWard(string Circleid)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,WardNo from tblWard where status=1 and Circleid=@Circleid order by WardNo ASC", con);
            cmd.Parameters.Add("@Circleid", Circleid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindWardNew()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from tblWard where status=1 order by WardNo ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindPropertyType()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,property_type from tbl_property_type_master where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindRevenueCircle()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,rev_circle from tbl_revenue_circle_master where status=1 order by id ASC", con);
            //cmd.Parameters.AddWithValue("@Circleid", Circleid);
            //cmd.Parameters.AddWithValue("@WardId", WardId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and ComplainStatus='Active' order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains_sanitization_officer()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.ComplainNo,a.added_by,a.status as sc_status,a.fw_DateTime,[dbo].[find_forwarder_name](a.added_by) as sender_name,a.fw_officer_Id,b.id,b.title,b.complainno as comp_no,b.MobileNo,b.name,b.DOE,b.ComplainStatus from CombatCell_forward_list a, combatcellTitle b where b.ComplainStatus='Forwarded' and a.fw_officer_Id=@user_id and a.ComplainNo = b.id and a.status='active' order by fw_DateTime DESC", con);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplainsUserwise()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and ComplainStatus='Active' order by Mode ASC", con);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplainsbyId(string Complainno)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and ComplainNo=@Complainno  order by Mode ASC", con);
            cmd.Parameters.Add("@Complainno", Complainno);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public string getsender(string userid)
    {
        string sender = string.Empty;

        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id,first_name+''+last_name as Fullname   from users where status='Active'  and id=@id", con);
            cmd.Parameters.Add("@id", userid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            if (ds.Tables[0] != null)
            {
                sender = ds.Tables[0].Rows[0]["Fullname"].ToString();
            }
        }
        return sender;

    }

    public DataTable BindAllCompImage(string combatcelltitleid)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from CombatCellAttatchment where combatcelltitleid=@combatcelltitleid and status=1 order by id ASC", con);
            cmd.Parameters.Add("@combatcelltitleid", combatcelltitleid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllCompImage_compliance(string combatcelltitleid)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from combat_compliance_images where complaint_no=@combatcelltitleid", con);
            cmd.Parameters.Add("@combatcelltitleid", combatcelltitleid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains_reviews(string complain_no)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select id,added_by,comment,authority_type,status,since,complain_no,[dbo].[find_forwarder_name](added_by) as added_by_name,[dbo].[find_authority_type](authority_type) as authority_type_type from combat_review where complain_no=@complain_no", con);
            cmd.Parameters.AddWithValue("@complain_no", complain_no);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // Added by arnav for showing all forwarded by combat cell data
    public DataTable BindAllComplains_forwarded_combat_cell()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.ComplainNo as a_id,a.added_by,b.id,b.ComplainNo,b.added_by,b.title,b.categoryid,b.subcategoryid,b.Longitude,b.latitude,b.area,b.circle,b.ward,b.MobileNo,b.EmailId,b.Mode,b.DOE,b.status,b.name,b.ComplainStatus from CombatCell_forward_list a, combatcellTitle b where a.ComplainNo = b.id and a.added_by=@added_by and b.ComplainStatus!='Active' order by b.id DESC", con);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            //SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and ComplainStatus!='Active' order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains_forwarded_combat_cell_user_wise(string user_id, string date, string date1)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("select a.ComplainNo as a_id,a.added_by,a.fw_DateTime,b.id,b.ComplainNo,b.added_by,b.title,b.categoryid,b.subcategoryid,b.Longitude,b.latitude,b.area,b.circle,b.ward,b.MobileNo,b.EmailId,b.Mode,b.DOE,b.status,b.name,b.ComplainStatus from CombatCell_forward_list a, combatcellTitle b where a.ComplainNo = b.id and a.added_by=@added_by and b.ComplainStatus!='Active' and CAST(a.fw_DateTime AS DATE) between @date and @date1 order by b.id DESC", con);
            cmd.Parameters.AddWithValue("@added_by", user_id);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@date1", date1);
            //SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and ComplainStatus!='Active' order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    public DataTable BindAllComplains_forwarded_si_cell()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.ComplainNo,a.added_by,a.fw_officer_Id,[dbo].[find_forwarder_name](fw_officer_Id) as fw_name,a.fw_DateTime,a.status as current_status,b.id,b.ComplainNo as comp_no,b.title,b.name,b.DOE from CombatCell_forward_list a,combatcellTitle b where a.added_by=@user_id and a.ComplainNo=b.id", con);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    // closed by chief officer
    public DataTable BindAllComplains_sanitization_officer_closed()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.ComplainNo,a.added_by,a.status as sc_status,a.fw_DateTime,[dbo].[find_forwarder_name](a.added_by) as sender_name,a.fw_officer_Id,b.id,b.title,b.complainno as comp_no,b.MobileNo,b.name,b.DOE,b.ComplainStatus from CombatCell_forward_list a, combatcellTitle b where b.ComplainStatus='closed' and a.fw_officer_Id=@user_id and a.ComplainNo = b.id and a.status='closed' order by fw_DateTime DESC", con);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public string find_closed_date_closed(string id)
    {
        string date = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 since from combat_review where added_by=@user_id and complain_no=@id order by id desc", con);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                date = Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MMM-yyyy");
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
        return date;
    }

    // closed by chief officer
    public DataTable BindAllComplains_sanitization_officer_resent()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.ComplainNo,a.added_by,a.status as sc_status,a.fw_DateTime,[dbo].[find_forwarder_name](a.added_by) as sender_name,a.fw_officer_Id,b.id,b.title,b.complainno as comp_no,b.MobileNo,b.name,b.DOE,b.ComplainStatus from CombatCell_forward_list a, combatcellTitle b where a.fw_officer_Id=@user_id and b.ComplainStatus='Forwarded' and a.ComplainNo = b.id and a.status='Resend' order by fw_DateTime DESC", con);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // overtime at si
    public DataTable BindAllComplains_sanitization_officer_overtime_at_si(string circle_id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo as comp_no,b.title,b.ComplainStatus,b.circle,c.id,c.ComplainNo,c.fw_DateTime,(DATEDIFF(HOUR ,(c.fw_DateTime) , getdate()))-( d.TimelimitinhrsforSI) as AboveTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c, CombatCellSubcategory d where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.subcategoryId=d.id and DATEDIFF(HOUR ,(c.fw_DateTime) , getdate())  >d.TimelimitinhrsforSI and b.circle=@circle_id order by b.id desc", con);
            //cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@circle_id", circle_id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    public DataTable BindAllComplains_received()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name,[dbo].[find_receive_mode](Mode) as rec_mode  from combatcellTitle where status='true' order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    public DataTable BindAllComplains_received_filter(string startdate, string enddate)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name,[dbo].[find_receive_mode](Mode) as rec_mode  from combatcellTitle where status='true' and ComplaintDate between @startdate and @enddate order by id DESC", con);
            cmd.Parameters.AddWithValue("@startdate", startdate);
            cmd.Parameters.AddWithValue("@enddate", enddate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    // all closed list
    public DataTable BindAllComplains_received_all_closed()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name from combatcellTitle where status='true' and complainstatus='closed'  order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains_received_all_closed_filter(string startdate, string enddate)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name from combatcellTitle where status='true' and complainstatus='closed' and ComplaintDate between @startdate and @enddate   order by id DESC", con);
            cmd.Parameters.AddWithValue("@startdate", startdate);
            cmd.Parameters.AddWithValue("@enddate", enddate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // all combat cell
    public DataTable BindAllComplains_received_all_pending_combat()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name from combatcellTitle where status='true' and ComplainStatus='Active'  order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains_received_all_pending_combat_filter(string startdate, string enddate)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select *,[dbo].[find_circle_name](Circle) as circle_name_name,[dbo].[find_ward_name](Ward) as ward_name_name from combatcellTitle where status='true' and ComplainStatus='Active' and ComplaintDate between @startdate and @enddate  order by id DESC", con);
            cmd.Parameters.AddWithValue("@startdate", startdate);
            cmd.Parameters.AddWithValue("@enddate", enddate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    // pending at si
    public DataTable BindAllComplains_received_all_pending_combat_si_pending()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id order by b.id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains_received_all_pending_combat_si_pending_filter(string startdate, string enddate)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,b.ComplainNo,b.circle,b.ward,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='4' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id and b.ComplaintDate between @startdate and @enddate order by b.id desc", con);
            cmd.Parameters.AddWithValue("@startdate", startdate);
            cmd.Parameters.AddWithValue("@enddate", enddate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    // pending at csi
    public DataTable BindAllComplains_received_all_pending_combat_csi_pending()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.ComplainNo,b.circle,b.ward,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id order by b.id desc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }


    public DataTable BindAllComplains_received_all_pending_combat_csi_pending_filter(string startdate, string enddate)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select a.id as auth_id,a.UserTypeid,(a.first_name + ' ' +a.last_name) as name,b.id,[dbo].[find_circle_name](b.Circle) as circle_name_name,[dbo].[find_ward_name](b.Ward) as ward_name_name,b.ComplainNo,b.circle,b.ward,b.title,b.ComplainStatus,b.doe,c.id,c.ComplainNo,c.fw_DateTime,c.fw_officer_Id,c.status as comp_status from users a,combatcellTitle b,CombatCell_forward_list c where a.UserTypeid='3' and b.id=c.ComplainNo and b.ComplainStatus ='Forwarded' and c.status='active' and c.fw_officer_Id=a.id  and b.ComplaintDate between @startdate and @enddate order by b.id desc", con);
            cmd.Parameters.AddWithValue("@startdate", startdate);
            cmd.Parameters.AddWithValue("@enddate", enddate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }



    public string find_closed_date_closed_sc_forward(string id)
    {
        string date = "";
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select top 1 since from combat_review where authority_type='5' and complain_no=@id order by id desc", con);
            cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["RA_user_ID"].ToString());
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                date = Convert.ToDateTime(dt.Rows[0]["since"]).ToString("dd-MMM-yyyy");
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
        return date;
    }

    public DataTable show_issue_details_tracking(string issue_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and id=@id", con);
            cmd.Parameters.AddWithValue("@id", issue_id);
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


    public DataTable show_issue_tracking_Check(string issue_id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id from combatcellTitle where status='true' and ComplainNo=@id", con);
            cmd.Parameters.AddWithValue("@id", issue_id);
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





    public DataTable BindAllComplains_forwarded_combat_cell_company()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select distinct(a.ComplainNo) as a_id,a.added_by,b.id,b.ComplainNo,b.added_by,b.title,b.categoryid,b.subcategoryid,b.Longitude,b.latitude,b.area,b.circle,b.ward,b.MobileNo,b.EmailId,b.Mode,b.DOE,b.status,b.name,b.ComplainStatus from CombatCell_forward_list a, combatcellTitle b where a.ComplainNo = b.id and a.added_by=@added_by and b.ComplainStatus='company_frwd' order by b.id DESC", con);
            cmd.Parameters.AddWithValue("@added_by", HttpContext.Current.Session["RA_user_ID"].ToString());
            //SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and ComplainStatus!='Active' order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindAllComplains_forwarded_combat_cell_company_user_wise(string user_id, string date, string date1)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select distinct(a.ComplainNo) as a_id,a.added_by,a.fw_DateTime,b.id,b.ComplainNo,b.added_by,b.title,b.categoryid,b.subcategoryid,b.Longitude,b.latitude,b.area,b.circle,b.ward,b.MobileNo,b.EmailId,b.Mode,b.DOE,b.status,b.name,b.ComplainStatus from CombatCell_forward_list a, combatcellTitle b where a.ComplainNo = b.id and a.added_by=@added_by and b.ComplainStatus='company_frwd' and CAST(a.fw_DateTime AS DATE)between @date and @date1 order by b.id DESC", con);
            cmd.Parameters.AddWithValue("@added_by", user_id);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@date1", date1);
            //SqlCommand cmd = new SqlCommand("select * from combatcellTitle where status='true' and ComplainStatus!='Active' order by id DESC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    /************************************************************
     * Till Here
     * **********************************************************/

}