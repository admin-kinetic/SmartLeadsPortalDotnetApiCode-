CREATE PROCEDURE [dbo].[sm_spDeactivateUsers]
@id INT
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[Users]
			SET IsActive = 0
			WHERE EmployeeId = @id
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO

