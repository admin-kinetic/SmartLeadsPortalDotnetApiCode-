CREATE   PROCEDURE [dbo].[sm_spSmartLeadsInvalidResponse]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Count(Id) AS TotalInvalidResponse FROM SmartLeadsExportedContacts WHERE HasReviewed = 0
END

GO

