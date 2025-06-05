CREATE PROCEDURE [dbo].[sm_spDashboardEmailCampaignCreatedBy] 
@startDate DATETIME, 
@endDate DATETIME
AS 
BEGIN 
	SET NOCOUNT ON;
	SELECT CASE 
			WHEN CreatedBy = '' THEN 'Other'
			ELSE CreatedBy 
		END AS LeadsUser,
		COUNT(Id) AS Total
	FROM 
		[dbo].[SmartLeadAllLeads]
	WHERE CreatedBy IS NOT NULL
	AND CreatedAt >= @startDate
 AND CreatedAt < DATEADD(DAY, 1, @endDate)
	GROUP BY 
	CASE 
			WHEN CreatedBy = '' THEN 'Other'
			ELSE CreatedBy 
		END
	ORDER BY Total DESC;
END
GO

