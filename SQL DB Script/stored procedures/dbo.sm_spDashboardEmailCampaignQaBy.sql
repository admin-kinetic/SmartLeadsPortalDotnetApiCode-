CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardEmailCampaignQaBy]
@startDate DATETIME, 
@endDate DATETIME
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT CASE 
			WHEN QABy = '' THEN 'Other'
			ELSE QABy 
		END AS LeadsUser,
		COUNT(Id) AS Total
	FROM 
		[dbo].[SmartLeadAllLeads]
	WHERE QABy IS NOT NULL
	AND CreatedAt >= @startDate
    AND CreatedAt < DATEADD(DAY, 1, @endDate)
	GROUP BY 
	CASE 
			WHEN QABy = '' THEN 'Other'
			ELSE QABy 
		END
	ORDER BY Total DESC;
END
GO