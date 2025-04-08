using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class UserRepository
{
    private readonly DbConnectionFactory connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<SmartleadsPortalUser> GetByEmployeeId(int employeeId)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT * FROM Users WHERE EmployeeId = @EmployeeId
            """;

            var result = await connection.QueryFirstOrDefaultAsync<SmartleadsPortalUser>(query, new { employeeId });
            return result;
        }
    }

    public async Task Update(int employeeId, UpdateUserRequest request)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var insert = """
                UPDATE Users 
                SET PhoneNumber = @PhoneNumber 
                WHERE EmployeeId = @EmployeeId
            """;

            var insertParam = new
            {
                employeeId,
                PhoneNumber = request.PhoneNumber
            };
            await connection.ExecuteAsync(insert, insertParam);
        }
    }

    internal async Task<List<SmartleadsPortalUser>> GetAll()
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT * FROM Users
            """;

            var result = await connection.QueryAsync<SmartleadsPortalUser>(query);
            return result.ToList();
        }
    }
}