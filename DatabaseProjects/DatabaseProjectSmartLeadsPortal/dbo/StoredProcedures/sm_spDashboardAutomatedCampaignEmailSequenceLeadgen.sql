CREATE PROCEDURE [dbo].[sm_spDashboardAutomatedCampaignEmailSequenceLeadgen]
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
 SELECT DATEADD(day, n, CAST(@startDate AS date)) AS ExportedDate FROM Numbers
 ),
 DailyTotals AS (
 -- Calculate total count per day
 SELECT CAST(sla.CreatedAt AS date) AS ExportedDate, COUNT(sla.Id) AS TotalCount
 FROM [dbo].[SmartLeadAllLeads] sla
		INNER JOIN [dbo].[SmartLeadsEmailStatistics] sle ON sla.Email = sle.LeadEmail
		WHERE sla.CreatedAt BETWEEN CAST(@startDate AS date) AND CAST(@endDate AS date)
		AND sle.SequenceNumber = 1
 GROUP BY CAST(CreatedAt AS date)
 ),
 DailyBreakdown AS (
 -- Calculate count per day per CreatedBy
 SELECT CAST(sla.CreatedAt AS date) AS ExportedDate, sla.CreatedBy AS LeadGen, COUNT(sla.Id) AS TotalCount
 FROM [dbo].[SmartLeadAllLeads] sla
		INNER JOIN [dbo].[SmartLeadsEmailStatistics] sle ON sla.Email = sle.LeadEmail
		WHERE sla.CreatedAt BETWEEN CAST(@startDate AS date) AND CAST(@endDate AS date)
		AND sle.SequenceNumber = 1
 GROUP BY CAST(sla.CreatedAt AS date), CreatedBy
 )
 -- Combine totals and breakdown, ensuring all dates appear
 SELECT dr.ExportedDate,
 COALESCE(NULLIF(db.LeadGen, ''), 'Others') AS LeadGen,
 COALESCE(CASE WHEN db.LeadGen IS NULL THEN dt.TotalCount ELSE db.TotalCount END, 0) AS TotalCount
 FROM DateRange dr
 LEFT JOIN DailyTotals dt ON dr.ExportedDate = dt.ExportedDate
 LEFT JOIN DailyBreakdown db ON dr.ExportedDate = db.ExportedDate
 ORDER BY dr.ExportedDate, CASE WHEN db.LeadGen IS NULL THEN 0 ELSE 1 END, db.LeadGen;
END
GO

