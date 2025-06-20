using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;
using System.Drawing;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class ProspectRepository
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        private readonly ILogger<ProspectRepository> logger;
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
        public ProspectRepository(DbConnectionFactory dbConnectionFactory, ILogger<ProspectRepository> logger)
        {
            this.dbConnectionFactory = dbConnectionFactory;
            this.logger = logger;
        }
        public async Task<TableResponse<Prospect>> Find(TableRequest request, CancellationToken cancellationToken)
        {
            using (var connection = dbConnectionFactory.GetSqlConnection())
            {
                var baseQuery = """ 
                    SELECT al.LeadId,
                        sle.LeadEmail AS Email, 
                        al.FirstName + ' ' + al.LastName as FullName
                    FROM [dbo].[SmartLeadsEmailStatistics] sle
                    INNER JOIN SmartLeadAllLeads al ON al.email = sle.LeadEmail
                """;
                var queryParam = new
                {
                    PageNumber = request.paginator.page,
                    PageSize = request.paginator.pageSize
                };

                var countQuery = """ 
                    SELECT
                        count(sle.Id) as Total
                    FROM [dbo].[SmartLeadsEmailStatistics] sle
                    INNER JOIN SmartLeadAllLeads al ON al.email = sle.LeadEmail
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
                            // case "fullname":
                            //     whereClause.Add("(al.FirstName + ' ' + al.LastName) LIKE @FullName");
                            //     parameters.Add("FullName", $"%{filter.Value}%");
                            //     break;
                            case "email":
                                whereClause.Add("sle.LeadEmail LIKE @Email");
                                parameters.Add("Email", $"%{filter.Value}%");
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

                // Add ORDER BY and pagination
                baseQuery += """
                    ORDER BY sle.LeadId DESC
                    OFFSET (@PageNumber - 1) * @PageSize ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                """;

                try
                {
                    var baseQueryCommand = new CommandDefinition(baseQuery, parameters, cancellationToken: cancellationToken);
                    var items = await connection.QueryAsync<Prospect>(baseQueryCommand);
                    var countQueryCommand = new CommandDefinition(countQuery, parameters, cancellationToken: cancellationToken);
                    var count = await connection.QueryFirstAsync<int>(countQueryCommand);
                    var response = new TableResponse<Prospect>
                    {
                        Items = items.ToList(),
                        Total = count
                    };
                    return response;
                }
                catch (OperationCanceledException ex)
                {
                    this.logger.LogError($"Operation Cancelled: {ex.Message}");
                    throw;
                }
            }
        }

        public List<string> AllColumns()
        {
            return new List<string>{
            // "Id",
            // "GuId",
            // "LeadId",
            "FullName",
            "Email",
            };
        }

        public async Task<ProspectResponseModel<Prospect>> GetSmartLeadsProspect(ExcludedKeywordsListRequest request)
        {
            try
            {
                IEnumerable<Prospect> list = new List<Prospect>();
                var param = new DynamicParameters();
                var count = 0;

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetSmartLeadsProspect";

                    param.Add("@PageNumber", request.Page);
                    param.Add("@PageSize", request.PageSize);

                    list = await connection.QueryAsync<Prospect>(_proc, param, commandType: CommandType.StoredProcedure);

                    var countProcedure = "sm_spGetSmartLeadsProspectCount";
                    count = await connection.QueryFirstOrDefaultAsync<int>(countProcedure, commandType: CommandType.StoredProcedure);

                    return new ProspectResponseModel<Prospect>
                    {
                        Items = list.ToList(),
                        Total = count
                    };
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<IEnumerable<Prospect>> GetSmartLeadsAllProspect(ProspectDropdownOptionParam request)
        {
            try
            {
                IEnumerable<Prospect> list = new List<Prospect>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetSmartLeadsAllProspect";
                    var param = new DynamicParameters();
                    param.Add("@Search", request.Search);

                    list = await connection.QueryAsync<Prospect>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<IEnumerable<Prospect>> GetSmartLeadsAllProspectPaginated(ProspectDropdownOptionParam request)
        {
            try
            {
                IEnumerable<Prospect> list = new List<Prospect>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetSmartLeadsAllProspectPaginated";
                    var param = new DynamicParameters();
                    param.Add("@Search", request.Search);
                    param.Add("@Page", request.Page);
                    param.Add("@PageSize", request.PageSize);

                    list = await connection.QueryAsync<Prospect>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
    }
}
