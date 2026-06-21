# PMC Smart QR Card System — Deployment & Testing Guide

## 📁 Folder Structure

```
PMC_QR_Card_System/
├── admin_dashboard/
│   ├── index.html          ← Admin Dashboard (open in browser)
│   └── citizen_portal.html ← Citizen-facing portal
│
├── backend_api/            ← Copy ALL .ashx files to your IIS application folder
│   ├── AdminDashboardHandler.ashx   ← NEW: Dashboard KPI aggregator
│   ├── LoginHandler.ashx
│   ├── ActivateCardHandler.ashx
│   ├── CardLookupHandler.ashx
│   ├── PropertyLookupHandler.ashx
│   └── CitizenPortal.ashx
│
├── database_scripts/
│   ├── 01_create_schema.sql    ← Run FIRST on SQL Server
│   └── insert_taxcollector.sql ← Sample data insert
│
└── README.md                   ← This file
```

---

## ⚙️ Step 1: Set Up the Database

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance
3. Open `database_scripts/01_create_schema.sql`
4. Change the target database at the top if needed (default: `PTax`)
5. Run the script — it creates all 4 tables + a default admin user
6. Run `database_scripts/insert_taxcollector.sql` for sample staff data

**Default Admin Login Created:**
| Field | Value |
|-------|-------|
| Login ID | `admin` |
| Password | `Admin@123` |
| Role | `ADMIN` |

> ⚠️ Change the admin password after first login!

---

## ⚙️ Step 2: Deploy the Backend (.ashx files)

### IIS Deployment

1. Open **IIS Manager**
2. Navigate to your application (e.g., `Default Web Site > PTax`)
3. Copy **all files** from `backend_api/` into the application's physical folder
   - Typically: `C:\inetpub\wwwroot\PTax\`
4. Ensure your `web.config` has the PTax connection string:

```xml
<connectionStrings>
  <add name="PTax"
       connectionString="Server=YOUR_SERVER;Database=PTax;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

5. Verify the App Pool is running **.NET Framework 4.x** (not .NET Core)
6. Restart the App Pool

### Test Backend is Working

Open your browser and navigate to:
```
http://YOUR_SERVER/PTax/AdminDashboardHandler.ashx?period=month
```
You should see a JSON response with `"success": true`.

---

## ⚙️ Step 3: Configure the Admin Dashboard

1. Open `admin_dashboard/index.html` in any text editor (Notepad, VS Code)
2. Find this line near the top of the `<script>` section:
   ```javascript
   API_BASE: 'http://localhost/PTax/',
   ```
3. Change it to your **actual server URL**, e.g.:
   ```javascript
   API_BASE: 'http://192.168.1.100/PTax/',
   ```
4. Save the file

---

## ⚙️ Step 4: Test the Application

### A) Admin Dashboard
1. Double-click `admin_dashboard/index.html` to open in browser
   *(Or serve via IIS/nginx for production)*
2. **Login Screen** will appear
3. Enter Login ID: `admin` / Password: `Admin@123`
4. Dashboard loads with live data from your database ✅

### B) Test Each Section
| Section | What to verify |
|---------|---------------|
| Dashboard | KPI numbers match your DB |
| Card Activators | Staff users shown from `tbl_QR_StaffUsers` |
| Tax Collectors | Collector list with ward data |
| Citizens | Properties from `All_Demand` |
| Revenue | Ward-wise breakdown |
| Export CSV | Downloads file with live data |
| PID Lookup | Click "Lookup PID" → search a property |

### C) Citizen Portal
- Open `admin_dashboard/citizen_portal.html` directly in browser
- Or deploy `backend_api/CitizenPortal.ashx` to IIS and access via URL with `?token=<QR_TOKEN>`

---

## 🔑 API Endpoints Reference

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `AdminDashboardHandler.ashx` | GET | All dashboard data (KPIs, tables, charts) |
| `AdminDashboardHandler.ashx?action=kpis` | GET | KPI totals only |
| `AdminDashboardHandler.ashx?action=activators` | GET | Activator list |
| `AdminDashboardHandler.ashx?action=tax_collectors` | GET | Tax collector list |
| `AdminDashboardHandler.ashx?action=citizens` | GET | Citizens list |
| `AdminDashboardHandler.ashx?action=revenue` | GET | Revenue data |
| `AdminDashboardHandler.ashx?action=card_status` | GET | QR card status counts |
| `LoginHandler.ashx` | POST | Admin login (SHA-256 password) |
| `PropertyLookupHandler.ashx?pid=<PID>` | GET | Property details |
| `CardLookupHandler.ashx?value=<TOKEN>` | GET | QR card lookup |
| `ActivateCardHandler.ashx` | POST | Activate QR card |
| `CitizenPortal.ashx?action=send_otp` | POST | Send OTP to citizen |
| `CitizenPortal.ashx?action=verify_otp` | POST | Verify OTP + get property |

### Period & Date Parameters
All `AdminDashboardHandler.ashx` calls accept:
- `?period=day` — Today's data (hourly)
- `?period=month` — This month's data (daily)  *(default)*
- `?period=year` — This year's data (monthly)
- `?date=YYYY-MM-DD` — Filter by specific date

---

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| Login shows "Could not reach backend" | Check `API_BASE` URL in `index.html`. Make sure IIS is running. |
| "Access denied" after login | User's `Role` must be `ADMIN` or `SUPERVISOR` in `tbl_QR_StaffUsers` |
| Dashboard shows "No records found" | Database tables are empty — add data or run the schema script |
| CORS errors in browser console | All .ashx files already include CORS headers (`Access-Control-Allow-Origin: *`) |
| SQL connection error | Check `web.config` connection string. Verify SQL Server is running. |
| Charts not loading | Check browser console for JS errors. Ensure Chart.js CDN is accessible. |

---

## 📞 Support

This system was developed for Patna Municipal Corporation (PMC) Smart QR Card Management.

**Database:** SQL Server (`PTax` database)  
**Backend:** ASP.NET Web Handlers (.ashx) on IIS  
**Frontend:** Pure HTML + JavaScript (no build tools required)  
**Mobile App:** Flutter (see `pmc_activation_app/` folder)

---

*Last updated: June 2026*
