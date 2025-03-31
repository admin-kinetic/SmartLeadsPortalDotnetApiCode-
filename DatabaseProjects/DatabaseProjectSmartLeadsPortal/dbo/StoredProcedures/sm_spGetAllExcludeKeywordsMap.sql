CREATE   PROCEDURE [dbo].[sm_spGetAllExcludeKeywordsMap]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, ExcludedKeyword, IsActive FROM ExcludeKeywords
END

GO

