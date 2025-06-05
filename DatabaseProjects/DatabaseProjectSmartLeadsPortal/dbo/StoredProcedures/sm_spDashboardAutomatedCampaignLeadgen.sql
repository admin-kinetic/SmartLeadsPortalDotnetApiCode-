CREATE PROCEDURE [dbo].[sm_spDashboardAutomatedCampaignLeadgen]
@startDate DATETIME, 
@endDate DATETIME
AS 
BEGIN 
 SET NOCOUNT ON;
 WITH Numbers AS (
		SELECT TOP (DATEDIFF(day, @startDate, @endDate) + 1)
			ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
		FROM sys.objects
	),
	DateRange AS (
		SELECT DATEADD(day, n, @startDate) AS ExportedDate
		FROM Numbers
	),
	DailyBreakdown AS (
		SELECT 
			CAST(sla.CreatedAt AS date) AS ExportedDate,
			ISNULL(NULLIF(sla.CreatedBy, ''), 'Others') AS LeadGen,
			COUNT(sla.Id) AS TotalCount
		FROM [dbo].[SmartLeadAllLeads] sla
		INNER JOIN [dbo].[SmartLeadsEmailStatistics] sle ON sla.Email = sle.LeadEmail
		WHERE sla.CreatedAt BETWEEN @startDate AND @endDate
			AND sle.SequenceNumber = 1
			AND sla.BDR = 'Steph'
		GROUP BY CAST(sla.CreatedAt AS date), ISNULL(NULLIF(sla.CreatedBy, ''), 'Others')
	),
	DistinctLeadGens AS (
		SELECT DISTINCT LeadGen FROM DailyBreakdown
	),
	-- Combine all dates with all leadgens
	DateLeadGenGrid AS (
		SELECT 
			dr.ExportedDate,
			lg.LeadGen
		FROM DateRange dr
		CROSS JOIN DistinctLeadGens lg
	)
	SELECT 
		CONVERT(VARCHAR(10), dlg.ExportedDate, 23) AS ExportedDate,
		dlg.LeadGen,
		ISNULL(db.TotalCount, 0) AS TotalCount
	FROM DateLeadGenGrid dlg
	LEFT JOIN DailyBreakdown db
		ON dlg.ExportedDate = db.ExportedDate AND dlg.LeadGen = db.LeadGen
	ORDER BY dlg.ExportedDate DESC, 
			 CASE WHEN dlg.LeadGen = 'Others' THEN 1 ELSE 0 END,
			 dlg.LeadGen;
END
GO

