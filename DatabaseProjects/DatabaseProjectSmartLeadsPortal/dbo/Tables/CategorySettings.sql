CREATE TABLE [dbo].[CategorySettings] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [GuId]         UNIQUEIDENTIFIER NULL,
    [CategoryName] VARCHAR (500)    NULL,
    [OpenCount]    INT              NULL,
    [ClickCount]   INT              NULL
);
GO

ALTER TABLE [dbo].[CategorySettings]
    ADD CONSTRAINT [PK_CategorySettings] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

