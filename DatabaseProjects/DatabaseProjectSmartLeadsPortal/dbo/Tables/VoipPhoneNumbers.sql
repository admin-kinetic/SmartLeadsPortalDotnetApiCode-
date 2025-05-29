CREATE TABLE [dbo].[VoipPhoneNumbers] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [GuId]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PhoneNumber] NVARCHAR (20)    NULL,
    [EmployeeId]  INT              NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

ALTER TABLE [dbo].[VoipPhoneNumbers]
    ADD CONSTRAINT [UQ_PhoneNumber] UNIQUE NONCLUSTERED ([PhoneNumber] ASC);
GO

