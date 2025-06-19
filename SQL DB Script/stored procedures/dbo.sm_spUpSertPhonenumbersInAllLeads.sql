CREATE OR ALTER PROCEDURE [dbo].[sm_spUpSertPhonenumbersInAllLeads]
@email VARCHAR(255),
@phonenumber VARCHAR(100)
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[SmartLeadAllLeads]
			SET PhoneNumber= @phonenumber
			WHERE Email = @email
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO