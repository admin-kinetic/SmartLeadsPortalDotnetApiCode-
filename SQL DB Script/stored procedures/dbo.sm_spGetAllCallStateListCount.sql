CREATE OR ALTER PROCEDURE [dbo].[sm_spGetAllCallStateListCount]
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(*) AS Total FROM CallState
	WHERE (@Search = '' OR StateName LIKE '%' + @Search + '%')
END
GO