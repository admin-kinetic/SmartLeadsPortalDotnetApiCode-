CREATE OR ALTER PROCEDURE [dbo].[sm_spUpdateCallDisposition]
@guid uniqueidentifier,
@calldisposition VARCHAR(500),
@isactive BIT
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE CallDisposition SET CallDispositionName = @calldisposition,
			IsActive = @isactive
			WHERE GuId = @guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO