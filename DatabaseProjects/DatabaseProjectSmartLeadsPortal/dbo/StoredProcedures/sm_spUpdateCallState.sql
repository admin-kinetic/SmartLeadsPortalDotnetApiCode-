CREATE   PROCEDURE [dbo].[sm_spUpdateCallState]
@guid uniqueidentifier,
@callstate VARCHAR(500),
@isactive BIT
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE CallState SET StateName = @callstate,
			IsActive = @isactive
			WHERE GuId = @guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO

