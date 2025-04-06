CREATE   PROCEDURE [dbo].[sm_spSmartLeadsOutOfOfficeResponse]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Count(Id) AS TotalOutOfOfficeResponse
	FROM SmartLeadsExportedContacts 
	WHERE ModifiedAt IS NOT NULL 
		AND ExportedDate >='2025-01-01'
		AND HasReply = 1
		AND (
        CHARINDEX('out of office', MessageHistory) > 0
        OR CHARINDEX('on leave', MessageHistory) > 0
        OR CHARINDEX('maternity leave', MessageHistory) > 0
        OR CHARINDEX('leave', MessageHistory) > 0
    );
END

GO

