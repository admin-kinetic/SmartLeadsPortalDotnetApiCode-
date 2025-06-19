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
	WHERE (@email = '' OR sal.Email =@email)
		AND (@hasReply IS NULL 
			OR (@hasReply = 1 AND (ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> ''))
			OR (@hasReply = 0 AND (ses.ReplyTime IS NULL OR LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) = '')))
		AND ((@startDate IS NULL OR @endDate IS NULL) 
			OR (ses.ReplyTime >= CONVERT(DATE, @startDate) 
			AND ses.ReplyTime <= CONVERT(DATE, @endDate)
			))
		AND (@Bdr IS NULL OR sal.Bdr = @Bdr)
		AND (@LeadGen IS NULL OR sal.CreatedBy = @LeadGen)
		AND (@QaBy IS NULL OR sal.QaBy = @QaBy)
	ORDER BY ses.SentTime DESC
	OFFSET (@Page - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO

