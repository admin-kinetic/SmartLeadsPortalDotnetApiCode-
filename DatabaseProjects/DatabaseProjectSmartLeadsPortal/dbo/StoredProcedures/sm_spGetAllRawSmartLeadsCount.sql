CREATE   PROCEDURE [dbo].[sm_spGetAllRawSmartLeadsCount]
    @email NVARCHAR(255) = NULL,
    @messagehistory NVARCHAR(MAX) = NULL,
    @hasReply BIT = NULL,
    @isValid BIT = NULL,
    @ExportedDateFrom DATETIME = NULL,
    @ExportedDateTo DATETIME = NULL
    --@ExcludeKeywords NVARCHAR(MAX) = NULL,
AS
BEGIN
    SET NOCOUNT ON;
	select COUNT(*) AS Total
	from SmartLeadsExportedContacts sl
	WHERE (@email = '' OR Email =@email)
	AND (@messagehistory = '' OR MessageHistory LIKE '%' + @messagehistory + '%')
	AND (@hasReply = 0 OR @hasReply is null OR HasReply = @hasReply)
	AND (@isValid = 0 OR @isValid is null OR HasReviewed = @isValid)
	AND ((@ExportedDateFrom IS NULL OR @ExportedDateTo IS NULL) 
		OR (sl.ExportedDate >= CONVERT(DATE, @ExportedDateFrom) 
		AND sl.ExportedDate <= CONVERT(DATE, @ExportedDateTo)))
	
	--AND (@excludeKeywords = '' 
	--		OR NOT EXISTS (
	--			SELECT 1 
	--			FROM STRING_SPLIT(@excludeKeywords, ',') AS k 
	--			WHERE sl.MessageHistory LIKE '%' + LTRIM(RTRIM(k.value)) + '%'
	--		)
	--	)
END

GO

