CREATE OR ALTER PROCEDURE [dbo].[sm_spUpdateCategorySettings]
@id INT,
@opencount INT,
@clickcount INT
AS
BEGIN
	BEGIN TRANSACTION; -- Start a transaction

		BEGIN TRY
			UPDATE [dbo].[CategorySettings]
			SET OpenCount= @opencount,
			ClickCount = @clickcount
			WHERE Id = @id
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK; -- Rollback the transaction in case of an error
		END CATCH
END
GO