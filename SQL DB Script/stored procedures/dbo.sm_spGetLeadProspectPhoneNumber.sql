CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadProspectPhoneNumber] @email VARCHAR(200)
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT phone FROM Robotcrawledcontacts WHERE email=@email
END
GO