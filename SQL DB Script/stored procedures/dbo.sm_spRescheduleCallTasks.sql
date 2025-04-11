CREATE OR ALTER PROCEDURE [dbo].[sm_spRescheduleCallTasks]
@guid uniqueidentifier,
@due DATETIME
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[SmartLeadsEmailStatistics] SET Due=@due WHERE GuId=@guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO