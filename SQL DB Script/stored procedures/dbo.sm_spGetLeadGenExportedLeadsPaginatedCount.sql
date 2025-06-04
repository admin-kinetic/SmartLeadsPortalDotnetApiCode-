CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadGenExportedLeadsPaginatedCount]
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
	AND (@hasReply = 0 OR @hasReply is null OR sec.HasReply = @hasReply)
	AND (@isValid = 0 OR @isValid is null OR sec.HasReviewed = @isValid)
	AND ((@startDate IS NULL OR @endDate IS NULL) 
		OR (sec.ExportedDate >= CONVERT(DATE, @startDate) 
		AND sec.ExportedDate <= CONVERT(DATE, @endDate)
		))
END
GO