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

    public async Task<TableResponse<SmartleadsPortalUser>> Find(TableRequest request)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var baseQuery = """ 
                SELECT Users.*, r.Name as RoleName From Users
                LEFT JOIN UserRoles ur ON ur.EmployeeId = Users.EmployeeId
                LEFT JOIN Roles r ON r.Id = ur.RoleId
                WHERE Users.IsActive = 1
             """;
            var queryParam = new
            {
                PageNumber = request.paginator.page,
                PageSize = request.paginator.pageSize
            };

            var countQuery = """ 
                SELECT Count(EmployeeId) From Users
                WHERE IsActive = 1
            """;

            var countQueryParam = new
            {
                PageNumber = request.paginator.page,
                PageSize = request.paginator.pageSize
            };
            // Build WHERE clause if filters exist
            var whereClause = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("PageNumber", request.paginator.page);
            parameters.Add("PageSize", request.paginator.pageSize);

            if (request.filters != null && request.filters.Count > 0)
            {
                foreach (var filter in request.filters)
                {
                    switch (filter.Column.ToLower())
                    {
                        case "fullname":
                            whereClause.Add("Users.FullName LIKE @FullName");
                            parameters.Add("FullName", $"%{filter.Value}%");
                            break;
                        default:
                            break;
                    }
                }
            }

            if (whereClause.Count > 0)
            {
                var filterClause = " AND " + string.Join(" AND ", whereClause);
                baseQuery += filterClause;
                countQuery += filterClause;
            }

            baseQuery += """
                    ORDER BY EmployeeId ASC
                    OFFSET (@PageNumber - 1) * @PageSize ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                """;


            var items = await connection.QueryAsync<SmartleadsPortalUser>(baseQuery, parameters);
            var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

            var response = new TableResponse<SmartleadsPortalUser>
            {
                Items = items.ToList(),
                Total = count
            };
            return response;
        }
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
                SELECT * FROM Users us 
                INNER JOIN UserRoles ur ON us.EmployeeId = ur.EmployeeId
                WHERE us.IsActive = 1
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

    internal async Task AssignRole(int employeeId, int roleId)
    {
        try
        {
            using var connection = connectionFactory.GetSqlConnection();

            var userRoleExistsQuery = """
                    SELECT COUNT(*) FROM UserRoles WHERE EmployeeId = @EmployeeId
                    """;
            var userRoleExistsParam = new { employeeId };
            var userRoleExistsResult = connection.QueryFirstOrDefault<int>(userRoleExistsQuery, userRoleExistsParam);
            if (userRoleExistsResult > 0)
            {
                var delete = """
                        DELETE FROM UserRoles WHERE EmployeeId = @EmployeeId
                        """;
                await connection.ExecuteAsync(delete, new { employeeId });
            }

            // Insert the new role
            var insert = """
                    INSERT INTO UserRoles (EmployeeId, RoleId) 
                    VALUES (@EmployeeId, @RoleId) 
                    """;
            await connection.ExecuteAsync(insert, new { employeeId, roleId });
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Role?> GetUserRole(string? employeeId)
    {
        try
        {
            using var connection = connectionFactory.GetSqlConnection();
            var query = """
                    Select cr.* From Users cu
                    Inner Join UserRoles cur ON cur.EmployeeId = cu.EmployeeId
                    Inner Join Roles cr ON cr.Id = cur.RoleId
                    Where cu.EmployeeId = @EmployeeId
                    """;
            var queryParam = new { EmployeeId = employeeId };
            return await connection.QueryFirstOrDefaultAsync<Role>(query, queryParam);
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal async Task<List<Permission>> GetUserPermissions(string? employeeId)
    {
        try
        {
            using var connection = connectionFactory.GetSqlConnection();
            var query = """
                    Select cp.* From Users cu
                    Inner Join UserRoles cur ON cur.EmployeeId = cu.EmployeeId
                    Inner Join Roles cr ON cr.Id = cur.RoleId
                    Inner Join RolePermission crp ON crp.RoleId = cr.Id
                    Inner join Permissions cp ON cp.Id = crp.PermissionId
                    Where cu.EmployeeId = @EmployeeId
                    
                    """;
            var queryParam = new { EmployeeId = employeeId };
            var result = await connection.QueryAsync<Permission>(query, queryParam);
            return result.ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal async Task RemoveRole(int employeeId)
    {
        using (var connection = this.connectionFactory.GetSqlConnection())
        {
            var delete = """
                        DELETE FROM UserRoles WHERE EmployeeId = @EmployeeId
                        """;
            await connection.ExecuteAsync(delete, new { employeeId });
        }
    }
    public async Task<int> DeactivateUsers(UsersUpdate request)
    {
        try
        {
            using (var connection = this.connectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spDeactivateUsers";
                var param = new DynamicParameters();
                param.Add("@id", request.Id);

                int ret = await connection.ExecuteAsync(_proc, param);

                return ret;
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Database error: " + ex.Message);
        }
    }

    public async Task<int> GetSmartleadAccountByEmployeeId(string? employeeId)
    {
        using var connection = this.connectionFactory.GetSqlConnection();

        var query = """
                Select Top 1 sla.Id From Users u
                INNER JOIN SmartleadsAccountUsers slau ON slau.EmployeeId = u.EmployeeId
                INNER JOIN SmartleadsAccounts sla ON sla.Id = slau.SmartleadsAccountId 
                Where u.EmployeeId = @EmployeeId
            """;
        return await connection.QueryFirstOrDefaultAsync<int>(query, new { employeeId });
    }

    internal async Task<bool> IsAdmin(string employeeId)
    {
        if (!int.TryParse(employeeId, out var v))
        {
            return false;
        }

        await using var connection = await this.connectionFactory.GetSqlConnectionAsync();

        var query = """
                SELECT COUNT(*) FROM UserRoles ur
                INNER JOIN Roles r ON r.Id = ur.RoleId
                WHERE ur.EmployeeId = @employeeId AND (r.Name = 'Admin' OR r.Name = 'Super Admin')
            """;
        var count = await connection.QueryFirstOrDefaultAsync<int>(query, new { employeeId });
        return count > 0;
    }
}