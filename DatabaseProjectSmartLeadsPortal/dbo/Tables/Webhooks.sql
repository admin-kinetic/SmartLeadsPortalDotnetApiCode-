CREATE TABLE [dbo].[Webhooks] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Request]   NVARCHAR (MAX) NULL,
    [CreatedAt] DATETIME       NULL,
    CONSTRAINT [PK_Webhooks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CHK_Request_IsJson] CHECK (isjson([Request])=(1))
);


GO

