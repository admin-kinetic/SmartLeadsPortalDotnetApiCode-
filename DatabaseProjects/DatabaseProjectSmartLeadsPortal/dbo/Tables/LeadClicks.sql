CREATE TABLE [dbo].[LeadClicks] (
    [Id]                  INT      IDENTITY (1, 1) NOT NULL,
    [LeadId]              INT      NOT NULL,
    [ClickCount]          INT      NULL,
    [LatestClickDateTime] DATETIME NOT NULL,
    [OpenCount]           INT      NULL,
    [LatestOpenDateTime]  DATETIME NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LeadsClick_Leads] FOREIGN KEY ([LeadId]) REFERENCES [dbo].[SmartLeadsExportedContacts] ([Id])
);


GO

