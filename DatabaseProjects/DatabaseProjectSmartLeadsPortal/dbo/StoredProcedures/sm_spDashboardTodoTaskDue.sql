CREATE   PROCEDURE [dbo].[sm_spDashboardTodoTaskDue]
AS
BEGIN
    SET NOCOUNT ON;

	SELECT TOP 6 JSON_VALUE(wh.Request, '$.to_name') AS FullName, sle.Due, sle.OpenCount
	FROM SmartLeadsEmailStatistics sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
	LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
	WHERE (sle.CallStateId IS NULL OR sle.CallStateId <> 1)
	AND (sle.Due IS NOT NULL AND sle.Due <= GETDATE())
	ORDER BY sle.OpenCount DESC
END
GO

