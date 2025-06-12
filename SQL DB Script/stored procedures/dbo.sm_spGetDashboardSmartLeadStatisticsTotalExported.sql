CREATE OR ALTER PROCEDURE [dbo].[sm_spGetDashboardSmartLeadStatisticsTotalExported]
@startDate DATETIME,
@endDate DATETIME,
@bdr VARCHAR(500),
@createdby VARCHAR(500),
@qaby VARCHAR(500),
@campaignid INT
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(DISTINCT sla.Email) AS TotalExported from SmartLeadAllLeads sla
	INNER JOIN SmartLeadsEmailStatistics ses ON sla.Email = ses.LeadEmail
	WHERE (sla.CreatedAt IS NOT NULL OR sla.CreatedAt <> '')
	AND (@bdr = '' OR sla.BDR = @bdr)
	AND (@createdby = '' OR sla.CreatedBy = @createdby)
	AND (@qaby = '' OR sla.QABy = @qaby)
	-- Campaign-specific logic
	AND (
		@campaignId IS NULL
		OR (@campaignId = 1 AND sla.CreatedBy <> 'Bots' AND sla.BDR <> 'Steph')
		OR (@campaignId = 2 AND sla.CreatedBy = 'Bots' AND sla.BDR = 'Steph')
		OR (@campaignId = 3 AND sla.CreatedBy <> 'Bots' AND sla.BDR = 'Steph')
	)
	-- Date range filter
	AND (
		@startDate IS NULL OR @endDate IS NULL
		OR (CONVERT(DATE, sla.CreatedAt) >= CONVERT(DATE, @startDate) 
			AND CONVERT(DATE, sla.CreatedAt) <= CONVERT(DATE, @endDate))
	)
END
GO