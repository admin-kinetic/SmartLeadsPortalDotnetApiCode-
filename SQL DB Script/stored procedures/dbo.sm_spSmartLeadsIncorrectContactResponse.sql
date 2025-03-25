CREATE OR ALTER PROCEDURE [dbo].[sm_spSmartLeadsIncorrectContactResponse]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Count(Id) AS TotalIncorrectContactResponse
	FROM SmartLeadsExportedContacts 
	WHERE ModifiedAt IS NOT NULL 
	AND ExportedDate >= '2025-01-01'
	AND HasReply = 1
	AND (
        CHARINDEX('not the right person', MessageHistory) > 0
        OR CHARINDEX('no longer working with', MessageHistory) > 0
        OR CHARINDEX('no longer work for', MessageHistory) > 0
        OR CHARINDEX('not interested', MessageHistory) > 0
		OR CHARINDEX('in charge', MessageHistory) > 0
		OR CHARINDEX('onshore', MessageHistory) > 0
		OR CHARINDEX('remove me', MessageHistory) > 0
		OR CHARINDEX('unsubscribe', MessageHistory) > 0
    );
END
GO