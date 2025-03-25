CREATE OR ALTER PROCEDURE [dbo].[sm_spSmartLeadsValidResponse]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Count(Id) AS TotalValidResponse FROM SmartLeadsExportedContacts WHERE HasReviewed = 1
END
GO