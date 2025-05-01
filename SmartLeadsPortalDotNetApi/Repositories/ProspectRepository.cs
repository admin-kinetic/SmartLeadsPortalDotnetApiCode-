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
        public ProspectRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<TableResponse<Prospect>> Find(TableRequest request)
        {
            using (var connection = dbConnectionFactory.GetSqlConnection())
            {
                var baseQuery = """ 
                SELECT DISTINCT JSON_VALUE(wh.Request, '$.sl_email_lead_id') AS LeadId,
                sle.LeadEmail AS Email, 
                JSON_VALUE(wh.Request, '$.to_name') AS FullName
                FROM [dbo].[SmartLeadsEmailStatistics] sle
                INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
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
               INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
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
                            case "fullname":
                                whereClause.Add("JSON_VALUE(wh.Request, '$.to_name') LIKE @FullName");
                                parameters.Add("FullName", $"%{filter.Value}%");
                                break;
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
                    OFFSET (@PageNumber - 1) * @PageSize ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                """;


                var items = await connection.QueryAsync<Prospect>(baseQuery, parameters);
                var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

                var response = new TableResponse<Prospect>
                {
                    Items = items.ToList(),
                    Total = count
                };
                return response;
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
        public async Task<IEnumerable<Prospect>> GetSmartLeadsAllProspect()
        {
            try
            {
                IEnumerable<Prospect> list = new List<Prospect>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetSmartLeadsAllProspect";

                    list = await connection.QueryAsync<Prospect>(_proc, commandType: CommandType.StoredProcedure);

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
