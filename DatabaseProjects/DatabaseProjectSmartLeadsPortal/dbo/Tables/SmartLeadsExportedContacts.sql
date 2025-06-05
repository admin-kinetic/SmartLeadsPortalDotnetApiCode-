CREATE TABLE [dbo].[SmartLeadsExportedContacts] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [ExportedDate]          DATETIME       NULL,
    [Email]                 VARCHAR (255)  NULL,
    [ContactSource]         VARCHAR (255)  NULL,
    [Rate]                  INT            NULL,
    [HasReply]              BIT            NULL,
    [ModifiedAt]            DATETIME       NULL,
    [Category]              VARCHAR (255)  NULL,
    [MessageHistory]        NVARCHAR (MAX) NULL,
    [LatestReplyPlainText]  VARCHAR (MAX)  NULL,
    [HasReviewed]           BIT            NULL,
    [SmartleadId]           INT            NULL,
    [ReplyDate]             DATETIME       NULL,
    [RepliedAt]             DATETIME       NULL,
    [FailedDelivery]        BIT            NULL,
    [RemovedFromSmartleads] BIT            NULL,
    [SmartLeadsStatus]      VARCHAR (50)   NULL,
    [SmartLeadsCategory]    VARCHAR (100)  NULL,
    [Bdr]                   NVARCHAR (150) NULL,
    [AssignedTo]            NVARCHAR (150) NULL,
    [AssignedToQA]          NVARCHAR (150) NULL,
    [SmartleadsCampaign]    NVARCHAR (250) NULL,
    CONSTRAINT [PK__SmartLea__3214EC07AD432290] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO

CREATE FULLTEXT INDEX ON [dbo].[SmartLeadsExportedContacts]
    ([MessageHistory] LANGUAGE 1033)
    KEY INDEX [PK__SmartLea__3214EC07AD432290]
    ON [SmartLeadsFTCatalog];


GO


CREATE NONCLUSTERED INDEX [IX_SmartleadsExportedContacts_Exported]
    ON [dbo].[SmartLeadsExportedContacts]([ExportedDate] ASC);
GO


CREATE NONCLUSTERED INDEX [IX_SmartleadsExportedContacts_AssignedTo]
    ON [dbo].[SmartLeadsExportedContacts]([AssignedTo] ASC);
GO


CREATE NONCLUSTERED INDEX [IX_SmartleadsExportedContacts_AssignedToQA]
    ON [dbo].[SmartLeadsExportedContacts]([AssignedToQA] ASC);
GO


CREATE NONCLUSTERED INDEX [IX_SmartleadsExportedContacts_Bdr]
    ON [dbo].[SmartLeadsExportedContacts]([Bdr] ASC);
GO


CREATE NONCLUSTERED INDEX [IX_SmartleadsExportedContacts_Email]
    ON [dbo].[SmartLeadsExportedContacts]([Email] ASC);
GO

