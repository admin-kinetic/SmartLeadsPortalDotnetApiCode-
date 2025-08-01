CREATE TABLE [dbo].[SmartLeadAllLeads] (
 [Id] DECIMAL (38) IDENTITY (1, 1) NOT NULL,
 [LeadId] DECIMAL (38) NULL,
 [CampaignId] INT NOT NULL,
 [FirstName] NVARCHAR (100) NULL,
 [LastName] NVARCHAR (100) NULL,
 [CreatedAt] DATETIME NOT NULL,
 [PhoneNumber] NVARCHAR (100) NULL,
 [CompanyName] NVARCHAR (100) NULL,
 [LeadStatus] NVARCHAR (100) NOT NULL,
 [Email] NVARCHAR (100) NULL,
 [BDR] NVARCHAR (250) NULL,
 [CreatedBy] NVARCHAR (250) NULL,
 [QABy] NVARCHAR (250) NULL,
 [Location] NVARCHAR (150) NULL,
 CONSTRAINT [PK__SmartLea__3214EC07F15C94D6] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

