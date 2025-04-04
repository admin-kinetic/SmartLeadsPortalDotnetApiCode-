CREATE   PROCEDURE [dbo].[sm_spGetAllExcludeKeywordsList]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, ExcludedKeyword, IsActive FROM ExcludeKeywords
	WHERE (@Search = '' OR ExcludedKeyword LIKE '%' + @Search + '%')
	ORDER BY Id ASC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END

GO

