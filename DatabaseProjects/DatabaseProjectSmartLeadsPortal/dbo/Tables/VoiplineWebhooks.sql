CREATE TABLE [dbo].[VoiplineWebhooks] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Request]      NVARCHAR (MAX) NULL,
    [CreatedAt]    DATETIME       NULL,
    [Type]         VARCHAR (50)   NULL,
    [UniqueCallId] AS             (json_value([Request],'$.unique_call_id')) PERSISTED
);
GO

