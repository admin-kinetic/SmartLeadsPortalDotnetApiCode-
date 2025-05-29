CREATE OR ALTER PROCEDURE [dbo].[sm_spGetAllCallPurposeList]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, CallPurposeName, IsActive FROM CallPurpose
	WHERE (@Search = '' OR CallPurposeName LIKE '%' + @Search + '%')
	ORDER BY Id ASC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO