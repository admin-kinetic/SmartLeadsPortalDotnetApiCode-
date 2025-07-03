CREATE OR ALTER PROCEDURE [dbo].[sm_spGetDashboardSmartLeadAnalyticsTotalReply]
@startDate DATETIME,
@endDate DATETIME,
@bdr VARCHAR(500),
@createdby VARCHAR(500),
@qaby VARCHAR(500),
@campaignid INT
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(DISTINCT sla.Email) AS TotalReplied from SmartLeadAllLeads sla
	INNER JOIN SmartLeadsEmailStatistics ses ON sla.Email = ses.LeadEmail
	WHERE (ses.ReplyTime IS NOT NULL OR ses.ReplyTime <> '')
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
	    OR (CONVERT(DATE, ses.ReplyTime) >= CONVERT(DATE, @startDate) 
	        AND CONVERT(DATE, ses.ReplyTime) <= CONVERT(DATE, @endDate))
	)
END
GO