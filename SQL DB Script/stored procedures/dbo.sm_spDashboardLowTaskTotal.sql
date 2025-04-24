CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardLowTaskTotal]
AS
BEGIN
    SET NOCOUNT ON;

	SELECT COUNT(*) AS Total
	FROM SmartLeadsEmailStatistics sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
	LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
	WHERE (sle.OpenCount <= 1 OR sle.OpenCount IS NULL)
END
GO