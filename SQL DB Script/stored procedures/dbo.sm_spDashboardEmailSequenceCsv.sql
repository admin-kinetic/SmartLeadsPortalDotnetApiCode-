CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardEmailSequenceCsv]
@startDate DATETIME, 
@endDate DATETIME,
@bdr VARCHAR(500),
@createdby VARCHAR(500),
@qaby VARCHAR(500),
@campaignid INT
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT DISTINCT sla.FirstName, 
	sla.LastName, 
	sla.Email, sla.CompanyName, sla.[Location] AS Country, sla.CreatedAt AS DateCreated, ses.SentTime AS DateEmailed,
	CASE WHEN (sla.BDR = '' OR sla.BDR IS NULL) THEN 'Other'
			ELSE sla.BDR 
		END AS BdrName,
	sla.CreatedBy, sla.QABy AS QaPassedBy, 
	CASE 
		WHEN CHARINDEX('re. your ', ses.EmailSubject) > 0 
			 AND CHARINDEX(' ad on', ses.EmailSubject) > CHARINDEX('re. your ', ses.EmailSubject)
		THEN TRIM(
			SUBSTRING(
				ses.EmailSubject,
				CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '),
				CHARINDEX(' ad on', ses.EmailSubject) - (CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '))
			)
		)
		ELSE NULL
	END AS RoleAdvertised
	FROM [dbo].[SmartLeadAllLeads] sla
	INNER JOIN SmartLeadsEmailStatistics ses ON sla.Email = ses.LeadEmail
	WHERE ses.SequenceNumber = 1
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
	AND (@startDate IS NULL OR @endDate IS NULL
    OR (CONVERT(DATE, ses.SentTime) >= CONVERT(DATE, @startDate) 
        AND CONVERT(DATE, ses.SentTime) <= CONVERT(DATE, @endDate))
	)
END
GO