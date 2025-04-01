CREATE OR ALTER PROCEDURE [dbo].[sm_spInsertCallState]
@callstate VARCHAR(500),
@isactive BIT
AS
BEGIN
	INSERT INTO CallState(GuId, StateName, IsActive)VALUES(NEWID(), @callstate, @isactive)
END
GO