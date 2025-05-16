CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardAutomatedCampaignExportedLeadgen]
@startDate DATETIME, 
@endDate DATETIME
AS  
BEGIN   
    SET NOCOUNT ON;
	WITH Numbers AS (
		-- Generate a sequence of numbers based on the date range
		SELECT TOP (DATEDIFF(day, CAST(@startDate AS date), CAST(@endDate AS date)) + 1)
			ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
		FROM sys.objects
	),
	DateRange AS (
		-- Generate a list of dates between startDate and endDate
		SELECT DATEADD(day, n, CAST(@startDate AS date)) AS ExportedDate
		FROM Numbers
	),
	DailyBreakdown AS (
		-- Calculate count per day per lead generator
		SELECT CAST(r.dateExported AS date) AS ExportedDate,
			e.firstname + ' ' + e.lastname AS LeadGen,
			COUNT(r.ID) AS TotalCount
		FROM [dbo].[Robotcrawledresult] r
		LEFT JOIN [dbo].[Employee] e ON r.assignedTo = e.ID
		WHERE r.dateExported BETWEEN CAST(@startDate AS date) AND CAST(@endDate AS date)
			AND r.ExportedTo = 4
			AND r.reviewStatus = 'Exported'
			AND r.assignedTo <> 2
		GROUP BY CAST(r.dateExported AS date),
			e.firstname + ' ' + e.lastname
	)
	-- Combine with date range, ensuring all dates and leadgens appear
	SELECT 
		dr.ExportedDate,
		COALESCE(db.LeadGen, 'Others') AS LeadGen,
		COALESCE(db.TotalCount, 0) AS TotalCount
	FROM DateRange dr
	CROSS JOIN (SELECT DISTINCT LeadGen FROM DailyBreakdown WHERE LeadGen IS NOT NULL UNION SELECT 'Others') leadgens
	LEFT JOIN DailyBreakdown db ON dr.ExportedDate = db.ExportedDate AND leadgens.LeadGen = db.LeadGen
	ORDER BY dr.ExportedDate,
		CASE WHEN db.LeadGen IS NULL THEN 0 ELSE 1 END, -- Show Others last
		db.LeadGen;
END
GO