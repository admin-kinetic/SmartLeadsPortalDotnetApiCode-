CREATE OR ALTER PROCEDURE [dbo].[sm_spGetSmartLeadsCallTasks]
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
		sle.Id,
		sle.GuId,
		JSON_VALUE(wh.Request, '$.sl_email_lead_id') AS LeadId, 
		sle.LeadEmail AS Email, 
		JSON_VALUE(wh.Request, '$.to_name') AS FullName, 
		sle.SequenceNumber,
		JSON_VALUE(wh.Request, '$.campaign_name') AS CampaignName, 
		sle.EmailSubject AS SubjectName, 
		sle.OpenCount, 
		sle.ClickCount,
		cs.StateName AS CallState
    FROM SmartLeadsEmailStatistics sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
    ORDER BY sle.OpenCount DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
    OPTION (RECOMPILE);
END
GO
