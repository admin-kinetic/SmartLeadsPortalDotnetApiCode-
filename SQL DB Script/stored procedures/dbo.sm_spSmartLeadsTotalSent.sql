CREATE OR ALTER PROCEDURE [dbo].[sm_spSmartLeadsTotalSent]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(Id) AS TotalLeadsSent FROM SmartLeadsExportedContacts WHERE CONVERT(DATE, ExportedDate)>='2025-01-01'
END
GO