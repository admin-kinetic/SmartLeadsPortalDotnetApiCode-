CREATE   PROCEDURE [dbo].[sm_spGetOutboundCallsByCallLogs] 
@callerid VARCHAR(200),
@destnumber VARCHAR(200)
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT TOP 1 
		UniqueCallId, 
		CallerId, 
		UserName, 
		DestNumber, 
		CallStartAt, 
		ConnectedAt, 
		CallDuration, 
		ConversationDuration, 
		AzureStorageCallRecordingLink FROM [dbo].[OutboundCalls] 
	WHERE CallerId = @callerid
	AND DestNumber = @destnumber
	ORDER BY CallStartAt DESC
END
GO

