CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadsExportCsv]
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
	SELECT sal.Email, 
	sal.PhoneNumber, 
	sal.FirstName,  
	sal.LastName, 
	sal.CompanyName, 
	sal.[Location] AS Country,
	CASE 
		WHEN CHARINDEX('re. your ', ses.EmailSubject) > 0 
			 AND CHARINDEX(' ad on', ses.EmailSubject) > CHARINDEX('re. your ', ses.EmailSubject)
		THEN TRIM(
			SUBSTRING(
				ses.EmailSubject,
				CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '),
				CHARINDEX(' ad on', ses.EmailSubject) - (CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '))
			)
		)
		ELSE NULL
	END AS RoleAdvertised,
	sec.ContactSource AS [Source],
	@startDate AS FromDateExported,
    @endDate AS ToDateExported,
	CASE 
        WHEN ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> '' THEN 'Yes'
        ELSE 'No'
    END AS HasReply,
	sal.CreatedAt AS ExportedDate, 
	sal.SmartleadCategory AS Category,
	sal.BDR as Bdr,
	sal.CreatedBy AS AssignedTo,
	slc.[Name] AS EmailCampaign,
	sal.CreatedBy AS LeadGen,
	sal.QABy AS QadBy,
	ses.OpenCount,
	ses.ClickCount
	FROM [dbo].[SmartLeadAllLeads] sal
	INNER JOIN [dbo].[SmartLeadCampaigns] slc ON sal.CampaignId = slc.Id
	LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
	LEFT JOIN [dbo].[SmartLeadsExportedContacts] sec ON sal.Email = sec.Email
	WHERE (@email = '' OR sal.Email =@email)
	AND (@hasReply IS NULL 
		OR (@hasReply = 1 AND (ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> ''))
		OR (@hasReply = 0 AND (ses.ReplyTime IS NULL OR LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) = '')))
	AND ((@startDate IS NULL OR @endDate IS NULL) 
		OR (sal.CreatedAt >= CONVERT(DATE, @startDate) 
		AND sal.CreatedAt <= CONVERT(DATE, @endDate)
		))
	ORDER BY sal.CreatedAt DESC
END
GO