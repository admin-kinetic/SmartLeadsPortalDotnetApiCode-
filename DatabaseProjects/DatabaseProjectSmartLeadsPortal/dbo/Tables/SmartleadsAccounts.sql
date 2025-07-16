CREATE TABLE [dbo].[SmartleadsAccounts] (
 [Id] INT IDENTITY (1, 1) NOT NULL,
 [Name] NVARCHAR (250) NOT NULL,
 [Description] NVARCHAR (500) NULL,
 [ApiKey] NVARCHAR (500) NULL,
 [IsDeleted] BIT NULL,
 PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

