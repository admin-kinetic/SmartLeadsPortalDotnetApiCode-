CREATE TABLE [dbo].[CallTaskStatus] (
    [Id]                 INT              IDENTITY (1, 1) NOT NULL,
    [GuId]               UNIQUEIDENTIFIER NULL,
    [CallTaskStatusName] VARCHAR (255)    NULL,
    [IsActive]           BIT              NULL
);
GO

ALTER TABLE [dbo].[CallTaskStatus]
    ADD CONSTRAINT [PK_CallTaskStatus] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

