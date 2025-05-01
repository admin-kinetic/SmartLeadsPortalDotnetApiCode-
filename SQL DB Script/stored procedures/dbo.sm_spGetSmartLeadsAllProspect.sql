CREATE OR ALTER PROCEDURE [dbo].[sm_spGetSmartLeadsAllProspect]
AS
BEGIN
    SET NOCOUNT ON;
	SELECT DISTINCT JSON_VALUE(wh.Request, '$.sl_email_lead_id') AS LeadId,
	sle.LeadEmail AS Email, 
	JSON_VALUE(wh.Request, '$.to_name') AS FullName
	from [dbo].[SmartLeadsEmailStatistics] sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	ORDER BY sle.LeadEmail DESC
END
GO