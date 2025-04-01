CREATE OR ALTER PROCEDURE [dbo].[sm_spGetAllCallDispositionList]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, CallDispositionName, IsActive FROM CallDisposition
	WHERE (@Search = '' OR CallDispositionName LIKE '%' + @Search + '%')
	ORDER BY Id ASC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO