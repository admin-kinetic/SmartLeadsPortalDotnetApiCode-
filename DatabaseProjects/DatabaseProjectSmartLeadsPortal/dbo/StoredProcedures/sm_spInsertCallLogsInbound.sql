CREATE   PROCEDURE [dbo].[sm_spInsertCallLogsInbound]
@usercaller VARCHAR(255),
@userphonenumber VARCHAR(100),
@prospectname VARCHAR(255),
@prospectnumber VARCHAR(100),
@leademail VARCHAR(255),
@callpurposeid INT,
@calldispositionid INT,
@calldirectionid INT,
@notes VARCHAR(MAX),
@calltagsid INT,
@callstateid INT,
@duration INT,
@addedby VARCHAR(200),
@userid INT,
@uniquecallid VARCHAR(255),
@calleddate DATETIME
AS
BEGIN
	BEGIN
		INSERT INTO Calls(GuId, 
		UserCaller, 
		UserPhoneNumber,
		LeadEmail,
		ProspectName, 
		ProspectNumber, 
		CalledDate, 
		CallPurposeId, 
		CallDispositionId, 
		CallDirectionId, 
		Notes, 
		CallTagsId,
		CallStateId,
		Duration,
		AddedBy, 
		AddedDate,
		UniqueCallId)
		VALUES(NEWID(), 
		@usercaller,
		@userphonenumber,
		@leademail,
		@prospectname, 
		@prospectnumber, 
		@calleddate, 
		@callpurposeid, 
		@calldispositionid, 
		@calldirectionid, 
		@notes,
		@calltagsid, 
		@callstateid,
		@duration,
		@addedby, 
		GETDATE(),
		@uniquecallid)
	END
END
GO

