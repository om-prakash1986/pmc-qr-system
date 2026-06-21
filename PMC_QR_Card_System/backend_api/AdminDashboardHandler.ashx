<%@ WebHandler Language="C#" Class="AdminDashboardHandler" %>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// AdminDashboardHandler.ashx
/// Code-behind for the PMC Admin Dashboard aggregate API.
///
/// Endpoint: GET /AdminDashboardHandler.ashx?action=<action>&period=<day|month|year>&date=YYYY-MM-DD
/// </summary>
public class AdminDashboardHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }

        if (context.Request.HttpMethod != "GET")
        {
            Send(context, 405, false, "Only GET is allowed", null);
            return;
        }

        string action  = (context.Request.QueryString["action"] ?? "").ToLower().Trim();
        string period  = (context.Request.QueryString["period"] ?? "month").ToLower().Trim();
        string dateStr = context.Request.QueryString["date"] ?? DateTime.Today.ToString("yyyy-MM-dd");

        DateTime selectedDate;
        if (!DateTime.TryParse(dateStr, out selectedDate))
            selectedDate = DateTime.Today;

        string connStr = System.Configuration.ConfigurationManager
                                .ConnectionStrings["PTax"].ConnectionString;

        try
        {
            switch (action)
            {
                case "kpis":           HandleKpis(context, connStr, period, selectedDate);          break;
                case "activators":     HandleActivators(context, connStr, period, selectedDate);    break;
                case "tax_collectors": HandleTaxCollectors(context, connStr);                       break;
                case "citizens":       HandleCitizens(context, connStr);                            break;
                case "revenue":        HandleRevenue(context, connStr, period, selectedDate);       break;
                case "card_status":    HandleCardStatus(context, connStr);                          break;
                default:               HandleAll(context, connStr, period, selectedDate);           break;
            }
        }
        catch (Exception ex)
        {
            Send(context, 500, false, "Database error: " + ex.Message, null);
        }
    }

    private void HandleAll(HttpContext context, string connStr, string period, DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            var result = new Dictionary<string, object>
            {
                { "kpis",          GetKpis(conn, period, date)          },
                { "activators",    GetActivators(conn, period, date)    },
                { "taxCollectors", GetTaxCollectors(conn)               },
                { "citizens",      GetCitizens(conn)                    },
                { "revenue",       GetRevenue(conn, period, date)       },
                { "cardStatus",    GetCardStatus(conn)                  },
            };
            Send(context, 200, true, "Dashboard data loaded", result);
        }
    }

    private void HandleKpis(HttpContext context, string connStr, string period, DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            Send(context, 200, true, "KPIs loaded", GetKpis(conn, period, date));
        }
    }

    private Dictionary<string, object> GetKpis(SqlConnection conn, string period, DateTime date)
    {
        int     totalActivations    = ExecScalar<int>(conn,     "SELECT COUNT(*) FROM QRMaster WHERE Status='ACTIVATED'");
        int     todayActivations    = ExecScalar<int>(conn,     "SELECT COUNT(*) FROM tbl_QR_ScanLog WHERE Scan_Type='ACTIVATION' AND CAST(Scan_Timestamp AS DATE)=CAST(GETDATE() AS DATE)");
        int     activeTaxCollectors = ExecScalar<int>(conn,     "SELECT COUNT(*) FROM tbl_QR_StaffUsers WHERE IsActive=1 AND Role IN ('TAX_COLLECTOR','TAXCOLLECTOR','Tax Collector')");
        int     totalActivators     = ExecScalar<int>(conn,     "SELECT COUNT(*) FROM tbl_QR_StaffUsers WHERE IsActive=1");
        int     citizensRegistered  = ExecScalar<int>(conn,     "SELECT COUNT(DISTINCT PropertyId) FROM QRMaster WHERE Status='ACTIVATED'");
        int     totalCitizens       = ExecScalar<int>(conn,     "SELECT COUNT(*) FROM tbl_property_detail WHERE status IN (1,2,3,4)");
        decimal totalRevenue        = ExecScalar<decimal>(conn, "SELECT ISNULL(SUM(CAST(y.total_tax AS DECIMAL(18,2))),0) FROM tbl_yearly_tax_assessment y JOIN tbl_property_detail pd ON pd.id = y.property_id WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4)");
        decimal monthRevenue        = ExecScalar<decimal>(conn, @"SELECT ISNULL(SUM(CAST(y.total_tax AS DECIMAL(18,2))),0) FROM tbl_yearly_tax_assessment y JOIN tbl_property_detail pd ON pd.id = y.property_id WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4) AND MONTH(ISNULL(pd.last_payment_date,GETDATE()))=MONTH(GETDATE()) AND YEAR(ISNULL(pd.last_payment_date,GETDATE()))=YEAR(GETDATE())");
        decimal todayRevenue        = ExecScalar<decimal>(conn, @"SELECT ISNULL(SUM(CAST(y.total_tax AS DECIMAL(18,2))),0) FROM tbl_yearly_tax_assessment y JOIN tbl_property_detail pd ON pd.id = y.property_id WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4) AND CAST(ISNULL(pd.last_payment_date,GETDATE()) AS DATE)=CAST(GETDATE() AS DATE)");

        return new Dictionary<string, object>
        {
            { "totalActivations",    totalActivations    },
            { "todayActivations",    todayActivations    },
            { "activeTaxCollectors", activeTaxCollectors },
            { "totalActivators",     totalActivators     },
            { "citizensRegistered",  citizensRegistered  },
            { "totalCitizens",       totalCitizens       },
            { "totalRevenue",        totalRevenue        },
            { "monthRevenue",        monthRevenue        },
            { "todayRevenue",        todayRevenue        },
        };
    }

    private void HandleActivators(HttpContext context, string connStr, string period, DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            Send(context, 200, true, "Activators loaded", GetActivators(conn, period, date));
        }
    }

    private List<Dictionary<string, object>> GetActivators(SqlConnection conn, string period, DateTime date)
    {
        string query = @"
            SELECT
                u.StaffId, u.LoginId, u.FullName, u.Ward_No, u.Circle,
                u.Mobile, u.IsActive, u.Last_Login,
                ISNULL(today.cnt,   0) AS Today_Count,
                ISNULL(month_c.cnt, 0) AS Month_Count,
                ISNULL(total_c.cnt, 0) AS Total_Count
            FROM tbl_QR_StaffUsers u
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt FROM tbl_QR_ScanLog
                WHERE Scan_Type='ACTIVATION' AND CAST(Scan_Timestamp AS DATE)=CAST(GETDATE() AS DATE)
                GROUP BY Staff_Id
            ) today   ON today.Staff_Id   = u.LoginId
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt FROM tbl_QR_ScanLog
                WHERE Scan_Type='ACTIVATION'
                  AND MONTH(Scan_Timestamp)=MONTH(GETDATE())
                  AND YEAR(Scan_Timestamp)=YEAR(GETDATE())
                GROUP BY Staff_Id
            ) month_c ON month_c.Staff_Id = u.LoginId
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt FROM tbl_QR_ScanLog
                WHERE Scan_Type='ACTIVATION'
                GROUP BY Staff_Id
            ) total_c ON total_c.Staff_Id = u.LoginId
            ORDER BY Today_Count DESC, Total_Count DESC";

        var list = new List<Dictionary<string, object>>();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                bool   isActive   = r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"]);
                string lastLogin  = r["Last_Login"] == DBNull.Value ? "-"
                    : Convert.ToDateTime(r["Last_Login"]).ToString("dd MMM yyyy, HH:mm");

                list.Add(new Dictionary<string, object>
                {
                    { "staffId",   r["StaffId"].ToString()                                              },
                    { "loginId",   r["LoginId"].ToString()                                              },
                    { "fullName",  r["FullName"].ToString()                                             },
                    { "wardNo",    r["Ward_No"] == DBNull.Value ? "" : r["Ward_No"].ToString()          },
                    { "circle",    r["Circle"]  == DBNull.Value ? "" : r["Circle"].ToString()           },
                    { "mobile",    r["Mobile"]  == DBNull.Value ? "" : r["Mobile"].ToString()           },
                    { "status",    isActive ? "Active" : "Inactive"                                     },
                    { "lastLogin", lastLogin                                                             },
                    { "today",     Convert.ToInt32(r["Today_Count"])                                    },
                    { "month",     Convert.ToInt32(r["Month_Count"])                                    },
                    { "total",     Convert.ToInt32(r["Total_Count"])                                    },
                });
            }
        }
        return list;
    }

    private void HandleTaxCollectors(HttpContext context, string connStr)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            Send(context, 200, true, "Tax collectors loaded", GetTaxCollectors(conn));
        }
    }

    private List<Dictionary<string, object>> GetTaxCollectors(SqlConnection conn)
    {
        string query = @"
            SELECT
                u.StaffId, u.LoginId, u.FullName, u.Ward_No, u.Circle, u.IsActive,
                ISNULL(visits.cnt,    0) AS Citizens_Visited,
                ISNULL(rev.collected, 0) AS Collected,
                ISNULL(tgt.target,    0) AS Target
            FROM tbl_QR_StaffUsers u
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt FROM tbl_QR_ScanLog
                WHERE Scan_Type IN ('TAX_COLLECTION','CITIZEN_VISIT','CITIZEN_PORTAL_SCAN')
                GROUP BY Staff_Id
            ) visits ON visits.Staff_Id = u.LoginId
            LEFT JOIN (
                SELECT pd.ward_id, SUM(CAST(y.total_tax AS DECIMAL(18,2))) AS collected
                FROM tbl_yearly_tax_assessment y
                JOIN tbl_property_detail pd ON pd.id = y.property_id
                WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4)
                GROUP BY pd.ward_id
            ) rev ON rev.ward_id = CAST(u.Ward_No AS VARCHAR(50))
            LEFT JOIN (
                SELECT pd.ward_id, SUM(CAST(y.total_tax AS DECIMAL(18,2))) AS target
                FROM tbl_yearly_tax_assessment y
                JOIN tbl_property_detail pd ON pd.id = y.property_id
                WHERE y.paid_status = 0 AND pd.status IN (1,2,3,4)
                GROUP BY pd.ward_id
            ) tgt ON tgt.ward_id = CAST(u.Ward_No AS VARCHAR(50))
            WHERE u.IsActive = 1
            ORDER BY Collected DESC";

        var list = new List<Dictionary<string, object>>();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                bool    isActive  = r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"]);
                decimal collected = r["Collected"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Collected"]);
                decimal target    = r["Target"]    == DBNull.Value ? 0 : Convert.ToDecimal(r["Target"]);

                list.Add(new Dictionary<string, object>
                {
                    { "staffId",   r["StaffId"].ToString()                                           },
                    { "loginId",   r["LoginId"].ToString()                                           },
                    { "fullName",  r["FullName"].ToString()                                          },
                    { "wardNo",    r["Ward_No"] == DBNull.Value ? "" : r["Ward_No"].ToString()       },
                    { "circle",    r["Circle"]  == DBNull.Value ? "" : r["Circle"].ToString()        },
                    { "status",    isActive ? "Active" : "Inactive"                                  },
                    { "visited",   Convert.ToInt32(r["Citizens_Visited"])                            },
                    { "collected", collected                                                          },
                    { "target",    target                                                             },
                });
            }
        }
        return list;
    }

    private void HandleCitizens(HttpContext context, string connStr)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            Send(context, 200, true, "Citizens loaded", GetCitizens(conn));
        }
    }

    private List<Dictionary<string, object>> GetCitizens(SqlConnection conn)
    {
        string query = @"
            SELECT TOP 200
                pd.pid AS PID, 
                own.owner_name AS Owner_Name, 
                own.guardian_name AS Guardian_Name, 
                own.mobile_no AS Mobile_No,
                pd.address AS Address, 
                pd.ward_id AS Ward_No, 
                '' AS Circle,
                ISNULL((SELECT SUM(total_tax) FROM tbl_yearly_tax_assessment WHERE property_id = pd.id AND paid_status = 0), 0) AS Total_Dues, 
                pd.assessment_year, 
                q.Status AS QR_Status, q.QRId, q.QRToken
            FROM tbl_property_detail pd
            LEFT JOIN tbl_owner_detail own ON pd.id = own.property_id
            LEFT JOIN QRMaster q ON q.PropertyId = pd.pid AND q.Status='ACTIVATED'
            WHERE pd.status IN (1,2,3,4)
            ORDER BY ISNULL(q.ActivatedDate, '1900-01-01') DESC, pd.pid DESC";

        var list = new List<Dictionary<string, object>>();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                string qrStatus   = r["QR_Status"]   == DBNull.Value ? "Not Assigned" : r["QR_Status"].ToString();
                
                string assessmentYear = r["assessment_year"] == DBNull.Value ? "" : r["assessment_year"].ToString();
                string currentYear = DateTime.Now.Year.ToString();
                string paymentStatus = assessmentYear.Contains(currentYear) ? "Paid" : "Outstanding";

                list.Add(new Dictionary<string, object>
                {
                    { "pid",           r["PID"].ToString()                                                        },
                    { "ownerName",     r["Owner_Name"].ToString()                                                 },
                    { "guardianName",  r["Guardian_Name"] == DBNull.Value ? "" : r["Guardian_Name"].ToString()    },
                    { "mobileNo",      r["Mobile_No"]     == DBNull.Value ? "" : r["Mobile_No"].ToString()        },
                    { "address",       r["Address"]        == DBNull.Value ? "" : r["Address"].ToString()         },
                    { "wardNo",        r["Ward_No"]        == DBNull.Value ? "" : r["Ward_No"].ToString()         },
                    { "circle",        r["Circle"]         == DBNull.Value ? "" : r["Circle"].ToString()          },
                    { "totalDues",     r["Total_Dues"]     == DBNull.Value ? 0  : Convert.ToDecimal(r["Total_Dues"]) },
                    { "paymentStatus", paymentStatus },
                    { "qrStatus",      qrStatus                                                                   },
                    { "qrId",          r["QRId"]    == DBNull.Value ? "" : r["QRId"].ToString()                   },
                    { "qrToken",       r["QRToken"] == DBNull.Value ? "" : r["QRToken"].ToString()                },
                });
            }
        }
        return list;
    }

    private void HandleRevenue(HttpContext context, string connStr, string period, DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            Send(context, 200, true, "Revenue loaded", GetRevenue(conn, period, date));
        }
    }

    private Dictionary<string, object> GetRevenue(SqlConnection conn, string period, DateTime date)
    {
        string wardQuery = @"
            SELECT
                pd.ward_id AS Ward_No,
                '' AS Circle,
                COUNT(DISTINCT pd.pid) AS Properties,
                ISNULL(SUM(CASE WHEN y.paid_status = 1 THEN CAST(y.total_tax AS DECIMAL(18,2)) ELSE 0 END),0) AS Collected,
                ISNULL(SUM(CASE WHEN y.paid_status = 0 THEN CAST(y.total_tax AS DECIMAL(18,2)) ELSE 0 END),0) AS Target
            FROM tbl_property_detail pd
            LEFT JOIN tbl_yearly_tax_assessment y ON y.property_id = pd.id
            WHERE pd.ward_id IS NOT NULL AND pd.ward_id <> '' AND pd.ward_id <> '0' AND pd.status IN (1,2,3,4)
            GROUP BY pd.ward_id
            ORDER BY pd.ward_id";

        var wardBreakdown = new List<Dictionary<string, object>>();
        using (SqlCommand cmd = new SqlCommand(wardQuery, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                wardBreakdown.Add(new Dictionary<string, object>
                {
                    { "ward",       r["Ward_No"].ToString()                                           },
                    { "circle",     r["Circle"] == DBNull.Value ? "" : r["Circle"].ToString()         },
                    { "properties", Convert.ToInt32(r["Properties"])                                  },
                    { "collected",  Convert.ToDecimal(r["Collected"])                                 },
                    { "target",     Convert.ToDecimal(r["Target"])                                    },
                });
            }
        }

        string trendQuery;
        if (period == "day")
        {
            trendQuery = @"SELECT DATEPART(HOUR,Scan_Timestamp) AS Period_Label, COUNT(*) AS Activations
                           FROM tbl_QR_ScanLog
                           WHERE CAST(Scan_Timestamp AS DATE)=CAST(GETDATE() AS DATE) AND Scan_Type='ACTIVATION'
                           GROUP BY DATEPART(HOUR,Scan_Timestamp) ORDER BY Period_Label";
        }
        else if (period == "month")
        {
            trendQuery = @"SELECT DAY(Scan_Timestamp) AS Period_Label, COUNT(*) AS Activations
                           FROM tbl_QR_ScanLog
                           WHERE MONTH(Scan_Timestamp)=MONTH(GETDATE()) AND YEAR(Scan_Timestamp)=YEAR(GETDATE()) AND Scan_Type='ACTIVATION'
                           GROUP BY DAY(Scan_Timestamp) ORDER BY Period_Label";
        }
        else
        {
            trendQuery = @"SELECT MONTH(Scan_Timestamp) AS Period_Label, COUNT(*) AS Activations
                           FROM tbl_QR_ScanLog
                           WHERE YEAR(Scan_Timestamp)=YEAR(GETDATE()) AND Scan_Type='ACTIVATION'
                           GROUP BY MONTH(Scan_Timestamp) ORDER BY Period_Label";
        }

        string[] monthNames = { "Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };
        var trendLabels = new List<string>();
        var trendValues = new List<int>();

        using (SqlCommand cmd = new SqlCommand(trendQuery, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                string lbl = r["Period_Label"].ToString();
                if (period == "day")  lbl = lbl + ":00";
                if (period == "year") { int m = Convert.ToInt32(lbl) - 1; lbl = (m >= 0 && m < 12) ? monthNames[m] : lbl; }
                trendLabels.Add(lbl);
                trendValues.Add(Convert.ToInt32(r["Activations"]));
            }
        }

        decimal todayRevenue = ExecScalar<decimal>(conn, @"SELECT ISNULL(SUM(CAST(y.total_tax AS DECIMAL(18,2))),0) FROM tbl_yearly_tax_assessment y JOIN tbl_property_detail pd ON pd.id = y.property_id WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4) AND CAST(ISNULL(pd.last_payment_date,GETDATE()) AS DATE)=CAST(GETDATE() AS DATE)");
        decimal monthRevenue = ExecScalar<decimal>(conn, @"SELECT ISNULL(SUM(CAST(y.total_tax AS DECIMAL(18,2))),0) FROM tbl_yearly_tax_assessment y JOIN tbl_property_detail pd ON pd.id = y.property_id WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4) AND MONTH(ISNULL(pd.last_payment_date,GETDATE()))=MONTH(GETDATE()) AND YEAR(ISNULL(pd.last_payment_date,GETDATE()))=YEAR(GETDATE())");
        decimal yearRevenue  = ExecScalar<decimal>(conn, @"SELECT ISNULL(SUM(CAST(y.total_tax AS DECIMAL(18,2))),0) FROM tbl_yearly_tax_assessment y JOIN tbl_property_detail pd ON pd.id = y.property_id WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4) AND YEAR(ISNULL(pd.last_payment_date,GETDATE()))=YEAR(GETDATE())");
        decimal totalRevenue = ExecScalar<decimal>(conn, @"SELECT ISNULL(SUM(CAST(y.total_tax AS DECIMAL(18,2))),0) FROM tbl_yearly_tax_assessment y JOIN tbl_property_detail pd ON pd.id = y.property_id WHERE y.paid_status = 1 AND pd.status IN (1,2,3,4)");

        return new Dictionary<string, object>
        {
            { "wardBreakdown", wardBreakdown },
            { "trendLabels",   trendLabels   },
            { "trendValues",   trendValues   },
            { "todayRevenue",  todayRevenue  },
            { "monthRevenue",  monthRevenue  },
            { "yearRevenue",   yearRevenue   },
            { "totalRevenue",  totalRevenue  },
        };
    }

    private void HandleCardStatus(HttpContext context, string connStr)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            Send(context, 200, true, "Card status loaded", GetCardStatus(conn));
        }
    }

    private Dictionary<string, object> GetCardStatus(SqlConnection conn)
    {
        string query = "SELECT Status, COUNT(*) AS Cnt FROM QRMaster GROUP BY Status";

        var statusMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "ACTIVATED",  0 },
            { "UNASSIGNED", 0 },
            { "REPLACED",   0 },
            { "PENDING",    0 },
        };

        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                string s   = r["Status"].ToString().ToUpper().Trim();
                int    cnt = Convert.ToInt32(r["Cnt"]);
                if (statusMap.ContainsKey(s)) statusMap[s] = cnt;
                else statusMap["PENDING"] += cnt;
            }
        }

        return new Dictionary<string, object>
        {
            { "activated",  statusMap["ACTIVATED"]  },
            { "unassigned", statusMap["UNASSIGNED"] },
            { "replaced",   statusMap["REPLACED"]   },
            { "pending",    statusMap["PENDING"]    },
        };
    }

    private T ExecScalar<T>(SqlConnection conn, string sql)
    {
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            object val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) return default(T);
            return (T)Convert.ChangeType(val, typeof(T));
        }
    }

    private void Send(HttpContext context, int code, bool success, string msg, object data)
    {
        context.Response.StatusCode = code;
        var ser = new JavaScriptSerializer { MaxJsonLength = 10000000 };
        context.Response.Write(ser.Serialize(new Dictionary<string, object>
        {
            { "success", success },
            { "message", msg     },
            { "data",    data    },
        }));
    }

    public bool IsReusable { get { return false; } }
}
