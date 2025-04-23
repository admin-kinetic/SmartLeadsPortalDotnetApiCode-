using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallsTableRepository
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

        public CallsTableRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<TableResponse<SmartLeadsCalls>> Find(TableRequest request)
        {
            using (var connection = dbConnectionFactory.GetSqlConnection())
            {
                var baseQuery = """ 
                SELECT cl.Id, 
            	    cl.GuId, 
            	    cl.UserCaller, 
            	    cl.UserPhoneNumber,
                    cl.LeadEmail,
            	    cl.ProspectName, 
            	    cl.ProspectNumber, 
            	    cl.CalledDate,
                    cl.CallStateId,
            	    cs.StateName AS CallState,
            	    cl.Duration,
                    cl.CallPurposeId,
            	    cp.CallPurposeName AS CallPurpose,
                    cl.CallDispositionId,
            	    cd.CallDispositionName AS CallDisposition,
            	    cl.Notes,
                    cl.CallTagsId,
            	    ct.TagName AS CallTags,
            	    cl.AddedBy,
                    ob.AzureStorageCallRecordingLink AS RecordedLink
                FROM [dbo].[Calls] cl
                LEFT JOIN CallPurpose cp ON cl.CallPurposeId = cp.Id
                LEFT JOIN CallDisposition cd ON cl.CallDispositionId = cd.Id
                LEFT JOIN CallState cs ON cl.CallStateId = cs.Id
                LEFT JOIN Tags ct ON cl.CallTagsId = ct.Id
                LEFT JOIN OutboundCalls ob ON cl.UniqueCallId = ob.UniqueCallId
            """;
                var queryParam = new
                {
                    PageNumber = request.paginator.page,
                    PageSize = request.paginator.pageSize
                };

                var countQuery = """ 
                SELECT
                    count(cl.Id) as Total
                FROM [dbo].[Calls] cl
                LEFT JOIN CallPurpose cp ON cl.CallPurposeId = cp.Id
                LEFT JOIN CallDisposition cd ON cl.CallDispositionId = cd.Id
                LEFT JOIN CallState cs ON cl.CallStateId = cs.Id
                LEFT JOIN Tags ct ON cl.CallTagsId = ct.Id
                LEFT JOIN OutboundCalls ob ON cl.UniqueCallId = ob.UniqueCallId
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
                            case "usercaller":
                                whereClause.Add("cl.UserCaller LIKE @UserCaller");
                                parameters.Add("UserCaller", $"%{filter.Value}%");
                                break;
                            case "userphonenumber":
                                whereClause.Add("cl.UserPhoneNumber LIKE @UserPhoneNumber");
                                parameters.Add("UserPhoneNumber", $"%{filter.Value}%");
                                break;
                            case "prospectname":
                                whereClause.Add("cl.ProspectName LIKE @ProspectName");
                                parameters.Add("ProspectName", $"%{filter.Value}%");
                                break;
                            case "prospectnumber":
                                whereClause.Add("cl.ProspectNumber LIKE @ProspectNumber");
                                parameters.Add("ProspectNumber", $"%{filter.Value}%");
                                break;
                            case "callstate":
                                whereClause.Add("cs.StateName LIKE @CallState");
                                parameters.Add("CallState", $"%{filter.Value}%");
                                break;
                            case "callpurpose":
                                whereClause.Add("cp.CallPurposeName LIKE @CallPurpose");
                                parameters.Add("CallPurpose", $"%{filter.Value}%");
                                break;
                            case "calldisposition":
                                whereClause.Add("cd.CallDispositionName LIKE @CallDisposition");
                                parameters.Add("CallDisposition", $"%{filter.Value}%");
                                break;
                            case "calltags":
                                whereClause.Add("ct.TagName LIKE @CallTags");
                                parameters.Add("CallTags", $"%{filter.Value}%");
                                break;
                            case "notes":
                                whereClause.Add("cl.Notes LIKE @Notes");
                                parameters.Add("Notes", $"%{filter.Value}%");
                                break;
                            case "addedby":
                                whereClause.Add("cl.AddedBy LIKE @AddedBy");
                                parameters.Add("AddedBy", $"%{filter.Value}%");
                                break;
                            case "calleddate":
                                whereClause.Add($"cl.CalledDate {this.operatorsMap[filter.Operator]} @CalledDate");
                                parameters.Add("CalledDate", filter.Value);
                                break;
                            case "duration":
                                whereClause.Add($"cl.Duration {this.operatorsMap[filter.Operator]} @Duration");
                                parameters.Add("Duration", filter.Value);
                                break;
                            case "leademail":
                                whereClause.Add("cl.LeadEmail LIKE @LeadEmail");
                                parameters.Add("LeadEmail", $"%{filter.Value}%");
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

                if(request.sorting != null){
                    // Sorting to follow after where
            if (request.sorting != null)
            {
                switch (request.sorting.column.ToLower())
                {
                    case "time & date":
                        baseQuery += $" ORDER BY cl.CalledDate {(request.sorting.direction == "asc" ? "ASC" : "DESC")} ";
                        break;
                    default:
                        baseQuery += $"  ORDER BY cl.CalledDate DESC";
                        break;
                }
            }
                }

                // Add ORDER BY and pagination
                baseQuery += """
                    OFFSET (@PageNumber - 1) * @PageSize ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                """;


                var items = await connection.QueryAsync<SmartLeadsCalls>(baseQuery, parameters);
                var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

                var response = new TableResponse<SmartLeadsCalls>
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
            "UserCaller",
            "UserPhoneNumber",
            "LeadEmail",
            "ProspectName",
            "ProspectNumber",
            "CalledDate",
            "CallState",
            "Duration",
            "CallPurpose",
            "CallDisposition",
            "Notes",
            "CallTags",
            "AddedBy"
        };
        }
    }
}
