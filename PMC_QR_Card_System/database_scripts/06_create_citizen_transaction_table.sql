-- ============================================================
-- Create tbl_QR_Citizen_Transaction
-- ============================================================

IF OBJECT_ID('dbo.tbl_QR_Citizen_Transaction', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbl_QR_Citizen_Transaction (
        TransactionId       BIGINT IDENTITY(1,1) PRIMARY KEY,
        PID                 NVARCHAR(50)  NOT NULL,
        OwnerName           NVARCHAR(200) NULL,
        MobileNo            NVARCHAR(20)  NULL,
        PaymentGatewayId    NVARCHAR(100) NOT NULL,
        AmountPaid          DECIMAL(18,2) NOT NULL,
        PaymentMode         NVARCHAR(50)  NULL,
        PaymentDate         DATETIME      NOT NULL DEFAULT GETDATE(),
        Status              NVARCHAR(50)  NOT NULL,
        IPAddress           NVARCHAR(50)  NULL,
        Remarks             NVARCHAR(500) NULL
    );
    PRINT 'Created tbl_QR_Citizen_Transaction';
END
GO
