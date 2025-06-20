CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadsEmailedPaginatedCount]
	@email NVARCHAR(255) = NULL,
	@hasReply BIT = NULL,
	@startDate DATETIME = NULL,
	@endDate DATETIME = NULL,
	@Bdr NVARCHAR(255) = NULL,
	@LeadGen NVARCHAR(255) = NULL,
	@QaBy NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		COUNT(*) AS TotalCount
	FROM [dbo].[SmartLeadAllLeads] sal
		LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
	WHERE ((@email IS NULL OR @email = '') OR sal.Email = @email)
		AND (((@startDate IS NULL OR @endDate IS NULL) OR @hasReply IS NOT NULL) OR (sal.CreatedAt IS NOT NULL AND CONVERT(DATE, sal.CreatedAt) >= @startDate AND CONVERT(DATE, sal.CreatedAt) <= @endDate))
		AND (((@startDate IS NULL OR @endDate IS NULL) OR @hasReply = 0) OR (ses.ReplyTime IS NOT NULL AND CONVERT(DATE, ses.ReplyTime) >= @startDate AND CONVERT(DATE, ses.ReplyTime) <= @endDate))
		AND (@hasReply = 1 OR ses.ReplyTime IS NULL)
		AND (@hasReply = 0 OR ses.ReplyTime IS NOT NULL)
		AND (@Bdr IS NULL OR sal.Bdr = @Bdr)
		AND (@LeadGen IS NULL OR sal.CreatedBy = @LeadGen)
		AND (@QaBy IS NULL OR sal.QaBy = @QaBy)
END
GO

