using Dapper;
using Microsoft.Graph.Models.Security;
using OfficeOpenXml.Drawing.Chart;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class SmartLeadsRepository : SQLDBService
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
            { "last x days", "{0} >= DATEADD(DAY, -1, CAST(GETDATE() AS DATE)) AND {0} < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))" },
            { "last x weeks", "{0} >= DATEADD(WEEK, -1, DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0)) AND {0} < DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0)" },
            { "last x months", "{0} >= DATEADD(MONTH, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)) AND {0} < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)" }
        };

        public SmartLeadsRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<SmartLeadsCallTasksResponseModel<SmartLeadsCallTasks>> GetAllSmartLeadsCallTaskList(SmartLeadsCallTasksRequest model, string employeeId)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                IEnumerable<SmartLeadsCallTasks> list = new List<SmartLeadsCallTasks>();

                _proc = "sm_spGetSmartLeadsCallTasks";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@EmployeeId", employeeId);

                list = await SqlMapper.QueryAsync<SmartLeadsCallTasks>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetSmartLeadsCallTasksCount";
                var countParam = new
                {
                    employeeId
                };

                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, countParam, commandType: CommandType.StoredProcedure);

                return new SmartLeadsCallTasksResponseModel<SmartLeadsCallTasks>
                {
                    Items = list.ToList(),
                    Total = count
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Dispose();
            }
        }
        public async Task<SmartLeadsProspectDetails?> GetSmartLeadsProspectDetails(ProspectModelParam request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spLeadProspectCallDetails";
                    var param = new DynamicParameters();
                    param.Add("@email", request.Email);
                    SmartLeadsProspectDetails? result = await connection.QuerySingleOrDefaultAsync<SmartLeadsProspectDetails>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<SmartLeadsExportedContactLeadGen>> GetAllLeadGenExportedLeads(SmartLeadRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var param = new DynamicParameters();
                    IEnumerable<SmartLeadsExportedContactLeadGen> list = new List<SmartLeadsExportedContactLeadGen>();

                    if (request.EmailAddress == null || request.EmailAddress == "" || request.EmailAddress == "null")
                    {
                        request.EmailAddress = "";
                    }

                    string baseQuery = """
                        SELECT sec.Id, sal.CreatedAt AS ExportedDate, sec.Email, sec.ContactSource, ses.SequenceNumber, ses.ReplyTime, sec.HasReviewed, ses.SentTime
                        FROM [dbo].[SmartLeadsExportedContacts] sec
                        LEFT JOIN [dbo].[SmartLeadAllLeads] sal ON sec.Email = sal.Email
                        LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sec.Email = ses.LeadEmail
                        WHERE sal.BDR ='Steph' AND sal.CreatedBy <> 'Bots'
                    """;

                    baseQuery = ComposeWhereConditions(baseQuery, request);

                    // Add ORDER BY clause and pagination
                    baseQuery += """
                        ORDER BY sal.CreatedAt DESC
                        OFFSET (@Page - 1) * @PageSize ROWS
                        FETCH NEXT @PageSize ROWS ONLY
                    """;

                    param.Add("@Page", request.Page);
                    param.Add("@PageSize", request.PageSize);
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@isValid", request.HasReview);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@leadGen", request.LeadGen);
                    param.Add("@qaBy", request.QaBy);

                    list = await connection.QueryAsync<SmartLeadsExportedContactLeadGen>(baseQuery, param);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int?> GetAllLeadGenExportedLeadsCount(SmartLeadRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var param = new DynamicParameters();

                    if (string.IsNullOrEmpty(request.EmailAddress) || request.EmailAddress == "null")
                    {
                        request.EmailAddress = "";
                    }

                    // string _proc = "sm_spGetLeadGenExportedLeadsPaginatedCount";

                    string countQuery = """
                        SELECT 
                            COUNT(sec.Id) AS TotalCount
                        FROM [dbo].[SmartLeadsExportedContacts] sec
                        LEFT JOIN [dbo].[SmartLeadAllLeads] sal ON sec.Email = sal.Email
                        LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sec.Email = ses.LeadEmail
                        WHERE sal.BDR ='Steph' AND sal.CreatedBy <> 'Bots'
                    """;

                    countQuery = ComposeWhereConditions(countQuery, request);

                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@isValid", request.HasReview);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@leadGen", request.LeadGen);
                    param.Add("@qaBy", request.QaBy);

                    var countResult = await connection.QueryFirstOrDefaultAsync<SmartLeadsExportedContactLeadGenCount?>(countQuery, param);

                    return countResult?.TotalCount;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static string ComposeWhereConditions(string baseQuery, SmartLeadRequest request)
        {
            var whereClause = new List<string>();

            if (!string.IsNullOrEmpty(request.EmailAddress) && request.EmailAddress != "null")
            {
                whereClause.Add("sal.Email = @email");
            }

            // Determine which date field to use based on HasReply filter
            string dateField;

            if (request.HasReply.HasValue)
            {
                dateField = request.HasReply.Value ? "ses.ReplyTime" : "ses.SentTime";
            }
            else
            {
                dateField = "sal.CreatedAt";
            }

            // Apply date filters using the determined field
            if (request.ExportedDateFrom.HasValue)
            {
                whereClause.Add($"CONVERT(DATE, {dateField}) >= @startDate");
            }

            if (request.ExportedDateTo.HasValue)
            {
                whereClause.Add($"CONVERT(DATE, {dateField}) <= @endDate");
            }

            if (!string.IsNullOrEmpty(request.Bdr))
            {
                whereClause.Add("sal.BDR = @bdr");
            }

            if (!string.IsNullOrEmpty(request.LeadGen))
            {
                whereClause.Add("sal.CreatedBy = @leadGen");
            }

            if (!string.IsNullOrEmpty(request.QaBy))
            {
                whereClause.Add("sal.QABy = @qaBy");
            }


            // Add WHERE clause if needed
            if (whereClause.Count > 0)
            {
                var filterClause = $"""
                                AND {string.Join(" AND ", whereClause)}
                            """;
                baseQuery += filterClause;
            }

            return baseQuery;
        }

        public async Task<IEnumerable<SmartLeadsExportedLeadsEmailed>> GetAllExportedLeadsEmailed(SmartLeadEmailedRequest request)
        {
            try
            {
                await using (var connection = await this.dbConnectionFactory.GetSqlConnectionAsync())
                {
                    var param = new DynamicParameters();
                    IEnumerable<SmartLeadsExportedLeadsEmailed> list = new List<SmartLeadsExportedLeadsEmailed>();

                    var baseQuery = """
                        SELECT
                            sal.LeadId AS Id,
                            sal.FirstName + ' ' + sal.LastName AS FullName,
                            sal.CompanyName,
                            sal.Email,
                            sal.PhoneNumber,
                            MAX(sal.CreatedAt) AS ExportedDate,
                            MAX(ses.SequenceNumber) AS SequenceNumber,
                            MAX(ses.ReplyTime) AS ReplyTime,
                            MIN(ses.SentTime) AS SentTime, 
                            sal.[Location] AS Country
                        FROM [dbo].[SmartLeadAllLeads] sal
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                        
                    """;

                    baseQuery = ComposeWhereConditions(baseQuery, request);

                    // Add ORDER BY clause
                    baseQuery += """
                        GROUP BY 
                            sal.LeadId,
                            sal.FirstName,
                            sal.LastName,
                            sal.CompanyName,
                            sal.Email,
                            sal.PhoneNumber,
                            sal.[Location]
                    """;

                    // Add ORDER BY clause
                    var wrappedBaseQuery = $"""
                        Select * From 
                        (
                            {baseQuery}
                        )  AS SQ
                        ORDER BY SQ.ExportedDate Desc
                        OFFSET (@Page - 1) * @PageSize ROWS
                        FETCH NEXT @PageSize ROWS ONLY
                    """;


                    param.Add("@Page", request.Page);
                    param.Add("@PageSize", request.PageSize);
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@startDate", request.FromDate);
                    param.Add("@endDate", request.ToDate);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@leadGen", request.LeadGen);
                    param.Add("@qaBy", request.QaBy);

                    list = await connection.QueryAsync<SmartLeadsExportedLeadsEmailed>(wrappedBaseQuery, param);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public async Task<int?> GetAllExportedLeadsEmailedCount(SmartLeadEmailedRequest request)
        {
            try
            {
                await using (var connection = await this.dbConnectionFactory.GetSqlConnectionAsync())
                {
                    var param = new DynamicParameters();

                    var countQuery = """
                        SELECT
                            COUNT(DISTINCT sal.Email) AS TotalCount
                        FROM [dbo].[SmartLeadAllLeads] sal
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                        
                    """;

                    countQuery = ComposeWhereConditions(countQuery, request);

                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@startDate", request.FromDate);
                    param.Add("@endDate", request.ToDate);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@leadGen", request.LeadGen);
                    param.Add("@qaBy", request.QaBy);

                    var countResult = await connection.QueryFirstOrDefaultAsync<SmartLeadsExportedContactLeadGenCount?>(countQuery, param);

                    return countResult?.TotalCount;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static string ComposeWhereConditions(string baseQuery, SmartLeadEmailedRequest request)
        {

            var whereClause = new List<string>();

            if (!string.IsNullOrEmpty(request.EmailAddress) && request.EmailAddress != "null")
            {
                whereClause.Add("sal.Email = @email");
            }

            // Determine which date field to use based on HasReply filter
            string dateField;
            if (request.Category == "reply-email" || request.HasReply.HasValue)
            {
                dateField = "ses.ReplyTime";
            }
            else if (request.Category == "email-error" || request.Category == "out-of-office" || request.Category == "incorrect-contact" || request.Category == "positive-response" || request.Category == "open-email")
            {
                dateField = "ses.SentTime";
            }
            // else if(request.Category == "positive-response")
            // {
            //     dateField = "sal.OpenTime";
            // }
            else
            {
                if (request.HasReply.HasValue)
                {
                    dateField = request.HasReply.Value ? "ses.ReplyTime" : "ses.SentTime";
                }
                else
                {
                    dateField = "sal.CreatedAt";
                }
            }

            // Apply date filters using the determined field
            if (request.FromDate.HasValue)
            {
                whereClause.Add($"CONVERT(DATE, {dateField}) >= @startDate");
            }

            if (request.ToDate.HasValue)
            {
                whereClause.Add($"CONVERT(DATE, {dateField}) <= @endDate");
            }

            if (!string.IsNullOrEmpty(request.Bdr))
            {
                whereClause.Add("sal.BDR = @bdr");
            }

            if (!string.IsNullOrEmpty(request.LeadGen))
            {
                whereClause.Add("sal.CreatedBy = @leadGen");
            }

            if (!string.IsNullOrEmpty(request.QaBy))
            {
                whereClause.Add("sal.QABy = @qaBy");
            }

            //Condition for Category
            if (!string.IsNullOrEmpty(request.Category) && request.Category == "positive-response")
            {
                whereClause.Add("(sal.SmartleadCategory = 'Information Request' OR sal.SmartleadCategory = 'Interested' OR sal.SmartleadCategory = 'Meeting Request')");
            }

            if (!string.IsNullOrEmpty(request.Category) && request.Category == "email-error")
            {
                whereClause.Add("(sal.SmartleadCategory = 'Bounced' OR sal.SmartleadCategory = 'Sender Originated Bounce')");
            }

            if (!string.IsNullOrEmpty(request.Category) && request.Category == "out-of-office")
            {
                whereClause.Add("sal.SmartleadCategory = 'Out Of Office'");
            }

            if (!string.IsNullOrEmpty(request.Category) && request.Category == "incorrect-contact")
            {
                whereClause.Add("sal.SmartleadCategory = 'Wrong Person'");
            }

            if (!string.IsNullOrEmpty(request.Category) && request.Category == "open-email")
            {
                whereClause.Add("ses.OpenTime IS NOT NULL OR ses.OpenTime <> ''");
            }

            if (request.CampaignType.HasValue && request.CampaignType.Value == 1)
            {
                whereClause.Add("sal.CreatedBy <> 'Bots' AND sal.BDR <> 'Steph'");
            }

            if (request.CampaignType.HasValue && request.CampaignType.Value == 2)
            {
                whereClause.Add("sal.CreatedBy = 'Bots' AND sal.BDR = 'Steph'");
            }

            if (request.CampaignType.HasValue && request.CampaignType.Value == 3)
            {
                whereClause.Add("sal.CreatedBy <> 'Bots' AND sal.BDR = 'Steph'");
            }

            // Add WHERE clause if needed
            if (whereClause.Count > 0)
            {
                var filterClause = $"""
                                WHERE {string.Join(" AND ", whereClause)}
                            """;
                baseQuery += filterClause;
            }

            return baseQuery;
        }

        public async Task<int> UpdateLeadsEmailDetails(SmartLeadsEmailedDetailsRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spUpdateLeadsEmailedDetails";
                    var param = new DynamicParameters();
                    param.Add("@email", request.Email);
                    param.Add("@phone", request.PhoneNumber);
                    param.Add("@country", request.Country);

                    int ret = await connection.ExecuteAsync(_proc, param, commandType: CommandType.StoredProcedure);

                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<LeadsDetailsModel?> GetLeadsProspectDetails(ProspectModelParam request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetLeadProspectDetails";
                    var param = new DynamicParameters();
                    param.Add("@email", request.Email);
                    LeadsDetailsModel? result = await connection.QuerySingleOrDefaultAsync<LeadsDetailsModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<SmartLeadsCallTasksExport>> GetAllExportedLeadsEmailedCsvExport(SmartLeadEmailedRequest request)
        {
            try
            {
                await using (var connection = await this.dbConnectionFactory.GetSqlConnectionAsync())
                {
                    var param = new DynamicParameters();
                    IEnumerable<SmartLeadsCallTasksExport> list = new List<SmartLeadsCallTasksExport>();

                    // if (request.EmailAddress == null || request.EmailAddress == "" || request.EmailAddress == "null")
                    // {
                    //     request.EmailAddress = "";
                    // }

                    // string _proc = "sm_spGetLeadsExportCsv";


                    var baseQuery = """
                        SELECT sal.Email, 
                            sal.PhoneNumber, 
                            sal.FirstName,  
                            sal.LastName, 
                            sal.CompanyName, 
                            sal.[Location] AS Country,
                            CASE 
                                WHEN CHARINDEX('re. your ', ses.EmailSubject) > 0 
                                    AND CHARINDEX(' ad on', ses.EmailSubject) > CHARINDEX('re. your ', ses.EmailSubject)
                                THEN TRIM(
                                    SUBSTRING(
                                        ses.EmailSubject,
                                        CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '),
                                        CHARINDEX(' ad on', ses.EmailSubject) - (CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '))
                                    )
                                )
                                ELSE NULL
                            END AS RoleAdvertised,
                            sec.ContactSource AS [Source],
                            @startDate AS FromDateExported,
                            @endDate AS ToDateExported,
                            CASE 
                                WHEN ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> '' THEN 1
                                ELSE 0
                            END AS HasReply,
                            sal.CreatedAt AS ExportedDate, 
                            sal.SmartleadCategory AS Category,
                            sal.BDR as Bdr,
                            sal.CreatedBy AS AssignedTo,
                            slc.[Name] AS EmailCampaign,
                            sal.CreatedBy AS LeadGen,
                            sal.QABy AS QadBy,
                            ses.OpenCount,
                            ses.ClickCount
                        FROM [dbo].[SmartLeadAllLeads] sal
                            INNER JOIN [dbo].[SmartLeadCampaigns] slc ON sal.CampaignId = slc.Id
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                            LEFT JOIN [dbo].[SmartLeadsExportedContacts] sec ON sal.Email = sec.Email
                        
                    """;

                    var whereClause = new List<string>();

                    if (!string.IsNullOrEmpty(request.EmailAddress) && request.EmailAddress != "null")
                    {
                        whereClause.Add("sal.Email = @email");
                    }

                    if (request.HasReply.HasValue)
                    {
                        whereClause.Add("(ses.ReplyTime IS NOT NULL OR ses.ReplyTime <> '')");
                    }

                    // Determine which date field to use based on HasReply filter
                    string dateField;
                    if (request.HasReply.HasValue)
                    {
                        dateField = request.HasReply.Value ? "ses.ReplyTime" : "ses.SentTime";
                    }
                    else
                    {
                        dateField = "sal.CreatedAt";
                    }

                    // Apply date filters using the determined field
                    if (request.FromDate.HasValue)
                    {
                        whereClause.Add($"CONVERT(DATE, {dateField}) >= @startDate");
                    }

                    if (request.ToDate.HasValue)
                    {
                        whereClause.Add($"CONVERT(DATE, {dateField}) <= @endDate");
                    }

                    if (!string.IsNullOrEmpty(request.Bdr))
                    {
                        whereClause.Add("sal.BDR = @bdr");
                    }

                    if (!string.IsNullOrEmpty(request.LeadGen))
                    {
                        whereClause.Add("sal.LeadGen = @leadGen");
                    }

                    if (!string.IsNullOrEmpty(request.QaBy))
                    {
                        whereClause.Add("sal.QABy = @qaBy");
                    }

                    // Add WHERE clause if needed
                    if (whereClause.Count > 0)
                    {
                        var filterClause = $"""
                                WHERE {string.Join(" AND ", whereClause)}
                            """;
                        baseQuery += filterClause;
                    }

                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@startDate", request.FromDate);
                    param.Add("@endDate", request.ToDate);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@leadGen", request.LeadGen);
                    param.Add("@qaBy", request.QaBy);

                    list = await connection.QueryAsync<SmartLeadsCallTasksExport>(baseQuery, param);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<TableResponse<SmartLeadsExportedLeadsEmailed>> Find(TableRequest request)
        {
            try
            {
                await using (var connection = await this.dbConnectionFactory.GetSqlConnectionAsync())
                {
                    var param = new DynamicParameters();

                    var baseQuery = """
                        SELECT
                            sal.LeadId AS Id,
                            sal.FirstName + ' ' + sal.LastName AS FullName,
                            sal.CompanyName,
                            sal.Email,
                            sal.PhoneNumber,
                            MAX(sal.CreatedAt) AS ExportedDate,
                            MAX(ses.SequenceNumber) AS SequenceNumber,
                            MAX(ses.ReplyTime) AS ReplyTime,
                            MIN(ses.SentTime) AS SentTime, 
                            sal.[Location] AS Country
                        FROM [dbo].[SmartLeadAllLeads] sal
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                        
                    """;

                    var countQuery = """
                        SELECT COUNT(DISTINCT sal.Email) AS TotalCount
                        FROM [dbo].[SmartLeadAllLeads] sal
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                    """;
                    countQuery = ComposeWhereConditionsForFind(countQuery, request, param);

                    baseQuery = ComposeWhereConditionsForFind(baseQuery, request, param);

                    // Add ORDER BY clause
                    baseQuery += """
                        GROUP BY 
                            sal.LeadId,
                            sal.FirstName,
                            sal.LastName,
                            sal.CompanyName,
                            sal.Email,
                            sal.PhoneNumber,
                            sal.[Location]
                    """;

                    // Add ORDER BY clause
                    var wrappedBaseQuery = $"""
                        Select * From 
                        (
                            {baseQuery}
                        )  AS SQ
                        ORDER BY SQ.ExportedDate Desc
                        OFFSET (@Page - 1) * @PageSize ROWS
                        FETCH NEXT @PageSize ROWS ONLY
                    """;


                    param.Add("@Page", request.paginator.page);
                    param.Add("@PageSize", request.paginator.pageSize);

                    var items = await connection.QueryAsync<SmartLeadsExportedLeadsEmailed>(wrappedBaseQuery, param);

                    var totalCount = request.paginator.total;
                    if (request.paginator.page == 1)
                    {
                        totalCount = await connection.QueryFirstOrDefaultAsync<int>(countQuery, param);
                    }

                    var response = new TableResponse<SmartLeadsExportedLeadsEmailed>
                    {
                        Items = items.ToList(),
                        Total = totalCount
                    };
                    return response;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<SmartLeadsCallTasksExport>> Export(TableRequest request)
        {
            try
            {
                await using (var connection = await this.dbConnectionFactory.GetSqlConnectionAsync())
                {
                    var param = new DynamicParameters();
                    param.Add("@startDate", "");
                    param.Add("@endDate", "");

                    ///hard limit to 2000 rows for export
                    var baseQuery = """
                        SELECT TOP 2000 sal.Email, 
                            sal.PhoneNumber, 
                            sal.FirstName,  
                            sal.LastName, 
                            sal.CompanyName, 
                            sal.[Location] AS Country,
                            CASE 
                                WHEN CHARINDEX('re. your ', ses.EmailSubject) > 0 
                                    AND CHARINDEX(' ad on', ses.EmailSubject) > CHARINDEX('re. your ', ses.EmailSubject)
                                THEN TRIM(
                                    SUBSTRING(
                                        ses.EmailSubject,
                                        CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '),
                                        CHARINDEX(' ad on', ses.EmailSubject) - (CHARINDEX('re. your ', ses.EmailSubject) + LEN('re. your '))
                                    )
                                )
                                ELSE NULL
                            END AS RoleAdvertised,
                            sec.ContactSource AS [Source],
                            @startDate AS FromDateExported,
                            @endDate AS ToDateExported,
                            CASE 
                                WHEN ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> '' THEN 1
                                ELSE 0
                            END AS HasReply,
                            sal.CreatedAt AS ExportedDate, 
                            sal.SmartleadCategory AS Category,
                            sal.BDR as Bdr,
                            sal.CreatedBy AS AssignedTo,
                            slc.[Name] AS EmailCampaign,
                            sal.CreatedBy AS LeadGen,
                            sal.QABy AS QadBy,
                            ses.OpenCount,
                            ses.ClickCount
                        FROM [dbo].[SmartLeadAllLeads] sal
                            INNER JOIN [dbo].[SmartLeadCampaigns] slc ON sal.CampaignId = slc.Id
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                            LEFT JOIN [dbo].[SmartLeadsExportedContacts] sec ON sal.Email = sec.Email
                        
                    """;

                    baseQuery = ComposeWhereConditionsForFind(baseQuery, request, param);

                    var baseQueryResult = await connection.QueryAsync<SmartLeadsCallTasksExport>(baseQuery, param);

                    return baseQueryResult.ToList();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private string ComposeWhereConditionsForFind(string baseQuery, TableRequest request, DynamicParameters param)
        {
            var whereClause = new List<string>();

            if (request.filters != null && request.filters.Count > 0)
            {
                // Determine which date field to use based on HasReply filter
                string dateField;

                var hasReplyFilter = request.filters.FirstOrDefault(f => f.Column.Equals("hasreply", StringComparison.OrdinalIgnoreCase));
                var categoryfilter = request.filters.FirstOrDefault(f => f.Column.Equals("category", StringComparison.OrdinalIgnoreCase));
                if (categoryfilter != null && categoryfilter.Value == "reply-email")
                {
                    dateField = "ses.ReplyTime";
                }
                else if (categoryfilter != null &&
                         (categoryfilter.Value == "email-error" || categoryfilter.Value == "out-of-office" || categoryfilter.Value == "incorrect-contact" || categoryfilter.Value == "positive-response" || categoryfilter.Value == "open-email"))
                {
                    dateField = "ses.SentTime";
                }
                else
                {
                    if (hasReplyFilter != null && bool.TryParse(hasReplyFilter.Value, out bool hasReplyValue))
                    {
                        dateField = hasReplyValue ? "ses.ReplyTime" : "ses.SentTime";
                    }
                    else
                    {
                        dateField = "sal.CreatedAt";
                    }
                }

                foreach (var filter in request.filters)
                {
                    switch (filter.Column.ToLower())
                    {
                        case "email":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sal.Email {this.operatorsMap[filter.Operator]}");
                                break; 
                            }

                            if (!string.IsNullOrEmpty(filter.Value) && filter.Value != "null")
                            {
                                whereClause.Add($"sal.Email {this.operatorsMap[filter.Operator]} @email");
                                param.Add("@email", filter.Value);
                            }
                            break;
                        case "fromdate":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"{dateField} {this.operatorsMap[filter.Operator]}");
                                break; 
                            }

                            if(filter.Operator.Contains("this", StringComparison.OrdinalIgnoreCase) 
                                || filter.Operator.Contains("last", StringComparison.OrdinalIgnoreCase))
                            {
                                var operatorValue = this.operatorsMap[filter.Operator].Replace("{0}", $"{dateField}");
                                whereClause.Add($"{operatorValue}");
                                break; 
                            }

                            if (DateTime.TryParse(filter.Value, out DateTime fromDate))
                            {
                                whereClause.Add($"CONVERT(DATE, {dateField}) {this.operatorsMap[filter.Operator]} @startDate");
                                param.Add("@startDate", fromDate);
                            }
                            break;
                        case "todate":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"{dateField} {this.operatorsMap[filter.Operator]}");
                                break; 
                            }

                            if(filter.Operator.Contains("this", StringComparison.OrdinalIgnoreCase) 
                                || filter.Operator.Contains("last", StringComparison.OrdinalIgnoreCase))
                            {
                                var operatorValue = this.operatorsMap[filter.Operator].Replace("{0}", $"{dateField}");
                                whereClause.Add($"{operatorValue}");
                                break; 
                            }
                            if (DateTime.TryParse(filter.Value, out DateTime toDate))
                            {
                                whereClause.Add($"CONVERT(DATE, {dateField}) {this.operatorsMap[filter.Operator]} @endDate");
                                param.Add("@endDate", toDate);
                            }
                            break;
                        case "bdr":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sal.BDR {this.operatorsMap[filter.Operator]}");
                                break; 
                            }

                            if (!string.IsNullOrEmpty(filter.Value) && filter.Value != "null")
                            {
                                whereClause.Add($"sal.BDR {this.operatorsMap[filter.Operator]} @bdr");
                                param.Add("@bdr", filter.Value);
                            }
                            break;
                        case "leadgen":
                            if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sal.CreatedBy {this.operatorsMap[filter.Operator]}");
                                break; 
                            }

                            if (!string.IsNullOrEmpty(filter.Value) && filter.Value != "null")
                            {
                                whereClause.Add($"sal.CreatedBy {this.operatorsMap[filter.Operator]} @leadGen");
                                param.Add("@leadGen", filter.Value);
                            }
                            break;
                        case "qaby":
                        if(this.operatorsMap[filter.Operator].Contains("null", StringComparison.OrdinalIgnoreCase))
                            {
                                whereClause.Add($"sal.QABy {this.operatorsMap[filter.Operator]}");
                                break; 
                            }
                            if (!string.IsNullOrEmpty(filter.Value) && filter.Value != "null")
                            {
                                whereClause.Add($"sal.QABy {this.operatorsMap[filter.Operator]} @qaBy");
                                param.Add("@qaBy", filter.Value);
                            }
                            break;
                        case "category":
                            if (filter.Value == "positive-response")
                            {
                                whereClause.Add("(sal.SmartleadCategory = 'Information Request' OR sal.SmartleadCategory = 'Interested' OR sal.SmartleadCategory = 'Meeting Request')");
                            }
                            else if (filter.Value == "email-error")
                            {
                                whereClause.Add("(sal.SmartleadCategory = 'Bounced' OR sal.SmartleadCategory = 'Sender Originated Bounce')");
                            }
                            else if (filter.Value == "out-of-office")
                            {
                                whereClause.Add("sal.SmartleadCategory = 'Out Of Office'");
                            }
                            else if (filter.Value == "incorrect-contact")
                            {
                                whereClause.Add("sal.SmartleadCategory = 'Wrong Person'");
                            }
                            else if (filter.Value == "open-email")
                            {
                                whereClause.Add("ses.OpenTime IS NOT NULL OR ses.OpenTime <> ''");
                            }
                            break;
                        case "campaigntype":
                            if (int.TryParse(filter.Value, out int campaignType))
                            {
                                if (campaignType == 1)
                                {
                                    whereClause.Add("sal.CreatedBy <> 'Bots' AND sal.BDR <> 'Steph'");
                                }
                                else if (campaignType == 2)
                                {
                                    whereClause.Add("sal.CreatedBy = 'Bots' AND sal.BDR = 'Steph'");
                                }
                                else if (campaignType == 3)
                                {
                                    whereClause.Add("sal.CreatedBy <> 'Bots' AND sal.BDR = 'Steph'");
                                }
                            }
                            break;
                    }
                }

            }

            // Add WHERE clause if needed
            if (whereClause.Count > 0)
            {
                var filterClause = $"""
                                WHERE {string.Join(" AND ", whereClause)}
                            """;
                baseQuery += filterClause;
            }

            return baseQuery;
        }
    }
}
