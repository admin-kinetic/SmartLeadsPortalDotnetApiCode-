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

    public CallTasksTableRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }
    public async Task<TableResponse<SmartLeadsCallTasks>> Find(TableRequest request, string employeeId, bool hasPagination = true)
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
                        WHEN slc.Name LIKE '%US/CA%' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Mountain Standard Time'
                        WHEN slc.Name LIKE '%AUS%' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Australia Standard Time'
                        WHEN slc.Name LIKE '%UK%' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
                        WHEN slc.Name LIKE '%NZ%' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'
                        WHEN slc.Name LIKE '%EU%' THEN 
                            SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
                    END AS LocalTime,
                    ABS(
                        DATEDIFF(
                            MINUTE, 
                            CAST(CASE 
                                    WHEN slc.Name LIKE '%US/CA%' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Mountain Standard Time'
                                    WHEN slc.Name LIKE '%AUS%' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Australia Standard Time'
                                    WHEN slc.Name LIKE '%UK%' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'GMT Standard Time'
                                    WHEN slc.Name LIKE '%NZ%' THEN 
                                        SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'
                                    WHEN slc.Name LIKE '%EU%' THEN 
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
                    sle.Due,
                    sle.IsDeleted,
                    ISNULL(cs_applied.CategoryName, 'Low') AS Category,
                    CASE 
                        WHEN ISNULL(cs_applied.CategoryName, 'Low') = 'Low' THEN 1
                        WHEN cs_applied.CategoryName = 'High' THEN 2
                        WHEN cs_applied.CategoryName = 'Urgent' THEN 3
                        ELSE 1
                    END AS SortOrder
                FROM SmartLeadsEmailStatistics sle
                INNER JOIN SmartLeadAllLeads slal ON slal.Email = sle.LeadEmail
                INNER JOIN SmartLeadCampaigns slc ON slc.Id = slal.CampaignId
                INNER JOIN SmartleadsAccountCampaigns ac ON ac.CampaignId = slc.id
                INNER JOIN SmartleadsAccountUsers au ON au.SmartleadsAccountId = ac.SmartleadsAccountId
                LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
                LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
                LEFT JOIN Calls c ON c.LeadEmail = sle.LeadEmail
                OUTER APPLY (
                    SELECT TOP 1 cs.CategoryName
                    FROM CategorySettings cs
                    WHERE sle.OpenCount >= cs.OpenCount OR sle.ClickCount >= cs.ClickCount
                    ORDER BY 
                        CASE 
                            WHEN sle.OpenCount >= cs.OpenCount AND sle.ClickCount >= cs.ClickCount THEN cs.OpenCount + cs.ClickCount
                            WHEN sle.OpenCount >= cs.OpenCount THEN cs.OpenCount
                            ELSE cs.ClickCount
                        END DESC
                    ) cs_applied
                WHERE (sle.IsDeleted IS NULL OR sle.IsDeleted = 0) AND au.EmployeeId = @EmployeeId 
            """;

            var countQuery = """ 
                SELECT
                    count(sle.Id) as Total
                FROM SmartLeadsEmailStatistics sle
                INNER JOIN SmartLeadAllLeads slal ON slal.Email = sle.LeadEmail
                INNER JOIN SmartLeadCampaigns slc ON slc.Id = slal.CampaignId
                INNER JOIN SmartleadsAccountCampaigns ac ON ac.CampaignId = slc.id
                INNER JOIN SmartleadsAccountUsers au ON au.SmartleadsAccountId = ac.SmartleadsAccountId
                LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
                LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
                LEFT JOIN Calls c ON c.LeadEmail = sle.LeadEmail
                OUTER APPLY (
                    SELECT TOP 1 cs.CategoryName
                    FROM CategorySettings cs
                    WHERE sle.OpenCount >= cs.OpenCount OR sle.ClickCount >= cs.ClickCount
                    ORDER BY 
                        CASE 
                            WHEN sle.OpenCount >= cs.OpenCount AND sle.ClickCount >= cs.ClickCount THEN cs.OpenCount + cs.ClickCount
                            WHEN sle.OpenCount >= cs.OpenCount THEN cs.OpenCount
                            ELSE cs.ClickCount
                        END DESC
                    ) cs_applied
                WHERE (sle.IsDeleted IS NULL OR sle.IsDeleted = 0) AND au.EmployeeId = @EmployeeId 
            """;

            // Build WHERE clause if filters exist
            var whereClause = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("PageNumber", request.paginator.page);
            parameters.Add("PageSize", request.paginator.pageSize);
            parameters.Add("EmployeeId", employeeId);

            if (request.filters != null && request.filters.Count > 0)
            {
                foreach (var filter in request.filters)
                {
                    // Handle different column types appropriately
                    switch (filter.Column.ToLower())
                    {
                        case "email":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.LeadEmail {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.LeadEmail {this.operatorsMap[filter.Operator]} @Email");
                            parameters.Add("Email", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "fullname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.FirstName + ' ' + slal.LastName) {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.FirstName + ' ' + slal.LastName) {this.operatorsMap[filter.Operator]} @FullName");
                            parameters.Add("FullName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "assignedto":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"us.FullName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"us.FullName {this.operatorsMap[filter.Operator]} @AssignedTo");
                            parameters.Add("AssignedTo", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "campaignname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slc.Name {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slc.Name {this.operatorsMap[filter.Operator]} @CampaignName");
                            parameters.Add("CampaignName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "subjectname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.EmailSubject {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.EmailSubject {this.operatorsMap[filter.Operator]} @SubjectName");
                            parameters.Add("SubjectName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
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
                        case "opencount":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.OpenCount {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.OpenCount {this.operatorsMap[filter.Operator]} @OpenCount");
                            parameters.Add("OpenCount", filter.Value);
                            break;
                        case "clickcount":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.ClickCount {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.ClickCount {this.operatorsMap[filter.Operator]} @ClickCount");
                            parameters.Add("ClickCount", filter.Value);
                            break;
                        case "priority":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cs_applied.CategoryName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cs_applied.CategoryName {this.operatorsMap[filter.Operator]} @Priority");
                            parameters.Add("Priority", $"{filter.Value}");
                            break;
                        case "sequencenumber":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.SequenceNumber {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.SequenceNumber {this.operatorsMap[filter.Operator]} @SequenceNumber");
                            parameters.Add("SequenceNumber", $"{filter.Value}");
                            break;
                        case "due":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.Due {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"CONVERT(DATE, sle.Due) {this.operatorsMap[filter.Operator]} @Due");
                            parameters.Add("Due", $"{filter.Value}");
                            break;
                        case "bdr":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.Bdr {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.Bdr {this.operatorsMap[filter.Operator]} @Bdr");
                            parameters.Add("Bdr", $"{filter.Value}");
                            break;
                        case "country":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.Location {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.Location {this.operatorsMap[filter.Operator]} @Country");
                            parameters.Add("Country", $"{filter.Value}");
                            break;
                        case "companyname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.CompanyName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.CompanyName {this.operatorsMap[filter.Operator]} @CompanyName");
                            parameters.Add("CompanyName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "lastemailedon":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.SentTime {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            
                            if (filter.Operator.Contains("this", StringComparison.OrdinalIgnoreCase)
                                || filter.Operator.Contains("last", StringComparison.OrdinalIgnoreCase))
                            {
                                var operatorValue = this.operatorsMap[filter.Operator].Replace("{0}", "sle.SentTime");
                                whereClause.Add($"{operatorValue}");
                                break;
                            }

                            whereClause.Add($"CONVERT(DATE, sle.SentTime) {this.operatorsMap[filter.Operator]} @SentTime");
                            parameters.Add("SentTime", $"{filter.Value}");
                            break;
                         case "lastcalledon":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"c.CalledDate {this.operatorsMap[filter.Operator]}");
                                break; 
                            }

                            if(filter.Operator.Contains("this", StringComparison.OrdinalIgnoreCase) 
                                || filter.Operator.Contains("last", StringComparison.OrdinalIgnoreCase))
                            {
                                var operatorValue = this.operatorsMap[filter.Operator].Replace("{0}", "c.CalledDate");
                                whereClause.Add($"{operatorValue}");
                                break; 
                            }


                            whereClause.Add($"CONVERT(DATE, c.CalledDate) {this.operatorsMap[filter.Operator]} @CalledDate");
                            parameters.Add("CalledDate", $"{filter.Value}");
                            break;
                        // Add more cases for other filterable columns
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

            // Sorting to follow after where
            if (request.sorting != null)
            {
                switch (request.sorting.column.ToLower())
                {
                    case "due":
                        baseQuery += $" ORDER BY sle.Due {(request.sorting.direction == "asc" ? "ASC" : "DESC")} ";
                        break;
                    case "priority":
                        baseQuery += $" ORDER BY SortOrder {(request.sorting.direction == "asc" ? "ASC" : "DESC")} ";
                        break;
                    case "sequence":
                        baseQuery += $" ORDER BY TimeDifferenceInMinutes {(request.sorting.direction == "asc" ? "ASC" : "DESC")} , sle.SequenceNumber DESC   ";
                        break;
                    default:
                        baseQuery += $" ORDER BY SortOrder DESC";
                        break;
                }
            }

            // Add ORDER BY and pagination
            if(hasPagination)
            {
                baseQuery += """
                    OFFSET (@PageNumber - 1) * @PageSize ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                """;
            }
            


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

    public async Task<TableResponse<SmartLeadsCallTasksExport>> Export(TableRequest request, string employeeId)
    {
        using (var connection = dbConnectionFactory.GetSqlConnection())
        {
            var baseQuery = """ 
                SELECT
                    sle.LeadEmail AS Email, 
                    slal.FirstName,
                    slal.LastName,
                    slal.PhoneNumber,
                    slal.CompanyName,
                    slal.Location as [Country],
                    slec.ContactSource as [Source],
                    CASE 
                        WHEN CHARINDEX('re. your ', sle.EmailSubject) > 0 
                            AND CHARINDEX(' ad on', sle.EmailSubject) > CHARINDEX('re. your ', sle.EmailSubject)
                        THEN TRIM(
                            SUBSTRING(
                                sle.EmailSubject,
                                CHARINDEX('re. your ', sle.EmailSubject) + LEN('re. your '),
                                CHARINDEX(' ad on', sle.EmailSubject) - (CHARINDEX('re. your ', sle.EmailSubject) + LEN('re. your '))
                            )
                        )
                        ELSE NULL
                    END AS RoleAdvertised,
                    slal.CreatedAt AS [ExportedDate],
                    CASE WHEN sle.SentTime IS NOT NULL THEN 1 ELSE 0 END AS HasReply,
                    ISNULL(cs_applied.CategoryName, 'Low') AS Category,
                    slal.bdr,
                    us.FullName AS AssignedTo,
                    slc.Name AS [EmailCampaign], 
                    slal.createdBy as [LeadGen],
                    slal.qaBy,
                    sle.OpenCount, 
                    sle.ClickCount
                FROM SmartLeadsEmailStatistics sle
                INNER JOIN SmartLeadAllLeads slal ON slal.Email = sle.LeadEmail
                INNER JOIN SmartLeadCampaigns slc ON slc.Id = slal.CampaignId
                INNER JOIN SmartleadsAccountCampaigns ac ON ac.CampaignId = slc.id
                INNER JOIN SmartleadsAccountUsers au ON au.SmartleadsAccountId = ac.SmartleadsAccountId
                LEFT JOIN CallState cs ON sle.CallStateId = cs.Id
                LEFT JOIN Users us ON sle.AssignedTo = us.EmployeeId
                LEFT JOIN Calls c ON c.LeadEmail = sle.LeadEmail
                LEFT JOIN SmartLeadsExportedContacts slec ON slec.Email = sle.LeadEmail
                OUTER APPLY (
                    SELECT TOP 1 cs.CategoryName
                    FROM CategorySettings cs
                    WHERE sle.OpenCount >= cs.OpenCount OR sle.ClickCount >= cs.ClickCount
                    ORDER BY 
                        CASE 
                            WHEN sle.OpenCount >= cs.OpenCount AND sle.ClickCount >= cs.ClickCount THEN cs.OpenCount + cs.ClickCount
                            WHEN sle.OpenCount >= cs.OpenCount THEN cs.OpenCount
                            ELSE cs.ClickCount
                        END DESC
                    ) cs_applied
                WHERE (sle.IsDeleted IS NULL OR sle.IsDeleted = 0) AND au.EmployeeId = @EmployeeId 
            """;

            // Build WHERE clause if filters exist
            var whereClause = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("PageNumber", request.paginator.page);
            parameters.Add("PageSize", request.paginator.pageSize);
            parameters.Add("EmployeeId", employeeId);

            if (request.filters != null && request.filters.Count > 0)
            {
                foreach (var filter in request.filters)
                {
                    // Handle different column types appropriately
                    switch (filter.Column.ToLower())
                    {
                        case "email":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.LeadEmail {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.LeadEmail {this.operatorsMap[filter.Operator]} @Email");
                            parameters.Add("Email", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "fullname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.FirstName + ' ' + slal.LastName) {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.FirstName + ' ' + slal.LastName) {this.operatorsMap[filter.Operator]} @FullName");
                            parameters.Add("FullName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "assignedto":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"us.FullName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"us.FullName {this.operatorsMap[filter.Operator]} @AssignedTo");
                            parameters.Add("AssignedTo", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "campaignname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slc.Name {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slc.Name {this.operatorsMap[filter.Operator]} @CampaignName");
                            parameters.Add("CampaignName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "subjectname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.EmailSubject {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.EmailSubject {this.operatorsMap[filter.Operator]} @SubjectName");
                            parameters.Add("SubjectName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
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
                        case "opencount":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.OpenCount {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.OpenCount {this.operatorsMap[filter.Operator]} @OpenCount");
                            parameters.Add("OpenCount", filter.Value);
                            break;
                        case "clickcount":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.ClickCount {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.ClickCount {this.operatorsMap[filter.Operator]} @ClickCount");
                            parameters.Add("ClickCount", filter.Value);
                            break;
                        case "priority":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"cs_applied.CategoryName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"cs_applied.CategoryName {this.operatorsMap[filter.Operator]} @Priority");
                            parameters.Add("Priority", $"{filter.Value}");
                            break;
                        case "sequencenumber":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.SequenceNumber {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"sle.SequenceNumber {this.operatorsMap[filter.Operator]} @SequenceNumber");
                            parameters.Add("SequenceNumber", $"{filter.Value}");
                            break;
                        case "due":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.Due {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"CONVERT(DATE, sle.Due) {this.operatorsMap[filter.Operator]} @Due");
                            parameters.Add("Due", $"{filter.Value}");
                            break;
                        case "bdr":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.Bdr {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.Bdr {this.operatorsMap[filter.Operator]} @Bdr");
                            parameters.Add("Bdr", $"{filter.Value}");
                            break;
                        case "country":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.Location {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.Location {this.operatorsMap[filter.Operator]} @Country");
                            parameters.Add("Country", $"{filter.Value}");
                            break;
                        case "companyname":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"slal.CompanyName {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            whereClause.Add($"slal.CompanyName {this.operatorsMap[filter.Operator]} @CompanyName");
                            parameters.Add("CompanyName", this.operatorsMap[filter.Operator].Contains("like", StringComparison.OrdinalIgnoreCase) ? $"%{filter.Value}%" : $"{filter.Value}");
                            break;
                        case "lastemailedon":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sle.SentTime {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            
                            if (filter.Operator.Contains("this", StringComparison.OrdinalIgnoreCase)
                                || filter.Operator.Contains("last", StringComparison.OrdinalIgnoreCase))
                            {
                                var operatorValue = this.operatorsMap[filter.Operator].Replace("{0}", "sle.SentTime");
                                whereClause.Add($"{operatorValue}");
                                break;
                            }

                            whereClause.Add($"CONVERT(DATE, sle.SentTime) {this.operatorsMap[filter.Operator]} @SentTime");
                            parameters.Add("SentTime", $"{filter.Value}");
                            break;
                         case "lastcalledon":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"c.CalledDate {this.operatorsMap[filter.Operator]}");
                                break; 
                            }

                            if(filter.Operator.Contains("this", StringComparison.OrdinalIgnoreCase) 
                                || filter.Operator.Contains("last", StringComparison.OrdinalIgnoreCase))
                            {
                                var operatorValue = this.operatorsMap[filter.Operator].Replace("{0}", "c.CalledDate");
                                whereClause.Add($"{operatorValue}");
                                break; 
                            }


                            whereClause.Add($"CONVERT(DATE, c.CalledDate) {this.operatorsMap[filter.Operator]} @CalledDate");
                            parameters.Add("CalledDate", $"{filter.Value}");
                            break;
                        // Add more cases for other filterable columns
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
                        baseQuery += $" ORDER BY SortOrder {(request.sorting.direction == "asc" ? "ASC" : "DESC")} ";
                        break;
                    case "sequence":
                        baseQuery += $" ORDER BY TimeDifferenceInMinutes {(request.sorting.direction == "asc" ? "ASC" : "DESC")} , sle.SequenceNumber DESC   ";
                        break;
                    default:
                        baseQuery += $" ORDER BY SortOrder DESC";
                        break;
                }
            }

            var items = await connection.QueryAsync<SmartLeadsCallTasksExport>(baseQuery, parameters);

            var response = new TableResponse<SmartLeadsCallTasksExport>
            {
                Items = items.ToList(),
                Total = 0
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
            "Priority",
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
    public async Task<int> DeleteCallTasks(CallTasksUpdateParam request)
    {
        try
        {
            using (var connection = this.dbConnectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spDeleteCallTasks";
                var param = new DynamicParameters();
                param.Add("@guid", request.GuId);

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