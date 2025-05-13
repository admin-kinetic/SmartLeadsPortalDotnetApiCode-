CREATE TABLE [dbo].[Permissions] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250)  NOT NULL,
    [Description] NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

