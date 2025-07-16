CREATE PROCEDURE [dbo].[sm_spDashboardEmailCampaignBDR]
@startDate DATETIME, 
@endDate DATETIME
AS 
BEGIN 
	SET NOCOUNT ON;
	SELECT CASE 
			WHEN BDR = '' THEN 'Other'
			ELSE BDR 
		END AS LeadsUser,
		COUNT(Id) AS Total
	FROM 
		[dbo].[SmartLeadAllLeads]
	WHERE BDR IS NOT NULL
	AND CreatedAt >= @startDate
 AND CreatedAt < DATEADD(DAY, 1, @endDate)
	GROUP BY 
	CASE 
			WHEN BDR = '' THEN 'Other'
			ELSE BDR 
		END
	ORDER BY Total DESC;
END
GO

