CREATE   PROCEDURE [dbo].[sm_spInsertCallLogs]
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
@statisticid INT,
@due DATETIME,
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

	
	-- Conditionally update statistics if @statisticid is NOT NULL
	IF @statisticid IS NOT NULL
	BEGIN
		-- Use GETDATE() if @due is NULL
		DECLARE @finalDue DATETIME = ISNULL(@due, GETDATE());

		BEGIN
			BEGIN TRANSACTION; -- Start a transaction

			BEGIN TRY
				UPDATE SmartLeadsEmailStatistics 
				SET CallStateId = @callstateid,
					AssignedTo = @userid,
					Due = @finalDue
				WHERE Id=@statisticid
				COMMIT;
			END TRY
			BEGIN CATCH
				ROLLBACK; -- Rollback the transaction in case of an error
			END CATCH

		END
	END
END
GO

