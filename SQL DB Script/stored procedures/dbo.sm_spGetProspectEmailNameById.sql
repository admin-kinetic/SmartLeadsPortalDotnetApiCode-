CREATE OR ALTER PROCEDURE [dbo].[sm_spGetProspectEmailNameById] 
@leadid VARCHAR(200)
AS  
BEGIN   
	SET NOCOUNT ON;
	SELECT DISTINCT Email, FirstName +' ' + LastName AS FullName FROM SmartLeadAllLeads WHERE LeadId=@leadid
END
GO