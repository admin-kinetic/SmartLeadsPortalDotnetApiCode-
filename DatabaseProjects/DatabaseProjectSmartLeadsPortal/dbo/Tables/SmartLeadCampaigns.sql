CREATE TABLE [dbo].[SmartLeadCampaigns] (
    [Id]     INT            NOT NULL,
    [Name]   NVARCHAR (250) NULL,
    [Status] NVARCHAR (50)  NULL,
    [Bdr]    NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

