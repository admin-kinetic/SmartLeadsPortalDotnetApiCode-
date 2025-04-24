CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardPastDueTaskTotal]
AS
BEGIN
    SET NOCOUNT ON;

	SELECT COUNT(*) AS Total
	FROM SmartLeadsEmailStatistics sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
	LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
	WHERE (sle.CallStateId IS NULL OR sle.CallStateId <> 1)
	AND (sle.Due IS NOT NULL AND sle.Due <= GETDATE())
END
GO