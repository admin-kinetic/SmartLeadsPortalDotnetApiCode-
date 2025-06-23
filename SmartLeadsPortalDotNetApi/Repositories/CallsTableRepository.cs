using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class CallsTableRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly Dictionary<string, string> operatorsMap = new Dictionary<string, string>
        {
            { "is", "=" },
        { "is not", "!=" },
        { "contains", "LIKE" },
        { "does not contains", "NOT LIKE" },
        { "contains data", "IS NOT NULL" },
        { "does not contains data", "IS NULL" },
        { "equal", "=" },
        { "not equal", "!=" },
        { "less than", "<" },
        { "less than equal", "<=" },
        { "greater than", ">" },
        { "greater than equal", ">=" },
        { "on", "=" },
        { "on or after", ">=" },
        { "on or before", "<=" },
        { "after", ">" },
        { "this week",  "{0} >= DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0) AND {0} < DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()) + 1, 0)"},
        { "last week",  "{0} >= DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()) - 1, 0) AND {0} < DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0)"},
        { "this month", "{0} >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) AND {0} < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, 0)" },
        { "last month", "{0} >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0) AND {0} < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)" },
        { "this year", "{0} >= DATEADD(YEAR, DATEDIFF(YEAR, 0, GETDATE()), 0) AND {0} < DATEADD(YEAR, DATEDIFF(YEAR, 0, GETDATE()) + 1, 0)" },
        { "last year", "{0} >= DATEADD(YEAR, DATEDIFF(YEAR, 0, GETDATE()) - 1, 0) AND {0} < DATEADD(YEAR, DATEDIFF(YEAR, 0, GETDATE()), 0)" },
        { "last x days", "{0} >= DATEADD(DAY, -{1}, CAST(GETDATE() AS DATE)) AND {0} < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))" },
        { "last x weeks", "{0} >= DATEADD(WEEK, -{1}, DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0) AND {0} < DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0)" },
        { "last x months", "{0} >= DATEADD(MONTH, -{1}, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)) AND {0} < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)" }
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
                    ob.AzureStorageCallRecordingLink AS RecordedLink,
                    ib.AzureStorageCallRecordingLink AS InboundRecordedLink,
                    cl.IsDeleted,
                    cl.CallDirectionId
                FROM [dbo].[Calls] cl
                LEFT JOIN CallPurpose cp ON cl.CallPurposeId = cp.Id
                LEFT JOIN CallDisposition cd ON cl.CallDispositionId = cd.Id
                LEFT JOIN CallState cs ON cl.CallStateId = cs.Id
                LEFT JOIN Tags ct ON cl.CallTagsId = ct.Id
                LEFT JOIN OutboundCalls ob ON cl.UniqueCallId = ob.UniqueCallId
                LEFT JOIN InboundCalls ib ON cl.UniqueCallId = ib.UniqueCallId
                WHERE (cl.IsDeleted IS NULL OR cl.IsDeleted = 0)
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
                LEFT JOIN InboundCalls ib ON cl.UniqueCallId = ib.UniqueCallId
                WHERE (cl.IsDeleted IS NULL OR cl.IsDeleted = 0)
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
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.UserCaller {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.UserCaller {this.operatorsMap[filter.Operator]} @UserCaller");
                            parameters.Add("UserCaller", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "userphonenumber":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.UserPhoneNumber {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.UserPhoneNumber {this.operatorsMap[filter.Operator]} @UserPhoneNumber");
                            parameters.Add("UserPhoneNumber", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "prospectname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.ProspectName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.ProspectName {this.operatorsMap[filter.Operator]} @ProspectName");
                            parameters.Add("ProspectName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "prospectnumber":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.ProspectNumber {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.ProspectNumber {this.operatorsMap[filter.Operator]} @ProspectNumber");
                            parameters.Add("ProspectNumber", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "callstate":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cs.StateName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cs.StateName {this.operatorsMap[filter.Operator]} @CallState");
                            parameters.Add("CallState", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "callpurpose":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cp.CallPurposeName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cp.CallPurposeName {this.operatorsMap[filter.Operator]} @CallPurpose");
                            parameters.Add("CallPurpose", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "calldisposition":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cd.CallDispositionName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cd.CallDispositionName {this.operatorsMap[filter.Operator]} @CallDisposition");
                            parameters.Add("CallDisposition", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "calltags":    
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"ct.TagName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"ct.TagName {this.operatorsMap[filter.Operator]} @CallTags");
                            parameters.Add("CallTags", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "notes":    
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.Notes {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.Notes {this.operatorsMap[filter.Operator]} @Notes");
                            parameters.Add("Notes", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "addedby":    
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.AddedBy {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.AddedBy {this.operatorsMap[filter.Operator]} @AddedBy");
                            parameters.Add("AddedBy", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "calleddate":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.CalledDate {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"CONVERT(DATE, cl.CalledDate) {this.operatorsMap[filter.Operator]} @CalledDate");
                            parameters.Add("CalledDate", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "duration":    
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.Duration {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.Duration {this.operatorsMap[filter.Operator]} @Duration");
                            parameters.Add("Duration", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "leademail":                            
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cl.LeadEmail {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cl.LeadEmail {this.operatorsMap[filter.Operator]} @LeadEmail");
                            parameters.Add("LeadEmail", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        default:
                            // For numeric fields or exact matches
                            whereClause.Add($"{filter.Column} {this.operatorsMap[filter.Operator]} @{filter.Column}");
                            parameters.Add(filter.Column, filter.Value);
                            break;
                    }
                }
            }

            // Add WHERE clause if needed
            if (whereClause.Count > 0)
            {
                var filterClause = " AND " + string.Join(" AND ", whereClause);
                baseQuery += filterClause;
                countQuery += filterClause;
            }

            if (request.sorting != null)
            {
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
