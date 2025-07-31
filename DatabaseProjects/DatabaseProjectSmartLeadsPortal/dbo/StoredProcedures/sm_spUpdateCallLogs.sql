CREATE PROCEDURE [dbo].[sm_spUpdateCallLogs]
	@guid uniqueidentifier,
	@callpurposeid INT,
	@calldispositionid INT,
	@notes VARCHAR(MAX),
	@calltagsid INT
AS
BEGIN
	BEGIN TRANSACTION;
	-- Start a transaction

	BEGIN TRY
		UPDATE [dbo].[Calls] SET 
			CallPurposeId = @callpurposeid,
			CallDispositionId = @calldispositionid, 
			Notes = @notes,
			CallTagsId = @calltagsid
		WHERE GuId = @guid
		COMMIT;
	END TRY
	BEGIN CATCH
		ROLLBACK; -- Rollback the transaction in case of an error
	END CATCH
END
GO

