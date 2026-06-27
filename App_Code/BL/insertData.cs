using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using PMC.DAL;

namespace PMC
{
    /// <summary>
    /// Summary description for insertData
    /// </summary>
    public class insertData
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public insertData()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public string insertPropDetail(string wardno,int wardid,string address,string pin,string email,string mobile,string cr_address,string cr_pin,int otid,int ptid,int stid,DateTime cd,string pa,string ca,int wh,int wfid,int wtid,int sid,string fy)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ward_id", wardid));
            param.Add(new SqlParameter("@address", address));
            param.Add(new SqlParameter("@pin", pin));
            param.Add(new SqlParameter("@email_id", email));
            param.Add(new SqlParameter("@mobile_no", mobile));
            param.Add(new SqlParameter("@cr_address", cr_address));
            param.Add(new SqlParameter("@cr_pin", cr_pin));
            param.Add(new SqlParameter("@ownership_type_id", otid));
            param.Add(new SqlParameter("@property_type_id", ptid));
            param.Add(new SqlParameter("@street_type_id", stid));
            param.Add(new SqlParameter("@construction_date", cd));
            param.Add(new SqlParameter("@plot_area", pa));
            param.Add(new SqlParameter("@constructed_area", ca));
            param.Add(new SqlParameter("@water_harvesting", wh));
            param.Add(new SqlParameter("@water_facility_id", wfid));
            param.Add(new SqlParameter("@water_tax_id", wtid));
            param.Add(new SqlParameter("@assessment_year", fy));
            param.Add(new SqlParameter("@street_id", sid));
            
            q = "INSERT INTO [dbo].[tbl_property_detail_tmp] ([ulb_id],[ward_id],[address],[pin],[email_id],[mobile_no],[cr_address],[cr_pin],[ownership_type_id]";
            q += ",[property_type_id],[street_type_id],[construction_date],[plot_area],[constructed_area],[water_harvesting],[water_facility_id],[water_tax_id],assessment_year";
            q += ",[application_type],[entry_date],[ip_address],[status],[user_id],[street_id],[applied_from]) ";
            q += "VALUES(1,@ward_id,@address,@pin,@email_id,@mobile_no,@cr_address,@cr_pin,@ownership_type_id,@property_type_id,@street_type_id,@construction_date,";
            q += "@plot_area,@constructed_area,@water_harvesting,@water_facility_id,@water_tax_id,@assessment_year,1,GETDATE(),convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),";
            q += "1,2,@street_id,'NOLP');select @@IDENTITY";
            dac = new DataAccessLayer();
            long Prop_Temp_ID=Convert.ToInt64(dac.Scalar(q, param));

            //update application no in property temp
            string app_no = "PMC/"+wardno+"/"+Prop_Temp_ID;
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@application_no", app_no));
            param.Add(new SqlParameter("@id", Prop_Temp_ID));
            q = "update [dbo].[tbl_property_detail_tmp] set application_no=@application_no where id=@id";
            dac = new DataAccessLayer();
            dac.update(q, param);

            return app_no;

        }
        public void insertPropGPS(long prop_id,string latitude,string longitude)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Prop_Temp_Id", prop_id));
            param.Add(new SqlParameter("@Property_GPS_Latitude", latitude));
            param.Add(new SqlParameter("@Property_GPS_Longitude", longitude));
            q = "INSERT INTO [dbo].[tbl_property_gps_tmp]([Prop_Temp_Id],[Property_GPS_Latitude],[Property_GPS_Longitude],[Entry_Date])";
            q += " Values(@Prop_Temp_Id,@Property_GPS_Latitude,@Property_GPS_Longitude,GETDATE())";
            dac = new DataAccessLayer();
            dac.update(q,param);

        }
        public void insertPropOthers(long prop_id, int bid)
        {
            BuildingMasterController bmc = new BuildingMasterController();
            DataTable dtBuild = bmc.GetBuilding(bid);

            if (dtBuild.Rows.Count > 0)
            {
                string q = "";
                //insert into property temp
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@property_id", prop_id));
                param.Add(new SqlParameter("@no_of_flat", Convert.ToInt32(dtBuild.Rows[0]["no_of_flat"].ToString())));
                param.Add(new SqlParameter("@building_name", dtBuild.Rows[0]["building_name"].ToString()));
                param.Add(new SqlParameter("@building_id", bid));
                q = "INSERT INTO [dbo].[tbl_property_detail_others_tmp]([ulb_id],[property_id],[no_of_flat],[building_name],[user_id],[entry_date],[ip_address],[building_id]) ";
                q += "Values(1,@property_id,@no_of_flat,@building_name,2,GETDATE(),convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),@building_id)";
                dac = new DataAccessLayer();
                dac.update(q, param);
            }

        }
        public void insertWaterTax(long prop_id, string am)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            param.Add(new SqlParameter("@amount", am));
            
            q = "INSERT INTO [dbo].[tbl_water_tax_detail_tmp]([property_id],[amount],[entry_date],[user_id],[paid_status],[status])";
            q += " Values(@property_id,@amount,GETDATE(),2,0,1)";
            dac = new DataAccessLayer();
            dac.update(q, param);

        }
        public void insertOccupancy(long prop_id, int floor_id, int use_type_id, int usage_type_id, int occupancy_type_id, int construction_type_id, decimal builtup_area, string from_year, string upto_year, string effect_from)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            param.Add(new SqlParameter("@floor_id", floor_id));
            param.Add(new SqlParameter("@use_type_id", use_type_id));
            param.Add(new SqlParameter("@usage_type_id", usage_type_id));
            param.Add(new SqlParameter("@occupancy_type_id", occupancy_type_id));
            param.Add(new SqlParameter("@construction_type_id", construction_type_id));
            param.Add(new SqlParameter("@builtup_area", builtup_area));
            param.Add(new SqlParameter("@from_year", from_year));
            param.Add(new SqlParameter("@upto_year", upto_year));
            param.Add(new SqlParameter("@effect_from", effect_from));
            

            q = "INSERT INTO [dbo].[tbl_occupancy_detail_tmp]([property_id],[floor_id],[use_type_id],[usage_type_id],[occupancy_type_id]";
            q+=",[construction_type_id],[builtup_area],[from_year],[upto_year],[status],[user_id],[entry_date],[effect_from]) ";
            q += " Values(@property_id,@floor_id,@use_type_id,@usage_type_id,@occupancy_type_id,@construction_type_id,@builtup_area,@from_year";
            q += ",@upto_year,1,2,GETDATE(),@effect_from)";
            dac = new DataAccessLayer();
            dac.update(q, param);

        }
        public long insertOwner(long prop_id, string gender,string oname,string guard_name,string rel,string aadhar,string electricity)
        {
            string q = "";
            long owener_id;
            
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            param.Add(new SqlParameter("@gender", gender));
            param.Add(new SqlParameter("@owner_name", oname));
            param.Add(new SqlParameter("@guardian_name", guard_name));
            param.Add(new SqlParameter("@relation", rel));
            param.Add(new SqlParameter("@aadhar_no", aadhar));
            param.Add(new SqlParameter("@electricity_no", electricity));

            q = "INSERT INTO [dbo].[tbl_owner_detail_tmp]([property_id],[gender],[owner_name],[guardian_name],[relation],[aadhar_no]";
            q += ",[electricity_no],[user_id],[entry_date],[status]) ";
            q += "Values(@property_id,@gender,@owner_name,@guardian_name,@relation,@aadhar_no,@electricity_no,2,GETDATE(),1);select @@IDENTITY ";
            dac = new DataAccessLayer();
            owener_id=Convert.ToInt64(dac.Scalar(q, param));
            return owener_id;

        }
        public void deleteOwner(long prop_id)
        {
            string q = "";
            //delete before new insert
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            q = "delete from [dbo].[tbl_owner_detail_tmp] where [property_id]=@property_id";
            dac = new DataAccessLayer();
            dac.update(q, param);
        }
        public void insertDoc(long prop_id,int document_master_id,string doc_name,int document_detail_id,long owner_id)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            param.Add(new SqlParameter("@document_master_id", document_master_id));
            param.Add(new SqlParameter("@doc_name", doc_name));
            param.Add(new SqlParameter("@document_detail_id", document_detail_id));
            param.Add(new SqlParameter("@owner_id", owner_id));

            q = "INSERT INTO [dbo].[tbl_prop_document_details_tmp]([property_id],[document_master_id],[doc_name],[entry_date],[user_id],[status]";
            q+=",[document_detail_id],[owner_id]) ";
            q += " Values(@property_id,@document_master_id,@doc_name,GETDATE(),2,1,@document_detail_id,@owner_id)";

            dac = new DataAccessLayer();
            dac.update(q, param);
        }
        public void insertOtherDoc(long prop_id, int document_master_id, string doc_name, int document_detail_id)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            param.Add(new SqlParameter("@document_master_id", document_master_id));
            param.Add(new SqlParameter("@doc_name", doc_name));
            param.Add(new SqlParameter("@document_detail_id", document_detail_id));
            

            q = "INSERT INTO [dbo].[tbl_prop_document_details_tmp]([property_id],[document_master_id],[doc_name],[entry_date],[user_id],[status]";
            q += ",[document_detail_id]) ";
            q += " Values(@property_id,@document_master_id,@doc_name,GETDATE(),2,1,@document_detail_id)";

            dac = new DataAccessLayer();
            dac.update(q, param);
        }
        public int getDocMasterID(int ptype,int otype,string doc_name)
        {
            int docmid;
            string q = "";
            
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@doc_name", doc_name));
            param.Add(new SqlParameter("@property_type_id", ptype));
            param.Add(new SqlParameter("@ownership_type_id", otype));

            q = "select id from tbl_document_master where application_type='1' and doc_name=@doc_name and property_type_id=@property_type_id and ownership_type_id=@ownership_type_id and status=1";
            dac = new DataAccessLayer();
            docmid = Convert.ToInt32(dac.Scalar(q, param));
            return docmid;
        }
        public int getDocID(int doc_mst_id, string doc_name)
        {
            int docid;
            string q = "";

            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@doc_master_id", doc_mst_id));
            param.Add(new SqlParameter("@document_name", doc_name));
            
            q = "select id from tbl_document_detail where doc_master_id=@doc_master_id and document_name=@document_name and status=1";
            dac = new DataAccessLayer();
            docid = Convert.ToInt32(dac.Scalar(q, param));
            return docid;
        }
        public void deleteDoc(long prop_id)
        {
            string q = "";
            //delete before new insert
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            q = "delete from [dbo].[tbl_prop_document_details_tmp] where [property_id]=@property_id";
            dac = new DataAccessLayer();
            dac.update(q, param);
        }
        public void deleteOccupancy(long prop_id)
        {
            string q = "";
            //delete before new insert
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            q = "delete from [dbo].[tbl_occupancy_detail_tmp] where [property_id]=@property_id";
            dac = new DataAccessLayer();
            dac.update(q, param);
        }
        public string updatePropDetail(long id, string wardno, int wardid, string address, string pin, string email, string mobile, string cr_address, string cr_pin, int otid, int ptid, int stid, DateTime cd, string pa, string ca, int wh, int wfid, int wtid, int sid,string fy)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ward_id", wardid));
            param.Add(new SqlParameter("@address", address));
            param.Add(new SqlParameter("@pin", pin));
            param.Add(new SqlParameter("@email_id", email));
            param.Add(new SqlParameter("@mobile_no", mobile));
            param.Add(new SqlParameter("@cr_address", cr_address));
            param.Add(new SqlParameter("@cr_pin", cr_pin));
            param.Add(new SqlParameter("@ownership_type_id", otid));
            param.Add(new SqlParameter("@property_type_id", ptid));
            param.Add(new SqlParameter("@street_type_id", stid));
            param.Add(new SqlParameter("@construction_date", cd));
            param.Add(new SqlParameter("@plot_area", pa));
            param.Add(new SqlParameter("@constructed_area", ca));
            param.Add(new SqlParameter("@water_harvesting", wh));
            param.Add(new SqlParameter("@water_facility_id", wfid));
            param.Add(new SqlParameter("@water_tax_id", wtid));
            param.Add(new SqlParameter("@street_id", sid));
            string app_no = "PMC/" + wardno + "/" + id;
            param.Add(new SqlParameter("@application_no", app_no));
            param.Add(new SqlParameter("@assessment_year", fy));
            param.Add(new SqlParameter("@id", id));

            q = "UPDATE [dbo].[tbl_property_detail_tmp] set [ward_id]=@ward_id,[address]=@address,[pin]=@pin,[email_id]=@email_id,[mobile_no]=@mobile_no";
            q += ",[cr_address]=@cr_address,[cr_pin]=@cr_pin,[ownership_type_id]=@ownership_type_id";
            q += ",[property_type_id]=@property_type_id,[street_type_id]=@street_type_id,[construction_date]=@construction_date,[plot_area]=@plot_area";
            q += ",[constructed_area]=@constructed_area,[water_harvesting]=@water_harvesting,[water_facility_id]=@water_facility_id,[water_tax_id]=@water_tax_id";
            q += ",[entry_date]=GETDATE(),[street_id]=@street_id,application_no=@application_no,assessment_year=@assessment_year where id=@id ";
            
            dac = new DataAccessLayer();
            dac.update(q, param);

            return app_no;

        }
        public void updatePropGPS(long prop_id, string latitude, string longitude)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Prop_Temp_Id", prop_id));
            param.Add(new SqlParameter("@Property_GPS_Latitude", latitude));
            param.Add(new SqlParameter("@Property_GPS_Longitude", longitude));
            q = "Update [dbo].[tbl_property_gps_tmp] set [Property_GPS_Latitude]=@Property_GPS_Latitude,[Property_GPS_Longitude]=@Property_GPS_Longitude,[Entry_Date]=GETDATE() ";
            q += " where [Prop_Temp_Id]=@Prop_Temp_Id";
            dac = new DataAccessLayer();
            dac.update(q, param);

        }
        public void updatePropOthers(long prop_id, int bid)
        {
            BuildingMasterController bmc = new BuildingMasterController();
            DataTable dtBuild = bmc.GetBuilding(bid);

            if (dtBuild.Rows.Count > 0)
            {
                string q = "";
                //insert into property temp
                q = "";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@property_id", prop_id));
                param.Add(new SqlParameter("@no_of_flat", Convert.ToInt32(dtBuild.Rows[0]["no_of_flat"].ToString())));
                param.Add(new SqlParameter("@building_name", dtBuild.Rows[0]["building_name"].ToString()));
                param.Add(new SqlParameter("@building_id", bid));
                q = "update [dbo].[tbl_property_detail_others_tmp] set [no_of_flat]=@no_of_flat,[building_name]=@building_name,[entry_date]=GETDATE(),[ip_address]=convert(varchar(100),CONNECTIONPROPERTY('client_net_address')),[building_id]=@building_id ";
                q += " where property_id=@property_id";
                dac = new DataAccessLayer();
                dac.update(q, param);
            }

        }
        public void updateWaterTax(long prop_id, string am)
        {
            string q = "";
            //insert into property temp
            q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            param.Add(new SqlParameter("@amount", am));

            q = "update [dbo].[tbl_water_tax_detail_tmp] set [amount]=@amount,[entry_date]=GETDATE(),[user_id]=2,[paid_status]=0,[status]=1";
            q += " where [property_id]=@property_id";
            dac = new DataAccessLayer();
            dac.update(q, param);

        }
        public int chkBuild(long prop_id)
        {
            int i = 0;
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            q = "select * from tbl_property_detail_others_tmp where property_id=@property_id";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            if (dt.Rows.Count > 0)
            {
                i = 1;
            }
            return i;
        }
        public void delBuild(long prop_id)
        {
            
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@property_id", prop_id));
            q = "delete from tbl_property_detail_others_tmp where property_id=@property_id";
            dac = new DataAccessLayer();
            dac.update(q, param);
            
        }
    }
    public class paymentRequestParameter
    {
        public string encryptedValue;
        public string accessKey;
    }
}