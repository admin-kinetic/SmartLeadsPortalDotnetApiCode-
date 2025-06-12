CREATE OR ALTER PROCEDURE [dbo].[sm_spGetDashboardCampaignsList]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT Id, GuId, CampaignName AS ValueName, IsActive  FROM [dbo].[Campaigns] WHERE IsActive=1
END
GO