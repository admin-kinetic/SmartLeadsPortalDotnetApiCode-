CREATE TABLE [dbo].[SmartLeadAllLeads] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [LeadId]      INT            NOT NULL,
    [CampaignId]  INT            NOT NULL,
    [FirstName]   NVARCHAR (100) NOT NULL,
    [LastName]    NVARCHAR (100) NOT NULL,
    [CreatedAt]   DATETIME       NOT NULL,
    [PhoneNumber] NVARCHAR (100) NULL,
    [CompanyName] NVARCHAR (100) NULL,
    [LeadStatus]  NVARCHAR (100) NOT NULL,
    [Email]       NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

