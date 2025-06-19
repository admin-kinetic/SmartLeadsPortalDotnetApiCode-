CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadsEmailedPaginated]
	@email NVARCHAR(255) = NULL,
	@hasReply BIT = NULL,
	@startDate DATETIME = NULL,
	@endDate DATETIME = NULL,
	@Page INT = 1,
	@PageSize INT = 10,
	@Bdr NVARCHAR(255) = NULL,
	@LeadGen NVARCHAR(255) = NULL,
	@QaBy NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		sal.FirstName + ' ' + sal.LastName AS FullName,
		sal.CompanyName,
		sal.Email,
		sal.PhoneNumber,
		sal.CreatedAt AS ExportedDate,
		ses.SequenceNumber,
		ses.ReplyTime,
		ses.SentTime, sal.[Location] AS Country
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
	ORDER BY ses.SentTime DESC
	OFFSET (@Page - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO

