CREATE TABLE [dbo].[InboundCalls] (
    [UniqueCallId]                  DECIMAL (20)    NOT NULL,
    [CallerId]                      NVARCHAR (100)  NULL,
    [CallerName]                    NVARCHAR (250)  NULL,
    [UserName]                      NVARCHAR (250)  NULL,
    [UserNumber]                    NVARCHAR (100)  NULL,
    [DestNumber]                    NVARCHAR (100)  NULL,
    [CallStartAt]                   DATETIME        NULL,
    [QueueName]                     NVARCHAR (100)  NULL,
    [Status]                        NVARCHAR (100)  NULL,
    [RingGroupName]                 NVARCHAR (100)  NULL,
    [ConnectedAt]                   DATETIME        NULL,
    [CallDuration]                  INT             NULL,
    [ConversationDuration]          INT             NULL,
    [RecordedAt]                    DATETIME        NULL,
    [Emails]                        NVARCHAR (150)  NULL,
    [EmailSubject]                  NVARCHAR (250)  NULL,
    [EmailMessage]                  NVARCHAR (MAX)  NULL,
    [CallRecordingLink]             NVARCHAR (1000) NULL,
    [AudioFile]                     NVARCHAR (MAX)  NULL,
    [LastEventType]                 NVARCHAR (150)  NULL,
    [AzureStorageCallRecordingLink] NVARCHAR (500)  NULL
);
GO

ALTER TABLE [dbo].[InboundCalls]
    ADD CONSTRAINT [PK_InboundCalls_UniqueCallId] PRIMARY KEY CLUSTERED ([UniqueCallId] ASC);
GO

