CREATE   PROCEDURE [dbo].[sm_spGetCallPurposeById]
	@guid uniqueidentifier
AS  
BEGIN   
	SELECT Id, GuId, CallPurposeName, IsActive FROM CallPurpose WHERE GuId=@guid
END
GO

