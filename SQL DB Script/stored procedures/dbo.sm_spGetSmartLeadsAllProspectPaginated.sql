CREATE OR ALTER PROCEDURE [dbo].[sm_spGetSmartLeadsAllProspectPaginated]
@Search VARCHAR(500)='',
@Page INT = 1,
@PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
	SELECT DISTINCT LeadId, 
       Email, 
       FirstName + ' ' + LastName AS FullName,
	   Phonenumber 
	FROM SmartLeadAllLeads
	WHERE Email NOT LIKE '%?%' 
	  AND FirstName NOT LIKE '%?%' 
	  AND LastName NOT LIKE '%?%'
	  AND (@Search = '' 
	       OR (CHARINDEX('@', @Search) > 0 AND Email LIKE '%' + @Search + '%')
	       OR (CHARINDEX('@', @Search) = 0 AND FirstName + ' ' + LastName LIKE '%' + @Search + '%'))
	ORDER BY Email ASC
	OFFSET (@Page - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	OPTION (RECOMPILE);
END
GO