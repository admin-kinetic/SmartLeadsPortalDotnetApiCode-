CREATE   PROCEDURE [dbo].[sm_spGetAllCallTagsListCount]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(*) AS Total FROM Tags
	WHERE (@Search = '' OR TagName LIKE '%' + @Search + '%')
END
GO

