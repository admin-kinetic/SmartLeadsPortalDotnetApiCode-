CREATE PROCEDURE [dbo].[sm_spSmartLeadCampaignsActive]
AS 
BEGIN
	SET NOCOUNT ON;
	SELECT Id FROM [dbo].[SmartLeadCampaigns] WHERE [Status]='active' AND Bdr = 'Steph'
END
GO

