CREATE   PROCEDURE [dbo].[sm_spGetSmartLeadsByRepliedDate]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT 
		CAST(RepliedAt AS DATE) AS Date, 
		COUNT(Id) AS Count
	FROM SmartLeadsExportedContacts
	WHERE RepliedAt >= CAST(GETDATE() - 10 AS DATE)
	GROUP BY CAST(RepliedAt AS DATE)
	ORDER BY Date DESC;
END

GO

