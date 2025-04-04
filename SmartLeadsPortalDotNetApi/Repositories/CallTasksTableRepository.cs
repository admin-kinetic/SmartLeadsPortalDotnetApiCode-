using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;

public class CallTasksTableRepository
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

    public CallTasksTableRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }
    public async Task<TableResponse<SmartLeadsCallTasks>> Find(TableRequest request)
    {
        using (var connection = dbConnectionFactory.GetSqlConnection())
        {
            var baseQuery = """ 
                SELECT
                    sle.Id,
                    sle.GuId,
                    JSON_VALUE(wh.Request, '$.sl_email_lead_id') AS LeadId, 
                    sle.LeadEmail AS Email, 
                    JSON_VALUE(wh.Request, '$.to_name') AS FullName, 
                    sle.SequenceNumber,
                    JSON_VALUE(wh.Request, '$.campaign_name') AS CampaignName, 
                    sle.EmailSubject AS SubjectName, 
                    sle.OpenCount, 
                    sle.ClickCount,
                    cs.StateName AS CallState
                FROM SmartLeadsEmailStatistics sle
                INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
                LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
            """;
            var queryParam = new
            {
                PageNumber = request.paginator.page,
                PageSize = request.paginator.pageSize
            };

            var countQuery = """ 
                SELECT
                    count(sle.Id) as Total
                FROM SmartLeadsEmailStatistics sle
                INNER JOIN Webhooks wh ON JSON_VALUE(wh.Request, '$.to_email') = sle.LeadEmail
                LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
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
                            whereClause.Add("sle.LeadEmail LIKE @Email");
                            parameters.Add("Email", $"%{filter.Value}%");
                            break;
                        case "fullname":
                            whereClause.Add("JSON_VALUE(wh.Request, '$.to_name') LIKE @FullName");
                            parameters.Add("FullName", $"%{filter.Value}%");
                            break;
                        case "campaignname":
                            whereClause.Add("JSON_VALUE(wh.Request, '$.campaign_name') LIKE @CampaignName");
                            parameters.Add("CampaignName", $"%{filter.Value}%");
                            break;
                        case "subjectname":
                            whereClause.Add("sle.EmailSubject LIKE @SubjectName");
                            parameters.Add("SubjectName", $"%{filter.Value}%");
                            break;
                        // case "callstate":
                        //     whereClause.Add("cs.StateName LIKE @CallState");
                        //     parameters.Add("CallState", $"%{filter.Value}%");
                        //     break;
                        case "opencount":
                            whereClause.Add($"sle.OpenCount {this.operatorsMap[filter.Operator]} @OpenCount");
                            parameters.Add("OpenCount", filter.Value);
                            break;
                        case "clickcount":
                            whereClause.Add($"sle.ClickCount {this.operatorsMap[filter.Operator]} @ClickCount");
                            parameters.Add("ClickCount", filter.Value);
                            break;
                        // Add more cases for other filterable columns
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
                ORDER BY sle.OpenCount DESC
                OFFSET (@PageNumber - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
            """;


            var items = await connection.QueryAsync<SmartLeadsCallTasks>(baseQuery, parameters);
            var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

            var response = new TableResponse<SmartLeadsCallTasks>
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
            "Email",
            "FullName",
            "SequenceNumber",
            "CampaignName",
            // "SubjectName",
            "OpenCount",
            "ClickCount",
            // "CallState"
        };
    }
}