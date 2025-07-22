CREATE OR ALTER PROCEDURE [dbo].[sm_spGetSmartLeadsAllProspectPaginated]
@Search VARCHAR(500)='',
@Page INT = 1,
@PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
	SELECT DISTINCT 
		slal.LeadId,
		slal.Email, 
		slal.FirstName + ' ' + slal.LastName AS FullName,
		slal.PhoneNumber,
		kd.PhoneNumber AS PortalPhoneNumber,
		kd.MobileNumber AS PortalMobileNumber,
		kd.OtherPhoneNumber AS PortalOtherPhoneNumber,
		kd.Country AS PortalCountry
	FROM SmartLeadAllLeads slal
	LEFT JOIN KineticPortalLeadContactDetails kd ON slal.Email = kd.Email
	WHERE slal.Email NOT LIKE '%?%' 
	  AND slal.FirstName NOT LIKE '%?%' 
	  AND slal.LastName NOT LIKE '%?%'
	  AND (@Search = '' 
		OR (CHARINDEX('@', @Search) > 0 AND slal.Email LIKE '%' + @Search + '%')
		OR (CHARINDEX('@', @Search) = 0 AND slal.FirstName + ' ' + slal.LastName LIKE '%' + @Search + '%'))
	ORDER BY slal.Email ASC
	OFFSET (@Page - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO