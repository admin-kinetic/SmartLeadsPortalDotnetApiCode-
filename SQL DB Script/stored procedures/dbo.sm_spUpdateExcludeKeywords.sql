CREATE OR ALTER PROCEDURE [dbo].[sm_spUpdateExcludeKeywords]
@guid uniqueidentifier,
@excludedkeywords VARCHAR(500),
@isactive BIT
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE ExcludeKeywords SET ExcludedKeyword = @excludedkeywords,
			IsActive = @isactive
			WHERE GuId = @guid
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO