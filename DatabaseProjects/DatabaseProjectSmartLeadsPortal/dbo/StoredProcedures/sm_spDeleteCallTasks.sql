CREATE   PROCEDURE [dbo].[sm_spDeleteCallTasks]
@guid uniqueidentifier
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[SmartLeadsEmailStatistics] SET IsDeleted = 1
			WHERE GuId=@guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO

