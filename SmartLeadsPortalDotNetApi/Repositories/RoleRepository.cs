using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class RoleRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public RoleRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<TableResponse<Role>> Find(TableRequest request)
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            var baseQuery = """ 
                SELECT * From Roles
             """;
            var queryParam = new
            {
                PageNumber = request.paginator.page,
                PageSize = request.paginator.pageSize
            };

            var countQuery = """ 
                SELECT Count(Id) From Roles
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
                        default:
                            break;
                    }
                }
            }

            if (whereClause.Count > 0)
            {
                var whereStatement = " WHERE " + string.Join(" AND ", whereClause);
                baseQuery += whereStatement;
                countQuery += whereStatement;
            }

            baseQuery += """
                    ORDER BY Id ASC
                    OFFSET (@PageNumber - 1) * @PageSize ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                """;


            var items = await connection.QueryAsync<Role>(baseQuery, parameters);
            var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

            var response = new TableResponse<Role>
            {
                Items = items.ToList(),
                Total = count
            };
            return response;
        }
    }

    public async Task Create(RoleCreate permission)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        var insert = """
            INSERT INTO Roles (Name, Description) VALUES (@Name, @Description)
        """;
        await connection.ExecuteAsync(insert, permission);
    }

    public async Task AssignPermission(int roleId, int permissionId)
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var insert = """
                        INSERT INTO RolePermission (RoleId, PermissionId) 
                        VALUES (@RoleId, @PermissionId) 
                    """;
            var inserParam = new { RoleId = roleId, PermissionId = permissionId };
            await connection.ExecuteAsync(insert, inserParam);

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task AssignPermissions(int roleId, int[] permissionIds)
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var insert = """
                        INSERT INTO RolePermission (RoleId, PermissionId) 
                        VALUES (@RoleId, @PermissionId) 
                    """;
            var inserParam = permissionIds.Select(permissionId => new { roleId, permissionId });
            await connection.ExecuteAsync(insert, inserParam);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Permission>> GetAssignedPermissions(int roleId)
    {

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var query = """
                    Select p.* From Roles cr
                    Inner Join RolePermission crp On crp.RoleId = cr.Id 
                    Inner Join Permissions p On p.Id = crp.PermissionId
                    Where cr.Id = @RoleId
                """;
            var queryParam = new { RoleId = roleId };
            var result = await connection.QueryAsync<Permission>(query, queryParam);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    internal async Task<List<Role>> GetAll()
    {
         try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var query = """
                    Select * From Roles
                """;
            var result = await connection.QueryAsync<Role>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    internal async Task DeletePermission(int roleId, int permissionId)
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var delete = """
                    DELETE FROM RolePermission WHERE RoleId = @RoleId AND PermissionId = @PermissionId
                """;
            var queryParam = new { roleId, permissionId };
            var result = await connection.ExecuteAsync(delete, queryParam);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    internal async Task Delete(int roleId)
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var delete = """
                    DELETE FROM Roles WHERE Id = @RoleId
                """;
            var queryParam = new { roleId };
            await connection.ExecuteAsync(delete, queryParam);

            var deletePermission = """
                    DELETE FROM RolePermission WHERE RoleId = @RoleId
                """;
            var deletePermissionParam = new { roleId };
            await connection.ExecuteAsync(deletePermission, deletePermissionParam);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
