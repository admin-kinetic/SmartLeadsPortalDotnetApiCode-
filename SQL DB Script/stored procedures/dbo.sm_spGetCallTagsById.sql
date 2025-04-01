CREATE OR ALTER PROCEDURE [dbo].[sm_spGetCallTagsById]
	@guid uniqueidentifier
AS  
BEGIN   
	SELECT Id, GuId, TagName, IsActive FROM Tags WHERE GuId=@guid
END
GO