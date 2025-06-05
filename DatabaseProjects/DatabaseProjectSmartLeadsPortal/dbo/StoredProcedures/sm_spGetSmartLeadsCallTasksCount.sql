CREATE   PROCEDURE [dbo].[sm_spGetSmartLeadsCallTasksCount]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS Total
    FROM SmartLeadsEmailStatistics sle
	INNER JOIN SmartLeadAllLeads al ON  al.Email = sle.LeadEmail
    INNER JOIN SmartLeadCampaigns c ON c.Id = al.CampaignId
    INNER JOIN SmartleadsAccountCampaigns ac ON ac.CampaignId = c.id
    INNER JOIN SmartleadsAccountUsers au ON au.SmartleadsAccountId = ac.SmartleadsAccountId
    LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
    WHERE au.EmployeeId = @EmployeeId 
END
GO

