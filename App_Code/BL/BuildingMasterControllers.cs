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
    /// Summary description for FloorMaster
    /// </summary>
    public class BuildingMasterController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public BuildingMasterController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static int Insert()
        {
            int ID = 0;

            try
            {


            }
            catch (Exception ex) { string mes = ex.Message.ToString(); }
            return ID;
        }
        public DataTable GetBuilding()
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            q = "select id, ulb_id, circle_id, ward_id, building_name, no_of_flat, address, road_type_id, entry_date, status, code from tbl_building_master where status=1 order by building_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetBuilding(int ID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@id", ID));
            q = "select id, ulb_id, circle_id, ward_id, building_name, no_of_flat, address, road_type_id, entry_date, status, code from tbl_building_master where status=1 and id=@id order by building_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetBuilding(int circleID, int wardID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@circle_id", circleID));
            param.Add(new SqlParameter("@ward_id", wardID));
            q = "select id,building_name, no_of_flat, address, road_type_id from tbl_building_master where status=1 and circle_id=@circle_id and ward_id=@ward_id  order by building_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetBuilding(int circleID, int wardID, int roadTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@circle_id", circleID));
            param.Add(new SqlParameter("@ward_id", wardID));
            param.Add(new SqlParameter("@road_type_id", roadTypeID));
            q = "select id,building_name, no_of_flat, address, road_type_id from tbl_building_master where status=1 and circle_id=@circle_id and ward_id=@ward_id  order by building_name ASC";// and road_type_id=@road_type_id
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
        public DataTable GetBuildingByRoadID(int roadTypeID)
        {
            dt = new DataTable();
            string q = "";
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@road_type_id", roadTypeID));
            q = "select building_name, no_of_flat, address, road_type_id from tbl_building_master where status=1 and road_type_id=@road_type_id  order by building_name ASC";
            dac = new DataAccessLayer();
            dt = dac.GetDataTable(q, param);
            return dt;
        }
    }
}

