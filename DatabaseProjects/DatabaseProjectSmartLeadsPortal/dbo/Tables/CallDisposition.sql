CREATE TABLE [dbo].[CallDisposition] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [GuId]                UNIQUEIDENTIFIER NULL,
    [CallDispositionName] VARCHAR (255)    NULL,
    [IsActive]            BIT              NULL
);
GO

ALTER TABLE [dbo].[CallDisposition]
    ADD CONSTRAINT [PK_CallDisposition] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

