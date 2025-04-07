CREATE OR ALTER PROCEDURE [dbo].[sm_spGetCallDispositionRetrieveAll]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, CallDispositionName, IsActive FROM CallDisposition
	ORDER BY CallDispositionName ASC
END
GO