<%@ WebHandler Language="C#" Class="AdminDashboardHandler" %>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// AdminDashboardHandler.ashx
/// Exposes aggregate KPI data for the PMC Admin Dashboard.
///
/// Endpoint:  GET /AdminDashboardHandler.ashx?action=&lt;action&gt;&period=&lt;day|month|year&gt;&date=YYYY-MM-DD
///
/// Actions:
///   kpis          – Total activations, active collectors, citizens count, total revenue
///   activators    – Per-agent activation counts (today / this month / all-time)
///   tax_collectors – Tax collector list with collection stats
///   citizens      – Citizen/property list from All_Demand
///   revenue       – Revenue summary by ward + time-series trend
///   card_status   – QR card status breakdown for donut chart
/// </summary>
public class AdminDashboardHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        // CORS headers – allow admin dashboard origin
        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        context.Response.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, X-Admin-Token");

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

        string action = (context.Request.QueryString["action"] ?? "").ToLower().Trim();
        string period = (context.Request.QueryString["period"] ?? "month").ToLower().Trim(); // day | month | year
        string dateStr = context.Request.QueryString["date"] ?? DateTime.Today.ToString("yyyy-MM-dd");

        DateTime selectedDate;
        if (!DateTime.TryParse(dateStr, out selectedDate))
            selectedDate = DateTime.Today;

        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

        try
        {
            switch (action)
            {
                case "kpis":
                    HandleKpis(context, connStr, period, selectedDate);
                    break;
                case "activators":
                    HandleActivators(context, connStr, period, selectedDate);
                    break;
                case "tax_collectors":
                    HandleTaxCollectors(context, connStr);
                    break;
                case "citizens":
                    HandleCitizens(context, connStr);
                    break;
                case "revenue":
                    HandleRevenue(context, connStr, period, selectedDate);
                    break;
                case "card_status":
                    HandleCardStatus(context, connStr);
                    break;
                default:
                    // Return all data in one shot (used by the dashboard on first load)
                    HandleAll(context, connStr, period, selectedDate);
                    break;
            }
        }
        catch (Exception ex)
        {
            Send(context, 500, false, "Database error: " + ex.Message, null);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  ALL-IN-ONE (default, no action param)
    // ─────────────────────────────────────────────────────────────────────────
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

    // ─────────────────────────────────────────────────────────────────────────
    //  KPI TOTALS
    // ─────────────────────────────────────────────────────────────────────────
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
        // Total activated QR cards (all-time)
        int totalActivations = ExecScalar<int>(conn,
            "SELECT COUNT(*) FROM QRMaster WHERE Status = 'ACTIVATED'");

        // Today's activations (from scan log)
        int todayActivations = ExecScalar<int>(conn,
            "SELECT COUNT(*) FROM tbl_QR_ScanLog WHERE Scan_Type='ACTIVATION' AND CAST(Scan_Timestamp AS DATE)=CAST(GETDATE() AS DATE)");

        // Active tax collectors
        int activeTaxCollectors = ExecScalar<int>(conn,
            "SELECT COUNT(*) FROM tbl_QR_StaffUsers WHERE IsActive=1 AND Role IN ('TAX_COLLECTOR','TAXCOLLECTOR','Tax Collector')");

        // Total staff users (activators)
        int totalActivators = ExecScalar<int>(conn,
            "SELECT COUNT(*) FROM tbl_QR_StaffUsers WHERE IsActive=1");

        // Citizens registered (properties with a card)
        int citizensRegistered = ExecScalar<int>(conn,
            "SELECT COUNT(*) FROM All_Demand WHERE Card_Status='ACTIVE'");

        // Total citizens in demand table
        int totalCitizens = ExecScalar<int>(conn,
            "SELECT COUNT(*) FROM All_Demand");

        // Revenue: sum of Total_Dues where Payment_Status = 'Paid'
        decimal totalRevenue = ExecScalar<decimal>(conn,
            "SELECT ISNULL(SUM(CAST(Total_Dues AS DECIMAL(18,2))),0) FROM All_Demand WHERE Payment_Status='Paid'");

        // Revenue this month
        decimal monthRevenue = ExecScalar<decimal>(conn, @"
            SELECT ISNULL(SUM(CAST(Total_Dues AS DECIMAL(18,2))),0) 
            FROM All_Demand 
            WHERE Payment_Status='Paid' 
              AND MONTH(ISNULL(Last_Scanned,GETDATE()))=MONTH(GETDATE()) 
              AND YEAR(ISNULL(Last_Scanned,GETDATE()))=YEAR(GETDATE())");

        // Revenue today
        decimal todayRevenue = ExecScalar<decimal>(conn, @"
            SELECT ISNULL(SUM(CAST(Total_Dues AS DECIMAL(18,2))),0) 
            FROM All_Demand 
            WHERE Payment_Status='Paid' 
              AND CAST(ISNULL(Last_Scanned,GETDATE()) AS DATE)=CAST(GETDATE() AS DATE)");

        return new Dictionary<string, object>
        {
            { "totalActivations",   totalActivations   },
            { "todayActivations",   todayActivations   },
            { "activeTaxCollectors",activeTaxCollectors },
            { "totalActivators",    totalActivators    },
            { "citizensRegistered", citizensRegistered },
            { "totalCitizens",      totalCitizens      },
            { "totalRevenue",       totalRevenue       },
            { "monthRevenue",       monthRevenue       },
            { "todayRevenue",       todayRevenue       },
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  CARD ACTIVATORS  (field staff)
    // ─────────────────────────────────────────────────────────────────────────
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
                u.StaffId,
                u.LoginId,
                u.FullName,
                u.Ward_No,
                u.Circle,
                u.Mobile,
                u.IsActive,
                u.Last_Login,
                ISNULL(today.cnt, 0)   AS Today_Count,
                ISNULL(month_c.cnt, 0) AS Month_Count,
                ISNULL(total_c.cnt, 0) AS Total_Count
            FROM tbl_QR_StaffUsers u
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt 
                FROM tbl_QR_ScanLog 
                WHERE Scan_Type='ACTIVATION' AND CAST(Scan_Timestamp AS DATE)=CAST(GETDATE() AS DATE)
                GROUP BY Staff_Id
            ) today ON today.Staff_Id = u.LoginId
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt 
                FROM tbl_QR_ScanLog 
                WHERE Scan_Type='ACTIVATION' 
                  AND MONTH(Scan_Timestamp)=MONTH(GETDATE()) 
                  AND YEAR(Scan_Timestamp)=YEAR(GETDATE())
                GROUP BY Staff_Id
            ) month_c ON month_c.Staff_Id = u.LoginId
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt 
                FROM tbl_QR_ScanLog 
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
                bool isActive = r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"]);
                string lastLogin = r["Last_Login"] == DBNull.Value ? "-"
                    : Convert.ToDateTime(r["Last_Login"]).ToString("dd MMM yyyy, HH:mm");

                list.Add(new Dictionary<string, object>
                {
                    { "staffId",   r["StaffId"].ToString()                     },
                    { "loginId",   r["LoginId"].ToString()                     },
                    { "fullName",  r["FullName"].ToString()                    },
                    { "wardNo",    r["Ward_No"] == DBNull.Value ? 0 : Convert.ToInt32(r["Ward_No"]) },
                    { "circle",    r["Circle"]  == DBNull.Value ? "" : r["Circle"].ToString()  },
                    { "mobile",    r["Mobile"]  == DBNull.Value ? "" : r["Mobile"].ToString()  },
                    { "status",    isActive ? "Active" : "Inactive"            },
                    { "lastLogin", lastLogin                                   },
                    { "today",     Convert.ToInt32(r["Today_Count"])           },
                    { "month",     Convert.ToInt32(r["Month_Count"])           },
                    { "total",     Convert.ToInt32(r["Total_Count"])           },
                });
            }
        }
        return list;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  TAX COLLECTORS
    // ─────────────────────────────────────────────────────────────────────────
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
        // Tax collectors: join staff with scan-log visit counts and All_Demand collected amounts per ward
        string query = @"
            SELECT 
                u.StaffId,
                u.LoginId,
                u.FullName,
                u.Ward_No,
                u.Circle,
                u.IsActive,
                ISNULL(visits.cnt, 0) AS Citizens_Visited,
                ISNULL(rev.collected, 0) AS Collected,
                ISNULL(tgt.target, 0) AS Target
            FROM tbl_QR_StaffUsers u
            LEFT JOIN (
                SELECT Staff_Id, COUNT(*) AS cnt
                FROM tbl_QR_ScanLog
                WHERE Scan_Type IN ('TAX_COLLECTION','CITIZEN_VISIT','CITIZEN_PORTAL_SCAN')
                GROUP BY Staff_Id
            ) visits ON visits.Staff_Id = u.LoginId
            LEFT JOIN (
                SELECT Ward_No, SUM(CAST(Total_Dues AS DECIMAL(18,2))) AS collected
                FROM All_Demand
                WHERE Payment_Status='Paid'
                GROUP BY Ward_No
            ) rev ON rev.Ward_No = u.Ward_No
            LEFT JOIN (
                SELECT Ward_No, SUM(CAST(Total_Dues AS DECIMAL(18,2))) AS target
                FROM All_Demand
                GROUP BY Ward_No
            ) tgt ON tgt.Ward_No = u.Ward_No
            WHERE u.Role IN ('TAX_COLLECTOR','TAXCOLLECTOR','Tax Collector','Admin','ADMIN','Supervisor','SUPERVISOR')
               OR u.IsActive = 1
            ORDER BY Collected DESC";

        var list = new List<Dictionary<string, object>>();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                bool isActive = r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"]);
                decimal collected = r["Collected"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Collected"]);
                decimal target    = r["Target"]    == DBNull.Value ? 0 : Convert.ToDecimal(r["Target"]);

                list.Add(new Dictionary<string, object>
                {
                    { "staffId",        r["StaffId"].ToString() },
                    { "loginId",        r["LoginId"].ToString()  },
                    { "fullName",       r["FullName"].ToString() },
                    { "wardNo",         r["Ward_No"] == DBNull.Value ? 0 : Convert.ToInt32(r["Ward_No"]) },
                    { "circle",         r["Circle"]  == DBNull.Value ? "" : r["Circle"].ToString()  },
                    { "status",         isActive ? "Active" : "Inactive" },
                    { "visited",        Convert.ToInt32(r["Citizens_Visited"]) },
                    { "collected",      collected },
                    { "target",         target    },
                });
            }
        }
        return list;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  CITIZENS
    // ─────────────────────────────────────────────────────────────────────────
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
        // Latest 200 citizens for the table (admin can search/filter on frontend)
        string query = @"
            SELECT TOP 200
                d.PID,
                d.Owner_Name,
                d.Guardian_Name,
                d.Mobile_No,
                d.Address,
                d.Ward_No,
                d.Circle,
                d.Total_Dues,
                d.Payment_Status,
                d.Card_Status,
                q.Status AS QR_Status,
                q.QRId,
                q.QRToken
            FROM All_Demand d
            LEFT JOIN QRMaster q ON q.PropertyId = d.PID AND q.Status='ACTIVATED'
            ORDER BY ISNULL(q.ActivatedDate, '1900-01-01') DESC, d.PID DESC";

        var list = new List<Dictionary<string, object>>();
        using (SqlCommand cmd = new SqlCommand(query, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                string qrStatus = r["QR_Status"] == DBNull.Value ? "Not Assigned" : r["QR_Status"].ToString();
                string cardStatus = r["Card_Status"] == DBNull.Value ? "" : r["Card_Status"].ToString();
                if (string.IsNullOrEmpty(qrStatus) || qrStatus == "Not Assigned")
                    qrStatus = string.IsNullOrEmpty(cardStatus) ? "Not Assigned" : cardStatus;

                list.Add(new Dictionary<string, object>
                {
                    { "pid",           r["PID"].ToString() },
                    { "ownerName",     r["Owner_Name"].ToString() },
                    { "guardianName",  r["Guardian_Name"] == DBNull.Value ? "" : r["Guardian_Name"].ToString() },
                    { "mobileNo",      r["Mobile_No"]     == DBNull.Value ? "" : r["Mobile_No"].ToString()     },
                    { "address",       r["Address"]       == DBNull.Value ? "" : r["Address"].ToString()       },
                    { "wardNo",        r["Ward_No"]       == DBNull.Value ? "" : r["Ward_No"].ToString()       },
                    { "circle",        r["Circle"]        == DBNull.Value ? "" : r["Circle"].ToString()        },
                    { "totalDues",     r["Total_Dues"]    == DBNull.Value ? 0  : Convert.ToDecimal(r["Total_Dues"]) },
                    { "paymentStatus", r["Payment_Status"]== DBNull.Value ? "Pending" : r["Payment_Status"].ToString() },
                    { "qrStatus",      qrStatus },
                    { "qrId",          r["QRId"]    == DBNull.Value ? "" : r["QRId"].ToString()    },
                    { "qrToken",       r["QRToken"] == DBNull.Value ? "" : r["QRToken"].ToString() },
                });
            }
        }
        return list;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  REVENUE
    // ─────────────────────────────────────────────────────────────────────────
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
        // Ward-level breakdown
        string wardQuery = @"
            SELECT 
                d.Ward_No,
                MAX(d.Circle) AS Circle,
                COUNT(d.PID) AS Properties,
                ISNULL(SUM(CASE WHEN d.Payment_Status='Paid' THEN CAST(d.Total_Dues AS DECIMAL(18,2)) ELSE 0 END),0) AS Collected,
                ISNULL(SUM(CAST(d.Total_Dues AS DECIMAL(18,2))),0) AS Target
            FROM All_Demand d
            WHERE d.Ward_No IS NOT NULL AND d.Ward_No > 0
            GROUP BY d.Ward_No
            ORDER BY d.Ward_No";

        var wardBreakdown = new List<Dictionary<string, object>>();
        using (SqlCommand cmd = new SqlCommand(wardQuery, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                wardBreakdown.Add(new Dictionary<string, object>
                {
                    { "ward",        r["Ward_No"].ToString()                 },
                    { "circle",      r["Circle"] == DBNull.Value ? "" : r["Circle"].ToString() },
                    { "properties",  Convert.ToInt32(r["Properties"])        },
                    { "collected",   Convert.ToDecimal(r["Collected"])       },
                    { "target",      Convert.ToDecimal(r["Target"])          },
                });
            }
        }

        // Time-series trend based on period (using tbl_QR_ScanLog as proxy for activity dates)
        string trendQuery = "";
        if (period == "day")
        {
            // Hourly today
            trendQuery = @"
                SELECT 
                    DATEPART(HOUR, Scan_Timestamp) AS Period_Label,
                    COUNT(*) AS Activations
                FROM tbl_QR_ScanLog
                WHERE CAST(Scan_Timestamp AS DATE) = CAST(GETDATE() AS DATE)
                  AND Scan_Type = 'ACTIVATION'
                GROUP BY DATEPART(HOUR, Scan_Timestamp)
                ORDER BY Period_Label";
        }
        else if (period == "month")
        {
            // Daily this month
            trendQuery = @"
                SELECT 
                    DAY(Scan_Timestamp) AS Period_Label,
                    COUNT(*) AS Activations
                FROM tbl_QR_ScanLog
                WHERE MONTH(Scan_Timestamp)=MONTH(GETDATE()) AND YEAR(Scan_Timestamp)=YEAR(GETDATE())
                  AND Scan_Type='ACTIVATION'
                GROUP BY DAY(Scan_Timestamp)
                ORDER BY Period_Label";
        }
        else // year
        {
            // Monthly this year
            trendQuery = @"
                SELECT 
                    MONTH(Scan_Timestamp) AS Period_Label,
                    COUNT(*) AS Activations
                FROM tbl_QR_ScanLog
                WHERE YEAR(Scan_Timestamp)=YEAR(GETDATE())
                  AND Scan_Type='ACTIVATION'
                GROUP BY MONTH(Scan_Timestamp)
                ORDER BY Period_Label";
        }

        var trendLabels  = new List<string>();
        var trendValues  = new List<int>();
        using (SqlCommand cmd = new SqlCommand(trendQuery, conn))
        using (SqlDataReader r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                string lbl = r["Period_Label"].ToString();
                if (period == "day")   lbl = lbl + ":00";
                if (period == "year")
                {
                    string[] months = {"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
                    int m = Convert.ToInt32(lbl) - 1;
                    lbl = (m >= 0 && m < 12) ? months[m] : lbl;
                }
                trendLabels.Add(lbl);
                trendValues.Add(Convert.ToInt32(r["Activations"]));
            }
        }

        // Revenue summary totals
        decimal todayRevenue = ExecScalar<decimal>(conn, @"
            SELECT ISNULL(SUM(CAST(Total_Dues AS DECIMAL(18,2))),0) 
            FROM All_Demand WHERE Payment_Status='Paid' 
              AND CAST(ISNULL(Last_Scanned,GETDATE()) AS DATE)=CAST(GETDATE() AS DATE)");
        decimal monthRevenue = ExecScalar<decimal>(conn, @"
            SELECT ISNULL(SUM(CAST(Total_Dues AS DECIMAL(18,2))),0) 
            FROM All_Demand WHERE Payment_Status='Paid'
              AND MONTH(ISNULL(Last_Scanned,GETDATE()))=MONTH(GETDATE()) 
              AND YEAR(ISNULL(Last_Scanned,GETDATE()))=YEAR(GETDATE())");
        decimal yearRevenue = ExecScalar<decimal>(conn, @"
            SELECT ISNULL(SUM(CAST(Total_Dues AS DECIMAL(18,2))),0) 
            FROM All_Demand WHERE Payment_Status='Paid'
              AND YEAR(ISNULL(Last_Scanned,GETDATE()))=YEAR(GETDATE())");
        decimal totalRevenue = ExecScalar<decimal>(conn, @"
            SELECT ISNULL(SUM(CAST(Total_Dues AS DECIMAL(18,2))),0) 
            FROM All_Demand WHERE Payment_Status='Paid'");

        return new Dictionary<string, object>
        {
            { "wardBreakdown", wardBreakdown  },
            { "trendLabels",   trendLabels    },
            { "trendValues",   trendValues    },
            { "todayRevenue",  todayRevenue   },
            { "monthRevenue",  monthRevenue   },
            { "yearRevenue",   yearRevenue    },
            { "totalRevenue",  totalRevenue   },
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  CARD STATUS BREAKDOWN (for donut chart)
    // ─────────────────────────────────────────────────────────────────────────
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
        string query = @"
            SELECT Status, COUNT(*) AS Cnt
            FROM QRMaster
            GROUP BY Status";

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
                string s = r["Status"].ToString().ToUpper().Trim();
                int cnt  = Convert.ToInt32(r["Cnt"]);
                if (statusMap.ContainsKey(s))
                    statusMap[s] = cnt;
                else
                    statusMap["PENDING"] += cnt; // lump unknowns into pending
            }
        }

        return new Dictionary<string, object>
        {
            { "activated",  statusMap["ACTIVATED"]  },
            { "unassigned", statusMap["UNASSIGNED"]  },
            { "replaced",   statusMap["REPLACED"]   },
            { "pending",    statusMap["PENDING"]    },
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────────────────────────────────
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
