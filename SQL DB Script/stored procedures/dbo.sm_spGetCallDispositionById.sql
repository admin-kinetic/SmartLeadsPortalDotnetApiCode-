CREATE OR ALTER PROCEDURE [dbo].[sm_spGetCallDispositionById]
	@guid uniqueidentifier
AS  
BEGIN   
	SELECT Id, GuId, CallDispositionName, IsActive FROM CallDisposition WHERE GuId=@guid
END
GO