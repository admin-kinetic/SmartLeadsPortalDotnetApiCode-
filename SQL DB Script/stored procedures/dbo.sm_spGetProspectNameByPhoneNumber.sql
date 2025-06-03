CREATE OR ALTER PROCEDURE [dbo].[sm_spGetProspectNameByPhoneNumber] @phone VARCHAR(100)
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT FirstName + ' ' + LastName AS FullName FROM [dbo].[SmartLeadAllLeads] WHERE PhoneNumber=@phone
END
GO