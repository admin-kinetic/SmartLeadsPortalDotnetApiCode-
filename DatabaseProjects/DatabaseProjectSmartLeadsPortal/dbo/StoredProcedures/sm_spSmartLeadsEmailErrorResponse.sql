CREATE   PROCEDURE [dbo].[sm_spSmartLeadsEmailErrorResponse]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Count(Id) AS TotalEmailErrorResponse
	FROM SmartLeadsExportedContacts 
	WHERE ModifiedAt IS NOT NULL 
		AND ExportedDate >='2025-01-01'
		AND HasReply = 1
		AND (
        CHARINDEX('error', MessageHistory) > 0
        OR CHARINDEX('not found', MessageHistory) > 0
        OR CHARINDEX('problem deliver', MessageHistory) > 0
        OR CHARINDEX('be delivered', MessageHistory) > 0
        OR CHARINDEX('blocked', MessageHistory) > 0
        OR CHARINDEX('unable to receive', MessageHistory) > 0
        OR CHARINDEX('unable to deliver', MessageHistory) > 0
    );
END

GO

