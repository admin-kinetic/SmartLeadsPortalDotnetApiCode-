CREATE OR ALTER PROCEDURE [dbo].[sm_spGetUsersPhoneById] @id INT
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT vp.Id, vp.GuId, vp.PhoneNumber, vp.EmployeeId, us.FullName FROM VoipPhoneNumbers vp
	INNER JOIN Users us ON vp.EmployeeId = us.EmployeeId
	WHERE vp.EmployeeId = @id
END
GO