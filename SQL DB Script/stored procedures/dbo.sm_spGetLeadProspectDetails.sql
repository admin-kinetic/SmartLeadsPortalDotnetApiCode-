CREATE OR ALTER PROCEDURE [dbo].[sm_spGetLeadProspectDetails] @email VARCHAR(200)
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT 
		slal.FirstName, 
		slal.LastName, 
		slal.Email, 
		slal.CompanyName, 
		slal.PhoneNumber, 
		kd.PhoneNumber AS PortalPhoneNumber, 
		kd.MobileNumber AS PortalMobileNumber, 
		kd.OtherPhoneNumber AS PortalOtherPhoneNumber, 
		kd.Country AS PortalCountry
	FROM [dbo].[SmartLeadAllLeads] slal
	LEFT JOIN KineticPortalLeadContactDetails kd ON slal.Email = kd.Email
	WHERE slal.Email= @email
END
GO