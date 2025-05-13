CREATE TABLE [dbo].[MessageHistory] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [StatsId]             UNIQUEIDENTIFIER NULL,
    [Type]                NVARCHAR (50)    NULL,
    [MessageId]           NVARCHAR (1000)  NULL,
    [Time]                DATETIME         NULL,
    [EmailBody]           NVARCHAR (MAX)   NULL,
    [Subject]             NVARCHAR (1000)  NULL,
    [EmailSequenceNumber] INT              NULL,
    [OpenCount]           INT              NULL,
    [ClickCount]          INT              NULL,
    [CrawledResultId]     INT              NULL,
    [LeadEmail]           NVARCHAR (250)   NULL
);
GO

