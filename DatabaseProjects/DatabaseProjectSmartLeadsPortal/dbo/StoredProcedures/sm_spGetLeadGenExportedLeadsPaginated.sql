CREATE PROCEDURE [dbo].[sm_spGetLeadGenExportedLeadsPaginated]
@email NVARCHAR(255) = NULL,
@hasReply BIT = NULL,
@isValid BIT = NULL,
@startDate DATETIME = NULL,
@endDate DATETIME = NULL,
@Page INT = 1,
@PageSize INT = 10
AS
BEGIN
 SET NOCOUNT ON;
	SELECT sec.Id, sal.CreatedAt AS ExportedDate, sec.Email, sec.ContactSource, ses.SequenceNumber, ses.ReplyTime, sec.HasReviewed, ses.SentTime
	FROM [dbo].[SmartLeadsExportedContacts] sec
	LEFT JOIN [dbo].[SmartLeadAllLeads] sal ON sec.Email = sal.Email
	LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sec.Email = ses.LeadEmail
	WHERE sal.BDR ='Steph'
	AND sal.CreatedBy <> 'Bots'
	--AND ses.SequenceNumber = 1
	AND (@email = '' OR sec.Email =@email)
	AND (@hasReply IS NULL 
		OR (@hasReply = 1 AND (ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> ''))
		OR (@hasReply = 0 AND (ses.ReplyTime IS NULL OR LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) = '')))
	AND (@isValid = 0 OR @isValid is null OR sec.HasReviewed = @isValid)
	AND ((@startDate IS NULL OR @endDate IS NULL) 
		OR (sal.CreatedAt >= CONVERT(DATE, @startDate) 
		AND sal.CreatedAt <= CONVERT(DATE, @endDate)
		))
	ORDER BY sal.CreatedAt DESC
	OFFSET (@Page - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO

