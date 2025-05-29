CREATE OR ALTER PROCEDURE [dbo].[sm_spUpdateOutboundCallInfo]
@uniquecallid VARCHAR(255),
@filename VARCHAR(500)
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[OutboundCalls]
			SET AzureStorageCallRecordingLink= @filename
			WHERE UniqueCallId=@uniquecallid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO