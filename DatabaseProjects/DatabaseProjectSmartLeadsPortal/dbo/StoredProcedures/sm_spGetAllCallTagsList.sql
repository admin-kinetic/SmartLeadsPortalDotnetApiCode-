CREATE   PROCEDURE [dbo].[sm_spGetAllCallTagsList]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, TagName, IsActive FROM Tags
	WHERE (@Search = '' OR TagName LIKE '%' + @Search + '%')
	ORDER BY Id ASC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO

