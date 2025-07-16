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
	        oc.CallerId
	    FROM OutboundCalls oc
		LEFT JOIN SmartLeadAllLeads sl 
		  ON REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(oc.DestNumber, ' ', ''), '+', ''), '-', ''), '(', ''), ')', ''), '.', '') 
			 = 
			 REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(sl.PhoneNumber, ' ', ''), '+', ''), '-', ''), '(', ''), ')', ''), '.', '')
	    WHERE (@Type IS NULL OR @Type = 1)
	      AND (
	          @StartDate IS NULL OR @EndDate IS NULL
	          OR (CONVERT(DATE, CallStartAt) >= CONVERT(DATE, @startDate) 
				AND CONVERT(DATE, CallStartAt) <= CONVERT(DATE, @endDate))
	      )
	      AND (
	          @Search IS NULL OR @Search = ''
	          OR oc.CallerId LIKE '%' + @Search + '%'
	          OR oc.UserName LIKE '%' + @Search + '%'
	          OR oc.DestNumber LIKE '%' + @Search + '%'
			  OR oc.EmailSubject LIKE '%' + @Search + '%'
			  OR sl.FirstName + ' ' + sl.LastName LIKE '%' + @Search + '%'
	      )
		   AND (
			  @Bdr IS NULL OR @Bdr = ''
			  OR oc.UserName = @Bdr
		  )

	    UNION ALL

	    SELECT 
	        ic.CallerId
	    FROM InboundCalls ic
		LEFT JOIN SmartLeadAllLeads sl 
		  ON REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(ic.DestNumber, ' ', ''), '+', ''), '-', ''), '(', ''), ')', ''), '.', '') 
			 = 
			 REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(sl.PhoneNumber, ' ', ''), '+', ''), '-', ''), '(', ''), ')', ''), '.', '')
	    WHERE (@Type IS NULL OR @Type = 2)
	      AND (
	          @StartDate IS NULL OR @EndDate IS NULL
	          OR (CONVERT(DATE, ic.CallStartAt) >= CONVERT(DATE, @startDate) 
				AND CONVERT(DATE, ic.CallStartAt) <= CONVERT(DATE, @endDate))
	      )
	      AND (
	          @Search IS NULL OR @Search = ''
	          OR ic.CallerId LIKE '%' + @Search + '%'
	          OR ic.UserName LIKE '%' + @Search + '%'
	          OR ic.DestNumber LIKE '%' + @Search + '%'
			  OR ic.EmailSubject LIKE '%' + @Search + '%'
			  OR ic.CallerName LIKE '%' + @Search + '%'
			  OR sl.FirstName + ' ' + sl.LastName LIKE '%' + @Search + '%'
	      )
		   AND (
			  @Bdr IS NULL OR @Bdr = ''
			  OR ic.UserName = @Bdr
		  )
	)

	SELECT COUNT(*) AS TotalCount
	FROM CombinedCalls;
END
GO