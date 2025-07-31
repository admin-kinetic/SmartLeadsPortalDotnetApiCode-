CREATE OR ALTER PROCEDURE [dbo].[sm_spLeadProspectCallDetails]
@email VARCHAR(200)
AS 
BEGIN 
 SET NOCOUNT ON;
	SELECT TOP 1 
		cl.GuId,
		cl.UserPhoneNumber, 
		CASE 
			WHEN cl.ProspectNumber IS NULL OR cl.ProspectNumber = '' THEN sal.PhoneNumber 
			ELSE cl.ProspectNumber 
		END AS ProspectNumber,
		cl.CallPurposeId, 
		cl.CallDispositionId, 
		cl.Notes, 
		cl.CallTagsId,
		cl.CallStateId
	FROM [dbo].[Calls] cl
	LEFT JOIN [dbo].[SmartLeadAllLeads] sal
		ON cl.LeadEmail = sal.Email
	WHERE sal.Email = @email
	ORDER BY cl.AddedDate DESC;
END
GO

