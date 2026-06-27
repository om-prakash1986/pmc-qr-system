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
    public class AssessmentRebatePenaltyController
    {
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        public AssessmentRebatePenaltyController()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
