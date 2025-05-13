CREATE   PROCEDURE [dbo].[sm_spDeleteCallDisposition]
@guid uniqueidentifier
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			DELETE FROM CallDisposition where GuId=@guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO

