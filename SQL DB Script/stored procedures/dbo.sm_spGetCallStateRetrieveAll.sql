CREATE OR ALTER PROCEDURE [dbo].[sm_spGetCallStateRetrieveAll]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, StateName, IsActive FROM CallState
	ORDER BY StateName ASC
END
GO