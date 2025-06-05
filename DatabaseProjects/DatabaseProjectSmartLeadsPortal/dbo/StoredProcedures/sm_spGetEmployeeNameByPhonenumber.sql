CREATE PROCEDURE [dbo].[sm_spGetEmployeeNameByPhonenumber] @phoneNumber VARCHAR(100)
AS 
BEGIN 
	SET NOCOUNT ON;
	SELECT us.FullName 
	FROM [dbo].[VoipPhoneNumbers] vp
	INNER JOIN [dbo].[Users] us ON vp.EmployeeId = us.EmployeeId
	where vp.PhoneNumber = @phoneNumber
END
GO

