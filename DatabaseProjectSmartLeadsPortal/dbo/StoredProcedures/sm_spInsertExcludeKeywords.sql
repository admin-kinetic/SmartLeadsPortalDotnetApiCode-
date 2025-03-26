CREATE   PROCEDURE [dbo].[sm_spInsertExcludeKeywords]
@excludedkeywords VARCHAR(500),
@isactive BIT
AS
BEGIN
	INSERT INTO ExcludeKeywords(GuId, ExcludedKeyword, IsActive)VALUES(NEWID(), @excludedkeywords, @isactive)
END

GO

