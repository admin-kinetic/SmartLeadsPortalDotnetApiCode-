CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadProspectDetails] @email VARCHAR(200)
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT FirstName, LastName, Email, CompanyName, PhoneNumber FROM [dbo].[SmartLeadAllLeads] WHERE Email=@email
END
GO