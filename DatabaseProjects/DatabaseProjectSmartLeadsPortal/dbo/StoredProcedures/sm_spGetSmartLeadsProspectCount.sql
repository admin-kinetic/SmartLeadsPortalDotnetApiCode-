CREATE   PROCEDURE [dbo].[sm_spGetSmartLeadsProspectCount]
AS
BEGIN
    SET NOCOUNT ON;
	SELECT DISTINCT COUNT(sle.Id) AS Total
	from [dbo].[SmartLeadsEmailStatistics] sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	--WHERE (@Search = '' OR sle.LeadEmail  LIKE '%' + @Search + '%')
END
GO

