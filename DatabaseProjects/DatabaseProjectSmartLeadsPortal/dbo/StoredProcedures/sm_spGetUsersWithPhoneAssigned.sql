CREATE   PROCEDURE [dbo].[sm_spGetUsersWithPhoneAssigned]
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT vp.Id, vp.GuId, vp.PhoneNumber, vp.EmployeeId, us.FullName FROM VoipPhoneNumbers vp
	INNER JOIN Users us ON vp.EmployeeId = us.EmployeeId
END
GO

