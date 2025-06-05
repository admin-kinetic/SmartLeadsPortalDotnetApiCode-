CREATE PROCEDURE [dbo].[sm_spGetSmartLeadsEmailStatisticsSent]
 @startDate DATE,
 @endDate DATE,
 @campaignIds NVARCHAR(MAX)
AS
BEGIN
 SET NOCOUNT ON;
 SELECT COUNT(*) AS TotalCount
 FROM [dbo].[SmartLeadsEmailStatistics] sle
 INNER JOIN [dbo].[SmartLeadAllLeads] sla ON sle.LeadEmail = sla.Email
 WHERE (CONVERT(DATE, sle.SentTime)>= @startDate AND CONVERT(Date, sle.SentTime) <= @endDate)
 AND sla.CampaignId IN (
 SELECT CAST([value] AS INT)
 FROM OPENJSON(@campaignIds)
 );
END
GO

