CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardUrgentTaskList]
AS
BEGIN
    SET NOCOUNT ON;

	SELECT TOP 5
		sle.Id,
		sle.GuId,
		JSON_VALUE(wh.Request, '$.sl_email_lead_id') AS LeadId, 
		sle.LeadEmail AS Email, 
		JSON_VALUE(wh.Request, '$.to_name') AS FullName, 
		sle.SequenceNumber,
		CASE 
			WHEN JSON_VALUE(wh.Request, '$.campaign_name') = 'Dondi (US/CA) Bot - Job Ads (manual)' 
				OR JSON_VALUE(wh.Request, '$.campaign_name') = '(US/CA) Bot - Job Ads (full auto)' THEN 
				SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Mountain Standard Time'
			WHEN JSON_VALUE(wh.Request, '$.campaign_name') = 'Dondi (AUS) Bot - Job Ads (manual)' 
				OR JSON_VALUE(wh.Request, '$.campaign_name') = '(AUS) Bot - Job Ads (full auto)' THEN 
				SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Australia Standard Time'
			WHEN JSON_VALUE(wh.Request, '$.campaign_name') = 'Dondi (UK) Bot - Job Ads (manual)' THEN 
				SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
			WHEN JSON_VALUE(wh.Request, '$.campaign_name') = '(NZ) Bot - Job Ads (full auto)' THEN 
				SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'
			WHEN JSON_VALUE(wh.Request, '$.campaign_name') = '(EU) Bot - Job Ads (full auto)' THEN 
				SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
		END AS LocalTime,
		ABS(
			DATEDIFF(
				MINUTE, 
				CAST(CASE 
						WHEN JSON_VALUE(wh.Request, '$.campaign_name') = 'Dondi (US/CA) Bot - Job Ads (manual)' 
							OR JSON_VALUE(wh.Request, '$.campaign_name') = '(US/CA) Bot - Job Ads (full auto)' THEN 
							SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Mountain Standard Time'
						WHEN JSON_VALUE(wh.Request, '$.campaign_name') = 'Dondi (AUS) Bot - Job Ads (manual)' 
							OR JSON_VALUE(wh.Request, '$.campaign_name') = '(AUS) Bot - Job Ads (full auto)' THEN 
							SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Australia Standard Time'
						WHEN JSON_VALUE(wh.Request, '$.campaign_name') = 'Dondi (UK) Bot - Job Ads (manual)' THEN 
							SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
						WHEN JSON_VALUE(wh.Request, '$.campaign_name') = '(NZ) Bot - Job Ads (full auto)' THEN 
							SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'
						WHEN JSON_VALUE(wh.Request, '$.campaign_name') = '(EU) Bot - Job Ads (full auto)' THEN 
							SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
					END AS TIME),
				CAST('09:00:00' AS TIME)
			)
		) AS TimeDifferenceInMinutes,
		JSON_VALUE(wh.Request, '$.campaign_name') AS CampaignName, 
		sle.EmailSubject AS SubjectName, 
		sle.OpenCount, 
		sle.ClickCount,
		cs.Id AS CallStateId,
		cs.StateName AS CallState,
		us.EmployeeId,
		us.FullName AS AssignedTo,
		sle.Notes,
		sle.Due
	FROM SmartLeadsEmailStatistics sle
	INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
	LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
	LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
	WHERE sle.CallStateId IS NULL
	ORDER BY sle.OpenCount DESC
END
GO