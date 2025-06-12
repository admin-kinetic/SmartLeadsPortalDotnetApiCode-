CREATE OR ALTER PROCEDURE [dbo].[sm_spDashboardAutomatedLeadsCampaignExportedCharts]
@startDate DATETIME, 
@endDate DATETIME,
@bdr VARCHAR(500),
@createdby VARCHAR(500),
@qaby VARCHAR(500),
@campaignid INT
AS  
BEGIN   
    SET NOCOUNT ON;

	DECLARE @sql NVARCHAR(MAX) = '';
	DECLARE @baseFilter NVARCHAR(MAX);

	SET @baseFilter = '
		FROM [dbo].[SmartLeadAllLeads] sla
		INNER JOIN [dbo].[SmartLeadsEmailStatistics] sle ON sla.Email = sle.LeadEmail
		WHERE (
			@campaignId IS NULL
			OR (@campaignId = 1 AND sla.CreatedBy <> ''Bots'' AND sla.BDR <> ''Steph'')
			OR (@campaignId = 2 AND sla.CreatedBy = ''Bots'' AND sla.BDR = ''Steph'')
			OR (@campaignId = 3 AND sla.CreatedBy <> ''Bots'' AND sla.BDR = ''Steph'')
		)
		AND (@bdr = '''' OR sla.BDR = @bdr)
		AND (@createdby = '''' OR sla.CreatedBy = @createdby)
		AND (@qaby = '''' OR sla.QABy = @qaby)
		AND CONVERT(DATE, sla.CreatedAt) BETWEEN @startDate AND @endDate
	';

	IF @bdr <> ''
	BEGIN
		SET @sql = '
		WITH DailyBreakdown AS (
			SELECT 
				CONVERT(VARCHAR(10), CAST(sla.CreatedAt AS DATE), 23) AS ExportedDate,
				ISNULL(NULLIF(sla.BDR, ''''), ''Others'') AS LeadUsers,
				COUNT(sla.Id) AS TotalCount
			' + @baseFilter + '
			AND sla.BDR = @bdr
			GROUP BY CAST(sla.CreatedAt AS DATE), sla.BDR
		)
		SELECT ExportedDate, LeadUsers, TotalCount
		FROM DailyBreakdown
		ORDER BY ExportedDate DESC, CASE WHEN LeadUsers = ''Others'' THEN 1 ELSE 0 END, LeadUsers
		';
	END
	ELSE IF @createdby <> ''
	BEGIN
		SET @sql = '
		WITH DailyBreakdown AS (
			SELECT 
				CONVERT(VARCHAR(10), CAST(sla.CreatedAt AS DATE), 23) AS ExportedDate,
				ISNULL(NULLIF(sla.CreatedBy, ''''), ''Others'') AS LeadUsers,
				COUNT(sla.Id) AS TotalCount
			' + @baseFilter + '
			AND sla.CreatedBy = @createdby
			GROUP BY CAST(sla.CreatedAt AS DATE), sla.CreatedBy
		)
		SELECT ExportedDate, LeadUsers, TotalCount
		FROM DailyBreakdown
		ORDER BY ExportedDate DESC, CASE WHEN LeadUsers = ''Others'' THEN 1 ELSE 0 END, LeadUsers
		';
	END
	ELSE IF @qaby <> ''
	BEGIN
		SET @sql = '
		WITH DailyBreakdown AS (
			SELECT 
				CONVERT(VARCHAR(10), CAST(sla.CreatedAt AS DATE), 23) AS ExportedDate,
				ISNULL(NULLIF(sla.QABy, ''''), ''Others'') AS LeadUsers,
				COUNT(sla.Id) AS TotalCount
			' + @baseFilter + '
			AND sla.QABy = @qaby
			GROUP BY CAST(sla.CreatedAt AS DATE), sla.QABy
		)
		SELECT ExportedDate, LeadUsers, TotalCount
		FROM DailyBreakdown
		ORDER BY ExportedDate DESC, CASE WHEN LeadUsers = ''Others'' THEN 1 ELSE 0 END, LeadUsers
		';
	END
	ELSE
	BEGIN
		SET @sql = '
		SELECT ExportedDate, LeadUsers, TotalCount
		FROM (
			SELECT 
				CONVERT(VARCHAR(10), CAST(sla.CreatedAt AS DATE), 23) AS ExportedDate,
				ISNULL(NULLIF(sla.BDR, ''''), ''Others'') AS LeadUsers,
				COUNT(sla.Id) AS TotalCount
			' + @baseFilter + '
			GROUP BY CAST(sla.CreatedAt AS DATE), sla.BDR
        
			UNION ALL
        
			SELECT 
				CONVERT(VARCHAR(10), CAST(sla.CreatedAt AS DATE), 23) AS ExportedDate,
				ISNULL(NULLIF(sla.CreatedBy, ''''), ''Others'') AS LeadUsers,
				COUNT(sla.Id) AS TotalCount
			' + @baseFilter + '
			GROUP BY CAST(sla.CreatedAt AS DATE), sla.CreatedBy
        
			UNION ALL
        
			SELECT 
				CONVERT(VARCHAR(10), CAST(sla.CreatedAt AS DATE), 23) AS ExportedDate,
				ISNULL(NULLIF(sla.QABy, ''''), ''Others'') AS LeadUsers,
				COUNT(sla.Id) AS TotalCount
			' + @baseFilter + '
			GROUP BY CAST(sla.CreatedAt AS DATE), sla.QABy
		) AS Combined
		ORDER BY ExportedDate DESC, CASE WHEN LeadUsers = ''Others'' THEN 1 ELSE 0 END, LeadUsers
		';
	END

	EXEC sp_executesql 
		@sql,
		N'@startDate DATETIME, @endDate DATETIME, @bdr VARCHAR(500), @createdby VARCHAR(500), @qaby VARCHAR(500), @campaignid INT',
		@startDate, @endDate, @bdr, @createdby, @qaby, @campaignid;
END
GO