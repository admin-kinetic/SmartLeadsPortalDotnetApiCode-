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
    CONSTRAINT [PK__SmartLea__3214EC07AD432290] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO

CREATE FULLTEXT INDEX ON [dbo].[SmartLeadsExportedContacts]
    ([MessageHistory] LANGUAGE 1033)
    KEY INDEX [PK__SmartLea__3214EC07AD432290]
    ON [SmartLeadsFTCatalog];


GO

