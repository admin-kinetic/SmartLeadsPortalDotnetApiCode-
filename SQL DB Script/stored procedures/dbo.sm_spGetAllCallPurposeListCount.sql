CREATE OR ALTER PROCEDURE [dbo].[sm_spGetAllCallPurposeListCount]
	@Search NVARCHAR(255) = ''
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT COUNT(*) AS Total FROM CallPurpose
	WHERE (@Search = '' OR CallPurposeName LIKE '%' + @Search + '%')
END
GO