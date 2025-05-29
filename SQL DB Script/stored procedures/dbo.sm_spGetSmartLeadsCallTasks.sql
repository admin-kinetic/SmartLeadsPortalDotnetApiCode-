CREATE OR ALTER PROCEDURE [dbo].[sm_spGetSmartLeadsCallTasks]
    @EmployeeId INT,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
        sle.Id,
        sle.GuId,
        al.LeadId, 
        sle.LeadEmail AS Email, 
        al.FirstName + ' ' + al.LastName AS [FullName], 
        sle.SequenceNumber,
        c.Name AS CampaignName, 
        sle.EmailSubject AS SubjectName, 
        sle.OpenCount, 
        sle.ClickCount,
        cs.StateName AS CallState
    FROM SmartLeadsEmailStatistics sle
    INNER JOIN SmartLeadAllLeads al ON  al.Email = sle.LeadEmail
    INNER JOIN SmartLeadCampaigns c ON c.Id = al.CampaignId
    INNER JOIN SmartleadsAccountCampaigns ac ON ac.CampaignId = c.id
    INNER JOIN SmartleadsAccountUsers au ON au.SmartleadsAccountId = ac.SmartleadsAccountId
    LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
    WHERE au.EmployeeId = @EmployeeId 
	ORDER BY sle.OpenCount DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
    OPTION (RECOMPILE);
END
GO
