CREATE OR ALTER PROCEDURE [dbo].[sm_spGetCallPurposeRetrieveAll]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, CallPurposeName, IsActive FROM CallPurpose
	ORDER BY CallPurposeName ASC
END
GO