CREATE TABLE [dbo].[Roles] (
    [ID]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250)  NOT NULL,
    [Description] NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

