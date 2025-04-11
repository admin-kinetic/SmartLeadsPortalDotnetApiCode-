using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using System;
using System.Data;

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

    public async Task<List<SmartleadsPortalUser>> GetAll()
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

    public async Task<List<SmartleadsPortalUser>> GetAllWithUnassignedPhoneNumbers()
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT * FROM Users Where PhoneNumber IS NULL OR  PhoneNumber = '' 
            """;

            var result = await connection.QueryAsync<SmartleadsPortalUser>(query);
            return result.ToList();
        }
    }

    public async Task<List<SmartleadsPortalUser>> GetAllWithAssignedPhoneNumbers()
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT * FROM Users Where PhoneNumber IS NOT NULL OR PhoneNumber <> '' 
            """;

            var result = await connection.QueryAsync<SmartleadsPortalUser>(query);
            return result.ToList();
        }
    }
    public async Task<UsersPhone?> GetUsersPhoneById(int id)
    {
        try
        {
            using (var connection = this.connectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spGetUsersPhoneById";
                var param = new DynamicParameters();
                param.Add("@id", id);
                UsersPhone? list = await connection.QueryFirstOrDefaultAsync<UsersPhone>(_proc, param);

                return list;
            }
            
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    public async Task<IEnumerable<UsersPhone?>> GetUsersWithPhoneAssigned()
    {
        try
        {
            using (var connection = this.connectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spGetUsersWithPhoneAssigned";
                IEnumerable<UsersPhone>? list = await connection.QueryAsync<UsersPhone>(_proc);

                return list;
            }

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}