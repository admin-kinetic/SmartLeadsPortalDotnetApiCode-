CREATE TABLE [dbo].[Users] (
    [EmployeeId]  INT           NOT NULL,
    [Email]       VARCHAR (150) NULL,
    [PhoneNumber] VARCHAR (20)  NULL,
    [FullName]    VARCHAR (200) NULL,
    [IsActive]    BIT           NULL,
    UNIQUE NONCLUSTERED ([EmployeeId] ASC)
);
GO

