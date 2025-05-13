CREATE TABLE [dbo].[CallDirection] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [GuId]              UNIQUEIDENTIFIER NULL,
    [CallDirectionName] VARCHAR (100)    NULL
);
GO

ALTER TABLE [dbo].[CallDirection]
    ADD CONSTRAINT [PK_CallDirection] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

