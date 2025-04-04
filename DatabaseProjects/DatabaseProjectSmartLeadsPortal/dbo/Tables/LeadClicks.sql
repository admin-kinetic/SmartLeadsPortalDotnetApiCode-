CREATE TABLE [dbo].[LeadClicks] (
    [Id]                  INT      IDENTITY (1, 1) NOT NULL,
    [LeadId]              INT      NOT NULL,
    [ClickCount]          INT      NOT NULL,
    [LatestClickDateTime] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LeadsClick_Leads] FOREIGN KEY ([LeadId]) REFERENCES [dbo].[SmartLeadsExportedContacts] ([Id])
);


GO

