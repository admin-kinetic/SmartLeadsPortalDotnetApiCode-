CREATE TABLE [dbo].[Calls] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [GuId]              UNIQUEIDENTIFIER NULL,
    [UserCaller]        VARCHAR (200)    NULL,
    [UserPhoneNumber]   VARCHAR (100)    NULL,
    [LeadEmail]         VARCHAR (255)    NULL,
    [ProspectName]      VARCHAR (255)    NULL,
    [ProspectNumber]    VARCHAR (100)    NULL,
    [CalledDate]        DATETIME         NULL,
    [CallStateId]       INT              NULL,
    [Duration]          VARCHAR (100)    NULL,
    [CallPurposeId]     INT              NULL,
    [CallDispositionId] INT              NULL,
    [CallDirectionId]   INT              NULL,
    [Notes]             VARCHAR (MAX)    NULL,
    [CallTagsId]        INT              NULL,
    [AddedBy]           VARCHAR (255)    NULL,
    [AddedDate]         DATETIME         NULL,
    [UniqueCallId]      VARCHAR (255)    NULL,
    [IsDeleted]         BIT              NULL
);
GO

ALTER TABLE [dbo].[Calls]
    ADD CONSTRAINT [PK_Calls] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

