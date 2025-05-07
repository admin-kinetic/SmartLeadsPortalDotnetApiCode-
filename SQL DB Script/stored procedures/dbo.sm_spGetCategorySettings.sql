CREATE OR ALTER PROCEDURE [dbo].[sm_spGetCategorySettings]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, CategoryName, OpenCount, ClickCount FROM [dbo].[CategorySettings]
END
GO