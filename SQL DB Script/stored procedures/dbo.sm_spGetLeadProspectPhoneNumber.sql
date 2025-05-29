CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadProspectPhoneNumber] @email VARCHAR(200)
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT PhoneNumber AS phone FROM [dbo].[SmartLeadAllLeads] WHERE Email=@email
END
GO