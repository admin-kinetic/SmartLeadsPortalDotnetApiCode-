CREATE OR ALTER PROCEDURE [dbo].[sm_spInsertCallDisposition]
@calldisposition VARCHAR(500),
@isactive BIT
AS
BEGIN
	INSERT INTO CallDisposition(GuId, CallDispositionName, IsActive)VALUES(NEWID(), @calldisposition, @isactive)
END
GO