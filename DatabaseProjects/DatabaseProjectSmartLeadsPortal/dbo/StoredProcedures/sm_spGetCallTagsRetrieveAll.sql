CREATE   PROCEDURE [dbo].[sm_spGetCallTagsRetrieveAll]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, TagName, IsActive FROM Tags
	ORDER BY TagName ASC
END
GO

