CREATE OR ALTER PROCEDURE [dbo].[sm_spInsertCallPurpose]
@callpurpose VARCHAR(500),
@isactive BIT
AS
BEGIN
	INSERT INTO CallPurpose(GuId, CallPurposeName, IsActive)VALUES(NEWID(), @callpurpose, @isactive)
END
GO