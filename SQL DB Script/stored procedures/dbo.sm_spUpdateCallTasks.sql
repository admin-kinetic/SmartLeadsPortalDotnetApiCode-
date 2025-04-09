CREATE OR ALTER PROCEDURE [dbo].[sm_spUpdateCallTasks]
@guid uniqueidentifier,
@stateid INT,
@assignto INT,
@notes VARCHAR(MAX)
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[SmartLeadsEmailStatistics] SET AssignedTo=@assignto, CallStateId=@stateid, Notes=@notes WHERE GuId=@guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO