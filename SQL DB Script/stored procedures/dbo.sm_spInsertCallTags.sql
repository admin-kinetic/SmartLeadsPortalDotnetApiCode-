CREATE OR ALTER PROCEDURE [dbo].[sm_spInsertCallTags]
@tagname VARCHAR(500),
@isactive BIT
AS
BEGIN
	INSERT INTO Tags(GuId, TagName, IsActive)VALUES(NEWID(), @tagname, @isactive)
END
GO