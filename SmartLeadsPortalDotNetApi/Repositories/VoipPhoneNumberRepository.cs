using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class VoipPhoneNumberRepository
{
    private readonly DbConnectionFactory connectionFactory;

    public VoipPhoneNumberRepository(DbConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<TableResponse<VoipPhoneNumberResponse>> GetAllVoipPhoneNumbers(Paginator paginator)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT VoipPhoneNumbers.*, Users.FullName AS [UserFullName] 
                FROM VoipPhoneNumbers 
                LEFT JOIN Users ON VoipPhoneNumbers.EmployeeId = Users.EmployeeId
                ORDER BY VoipPhoneNumbers.Id ASC
                OFFSET (@PageNumber - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
            """;
            var queryParam = new
            {
                PageNumber = paginator.page,
                PageSize = paginator.pageSize
            };
            var queryResult = await connection.QueryAsync<VoipPhoneNumberResponse>(query, queryParam);

            var queryTotal = """
                SELECT Count(VoipPhoneNumbers.Id) 
                FROM VoipPhoneNumbers 
                LEFT JOIN Users ON VoipPhoneNumbers.EmployeeId = Users.EmployeeId
            """;
            var queryTotalResult = await connection.QueryFirstAsync<int>(queryTotal);


            return new TableResponse<VoipPhoneNumberResponse>
            {
                Items = queryResult.ToList(),
                Total = queryTotalResult
            };
        }
    }

    public async Task<List<VoipPhoneNumber>> GetAllUnAssignedVoipPhoneNumbers()
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT * FROM VoipPhoneNumbers 
                WHERE EmployeeId IS NULL;
            """;
            var result = await connection.QueryAsync<VoipPhoneNumber>(query);
            return result.ToList();
        }
    }


    public async Task AddVoipPhoneNumber(AddVoipPhoneNumberRequest request)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var insert = """
                INSERT INTO VoipPhoneNumbers (PhoneNumber, EmployeeId) VALUES (@phoneNumber, @employeeId);
            """;
            await connection.ExecuteAsync(insert, request);
        }
    }

    public async Task AssignVoipPhoneNumber(int employeeId, string phoneNumber)
    {
        await using var connection = await this.connectionFactory.GetSqlConnectionAsync();

        // check if employeeId has an existing phone number assigned to the phone number
        var checkEmployeeHasAssignedNumber = """
                SELECT COUNT(*) FROM VoipPhoneNumbers 
                WHERE EmployeeId = @employeeId;
            """;
        var checkEmployeeHasAssignedNumberCount = await connection.ExecuteScalarAsync<int>(checkEmployeeHasAssignedNumber, new { employeeId });
        if (checkEmployeeHasAssignedNumberCount > 0) // has existing phone number
        {
            // unassign the existing phone number
            var updateUserPhoneNumber = """
                    UPDATE VoipPhoneNumbers 
                    SET EmployeeId = NULL
                    WHERE EmployeeId = @employeeId;
                """;
            await connection.ExecuteAsync(updateUserPhoneNumber, new { employeeId });
        }

        // check if phone number is available for assignment
        var checkIfPhoneNumberIsAvailableForAssignment = """
                SELECT COUNT(*) FROM VoipPhoneNumbers 
                WHERE PhoneNumber = @phoneNumber AND (EmployeeId IS NULL OR EmployeeId = '');
            """;
        var phoneNumberIsAvailableForAssignmentCount = await connection.ExecuteScalarAsync<int>(checkIfPhoneNumberIsAvailableForAssignment, new { phoneNumber, employeeId });
        if (phoneNumberIsAvailableForAssignmentCount > 0)
        {
            var assignPhoneNumberToUser = """
                UPDATE VoipPhoneNumbers 
                    SET EmployeeId = @employeeId 
                WHERE PhoneNumber = @phoneNumber;
            """;
            await connection.ExecuteAsync(assignPhoneNumberToUser, new { phoneNumber, employeeId });
            return; // phone number was successfully assigned
        }

        // if phone number was already assigned to another employee
        var checkIfPhoneNumberWasAssignedToOtherUser = """
                SELECT COUNT(*) FROM VoipPhoneNumbers 
                WHERE PhoneNumber = @phoneNumber AND EmployeeId <> @employeeId;
            """;
        var phoneNumberWasAssignedToOtherUserCount = await connection.ExecuteScalarAsync<int>(checkIfPhoneNumberWasAssignedToOtherUser, new { phoneNumber, employeeId });
        if (phoneNumberWasAssignedToOtherUserCount > 0)
        {
            throw new Exception("Phone number is already assigned to another employee.");
        }

        var insert = """
                INSERT INTO VoipPhoneNumbers (EmployeeId, PhoneNumber)
                VALUES (@employeeId, @phoneNumber);
            """;
        await connection.ExecuteAsync(insert, new { phoneNumber, employeeId });

    }

    public async Task<int> UpSertVoipnumbers(AddVoipPhoneNumberRequest request)
    {
        try
        {
            using (var connection = this.connectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spUpSertPhonenumberUsers";
                var param = new DynamicParameters();
                param.Add("@id", request.Id);
                param.Add("@employeeid", request.EmployeeId);
                param.Add("@phonenumber", request.PhoneNumber);

                int ret = await connection.ExecuteAsync(_proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Database error: " + ex.Message);
        }
    }
    public async Task<int> DeleteVoipNumber(Guid guid)
    {
        try
        {
            using (var connection = this.connectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spDeleteVoipNumber";
                var param = new DynamicParameters();
                param.Add("@guid", guid);

                int ret = await connection.ExecuteAsync(_proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Database error: " + ex.Message);
        }
    }

    public async Task<List<VoipPhoneNumber>> GetAllAssignableVoipPhoneNumbers(int employeeId)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT * FROM VoipPhoneNumbers 
                WHERE EmployeeId IS NULL OR EmployeeId = @employeeId;
            """;
            var result = await connection.QueryAsync<VoipPhoneNumber>(query, new { employeeId });
            return result.ToList();
        }
    }
}
