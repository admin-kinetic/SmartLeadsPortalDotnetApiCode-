CREATE TABLE [dbo].[OutboundCalls] (
    [UniqueCallId]                  DECIMAL (20)    NOT NULL,
    [CallerId]                      NVARCHAR (100)  NULL,
    [UserName]                      NVARCHAR (250)  NULL,
    [UserNumber]                    NVARCHAR (100)  NULL,
    [DestNumber]                    NVARCHAR (100)  NULL,
    [CallStartAt]                   DATETIME        NULL,
    [ConnectedAt]                   DATETIME        NULL,
    [CallDuration]                  INT             NULL,
    [ConversationDuration]          INT             NULL,
    [RecordedAt]                    DATETIME        NULL,
    [Emails]                        NVARCHAR (150)  NULL,
    [EmailSubject]                  NVARCHAR (250)  NULL,
    [EmailMessage]                  NVARCHAR (MAX)  NULL,
    [CallRecordingLink]             NVARCHAR (1000) NULL,
    [LastEventType]                 NVARCHAR (150)  NULL,
    [AzureStorageCallRecordingLink] NVARCHAR (500)  NULL
);
GO

ALTER TABLE [dbo].[OutboundCalls]
    ADD CONSTRAINT [PK_OutboundCalls_UniqueCallId] PRIMARY KEY CLUSTERED ([UniqueCallId] ASC);
GO

