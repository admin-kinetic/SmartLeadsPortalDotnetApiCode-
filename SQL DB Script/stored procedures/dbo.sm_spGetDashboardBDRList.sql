CREATE OR ALTER PROCEDURE [dbo].[sm_spGetDashboardBDRList]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, BdName AS ValueName, IsActive FROM [dbo].[Bdr] WHERE IsActive=1
END
GO