CREATE   PROCEDURE [dbo].[sm_spGetSmartLeadsByExportedDate]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT 
		CAST(ExportedDate AS DATE) AS Date, 
		COUNT(Id) AS Count
	FROM SmartLeadsExportedContacts
	WHERE ExportedDate >= CAST(GETDATE() - 10 AS DATE)
	GROUP BY CAST(ExportedDate AS DATE)
	ORDER BY Date DESC;
END

GO

