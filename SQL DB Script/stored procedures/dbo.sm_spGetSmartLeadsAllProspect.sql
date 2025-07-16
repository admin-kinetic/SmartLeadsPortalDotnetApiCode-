CREATE OR ALTER PROCEDURE [dbo].[sm_spGetSmartLeadsAllProspect]
@Search VARCHAR(500)=''
AS
BEGIN
    SET NOCOUNT ON;
	SELECT DISTINCT LeadId, 
       Email, 
       FirstName + ' ' + LastName AS FullName 
	FROM SmartLeadAllLeads
	WHERE Email NOT LIKE '%?%' 
	  AND FirstName NOT LIKE '%?%' 
	  AND LastName NOT LIKE '%?%'
	  AND (FirstName + ' ' + LastName LIKE '%' + @Search + '%' OR @Search = '')
	ORDER BY Email ASC
END
GO