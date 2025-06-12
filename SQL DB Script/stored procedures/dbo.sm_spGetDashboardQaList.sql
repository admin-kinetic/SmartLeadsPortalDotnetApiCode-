CREATE OR ALTER PROCEDURE [dbo].[sm_spGetDashboardQaList]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, QaName AS ValueName, IsActive FROM [dbo].[Qa]WHERE IsActive=1
END
GO