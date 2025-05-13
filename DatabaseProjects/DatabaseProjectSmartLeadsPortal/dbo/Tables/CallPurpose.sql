CREATE TABLE [dbo].[CallPurpose] (
    [Id]              INT              IDENTITY (1, 1) NOT NULL,
    [GuId]            UNIQUEIDENTIFIER NULL,
    [CallPurposeName] VARCHAR (255)    NULL,
    [IsActive]        BIT              NULL
);
GO

ALTER TABLE [dbo].[CallPurpose]
    ADD CONSTRAINT [PK_CallPurpose] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

