CREATE OR ALTER PROCEDURE [dbo].[sm_spGetInboundCallsByCallLogs] 
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
		AzureStorageCallRecordingLink FROM [dbo].[InboundCalls]
	WHERE CallerId = @callerid
	AND DestNumber LIKE '%' +@destnumber+'%'
	ORDER BY CallStartAt DESC
END
GO