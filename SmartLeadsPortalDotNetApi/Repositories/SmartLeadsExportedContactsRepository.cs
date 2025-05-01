using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartLeadsExportedContactsRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    private readonly Dictionary<string, string> operatorsMap = new Dictionary<string, string>
        {
            { "is", "=" },
            { "is not", "!=" },
            { "less than", "<" },
            { "less than equal", "<=" },
            { "greater than", ">" },
            { "greater than equal", ">=" },
            { "contains", "LIKE" }
        };

    public SmartLeadsExportedContactsRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }


    public async Task<TableResponse<SmartLeadsExportedContact>> Find(TableRequest request)
    {
        using (var connection = dbConnectionFactory.GetSqlConnection())
        {
            var baseQuery = """ 
                SELECT *
                FROM SmartLeadsExportedContacts slec
            """;

            var queryParam = new
            {
                PageNumber = request.paginator.page,
                PageSize = request.paginator.pageSize
            };

            var countQuery = """ 
                SELECT COUNT(slec.Id) AS TotalCount
                FROM SmartLeadsExportedContacts slec
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
                    // Handle different column types appropriately
                    switch (filter.Column.ToLower())
                    {
                        case "email":
                            whereClause.Add("slec.Email LIKE @Email");
                            parameters.Add("Email", $"%{filter.Value}%");
                            break;
                        case "hasreply":
                            whereClause.Add("slec.HasReply = @HasReply");
                            parameters.Add("HasReply", filter.Value);
                            break;
                        case "hasreviewed":
                            whereClause.Add("slec.hHasReview = @HasRevew");
                            parameters.Add("HasReview", filter.Value);
                            break;
                        case "exporteddatefrom":
                            whereClause.Add("slec.ExportedDate <= @ExportedDateFrom");
                            parameters.Add("ExportedDateFrom", $"%{filter.Value}%");
                            break;
                        case "exporteddateto":
                            whereClause.Add("slec.ExportedDate <= @ExportedDateTo");
                            parameters.Add("ExportedDateTo", $"%{filter.Value}%");
                            break;
                        default:
                            // For numeric fields or exact matches
                            whereClause.Add($"{filter.Column} = @{filter.Column}");
                            parameters.Add(filter.Column, filter.Value);
                            break;
                    }
                }
            }

            // Add WHERE clause if needed
            if (whereClause.Count > 0)
            {
                var whereStatement = " WHERE " + string.Join(" AND ", whereClause);
                baseQuery += whereStatement;
                countQuery += whereStatement;
            }

            // Sorting to follow after where
            if (request.sorting != null)
            {
                switch (request.sorting.column.ToLower())
                {
                    default:
                        baseQuery += $" ORDER BY slec.Id DESC";
                        break;
                }
            }

            // Add ORDER BY and pagination
            baseQuery += """
                OFFSET (@PageNumber - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
            """;


            var items = await connection.QueryAsync<SmartLeadsExportedContact>(baseQuery, parameters);
            var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

            var response = new TableResponse<SmartLeadsExportedContact>
            {
                Items = items.ToList(),
                Total = count
            };
            return response;
        }
    }
}
