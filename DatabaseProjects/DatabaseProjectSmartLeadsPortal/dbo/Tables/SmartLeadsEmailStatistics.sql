CREATE TABLE [dbo].[SmartLeadsEmailStatistics] (
 [Id] INT IDENTITY (1, 1) NOT NULL,
 [GuId] UNIQUEIDENTIFIER NULL,
 [LeadId] BIGINT NULL,
 [LeadEmail] VARCHAR (255) NULL,
 [SequenceNumber] INT NULL,
 [EmailSubject] VARCHAR (500) NULL,
 [EmailMessage] VARCHAR (MAX) NULL,
 [SentTime] DATETIME NULL,
 [OpenTime] DATETIME NULL,
 [ClickTime] DATETIME NULL,
 [ReplyTime] DATETIME NULL,
 [OpenCount] INT NULL,
 [ClickCount] INT NULL,
 [CallStateId] INT NULL,
 [AssignedTo] INT NULL,
 [Notes] VARCHAR (MAX) NULL,
 [Due] DATETIME NULL,
 [IsDeleted] BIT NULL,
 [LeadName] NVARCHAR (150) NULL,
 CONSTRAINT [PK_SmartLeadsEmailStatistics] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [idx_leademail_sequencenumber]
 ON [dbo].[SmartLeadsEmailStatistics]([LeadEmail] ASC, [SequenceNumber] ASC);
GO

