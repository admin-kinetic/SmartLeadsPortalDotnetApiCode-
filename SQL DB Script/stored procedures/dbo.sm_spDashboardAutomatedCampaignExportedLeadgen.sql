CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardAutomatedCampaignExportedLeadgen]
@startDate DATETIME, 
@endDate DATETIME
AS  
BEGIN   
    SET NOCOUNT ON;
	-- Generate date range
	WITH Numbers AS (
		SELECT TOP (DATEDIFF(day, @startDate, @endDate) + 1)
			ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
		FROM sys.objects
	),
	DateRange AS (
		SELECT DATEADD(day, n, @startDate) AS ExportedDate
		FROM Numbers
	),
	-- Main data with LeadGen and totals
	DailyBreakdown AS (
		SELECT 
			CAST(r.dateExported AS date) AS ExportedDate,
			e.firstname + ' ' + e.lastname AS LeadGen,
			COUNT(r.ID) AS TotalCount
		FROM [dbo].[Robotcrawledresult] r
		LEFT JOIN [dbo].[Employee] e ON r.assignedTo = e.ID
		WHERE r.dateExported BETWEEN @startDate AND @endDate
			AND r.ExportedTo = 4
			AND r.reviewStatus = 'Exported'
			AND r.assignedTo <> 2
		GROUP BY CAST(r.dateExported AS date), e.firstname, e.lastname
	),
	-- List of distinct LeadGens + 'Others'
	LeadGens AS (
		SELECT DISTINCT LeadGen FROM DailyBreakdown WHERE LeadGen IS NOT NULL
		UNION
		SELECT 'Others'
	)
	-- Final result: all combinations of Date x LeadGen, filled with 0 when missing
	SELECT 
		CONVERT(VARCHAR(10), dr.ExportedDate, 23) AS ExportedDate,
		lg.LeadGen,
		COALESCE(db.TotalCount, 0) AS TotalCount
	FROM DateRange dr
	CROSS JOIN LeadGens lg
	LEFT JOIN DailyBreakdown db
		ON dr.ExportedDate = db.ExportedDate AND db.LeadGen = lg.LeadGen
	ORDER BY dr.ExportedDate, 
			 CASE WHEN lg.LeadGen = 'Others' THEN 1 ELSE 0 END, 
			 lg.LeadGen;
END
GO