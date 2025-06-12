CREATE OR ALTER PROCEDURE [dbo].[sm_spGetDashboardLeadgenList]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, LeadgenName AS ValueName, IsActive FROM [dbo].[Leadgens] WHERE IsActive=1
END
GO