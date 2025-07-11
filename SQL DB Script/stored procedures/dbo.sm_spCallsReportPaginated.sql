CREATE OR ALTER PROCEDURE [dbo].[sm_spCallsReportPaginated]
@Type INT = NULL,
@StartDate DATE = NULL,
@EndDate DATE = NULL,
@Search NVARCHAR(500) = '',
@Bdr VARCHAR(100),
@PageNumber INT = 1,
@PageSize INT = 10
AS
BEGIN
	WITH CombinedCalls AS (
		SELECT UniqueCallId,
			'Outbound' AS CallType,
			CallerId,
			UserName,
			DestNumber,
			CallStartAt,
			CallDuration,
			ConversationDuration,
			AzureStorageCallRecordingLink
		FROM OutboundCalls
		WHERE (@Type IS NULL OR @Type = 1)
		  AND (
			  @startDate IS NULL OR @endDate IS NULL
				OR (CONVERT(DATE, CallStartAt) >= CONVERT(DATE, @startDate) 
				AND CONVERT(DATE, CallStartAt) <= CONVERT(DATE, @endDate))
		  )
		  AND (
			  @Search IS NULL OR @Search = ''
			  OR CallerId LIKE '%' + @Search + '%'
			  OR UserName LIKE '%' + @Search + '%'
			  OR DestNumber LIKE '%' + @Search + '%'
			  OR EmailSubject LIKE '%' + @Search + '%'
		  )
		  AND (
			  @Bdr IS NULL OR @Bdr = ''
			  OR UserName = @Bdr
		  )

		UNION ALL

		SELECT UniqueCallId,
			'Inbound' AS CallType,
			CallerId,
			UserName,
			DestNumber,
			CallStartAt,
			CallDuration,
			ConversationDuration,
			AzureStorageCallRecordingLink
		FROM InboundCalls
		WHERE (@Type IS NULL OR @Type = 2)
		  AND (
			  @StartDate IS NULL OR @EndDate IS NULL
			  OR (CONVERT(DATE, CallStartAt) >= CONVERT(DATE, @startDate) 
				AND CONVERT(DATE, CallStartAt) <= CONVERT(DATE, @endDate))
		  )
		  AND (
			  @Search IS NULL OR @Search = ''
			  OR CallerId LIKE '%' + @Search + '%'
			  OR UserName LIKE '%' + @Search + '%'
			  OR DestNumber LIKE '%' + @Search + '%'
			  OR EmailSubject LIKE '%' + @Search + '%'
			  OR CallerName LIKE '%' + @Search + '%'
		  )
		   AND (
			  @Bdr IS NULL OR @Bdr = ''
			  OR UserName = @Bdr
		  )
	)

	SELECT *
	FROM CombinedCalls
	ORDER BY CallStartAt DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO