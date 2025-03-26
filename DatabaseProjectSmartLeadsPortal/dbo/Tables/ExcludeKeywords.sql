CREATE TABLE [dbo].[ExcludeKeywords] (
    [Id]              INT              IDENTITY (1, 1) NOT NULL,
    [GuId]            UNIQUEIDENTIFIER NULL,
    [ExcludedKeyword] VARCHAR (MAX)    NULL,
    [IsActive]        BIT              NULL,
    CONSTRAINT [PK_ExcludeKeywords] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO

