-- USE [db_patna_property_tax]
-- GO
-- /****** Object:  StoredProcedure [dbo].[sp_temp_yearly_tax_test]    Script Date: 6/20/2026 1:13:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_temp_yearly_tax_test]
    @property_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    ------------------------------------------------------------
    -- PROPERTY BASIC INFO
    ------------------------------------------------------------
    DECLARE @assessment_year INT,
            @property_type_id INT,
            @current_year INT = YEAR(GETDATE());

    SELECT 
        @assessment_year = TRY_CAST(LEFT(assessment_year,4) AS INT),
        @property_type_id = property_type_id
    FROM tbl_property_detail
    WHERE id = @property_id;

    ------------------------------------------------------------
    -- FY LIST
    ------------------------------------------------------------
    IF OBJECT_ID('tempdb..#fy') IS NOT NULL DROP TABLE #fy;

    ;WITH cte AS (
        SELECT @assessment_year + 1 AS yr
        UNION ALL
        SELECT yr + 1 FROM cte WHERE yr + 1 <= @current_year
    )
    SELECT 
        yr,
        CAST(yr AS VARCHAR) + '-' + CAST(yr + 1 AS VARCHAR) AS fin_year
    INTO #fy
    FROM cte
    OPTION (MAXRECURSION 100);

    ------------------------------------------------------------
    -- YEARLY DATA
    ------------------------------------------------------------
    IF OBJECT_ID('tempdb..#yearly') IS NOT NULL DROP TABLE #yearly;

    SELECT 
        TRY_CAST(LEFT(fin_year,4) AS INT) AS yr,
        total_tax,
        vacant_tax
    INTO #yearly
    FROM tbl_yearly_tax_assessment
    WHERE property_id = @property_id;

    ------------------------------------------------------------
    -- BASE (LAST AVAILABLE TAX FOR CARRY FORWARD)
    ------------------------------------------------------------
    DECLARE @base_total_tax NUMERIC(18,2) = 0,
            @base_vacant_tax NUMERIC(18,2) = 0;

    SELECT TOP 1 
        @base_total_tax = ISNULL(total_tax,0),
        @base_vacant_tax = ISNULL(vacant_tax,0)
    FROM #yearly
    ORDER BY yr DESC;

    ------------------------------------------------------------
    -- LAST VACANT TAX
    ------------------------------------------------------------
    DECLARE @last_vacant_tax NUMERIC(18,2) = 0;

    SELECT TOP 1 @last_vacant_tax = ISNULL(vacant_tax,0)
    FROM #yearly
    ORDER BY yr DESC;

    ------------------------------------------------------------
    -- FLOOR TAX (NEW SYSTEM CALCULATION)
    ------------------------------------------------------------
    DECLARE @floor_tax NUMERIC(18,2);

    SELECT @floor_tax = ISNULL(SUM(ROUND(
    (
        (CASE WHEN o.use_type_id = 1 THEN o.builtup_area*0.70 ELSE o.builtup_area*0.80 END)
        * ISNULL(arv.rate,0)
        * ISNULL(ofac.multipication_factor,1)
        * ISNULL(umf.multipication_factor,1)
        * (tr.holding_tax + tr.water_tax + tr.latrine_tax 
           + tr.education_cess + tr.health_cess)/100
    ),0)),0)
    FROM tbl_occupancy_detail o
    JOIN tbl_property_detail pd ON pd.id = o.property_id

    OUTER APPLY (
        SELECT TOP 1 *
        FROM view_rate_details r
        WHERE r.street_type_id = pd.street_type_id
          AND r.use_type_id = o.use_type_id
          AND r.construction_type_id = o.construction_type_id
        ORDER BY effect_date DESC
    ) arv

    LEFT JOIN tbl_tax_rate_master tr 
        ON tr.arv_master_id = arv.arv_master_id

    OUTER APPLY (
        SELECT TOP 1 multipication_factor 
        FROM tbl_occupancy_type_detail 
        WHERE occupancy_type_id = o.occupancy_type_id
        ORDER BY effect_date DESC
    ) ofac

    OUTER APPLY (
        SELECT TOP 1 multipication_factor 
        FROM tbl_usage_multiplication_factor 
        WHERE usage_type_id = o.usage_type_id
        ORDER BY effect_date DESC
    ) umf

    WHERE o.property_id = @property_id 
      AND o.status = 1;

    ------------------------------------------------------------
    -- FINAL OUTPUT
    ------------------------------------------------------------
    With TaxData AS 
	(SELECT 
        f.fin_year,
        @property_id AS property_id,

        --------------------------------------------------------
        -- FLOOR TAX
        --------------------------------------------------------
        CASE 
            WHEN @property_type_id = 1 THEN @base_total_tax

            -- OLD YEARS + 2024-25 => CARRY FORWARD
            WHEN f.yr <= 2024 THEN @base_total_tax

            -- NEW SYSTEM FROM 2025-26
            WHEN f.yr >= 2025 THEN @floor_tax

            ELSE 0
        END AS floor_tax,

        --------------------------------------------------------
        -- VACANT TAX
        --------------------------------------------------------
        CASE 
            WHEN @property_type_id = 1 THEN @base_vacant_tax

            WHEN f.yr <= 2024 THEN @base_vacant_tax

            WHEN f.yr >= 2025 THEN @last_vacant_tax

            ELSE 0
        END AS vacant_tax,

        --------------------------------------------------------
        -- TOTAL TAX
        --------------------------------------------------------
        CASE 
            WHEN @property_type_id = 1 THEN @base_total_tax

            WHEN f.yr <= 2024 THEN @base_total_tax

            WHEN f.yr >= 2025 
            THEN @floor_tax + @last_vacant_tax

            ELSE 0
        END AS total_tax 

    FROM #fy f
   -- ORDER BY f.yr
	)
	SELECT
    fin_year,
    property_id,
    floor_tax,
    vacant_tax,
    total_tax
FROM TaxData

UNION ALL

SELECT
    'TOTAL',
    @property_id,
    NULL,
    NULL,
    SUM(total_tax)
FROM TaxData

ORDER BY fin_year;

END
