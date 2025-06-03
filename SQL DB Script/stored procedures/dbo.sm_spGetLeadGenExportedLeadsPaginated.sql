CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadGenExportedLeadsPaginated]
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
	SELECT sec.Id, sec.ExportedDate, sec.Email, sec.ContactSource, ses.SequenceNumber, sec.HasReply, sec.HasReviewed, sec.MessageHistory, ses.SentTime 
	FROM [dbo].[SmartLeadsExportedContacts] sec
	LEFT JOIN [dbo].[SmartLeadAllLeads] sal ON sec.Email = sal.Email
	LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sec.Email = ses.LeadEmail
	WHERE sal.BDR ='Steph'
	AND sal.CreatedBy <> 'Bots'
	--AND ses.SequenceNumber = 1
	AND (@email = '' OR sec.Email =@email)
	AND (@hasReply = 0 OR @hasReply is null OR sec.HasReply = @hasReply)
	AND (@isValid = 0 OR @isValid is null OR sec.HasReviewed = @isValid)
	AND ((@startDate IS NULL OR @endDate IS NULL) 
		OR (sec.ExportedDate >= CONVERT(DATE, @startDate) 
		AND sec.ExportedDate <= CONVERT(DATE, @endDate)
		))
	ORDER BY sec.ExportedDate DESC
	OFFSET (@Page - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO