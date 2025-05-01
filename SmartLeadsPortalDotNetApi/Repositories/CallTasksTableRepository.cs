using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;
using System.Drawing;

namespace SmartLeadsPortalDotNetApi.Repositories;

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
                    slal.LeadId, 
                    sle.LeadEmail AS Email, 
                    slal.FirstName + ' ' + slal.LastName AS FullName, 
                    sle.SequenceNumber,
                    CASE 
                        WHEN slc.Name = 'Dondi (US/CA) Bot - Job Ads (manual)' 
                            OR  slc.Name = '(US/CA) Bot - Job Ads (full auto)' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Mountain Standard Time'
                        WHEN slc.Name = 'Dondi (AUS) Bot - Job Ads (manual)' 
                            OR  slc.Name = '(AUS) Bot - Job Ads (full auto)' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Australia Standard Time'
                        WHEN slc.Name = 'Dondi (UK) Bot - Job Ads (manual)' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
                        WHEN slc.Name = '(NZ) Bot - Job Ads (full auto)' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'
                        WHEN slc.Name = '(EU) Bot - Job Ads (full auto)' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
                    END AS LocalTime,
                    ABS(
                        DATEDIFF(
                            MINUTE, 
                            CAST(CASE 
                                    WHEN slc.Name = 'Dondi (US/CA) Bot - Job Ads (manual)' 
                                        OR slc.Name = '(US/CA) Bot - Job Ads (full auto)' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Mountain Standard Time'
                                    WHEN slc.Name = 'Dondi (AUS) Bot - Job Ads (manual)' 
                                        OR slc.Name = '(AUS) Bot - Job Ads (full auto)' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Australia Standard Time'
                                    WHEN slc.Name = 'Dondi (UK) Bot - Job Ads (manual)' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
                                    WHEN slc.Name = '(NZ) Bot - Job Ads (full auto)' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'
                                    WHEN slc.Name = '(EU) Bot - Job Ads (full auto)' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
                                END AS TIME),
                            CAST('09:00:00' AS TIME)
                        )
                    ) AS TimeDifferenceInMinutes,
                    slc.Name AS CampaignName, 
                    sle.EmailSubject AS SubjectName, 
                    sle.OpenCount, 
                    sle.ClickCount,
                    cs.Id AS CallStateId,
                    cs.StateName AS CallState,
                    us.EmployeeId,
                    us.FullName AS AssignedTo,
                    sle.Notes,
                    sle.Due
                FROM SmartLeadsEmailStatistics sle
                INNER JOIN SmartLeadAllLeads slal ON slal.Email = sle.LeadEmail
                INNER JOIN SmartLeadCampaigns slc ON slc.Id = slal.CampaignId
                LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
                LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
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
                INNER JOIN SmartLeadAllLeads slal ON slal.Email = sle.LeadEmail
                INNER JOIN SmartLeadCampaigns slc ON slc.Id = slal.CampaignId
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
                            whereClause.Add("slal.FirstName + ' ' + slal.LastName) LIKE @FullName");
                            parameters.Add("FullName", $"%{filter.Value}%");
                            break;
                        case "campaignname":
                            whereClause.Add("slc.Name LIKE @CampaignName");
                            parameters.Add("CampaignName", $"%{filter.Value}%");
                            break;
                        case "subjectname":
                            whereClause.Add("sle.EmailSubject LIKE @SubjectName");
                            parameters.Add("SubjectName", $"%{filter.Value}%");
                            break;
                        case "callstate":
                            whereClause.Add("cs.StateName LIKE @CallState");
                            parameters.Add("CallState", $"%{filter.Value}%");
                            break;
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

            // Sorting to follow after where
            if (request.sorting != null)
            {
                switch (request.sorting.column.ToLower())
                {
                    case "due":
                        baseQuery += $" ORDER BY sle.Due {(request.sorting.direction == "asc" ? "ASC" : "DESC")} ";
                        break;
                    case "priority":
                        baseQuery += $" ORDER BY sle.OpenCount {(request.sorting.direction == "asc" ? "ASC" : "DESC")} ";
                        break;
                    case "sequence":
                        baseQuery += $" ORDER BY TimeDifferenceInMinutes {(request.sorting.direction == "asc" ? "ASC" : "DESC")} , sle.SequenceNumber DESC   ";
                        break;
                    default:
                        baseQuery += $" ORDER BY sle.OpenCount DESC";
                        break;
                }
            }

            // Add ORDER BY and pagination
            baseQuery += """
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
             "SubjectName",
            "OpenCount",
            "ClickCount",
            "Due",
            "AssignedTo"
        };
    }

    public async Task<int> UpdateCallTasks(CallTasksUpdateParam request)
    {
        try
        {
            using (var connection = this.dbConnectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spUpdateCallTasks";
                var param = new DynamicParameters();
                param.Add("@guid", request.GuId);
                param.Add("@due", request.Due);
                param.Add("@assignto", request.AssignedTo);
                param.Add("@notes", request.Notes);

                int ret = await connection.ExecuteAsync(_proc, param);

                return ret;
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Database error: " + ex.Message);
        }
    }
    public async Task<int> RescheduleCallTasks(CallTasksUpdateParam request)
    {
        try
        {
            using (var connection = this.dbConnectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spRescheduleCallTasks";
                var param = new DynamicParameters();
                param.Add("@guid", request.GuId);
                param.Add("@due", request.Due);

                int ret = await connection.ExecuteAsync(_proc, param);

                return ret;
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Database error: " + ex.Message);
        }
    }
}