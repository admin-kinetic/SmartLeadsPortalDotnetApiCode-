CREATE OR ALTER PROCEDURE [dbo].[sm_spGetAllExcludeKeywordsListCount]
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(*) AS Total FROM ExcludeKeywords
	WHERE (@Search = '' OR ExcludedKeyword LIKE '%' + @Search + '%')
END
GO