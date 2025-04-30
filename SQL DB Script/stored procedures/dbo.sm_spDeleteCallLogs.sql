CREATE OR ALTER PROCEDURE [dbo].[sm_spDeleteCallLogs]
@guid uniqueidentifier
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[Calls] SET IsDeleted = 1
			WHERE GuId=@guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO