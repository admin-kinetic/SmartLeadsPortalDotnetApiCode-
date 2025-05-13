CREATE   PROCEDURE [dbo].[sm_spGetSmartLeadsProspect]
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
	SELECT DISTINCT JSON_VALUE(wh.Request, '$.sl_email_lead_id') AS LeadId,
	sle.LeadEmail AS Email, 
	JSON_VALUE(wh.Request, '$.to_name') AS FullName
	from [dbo].[SmartLeadsEmailStatistics] sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	--WHERE (@Search = '' OR sle.LeadEmail  LIKE '%' + @Search + '%')
	ORDER BY sle.LeadEmail DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO

