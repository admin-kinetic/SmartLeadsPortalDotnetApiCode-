CREATE   PROCEDURE [dbo].[sm_spGetAllCallStateList]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, StateName, IsActive FROM CallState
	WHERE (@Search = '' OR StateName LIKE '%' + @Search + '%')
	ORDER BY Id ASC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO

