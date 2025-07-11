CREATE OR ALTER PROCEDURE [dbo].[sm_spCallsReportPaginatedCount]
@Type INT = NULL,
@StartDate DATE = NULL,
@EndDate DATE = NULL,
@Search NVARCHAR(500) = '',
@Bdr VARCHAR(100)
AS
BEGIN
	WITH CombinedCalls AS (
	    SELECT 
	        CallerId
	    FROM OutboundCalls
	    WHERE (@Type IS NULL OR @Type = 1)
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
	      )
		   AND (
			  @Bdr IS NULL OR @Bdr = ''
			  OR UserName = @Bdr
		  )

	    UNION ALL

	    SELECT 
	        CallerId
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

	SELECT COUNT(*) AS TotalCount
	FROM CombinedCalls;
END
GO