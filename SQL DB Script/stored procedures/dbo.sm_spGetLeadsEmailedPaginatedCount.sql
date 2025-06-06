CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadsEmailedPaginatedCount]
@email NVARCHAR(255) = NULL,
@hasReply BIT = NULL,
@startDate DATETIME = NULL,
@endDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
	SELECT COUNT(*) AS TotalCount
	FROM [dbo].[SmartLeadAllLeads] sal
	LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
	WHERE (@email = '' OR sal.Email =@email)
	AND (@hasReply IS NULL 
		OR (@hasReply = 1 AND (ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> ''))
		OR (@hasReply = 0 AND (ses.ReplyTime IS NULL OR LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) = '')))
	AND ((@startDate IS NULL OR @endDate IS NULL) 
		OR (ses.ReplyTime >= CONVERT(DATE, @startDate) 
		AND ses.ReplyTime <= CONVERT(DATE, @endDate)
		))
END
GO