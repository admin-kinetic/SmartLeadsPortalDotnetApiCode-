CREATE OR ALTER PROCEDURE [dbo].[sm_spGetCallStateById]
	@guid uniqueidentifier
AS  
BEGIN   
	SELECT Id, GuId, StateName, IsActive FROM CallState WHERE GuId=@guid
END
GO