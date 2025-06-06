CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardAutomatedCampaignEmailSequence]
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
	DailyTotals AS (
		-- Calculate total count per day
		SELECT CAST(sla.CreatedAt AS date) AS ExportedDate, COUNT(sla.Id) AS TotalCount
		FROM [dbo].[SmartLeadAllLeads] sla
		INNER JOIN [dbo].[SmartLeadsEmailStatistics] sle ON sla.Email = sle.LeadEmail
		WHERE sla.CreatedAt BETWEEN CAST(@startDate AS date) AND CAST(@endDate AS date)
		AND sle.SequenceNumber = 1
		AND sla.BDR = 'Steph'
		GROUP BY CAST(sla.CreatedAt AS date)
	)
	-- Combine totals with date range, ensuring all dates appear
	SELECT dr.ExportedDate, COALESCE(dt.TotalCount, 0) AS TotalCount
	FROM DateRange dr
	LEFT JOIN DailyTotals dt ON dr.ExportedDate = dt.ExportedDate
	ORDER BY dr.ExportedDate DESC;
END
GO