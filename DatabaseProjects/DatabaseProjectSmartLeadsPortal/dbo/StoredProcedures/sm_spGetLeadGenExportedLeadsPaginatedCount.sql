CREATE PROCEDURE [dbo].[sm_spGetLeadGenExportedLeadsPaginatedCount]
@email NVARCHAR(255) = NULL,
@hasReply BIT = NULL,
@isValid BIT = NULL,
@startDate DATETIME = NULL,
@endDate DATETIME = NULL
AS
BEGIN
 SET NOCOUNT ON;
	SELECT COUNT(*) AS TotalCount
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
		OR (sec.ExportedDate >= CONVERT(DATE, @startDate) 
		AND sec.ExportedDate <= CONVERT(DATE, @endDate)
		))
END
GO

