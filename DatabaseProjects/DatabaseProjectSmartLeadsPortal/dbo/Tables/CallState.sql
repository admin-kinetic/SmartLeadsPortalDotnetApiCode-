CREATE TABLE [dbo].[CallState] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [GuId]      UNIQUEIDENTIFIER NULL,
    [StateName] VARCHAR (255)    NULL,
    [IsActive]  BIT              NULL
);
GO

ALTER TABLE [dbo].[CallState]
    ADD CONSTRAINT [PK_CallState] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

