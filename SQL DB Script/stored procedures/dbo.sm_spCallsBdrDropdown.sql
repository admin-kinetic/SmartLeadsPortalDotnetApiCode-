CREATE OR ALTER PROCEDURE [dbo].[sm_spCallsBdrDropdown]
AS
BEGIN
	SELECT DISTINCT UserName
	FROM (
		SELECT UserName FROM OutboundCalls WHERE UserName IS NOT NULL
		UNION
		SELECT UserName FROM InboundCalls WHERE UserName IS NOT NULL
	) AS CombinedUserNames;
END
GO