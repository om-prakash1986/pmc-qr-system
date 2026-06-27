using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Created by Gyan Chand Verma for the creation and maintenance of Fuel Management System under Patna Municipal Corporation
/// June 28, 2020
/// </summary>
public class fuelManagement
{
    string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;

    #region Login page
    //This is hardcoded by Gyan Chand Verma for the differentiation of all three section login.
    //Headquater = 100, Circle=200, Vender=300
    public int headquaterLogin(string contact, string password)
    {
        int number = 0;
        // Maurya_shop ms = new Maurya_shop();
        encryption ms = new encryption();
        string passw = ms.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("PROFUELHEADQUATERLOGIN"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Contact", contact);
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
                    HttpContext.Current.Session["fuel_U_Name"] = null;
                    HttpContext.Current.Session["fuel_U_ID"] = null;
                    HttpContext.Current.Session["fuel_ID"] = null;
                    HttpContext.Current.Session["fuel_U_Name"] = dt.Rows[0]["userName"].ToString();
                    HttpContext.Current.Session["fuel_U_ID"] = "100";
                    HttpContext.Current.Session["fuel_ID"] = dt.Rows[0]["headquaterId"].ToString();
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

    public int circleLogin(string contact, string password)
    {
        int number = 0;
        encryption ms = new encryption();
        string passw = ms.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("PROFUELCIRCLELOGIN"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Contact", contact);
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
                    HttpContext.Current.Session["fuel_U_Name"] = null;
                    HttpContext.Current.Session["fuel_U_ID"] = null;
                    HttpContext.Current.Session["fuel_ID"] = null;
                    HttpContext.Current.Session["fuel_Circle_ID"] = null;
                    HttpContext.Current.Session["fuel_U_Name"] = dt.Rows[0]["officerName"].ToString();
                    HttpContext.Current.Session["fuel_U_ID"] = "200";
                    HttpContext.Current.Session["fuel_Circle_ID"] = dt.Rows[0]["circleId"].ToString();
                    HttpContext.Current.Session["fuel_ID"] = dt.Rows[0]["circleLoginId"].ToString();
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

    public int VenderLogin(string contact, string password)
    {
        int number = 0;
        encryption ms = new encryption();
        string passw = ms.GetMD5(password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("PROFUELVENDERLOGIN"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Contact", contact);
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
                    HttpContext.Current.Session["fuel_U_Name"] = null;
                    HttpContext.Current.Session["fuel_U_ID"] = null;
                    HttpContext.Current.Session["fuel_ID"] = null;
                    HttpContext.Current.Session["fuel_U_Name"] = dt.Rows[0]["venderName"].ToString();
                    HttpContext.Current.Session["fuel_U_ID"] = "300";
                    HttpContext.Current.Session["fuel_ID"] = dt.Rows[0]["venderLoginId"].ToString();
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
    #endregion

    #region Headquater Login
    public int addHeadquaterLogin(int id, string _username, string _designation, string _contact, string _password, int _status)
    {
        int number = 0;
        encryption ms = new encryption();
        string password = ms.GetMD5(_password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insertintofuelHeadquaterLogin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@username", _username);
            cmd.Parameters.AddWithValue("@designation", _designation);
            cmd.Parameters.AddWithValue("@contact", _contact);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@modifiedby", HttpContext.Current.Session["fuel_ID"].ToString());
            cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@status", _status);
            cmd.Parameters.AddWithValue("@since", DateTime.Now);
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

    public DataTable deletedById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("update fuelHeadquaterLogin set status='0' where headquaterId = @id", con);
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

    public DataTable viewDetailsById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from fuelHeadquaterLogin where headquaterId = @id order by headquaterId asc", con);
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

    public DataTable viewAll()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from fuelHeadquaterLogin order by headquaterId asc", con);
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

    #region Circle Login
    public DataTable BindAllCircles()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, circle from FUEL_CIRCLE_MANAGE where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int circleLogin(int id, int _drop1, string _username, string _contact, string _password, int _status)
    {
        int number = 0;
        encryption ms = new encryption();
        string password = ms.GetMD5(_password);

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insertintofuelCircleLogin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@drop1", _drop1);
            cmd.Parameters.AddWithValue("@username", _username);
            cmd.Parameters.AddWithValue("@contact", _contact);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@status", _status);
            cmd.Parameters.AddWithValue("@modifiedby", HttpContext.Current.Session["fuel_ID"].ToString());
            cmd.Parameters.AddWithValue("@modifieddate", DateTime.Now);
            cmd.Parameters.AddWithValue("@since", DateTime.Now);
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

    public DataTable viewUserDetailById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from fuelCircleLogin where circleLoginId = @id order by circleLoginId asc", con);
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

    public DataTable bindCirclebyId(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id, circle from FUEL_CIRCLE_MANAGE where id=@id", con);
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

    public DataTable deletedCircleLoginById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("update fuelCircleLogin set status='0' where circleLoginId = @id", con);
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

    public DataTable BindAllCircleLoginUser()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select fcl.officerName,fcl.circleLoginId as circleLogin, c.Circle, fcl.contactNo,fcl.status from fuelCircleLogin fcl, FUEL_CIRCLE_MANAGE c where fcl.circleLoginId=c.id order by circleLogin asc", con);
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

    #region Vendor Login
    public int vendorLogin(int id, string _username, string _address, int _chppc, int _chncc, int _chkbc, int _chbpc, int _chabc, int _chpcc, int _chhqt, string _contact, string _password, int _status)
    {
        encryption ms = new encryption();
        string password = ms.GetMD5(_password);

        int number = 0;
        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insertintofuelVendorLogin"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@username", _username);
            cmd.Parameters.AddWithValue("@address", _address);
            cmd.Parameters.AddWithValue("@chppc", _chppc);
            cmd.Parameters.AddWithValue("@chncc", _chncc);
            cmd.Parameters.AddWithValue("@chkbc", _chkbc);
            cmd.Parameters.AddWithValue("@chbpc", _chbpc);
            cmd.Parameters.AddWithValue("@chabc", _chabc);
            cmd.Parameters.AddWithValue("@chpcc", _chpcc);
            cmd.Parameters.AddWithValue("@chhqt", _chhqt);
            cmd.Parameters.AddWithValue("@contact", _contact);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@modified_by", HttpContext.Current.Session["fuel_ID"].ToString());
            cmd.Parameters.AddWithValue("@modified_date", DateTime.Now);
            cmd.Parameters.AddWithValue("@status", _status);
            cmd.Parameters.AddWithValue("@since", DateTime.Now);
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

    public DataTable BindAllVendor()
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select fvl.venderLoginId as venderId, fvl.venderName, fvl.venderAddress, fvl.contactNo, fvl.status from fuelVendorLogin fvl order by fvl.venderLoginId asc", con);
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

    public DataTable viewVenderById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from fuelVendorLogin where venderLoginId = @id order by venderLoginId asc", con);
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

    public DataTable deletedVenderLoginById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("update fuelVendorLogin set status='0' where venderLoginId = @id", con);
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
    #endregion

    #region Manage Vehicle
    public DataTable BindAllVehicle()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, vehicle_name from fuelvehicle where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable BindVehicleById(string Circle_Id, string vehicle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select id, restrationNo from fuelVehicleDetails where circleIssueId=@Circle_Id and vehicleId=@vehicle_Id and status=1", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            cmd.Parameters.AddWithValue("@vehicle_Id", vehicle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int insertVehicleDetails(int id, int _listVehicle, string _regisNo, string _insurance, string _policy, string _registration)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insertintofuelVehicleDetails"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@listVehicle", _listVehicle);
            cmd.Parameters.AddWithValue("@regisNo", _regisNo);
            cmd.Parameters.AddWithValue("@insurance", Convert.ToDateTime(_insurance));
            cmd.Parameters.AddWithValue("@policy", Convert.ToDateTime(_policy));
            cmd.Parameters.AddWithValue("@registration", Convert.ToDateTime(_registration));
            cmd.Parameters.AddWithValue("@status", 1);
            cmd.Parameters.AddWithValue("@circleIssueId", HttpContext.Current.Session["fuel_Circle_ID"].ToString());
            cmd.Parameters.AddWithValue("@modifiedby", HttpContext.Current.Session["fuel_ID"].ToString());
            cmd.Parameters.AddWithValue("@modifieddate", DateTime.Now);
            cmd.Parameters.AddWithValue("@since", DateTime.Now);
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

    public DataTable BindDetailsGrid(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fvd.id,fv.vehicle_name, fvd.restrationNo,CONVERT(VARCHAR(10), fvd.insExpiryDate, 103) AS Insurance ,CONVERT(VARCHAR(10), fvd.polExpiryDate, 103) AS Polution,CONVERT(VARCHAR(10), fvd.regExpiryDate, 103) AS Regstrations from fuelVehicleDetails fvd, fuelvehicle fv where fvd.vehicleId=fv.id and fvd.circleIssueId= @Circle_Id and fvd.status=1 order by fvd.id desc", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable viewVehicleDetailById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select * from fuelVehicleDetails where id = @id order by id asc", con);
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

    public DataTable bindVehiclebyId(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id, vehicle_name from fuelvehicle where id=@id", con);
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
    #endregion

    #region Issue Fuel
    public DataTable BindAllFuelVendor()
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select venderLoginId, venderName, ppcId, nccId, kbcId, bpcId, abcId, pccId, hqtId from fuelVendorLogin where status=1", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int insertIssueFuel(int id, int _vehicleType, string _vehicleNo, int _vendorid, string CoupanNo, DateTime validDate, double _quantity, string _issuePerson, string _IssueContactNo, string _meterReading)
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insertIntofuelIssueCoupon"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@vehicleType", _vehicleType);
            cmd.Parameters.AddWithValue("@vehicleNo", _vehicleNo);
            cmd.Parameters.AddWithValue("@vendorid", _vendorid);
            cmd.Parameters.AddWithValue("@quantity", _quantity);
            cmd.Parameters.AddWithValue("@issuePerson", _issuePerson);
            cmd.Parameters.AddWithValue("@CoupanNo", CoupanNo);
            cmd.Parameters.AddWithValue("@validDate", validDate);
            cmd.Parameters.AddWithValue("@IssueContactNo", _IssueContactNo);
            cmd.Parameters.AddWithValue("@meterReading", _meterReading);
            cmd.Parameters.AddWithValue("@fuelStatusId", 1);
            cmd.Parameters.AddWithValue("@fuelValidiyStatusId", 1);
            cmd.Parameters.AddWithValue("@status", 1);
            cmd.Parameters.AddWithValue("@modifiedby", HttpContext.Current.Session["fuel_ID"].ToString());
            cmd.Parameters.AddWithValue("@circleIssueId", HttpContext.Current.Session["fuel_Circle_ID"].ToString());
            cmd.Parameters.AddWithValue("@modifieddate", DateTime.Now);
            cmd.Parameters.AddWithValue("@since", System.DateTime.Now.ToString("dd-MMM-yyyy"));
            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToInt32(cmd.ExecuteScalar());
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

    public DataTable BindFuelCoupon(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select vf.vehicle_name, fic.vehicleNo, fru.venderName, fic.issueBy, fic.issuePersonName, fic.issuePersonContactNo,fic.validity, fic.lastModifiedDate, fic.couponNo, fic.quantity, fic.circleIssueId, fic.meterReading from fuelIssueCoupon fic, fuelvehicle vf, fuelVendorLogin fru where fic.vehicletypeId = vf.id and fic.vendorId = fru.venderLoginId and fic.id = @id", con);
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

    public DataTable BindFuelIssuedName(string id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("select id, Circle, Code from FUEL_CIRCLE_MANAGE where id=@ide", con);
            cmd.Parameters.AddWithValue("@ide", id);
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

    public DataTable bindIssueDetails(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fic.id, fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fic.couponNo, fic.validity, fic.lastModifiedDate from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru where fic.vehicletypeId = fv.id and fic.vendorId = fru.venderLoginId and fic.circleIssueId=@Circle_Id and fic.fuelStatusId=1 and fic.status=1 order by fic.id desc", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    #endregion

    #region Generate Coupon ID
    public int CreateCouponNo()
    {
        int number = 0;

        SqlConnection con = new SqlConnection(strcon);
        using (SqlCommand cmd = new SqlCommand("insertIntoFuelCoupon"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", HttpContext.Current.Session["fuel_ID"].ToString());
            cmd.Connection = con;
            try
            {
                con.Open();
                number = Convert.ToInt32(cmd.ExecuteScalar());
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
    #endregion

    #region Insert Fuel Rate
    public int insertIntoFuelRate(double _rate)
    {
        int numb = 0;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("insertIntoFuelRate");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@rate", _rate);
            cmd.Parameters.AddWithValue("@venderId", HttpContext.Current.Session["fuel_ID"]);
            cmd.Parameters.AddWithValue("@datetime", DateTime.Now);
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                numb = 1;
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
        return numb;
    }
    #endregion

    #region check Fuel Rate
    public DataTable checkFuelTodayDate(string _venderId)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            //SqlCommand cmd = new SqlCommand("select top 1* from fuelRate where venderId=@vendorId order by since desc", con);
            SqlCommand cmd = new SqlCommand("select top 1* from fuelRate where venderId=@vendorId  and CAST(since AS DATE)=@current_date order by since desc", con);
            cmd.Parameters.AddWithValue("@vendorId", _venderId);
            cmd.Parameters.AddWithValue("@current_date", System.DateTime.Now.Date);
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

    #region Verify Coupon
    public DataTable verifyCoupon(string _couponNo)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select validity, fuelStatusId, fuelValidityStatusId, couponNo, vendorId, fuelRateId, circleIssueId, issuePersonContactNo from fuelIssueCoupon where couponNo=@couponNo and status=1 ", con);
            cmd.Parameters.AddWithValue("@couponNo", _couponNo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable deletedCouponById(int id)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strcon);
        try
        {
            SqlCommand cmd = new SqlCommand("update fuelIssueCoupon set status=0 where id = @id", con);
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
    #endregion

    #region Update Coupon Status
    public DataTable getContactNo(string circleId)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select contactNo from fuelCircleLogin where circleId=@couponNo", con);
            cmd.Parameters.AddWithValue("@couponNo", circleId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public int CouponSuccessStatusUpdate(string _id, string _couponNo)
    {
        int num = 0;
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update fuelIssueCoupon set fuelRateId=@id, fuelStatusId = 2, VerificationDate_Time=@date_time where status=1 and couponNo=@couponNo", con);
            cmd.Parameters.AddWithValue("@id", _id);
            cmd.Parameters.AddWithValue("@couponNo", _couponNo);
            cmd.Parameters.AddWithValue("@date_time", DateTime.Now);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                num = 1;
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
        return num;
    }
    public int CouponTimeoutStatusUpdate(string _couponNo)
    {
        int numb = 0;
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            SqlCommand cmd = new SqlCommand("update fuelIssueCoupon set fuelValidityStatusId = 2, timeOutDate_Time=@date_time where status=1 and couponNo=@couponNo", con);
            cmd.Parameters.AddWithValue("@couponNo", _couponNo);
            cmd.Parameters.AddWithValue("@date_time", DateTime.Now);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                numb = 1;
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
        return numb;
    }
    #endregion

    #region Fuel Station Statement
    public DataTable bindfuelStationStatement(string Fuel_Id, string fromdate, string uptodate)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            //select fic.id, fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fic.couponNo, fic.validity, fic.lastModifiedDate from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru where fic.vehicletypeId = fv.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId = 2 and fic.fuelValidityStatusId = 1 and fic.vendorId = @Fuel_Id and fic.status = 1
            //string query = "select fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fr.fuelRateID, (fic.quantity * fr.fuelRateID) as Amount, fic.couponNo, fic.validity from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru, fuelRate fr where fic.vehicletypeId = fv.id and fic.fuelRateId = fr.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId = 2 and fic.fuelValidityStatusId = 1 and fic.vendorId = @Fuel_Id and fic.status = 1 ";
            // string query = "select fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fic.vendorId,[dbo].[find_fuel_rate] (fic.vendorId, fic.since) as fuelRateID,(fic.quantity * [dbo].[find_fuel_rate](fic.vendorId,fic.since)) as Amount, fic.couponNo, fic.validity, fic.since from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru where fic.vehicletypeId = fv.id and fic.fuelStatusId =2 and fic.fuelValidityStatusId =1 and fic.vendorId = @Fuel_Id and fic.status = 1";
            string query = "select fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fic.vendorId,[dbo].[find_fuel_rate] (fic.vendorId, fic.since) as fuelRateID,(fic.quantity * [dbo].[find_fuel_rate](fic.vendorId,fic.since)) as Amount, fic.couponNo, fic.validity, fic.lastModifiedDate, fic.since from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru where fic.vehicletypeId = fv.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId =2 and fic.fuelValidityStatusId =1 and fic.vendorId  = @Fuel_Id and fic.status = 1";
            string subquery = "";
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(uptodate))
            {
                //subquery += " and fic.since between @fromdate and @uptodate";
                subquery += " and CAST(fic.since AS DATE) between @fromdate and @uptodate";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery;
            }

            string newquery = query;
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Fuel_Id", Fuel_Id);
            cmd.Parameters.AddWithValue("@fromdate", Convert.ToDateTime(fromdate));
            cmd.Parameters.AddWithValue("@uptodate", Convert.ToDateTime(uptodate));
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

    #region Fuel Circle Statement
    public DataTable bindfuelCircleStatement(string Circle_ID, string fromdate, string uptodate, string status)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            //string query = "select fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fr.fuelRateID, (fic.quantity * fr.fuelRateID) as Amount, fic.couponNo, fic.validity from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru, fuelRate fr where fic.vehicletypeId = fv.id and fic.fuelRateId = fr.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId = 2 and fic.fuelValidityStatusId = 1 and fic.circleIssueId = @Circle_ID and fic.status = 1 ";
            string query = "select fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fic.vendorId,[dbo].[find_fuel_rate] (fic.vendorId, fic.since) as fuelRateID,(fic.quantity * [dbo].[find_fuel_rate](fic.vendorId,fic.since)) as Amount, fic.couponNo, fic.validity, fic.lastModifiedDate, fic.since from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru where fic.vehicletypeId = fv.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId =@status and fic.fuelValidityStatusId =1 and fic.circleIssueId = @Circle_ID and fic.status = 1";
            string subquery = "";
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(uptodate))
            {
                subquery += "and fic.since between @fromdate and @uptodate";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery;
            }

            string newquery = query;
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Circle_ID", Circle_ID);
            cmd.Parameters.AddWithValue("@fromdate", Convert.ToDateTime(fromdate));
            cmd.Parameters.AddWithValue("@uptodate", Convert.ToDateTime(uptodate));
            cmd.Parameters.AddWithValue("@status", status);
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

    #region Fuel Full Filter
    public DataTable bindfuelFullFilterStatement(string Circle_ID, string fromdate, string uptodate, string _status, string _validity)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            //string query = "select fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fr.fuelRateID, (fic.quantity * fr.fuelRateID) as Amount, fic.couponNo, fic.validity from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru, fuelRate fr where fic.vehicletypeId = fv.id and fic.fuelRateId = fr.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId = @status and fic.fuelValidityStatusId = @validity and fic.circleIssueId = @Circle_ID and fic.status = 1 ";
            string query = "select fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fic.vendorId,[dbo].[find_fuel_rate] (fic.vendorId, fic.since) as fuelRateID,(fic.quantity * [dbo].[find_fuel_rate](fic.vendorId,fic.since)) as Amount, fic.couponNo, fic.validity, fic.lastModifiedDate, fic.since from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru where fic.vehicletypeId = fv.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId =@status and fic.fuelValidityStatusId =@validity and fic.circleIssueId = @Circle_ID and fic.status = 1";
            string subquery = "";
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(uptodate))
            {
                subquery += "and fic.since between @fromdate and @uptodate";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery;
            }

            string newquery = query;
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Circle_ID", Circle_ID);
            cmd.Parameters.AddWithValue("@status", _status);
            cmd.Parameters.AddWithValue("@validity", _validity);
            cmd.Parameters.AddWithValue("@fromdate", Convert.ToDateTime(fromdate));
            cmd.Parameters.AddWithValue("@uptodate", Convert.ToDateTime(uptodate));
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

    #region Bind Circle vehicle Status
    public DataTable bindInsuranceDetails(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fvd.id, fv.vehicle_name, fvd.restrationNo, CONVERT(VARCHAR(10), fvd.insExpiryDate, 103) AS Insurance from fuelVehicleDetails fvd, fuelvehicle fv where fvd.vehicleId=fv.id and fvd.circleIssueId= @Circle_Id", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindPolutionDetails(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fvd.id, fv.vehicle_name, fvd.restrationNo, CONVERT(VARCHAR(10), fvd.polExpiryDate, 103) AS Polution from fuelVehicleDetails fvd, fuelvehicle fv where fvd.vehicleId=fv.id and fvd.circleIssueId= @Circle_Id", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindRegistrationDetails(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fvd.id, fv.vehicle_name, fvd.restrationNo, CONVERT(VARCHAR(10), fvd.regExpiryDate, 103) AS Regstrations from fuelVehicleDetails fvd, fuelvehicle fv where fvd.vehicleId=fv.id and fvd.circleIssueId= @Circle_Id", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    #endregion

    #region Bind Dashboard data
    public DataTable bindInsuranceDetailsDash(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fvd.id as vec_Id,fv.vehicle_name, fvd.restrationNo, CONVERT(VARCHAR(10), fvd.insExpiryDate, 103) AS Insurance from fuelVehicleDetails fvd, fuelvehicle fv where fvd.vehicleId=fv.id", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindPolutionDetailsDash(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fvd.id, fv.vehicle_name, fvd.restrationNo, CONVERT(VARCHAR(10), fvd.polExpiryDate, 103) AS Polution from fuelVehicleDetails fvd, fuelvehicle fv where fvd.vehicleId=fv.id", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindRegistrationDetailsDash(string Circle_Id)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select fvd.id, fv.vehicle_name, fvd.restrationNo, CONVERT(VARCHAR(10), fvd.regExpiryDate, 103) AS Regstrations from fuelVehicleDetails fvd, fuelvehicle fv where fvd.vehicleId=fv.id", con);
            cmd.Parameters.AddWithValue("@Circle_Id", Circle_Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }
    #endregion

    public DataTable checkAuthority(string VendorLoginID)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        using (SqlConnection con = new SqlConnection(strcon))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select ppcId, nccId, kbcId, bpcId, abcId, pccId, hqtId from fuelVendorLogin where venderLoginId=@VendorLoginID and status =1", con);
            cmd.Parameters.AddWithValue("@VendorLoginID", VendorLoginID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        return dt;
    }

    public DataTable bindVehicleDetailsByCircle(string Fuel_Id, string fromdate, string uptodate)
    {
        SqlConnection con = new SqlConnection(strcon);
        DataTable dt = new DataTable();
        try
        {
            string query = "select fic.id, fv.vehicle_name, fic.vehicleNo, fru.venderName, fic.quantity, fic.couponNo, fic.validity, fic.lastModifiedDate from fuelIssueCoupon fic, fuelvehicle fv, fuelVendorLogin fru where fic.vehicletypeId = fv.id and fic.vendorId = fru.venderLoginId and fic.fuelStatusId=2 and fic.fuelValidityStatusId=1 and fic.vendorId = @Fuel_Id and fic.status = 1";
            string subquery = "";

            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(uptodate))
            {
                subquery += " and convert(varchar(10),fic.since,105) between @fromdate and @uptodate";
            }

            if (!string.IsNullOrEmpty(subquery))
            {
                query = query + " " + subquery;
            }

            string newquery = query;
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Fuel_Id", Fuel_Id);
            cmd.Parameters.AddWithValue("@fromdate", fromdate);
            cmd.Parameters.AddWithValue("@uptodate", uptodate);
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