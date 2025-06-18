using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;

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

            var updateUser = """
                UPDATE Users 
                    SET PhoneNumber = @phoneNumber 
                WHERE EmployeeId = @employeeId;
            """;
            await connection.ExecuteAsync(updateUser, request);
        }
    }

    public async Task AssignVoipPhoneNumber(int employeeId, string phoneNumber)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var insert = """
                UPDATE VoipPhoneNumbers 
                SET EmployeeId = @employeeId 
                WHERE PhoneNumber = @phoneNumber;
            """;
            await connection.ExecuteAsync(insert, new { phoneNumber, employeeId });
        }
    }

    public async Task Update(int id, AddVoipPhoneNumberRequest request)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var update = """
                UPDATE VoipPhoneNumbers 
                SET EmployeeId = @employeeId,
                    PhoneNumber = @phoneNumber
                WHERE Id = @id;
            """;
            var updateParam = new {
                employeeId = request.EmployeeId,
                phoneNumber = request.PhoneNumber,
                id
            };
            await connection.ExecuteAsync(update, updateParam);

            if (request.EmployeeId != null)
            {
                var updateEmployee = """
                    UPDATE Users 
                        SET PhoneNumber = @phoneNumber 
                    WHERE EmployeeId = @employeeId;
                """;
                await connection.ExecuteAsync(updateEmployee, request);
            }
        }
    }
}
