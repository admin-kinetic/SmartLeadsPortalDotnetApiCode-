CREATE OR ALTER PROCEDURE [dbo].[sm_spUpSertPhonenumberUsers]
@id INT,
@employeeid INT,
@phonenumber VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION; -- Start a transaction

    BEGIN TRY
        -- Check if the phone number is already assigned to another user
        DECLARE @existingEmployeeId INT;
        SELECT @existingEmployeeId = EmployeeId 
        FROM Users 
        WHERE PhoneNumber = @phoneNumber 
        AND EmployeeId != @employeeid;

        -- If phone number exists for another user, set their phone number to NULL
        IF @existingEmployeeId IS NOT NULL
        BEGIN
            UPDATE Users
            SET PhoneNumber = NULL
            WHERE EmployeeId = @existingEmployeeId;
        END

        -- Update or insert the phone number for VoipPhoneNumbers
        IF EXISTS (SELECT 1 FROM VoipPhoneNumbers WHERE Id = @id)
        BEGIN
            UPDATE VoipPhoneNumbers 
            SET EmployeeId = @employeeId,
                PhoneNumber = @phoneNumber
            WHERE Id = @id;
        END
		ELSE
        BEGIN
            INSERT INTO VoipPhoneNumbers (EmployeeId, PhoneNumber)
            VALUES (@employeeId, @phoneNumber);
        END

        -- Update the user's phone number
        UPDATE Users 
        SET PhoneNumber = @phoneNumber 
        WHERE EmployeeId = @employeeId;

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK; -- Rollback the transaction in case of an error
    END CATCH
END
GO