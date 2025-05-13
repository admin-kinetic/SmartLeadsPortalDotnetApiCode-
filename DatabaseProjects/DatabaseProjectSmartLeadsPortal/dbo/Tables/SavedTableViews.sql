CREATE TABLE [dbo].[SavedTableViews] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [Guid]        UNIQUEIDENTIFIER NULL,
    [TableName]   NVARCHAR (250)   NULL,
    [ViewName]    NVARCHAR (250)   NULL,
    [ViewFilters] NVARCHAR (MAX)   NULL,
    [OwnerId]     INT              NULL,
    [Sharing]     INT              NULL,
    [CreatedAt]   DATETIME         NULL,
    [CreatedBy]   INT              NULL,
    [ModifiedAt]  DATETIME         NULL,
    [ModifiedBy]  INT              NULL,
    [IsDefault]   BIT              NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

