CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadProspectPhoneNumber] @email VARCHAR(200)
AS 
BEGIN 
	SET NOCOUNT ON;
	SELECT 
		slal.PhoneNumber AS phone, 
		kd.PhoneNumber AS portalPhoneNumber,
		kd.MobileNumber AS portalMobileNumber,
		kd.OtherPhoneNumber AS portalOtherPhoneNumber
	FROM [dbo].[SmartLeadAllLeads] slal
	LEFT JOIN KineticPortalLeadContactDetails kd ON slal.Email = kd.Email
	WHERE slal.Email = @email
END
GO

