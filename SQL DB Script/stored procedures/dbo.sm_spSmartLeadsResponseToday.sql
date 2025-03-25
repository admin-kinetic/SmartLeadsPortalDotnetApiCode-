CREATE OR ALTER PROCEDURE [dbo].[sm_spSmartLeadsResponseToday]
AS  
BEGIN   
	SET NOCOUNT ON;

    DECLARE @StartUtc DATETIME, @EndUtc DATETIME;

    -- Get Singapore Time (UTC+8)
    DECLARE @SingaporeTime DATETIME = GETUTCDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'Singapore Standard Time';
    
    -- Calculate Start and End of Today in Singapore Time
    SET @StartUtc = DATEADD(DAY, DATEDIFF(DAY, 0, @SingaporeTime), 0);
    SET @EndUtc = DATEADD(DAY, 1, @StartUtc);

    -- Convert to UTC
    SET @StartUtc = @StartUtc AT TIME ZONE 'Singapore Standard Time' AT TIME ZONE 'UTC';
    SET @EndUtc = @EndUtc AT TIME ZONE 'Singapore Standard Time' AT TIME ZONE 'UTC';

    -- Fetch count
    SELECT COUNT(Id) AS TotalResponseToday
    FROM SmartLeadsExportedContacts
    WHERE RepliedAt BETWEEN @StartUtc AND @EndUtc;
END
GO