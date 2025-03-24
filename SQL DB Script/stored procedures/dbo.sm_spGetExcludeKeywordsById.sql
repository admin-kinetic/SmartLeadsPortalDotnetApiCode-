CREATE OR ALTER PROCEDURE [dbo].[sm_spGetExcludeKeywordsById]
	@guid uniqueidentifier
AS  
BEGIN   
	SELECT Id, GuId, ExcludedKeyword, IsActive FROM ExcludeKeywords WHERE GuId=@guid
END
GO