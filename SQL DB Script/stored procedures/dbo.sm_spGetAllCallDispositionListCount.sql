CREATE OR ALTER PROCEDURE [dbo].[sm_spGetAllCallDispositionListCount]
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(*) AS Total FROM CallDisposition
	WHERE (@Search = '' OR CallDispositionName LIKE '%' + @Search + '%')
END
GO