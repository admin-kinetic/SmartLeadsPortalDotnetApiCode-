CREATE OR ALTER PROCEDURE [dbo].[sm_spGetSmartLeadsCallTasks]
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    --WITH ClickCounts AS (
    --    SELECT 
    --        sl.LeadId,
    --        sl.Email,
    --        sl.FirstName,
    --        sl.LastName,
    --        JSON_VALUE(wh.Request, '$.sequence_number') AS SequenceNumber,
    --        JSON_VALUE(wh.Request, '$.campaign_name') AS CampaignName,
    --        JSON_VALUE(wh.Request, '$.subject') AS SubjectName,
    --        COUNT(*) AS ClickCount 
    --    FROM Webhooks wh
    --    INNER JOIN SmartLeadAllLeads sl
    --        ON JSON_VALUE(wh.Request, '$.sl_email_lead_id') = sl.LeadId
    --    WHERE JSON_VALUE(wh.Request, '$.time_clicked') IS NOT NULL
    --    GROUP BY 
    --        sl.LeadId, sl.Email, sl.FirstName, sl.LastName,
    --        JSON_VALUE(wh.Request, '$.sequence_number'),
    --        JSON_VALUE(wh.Request, '$.campaign_name'),
    --        JSON_VALUE(wh.Request, '$.subject')
    --)

    --SELECT * 
    --FROM ClickCounts
    --ORDER BY ClickCount DESC
	SELECT 
		JSON_VALUE(wh.Request, '$.sl_email_lead_id') AS LeadId, 
		sle.LeadEmail AS Email, 
		JSON_VALUE(wh.Request, '$.to_name') AS FullName, 
		sle.SequenceNumber,
		JSON_VALUE(wh.Request, '$.campaign_name') AS CampaignName, 
		sle.EmailSubject AS SubjectName, 
		sle.OpenCount, 
		sle.ClickCount
    FROM SmartLeadsEmailStatistics sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
    ORDER BY sle.OpenCount DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
    OPTION (RECOMPILE);
END
GO
