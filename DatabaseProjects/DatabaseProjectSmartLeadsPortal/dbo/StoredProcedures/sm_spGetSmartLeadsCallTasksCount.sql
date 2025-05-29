CREATE   PROCEDURE [dbo].[sm_spGetSmartLeadsCallTasksCount]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS Total
    FROM SmartLeadsEmailStatistics sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
        --FROM Webhooks wh
        --INNER JOIN SmartLeadAllLeads sl
        --    ON JSON_VALUE(wh.Request, '$.sl_email_lead_id') = sl.LeadId
        --WHERE JSON_VALUE(wh.Request, '$.time_clicked') IS NOT NULL
END
GO

