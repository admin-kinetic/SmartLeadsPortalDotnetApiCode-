using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class PermissionRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public PermissionRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<TableResponse<Permission>> Find(TableRequest request)
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            var baseQuery = """ 
                SELECT * From Permissions
             """;
            var queryParam = new
            {
                PageNumber = request.paginator.page,
                PageSize = request.paginator.pageSize
            };

            var countQuery = """ 
                SELECT Count(Id) From Permissions
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


            var items = await connection.QueryAsync<Permission>(baseQuery, parameters);
            var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

            var response = new TableResponse<Permission>
            {
                Items = items.ToList(),
                Total = count
            };
            return response;
        }
    }

    public async Task Create(PermissionCreate permission)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        var insert = """
            INSERT INTO Permissions (Name, Description) VALUES (@Name, @Description)
        """;
        await connection.ExecuteAsync(insert, permission);
    }

    internal async Task<List<Permission>> GetAll()
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var query = """
                    Select * From Permissions
                """;
            var result = await connection.QueryAsync<Permission>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

