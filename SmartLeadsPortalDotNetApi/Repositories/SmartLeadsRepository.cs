using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class SmartLeadsRepository : SQLDBService
    {
        private readonly DbConnectionFactory dbConnectionFactory;
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

                    string _proc = "sm_spGetLeadGenExportedLeadsPaginated";
                    param.Add("@Page", request.Page);
                    param.Add("@PageSize", request.PageSize);
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@isValid", request.HasReview);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);

                    list = await connection.QueryAsync<SmartLeadsExportedContactLeadGen>(_proc, param, commandType: CommandType.StoredProcedure);

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

                    string _proc = "sm_spGetLeadGenExportedLeadsPaginatedCount";
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@isValid", request.HasReview);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);

                    var countResult = await connection.QueryFirstOrDefaultAsync<SmartLeadsExportedContactLeadGenCount?>(_proc, param, commandType: CommandType.StoredProcedure);

                    return countResult?.TotalCount;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                            sal.FirstName + ' ' + sal.LastName AS FullName,
                            sal.CompanyName,
                            sal.Email,
                            sal.PhoneNumber,
                            sal.CreatedAt AS ExportedDate,
                            ses.SequenceNumber,
                            ses.ReplyTime,
                            ses.SentTime, sal.[Location] AS Country
                        FROM [dbo].[SmartLeadAllLeads] sal
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                        
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

                    // Add ORDER BY clause
                    baseQuery += """
                        ORDER BY sal.CreatedAt DESC
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

                    list = await connection.QueryAsync<SmartLeadsExportedLeadsEmailed>(baseQuery, param);

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

                    if (string.IsNullOrEmpty(request.EmailAddress) || request.EmailAddress == "null")
                    {
                        request.EmailAddress = "";
                    }

                    // string _proc = "sm_spGetLeadsEmailedPaginatedCount";

                    var countQuery = """
                        SELECT COUNT(sal.Id) AS TotalCount
                        FROM [dbo].[SmartLeadAllLeads] sal
                            LEFT JOIN [dbo].[SmartLeadsEmailStatistics] ses ON sal.Email = ses.LeadEmail
                        
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
                        countQuery += filterClause;
                    }

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
        public async Task<IEnumerable<SmartLeadsExportedLeadsEmailed>> GetAllExportedLeadsEmailedCsvExport(SmartLeadEmailedRequest request)
        {
            try
            {
                await using (var connection = await this.dbConnectionFactory.GetSqlConnectionAsync())
                {
                    var param = new DynamicParameters();
                    IEnumerable<SmartLeadsExportedLeadsEmailed> list = new List<SmartLeadsExportedLeadsEmailed>();

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
                                WHEN ses.ReplyTime IS NOT NULL AND LTRIM(RTRIM(CAST(ses.ReplyTime AS NVARCHAR))) <> '' THEN 'Yes'
                                ELSE 'No'
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

                    list = await connection.QueryAsync<SmartLeadsExportedLeadsEmailed>(baseQuery, param);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
