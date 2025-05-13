CREATE TABLE [dbo].[Tags] (
    [Id]       INT              IDENTITY (1, 1) NOT NULL,
    [GuId]     UNIQUEIDENTIFIER NULL,
    [TagName]  VARCHAR (255)    NULL,
    [IsActive] BIT              NULL
);
GO

ALTER TABLE [dbo].[Tags]
    ADD CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

