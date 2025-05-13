CREATE   PROCEDURE [dbo].[sm_spUpdateCallPurpose]
@guid uniqueidentifier,
@callpurpose VARCHAR(500),
@isactive BIT
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE CallPurpose SET CallPurposeName = @callpurpose,
			IsActive = @isactive
			WHERE GuId = @guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO

