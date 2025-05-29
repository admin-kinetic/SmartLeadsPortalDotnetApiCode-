CREATE   PROCEDURE [dbo].[sm_spGetHasReplyCount]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(Id) AS HasReplyCount FROM SmartLeadsExportedContacts WHERE HasReply = 1
END

GO

