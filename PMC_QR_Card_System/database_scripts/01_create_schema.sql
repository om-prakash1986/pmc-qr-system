-- ============================================================
--  PMC Smart QR Card System — Full Database Schema Script
--  Run this on your SQL Server against the [PTax] database
-- ============================================================

-- ─────────────────────────────────────────────
--  1. STAFF USERS TABLE (Admin, Activators, Tax Collectors)
-- ─────────────────────────────────────────────
IF OBJECT_ID('dbo.tbl_QR_StaffUsers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbl_QR_StaffUsers (
        StaffId       INT IDENTITY(1,1) PRIMARY KEY,
        LoginId       NVARCHAR(50)  NOT NULL UNIQUE,
        PasswordHash  NVARCHAR(256) NOT NULL,   -- SHA-256 hex string
        FullName      NVARCHAR(100) NOT NULL,
        Role          NVARCHAR(50)  NOT NULL,   -- ADMIN | SUPERVISOR | TAX_COLLECTOR | ACTIVATOR
        Ward_No       INT           NULL,
        Circle        NVARCHAR(100) NULL,
        Mobile        NVARCHAR(20)  NULL,
        IsActive      BIT           NOT NULL DEFAULT 1,
        Last_Login    DATETIME      NULL,
        Login_Failures INT          NOT NULL DEFAULT 0,
        Created_At    DATETIME      NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Created tbl_QR_StaffUsers';
END
GO

-- ─────────────────────────────────────────────
--  2. QR MASTER TABLE (card records)
-- ─────────────────────────────────────────────
IF OBJECT_ID('dbo.QRMaster', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.QRMaster (
        QRId          NVARCHAR(100) PRIMARY KEY,
        QRToken       NVARCHAR(200) NOT NULL,
        QRUrl         NVARCHAR(500) NULL,
        Status        NVARCHAR(50)  NOT NULL DEFAULT 'UNASSIGNED',
        -- Status values: UNASSIGNED | ACTIVATED | REPLACED | PENDING
        PropertyId    NVARCHAR(50)  NULL,
        CreatedDate   DATETIME      NOT NULL DEFAULT GETDATE(),
        ActivatedDate DATETIME      NULL,
        ActivatedBy   NVARCHAR(50)  NULL
    );
    PRINT 'Created QRMaster';
END
GO

-- ─────────────────────────────────────────────
--  3. ALL DEMAND TABLE (property / citizen records)
-- ─────────────────────────────────────────────
IF OBJECT_ID('dbo.All_Demand', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.All_Demand (
        PID             NVARCHAR(50)    PRIMARY KEY,
        Owner_Name      NVARCHAR(200)   NOT NULL,
        Guardian_Name   NVARCHAR(200)   NULL,
        Mobile_No       NVARCHAR(20)    NULL,
        Address         NVARCHAR(500)   NULL,
        Ward_No         INT             NULL,
        Circle          NVARCHAR(100)   NULL,
        Total_Dues      DECIMAL(18,2)   NULL DEFAULT 0,
        Payment_Status  NVARCHAR(50)    NULL DEFAULT 'Pending',
        Card_Status     NVARCHAR(50)    NULL,
        Card_Print_Date DATETIME        NULL,
        Last_Scanned    DATETIME        NULL,
        Scan_Count      INT             NULL DEFAULT 0
    );
    PRINT 'Created All_Demand';
END
GO

-- ─────────────────────────────────────────────
--  4. QR SCAN LOG TABLE
-- ─────────────────────────────────────────────
IF OBJECT_ID('dbo.tbl_QR_ScanLog', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbl_QR_ScanLog (
        LogId           BIGINT IDENTITY(1,1) PRIMARY KEY,
        PID             BIGINT          NULL,
        Staff_Id        NVARCHAR(50)    NULL,
        Scan_Type       NVARCHAR(50)    NULL,
        -- Scan_Type values: ACTIVATION | CITIZEN_PORTAL_SCAN | TAX_COLLECTION | CITIZEN_VISIT
        Scan_Timestamp  DATETIME        NOT NULL DEFAULT GETDATE(),
        Device_ID       NVARCHAR(200)   NULL,
        Geo_Lat         DECIMAL(10,6)   NULL,
        Geo_Long        DECIMAL(10,6)   NULL,
        IP_Address      NVARCHAR(50)    NULL,
        Is_Suspicious   BIT             NOT NULL DEFAULT 0,
        Remarks         NVARCHAR(500)   NULL
    );
    PRINT 'Created tbl_QR_ScanLog';
END
GO

-- ─────────────────────────────────────────────
--  5. DEFAULT ADMIN USER  (change password after first login!)
--     Password below is SHA-256 of "Admin@123"
-- ─────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM dbo.tbl_QR_StaffUsers WHERE LoginId = 'admin')
BEGIN
    INSERT INTO dbo.tbl_QR_StaffUsers (LoginId, PasswordHash, FullName, Role, IsActive)
    VALUES (
        'admin',
        '5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5', -- SHA-256 of "Admin@123"
        'System Administrator',
        'ADMIN',
        1
    );
    PRINT 'Inserted default admin user (LoginId=admin, Password=Admin@123)';
END
GO

-- ─────────────────────────────────────────────
--  6. USEFUL INDEXES
-- ─────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QRMaster_PropertyId')
    CREATE INDEX IX_QRMaster_PropertyId ON dbo.QRMaster(PropertyId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QRMaster_Status')
    CREATE INDEX IX_QRMaster_Status ON dbo.QRMaster(Status);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ScanLog_Timestamp')
    CREATE INDEX IX_ScanLog_Timestamp ON dbo.tbl_QR_ScanLog(Scan_Timestamp);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ScanLog_StaffId')
    CREATE INDEX IX_ScanLog_StaffId ON dbo.tbl_QR_ScanLog(Staff_Id);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AllDemand_WardNo')
    CREATE INDEX IX_AllDemand_WardNo ON dbo.All_Demand(Ward_No);

PRINT 'Indexes created.';
GO

-- Done
PRINT '=== PMC QR Card System Schema installed successfully ===';
GO
