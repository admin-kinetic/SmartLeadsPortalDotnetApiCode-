using Dapper;
using MySqlConnector;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class AutomatedLeadsRepository : SQLDBService
    {
        private readonly string _mysqlconnectionString;
        private readonly DbConnectionFactory dbConnectionFactory;
        private readonly ILogger<AutomatedLeadsRepository> logger;

        public AutomatedLeadsRepository(IConfiguration configuration, DbConnectionFactory dbConnectionFactory, ILogger<AutomatedLeadsRepository> logger)
        {
            _mysqlconnectionString = configuration.GetConnectionString("MySQLDBConnectionString");
            this.dbConnectionFactory = dbConnectionFactory;
            this.logger = logger;
        }

        //MSSQL
        public async Task<IEnumerable<ExportedDateResult>> GetSmartLeadsByExportedDate()
        {
            try
            {
                string _proc = "sm_spGetSmartLeadsByExportedDate";
                IEnumerable<ExportedDateResult> list = await SqlMapper.QueryAsync<ExportedDateResult>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);
                return list;
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
        public async Task<IEnumerable<ExportedDateResult>> GetSmartLeadsByRepliedDate()
        {
            try
            {
                string _proc = "sm_spGetSmartLeadsByRepliedDate";
                IEnumerable<ExportedDateResult> list = await SqlMapper.QueryAsync<ExportedDateResult>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);
                return list;
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
        public async Task<HasReplyCountModel?> GetSmartLeadsHasReplyCount()
        {
            try
            {
                string _proc = "sm_spGetHasReplyCount";
                HasReplyCountModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<HasReplyCountModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);

                return ret;
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
        public async Task<TotalLeadsSentModel?> GetSmartLeadsTotalLeadsSent()
        {
            try
            {
                string _proc = "sm_spSmartLeadsTotalSent";
                TotalLeadsSentModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<TotalLeadsSentModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);

                return ret;
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
        public async Task<TotalEmailErrorResponseModel?> GetSmartLeadsEmailErrorResponse()
        {
            try
            {
                string _proc = "sm_spSmartLeadsEmailErrorResponse";
                TotalEmailErrorResponseModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<TotalEmailErrorResponseModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);

                return ret;
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

        public async Task<TotalOutOfOfficeResponseModel?> GetSmartLeadsOutOfOfficeResponse()
        {
            try
            {
                string _proc = "sm_spSmartLeadsOutOfOfficeResponse";
                TotalOutOfOfficeResponseModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<TotalOutOfOfficeResponseModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);
                return ret;
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
        public async Task<TotalResponseTodayModel?> GetSmartLeadsResponseToday()
        {
            try
            {
                string _proc = "sm_spSmartLeadsResponseToday";
                TotalResponseTodayModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<TotalResponseTodayModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);
                return ret;
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
        public async Task<TotalValidResponseModel?> GetSmartLeadsValidResponse()
        {
            try
            {
                string _proc = "sm_spSmartLeadsValidResponse";
                TotalValidResponseModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<TotalValidResponseModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);
                return ret;
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
        public async Task<TotalIncorrectContactResponseModel?> GetSmartLeadsIncorrectContactsResponse()
        {
            try
            {
                string _proc = "sm_spSmartLeadsIncorrectContactResponse";
                TotalIncorrectContactResponseModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<TotalIncorrectContactResponseModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);
                return ret;
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
        public async Task<TotalInvalidResponseModel?> GetSmartLeadsInvalidResponse()
        {
            try
            {
                string _proc = "sm_spSmartLeadsInvalidResponse";
                TotalInvalidResponseModel? ret = await SqlMapper.QuerySingleOrDefaultAsync<TotalInvalidResponseModel>(con, _proc, commandType: CommandType.StoredProcedure, commandTimeout: 6000);
                return ret;
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

        public async Task<SmartLeadsResponseModel<SmartLeadsExportedContact>> GetAllRawPaginated(SmartLeadRequest request)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<SmartLeadsExportedContact> list = new List<SmartLeadsExportedContact>();

                if (request.EmailAddress == null || request.EmailAddress == "" || request.EmailAddress == "null")
                {
                    request.EmailAddress = "";
                }

                if (request.History == null || request.History == "" || request.History == "null")
                {
                    request.History = "";
                }

                _proc = "sm_spGetAllRawSmartLeads";
                param.Add("@Page", request.Page);
                param.Add("@PageSize", request.PageSize);
                param.Add("@email", request.EmailAddress);
                param.Add("@messagehistory", request.History);
                param.Add("@hasReply", request.HasReply);
                param.Add("@isValid", request.HasReview);
                param.Add("@ExportedDateFrom", request.ExportedDateFrom);
                param.Add("@ExportedDateTo", request.ExportedDateTo);

                list = await SqlMapper.QueryAsync<SmartLeadsExportedContact>(con, _proc, param, commandType: CommandType.StoredProcedure);

                param2.Add("@email", request.EmailAddress);
                param2.Add("@messagehistory", request.History);
                param2.Add("@hasReply", request.HasReply);
                param2.Add("@isValid", request.HasReview);
                param2.Add("@ExportedDateFrom", request.ExportedDateFrom);
                param2.Add("@ExportedDateTo", request.ExportedDateTo);

                var countProcedure = "sm_spGetAllRawSmartLeadsCount";
                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, param2, commandType: CommandType.StoredProcedure);

                return new SmartLeadsResponseModel<SmartLeadsExportedContact>
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




        //MYSQL DATA
        public async Task<SmartLeadsResponseModel<SmartLeadsExportedContact>> GetAllRaw(SmartLeadRequest request)
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM SmartLeadsExportedContacts";
                    var countQuery = "SELECT COUNT(Id) as Count FROM SmartLeadsExportedContacts";
                    var conditions = new List<string>();
                    var parameters = new DynamicParameters();

                    conditions.Add("ModifiedAt IS NOT NULL");

                    if (!string.IsNullOrEmpty(request.EmailAddress))
                    {
                        conditions.Add("Email = @Email");
                        parameters.Add("Email", request.EmailAddress);
                    }

                    if (!string.IsNullOrEmpty(request.History))
                    {
                        conditions.Add("MessageHistory LIKE @History");
                        parameters.Add("History", $"%{request.History}%");
                    }

                    if (request.HasReply != null)
                    {
                        conditions.Add("HasReply = @HasReply");
                        parameters.Add("HasReply", request.HasReply);
                    }

                    if (request.HasReview != null)
                    {
                        conditions.Add("HasReviewed = @HasReview");
                        parameters.Add("HasReview", request.HasReview);
                    }

                    if (request.ExportedDateFrom.HasValue)
                    {
                        conditions.Add("ExportedDate >= @ExportedDateFrom");
                        parameters.Add("ExportedDateFrom", request.ExportedDateFrom.Value);
                    }

                    if (request.ExportedDateTo.HasValue)
                    {
                        conditions.Add("ExportedDate <= @ExportedDateTo");
                        parameters.Add("ExportedDateTo", request.ExportedDateTo.Value);
                    }

                    if (request.IsResponsesToday.HasValue && request.IsResponsesToday.Value)
                    {
                        var startUtc = DateTime.UtcNow.Date;
                        var endUtc = startUtc.AddDays(1);
                        conditions.Add("RepliedAt BETWEEN @StartUtc AND @EndUtc");
                        parameters.Add("StartUtc", startUtc);
                        parameters.Add("EndUtc", endUtc);
                    }

                    if (request.IsOutOfOffice.HasValue && request.IsOutOfOffice.Value)
                    {
                        conditions.Add("MessageHistory REGEXP @IsOutOfOffice");
                        parameters.Add("IsOutOfOffice", "out of office|on leave|maternity leave|leave");
                    }

                    if (request.IsIncorrectContact.HasValue && !request.IsIncorrectContact.Value)
                    {
                        conditions.Add("MessageHistory REGEXP @IsIncorrectContact");
                        parameters.Add("IsIncorrectContact", "not the right person|no longer working with|no longer work for|not interested|in charge|onshore|remove me|unsubscribe");
                    }

                    if (request.IsEmailError.HasValue && !request.IsEmailError.Value)
                    {
                        conditions.Add("MessageHistory REGEXP @IsEmailError");
                        parameters.Add("IsEmailError", "error|not found|problem deliver|be delivered|blocked|unable to recieve|unable to deliver");
                    }

                    if (!string.IsNullOrEmpty(request.ExcludeKeywords))
                    {
                        var excludeKeywords = ((string)request.ExcludeKeywords).Split(',').Select(keyword => keyword.Trim()).ToArray();
                        var excludeRegex = string.Join("|", excludeKeywords);
                        conditions.Add("MessageHistory NOT REGEXP @ExcludeRegex");
                        parameters.Add("ExcludeRegex", excludeRegex);
                    }

                    if (conditions.Any())
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                        countQuery += " WHERE " + string.Join(" AND ", conditions);
                    }

                    query += " ORDER BY COALESCE(RepliedAt, ExportedDate) DESC, ExportedDate DESC LIMIT @Limit OFFSET @Offset";
                    parameters.Add("Limit", (int)request.PageSize);
                    parameters.Add("Offset", ((int)request.Page - 1) * (int)request.PageSize);

                    var items = await connection.QueryAsync<SmartLeadsExportedContact>(query, parameters);
                    var total = await connection.QueryFirstOrDefaultAsync<int>(countQuery, parameters);

                    return new SmartLeadsResponseModel<SmartLeadsExportedContact> { Items = items.ToList(), Total = total };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SmartLeadsResponseModel<SmartLeadsExportedContact>> GetAllDataExport(SmartLeadRequest request)
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM SmartLeadsExportedContacts";
                    var countQuery = "SELECT COUNT(Id) as Count FROM SmartLeadsExportedContacts";
                    var conditions = new List<string>();
                    var parameters = new DynamicParameters();

                    conditions.Add("ModifiedAt IS NOT NULL");

                    if (!string.IsNullOrEmpty(request.EmailAddress))
                    {
                        conditions.Add("Email = @Email");
                        parameters.Add("Email", request.EmailAddress);
                    }

                    if (!string.IsNullOrEmpty(request.History))
                    {
                        conditions.Add("MessageHistory LIKE @History");
                        parameters.Add("History", $"%{request.History}%");
                    }

                    if (request.HasReply != null)
                    {
                        conditions.Add("HasReply = @HasReply");
                        parameters.Add("HasReply", request.HasReply);
                    }

                    if (request.HasReview != null)
                    {
                        conditions.Add("HasReviewed = @HasReview");
                        parameters.Add("HasReview", request.HasReview);
                    }

                    if (request.ExportedDateFrom.HasValue)
                    {
                        conditions.Add("ExportedDate >= @ExportedDateFrom");
                        parameters.Add("ExportedDateFrom", request.ExportedDateFrom.Value);
                    }

                    if (request.ExportedDateTo.HasValue)
                    {
                        conditions.Add("ExportedDate <= @ExportedDateTo");
                        parameters.Add("ExportedDateTo", request.ExportedDateTo.Value);
                    }

                    if (request.IsResponsesToday.HasValue && request.IsResponsesToday.Value)
                    {
                        conditions.Add("RepliedAt = @RepliedAt");
                        parameters.Add("RepliedAt", DateTime.UtcNow.Date);
                    }

                    if (!string.IsNullOrEmpty(request.ExcludeKeywords))
                    {
                        var excludeKeywords = ((string)request.ExcludeKeywords).Split(',').Select(keyword => keyword.Trim()).ToArray();
                        var excludeRegex = string.Join("|", excludeKeywords);
                        conditions.Add("MessageHistory NOT REGEXP @ExcludeRegex");
                        parameters.Add("ExcludeRegex", excludeRegex);
                    }

                    if (conditions.Any())
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                        countQuery += " WHERE " + string.Join(" AND ", conditions);
                    }

                    query += " ORDER BY COALESCE(RepliedAt, ExportedDate) DESC, ExportedDate DESC";

                    var items = await connection.QueryAsync<SmartLeadsExportedContact>(query, parameters);
                    var total = await connection.QueryFirstOrDefaultAsync<int>(countQuery, parameters);

                    return new SmartLeadsResponseModel<SmartLeadsExportedContact> { Items = items.ToList(), Total = total };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task UpdateReviewStatus(SmartLeadRequestUpdateModel request)
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "UPDATE SmartLeadsExportedContacts SET HasReviewed = 1 WHERE Id = @Id";
                    await connection.ExecuteAsync(query, new { Id = request.Id });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task RevertReviewStatus(SmartLeadRequestUpdateModel request)
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "UPDATE SmartLeadsExportedContacts SET HasReviewed = 0 WHERE Id = @Id";
                    await connection.ExecuteAsync(query, new { Id = request.Id });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<ExportedDateResult>> GetByExportedDate()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                    SELECT DATE(ExportedDate) Date, COUNT(id) AS Count 
                    FROM SmartLeadsExportedContacts
                    WHERE ExportedDate >= CURDATE() - INTERVAL 10 DAY
                    GROUP BY DATE(ExportedDate) 
                    ORDER BY ExportedDate DESC";
                    var result = await connection.QueryAsync<ExportedDateResult>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<ExportedDateResult>> GetByRepliedDate()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                    SELECT DATE(RepliedAt) Date, COUNT(id) AS Count 
                    FROM SmartLeadsExportedContacts
                    WHERE RepliedAt >= CURDATE() - INTERVAL 10 DAY
                    GROUP BY DATE(RepliedAt) 
                    ORDER BY RepliedAt DESC";
                    var result = await connection.QueryAsync<ExportedDateResult>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<HasReplyCountModel> GetHasReplyCount()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT Count(Id) AS HasReplyCount FROM SmartLeadsExportedContacts WHERE HasReply = 1";
                    var result = await connection.QueryFirstOrDefaultAsync<HasReplyCountModel>(query);
                    return result ?? new HasReplyCountModel { HasReplyCount = 0 };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TotalResponseTodayModel> GetNumberOfResponseToday()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var singaporeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                    var nowInSingapore = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, singaporeTimeZone);
                    var todayStartLocal = new DateTime(nowInSingapore.Year, nowInSingapore.Month, nowInSingapore.Day,
                        0, 0, 0, DateTimeKind.Unspecified);
                    var todayEndLocal = todayStartLocal.AddDays(1);

                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(todayStartLocal, singaporeTimeZone);
                    var endUtc = TimeZoneInfo.ConvertTimeToUtc(todayEndLocal, singaporeTimeZone);
                    var query = "SELECT Count(Id) AS TotalResponseToday FROM SmartLeadsExportedContacts WHERE RepliedAt BETWEEN @StartUtc AND @EndUtc";
                    var result = await connection.QueryFirstOrDefaultAsync<TotalResponseTodayModel>(query, new
                    {
                        StartUtc = startUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                        EndUtc = endUtc.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                                );
                    return result ?? new TotalResponseTodayModel { TotalResponseToday = 0 };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TotalValidResponseModel> GetNumberOfValidResponse()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT Count(Id) AS TotalValidResponse FROM SmartLeadsExportedContacts WHERE HasReviewed = 1";
                    var result = await connection.QueryFirstOrDefaultAsync<TotalValidResponseModel>(query);
                    return result ?? new TotalValidResponseModel { TotalValidResponse = 0 };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TotalInvalidResponseModel> GetNumberOfInvalidResponse()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT Count(Id) AS TotalInvalidResponse FROM SmartLeadsExportedContacts WHERE HasReviewed = 0";
                    var result = await connection.QueryFirstOrDefaultAsync<TotalInvalidResponseModel>(query);
                    return result ?? new TotalInvalidResponseModel { TotalInvalidResponse = 0 };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TotalLeadsSentModel> GetNumberOfLeadsSent()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT Count(Id) AS TotalLeadsSent FROM SmartLeadsExportedContacts WHERE ExportedDate >= '2025-01-01'";
                    var result = await connection.QueryFirstOrDefaultAsync<TotalLeadsSentModel>(query);
                    return result ?? new TotalLeadsSentModel { TotalLeadsSent = 0 };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TotalEmailErrorResponseModel> GetEmailErrorResponse()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                    SELECT Count(Id) AS TotalEmailErrorResponse
                    FROM SmartLeadsExportedContacts 
                    WHERE ModifiedAt IS NOT NULL 
                        AND ExportedDate >= '2025-01-01'
                        AND HasReply = 1
                        AND MessageHistory REGEXP 'error|not found|problem deliver|be delivered|blocked|unable to recieve|unable to deliver'";
                    var result = await connection.QueryFirstOrDefaultAsync<TotalEmailErrorResponseModel>(query);
                    return result ?? new TotalEmailErrorResponseModel { TotalEmailErrorResponse = 0 };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TotalOutOfOfficeResponseModel> GetOutOfOfficeResponse()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                    SELECT Count(Id) AS TotalOutOfOfficeResponse
                    FROM SmartLeadsExportedContacts 
                    WHERE ModifiedAt IS NOT NULL 
                        AND ExportedDate >= '2025-01-01'
                        AND HasReply = 1
                        AND MessageHistory REGEXP 'out of office|on leave|maternity leave|leave'";
                    var result = await connection.QueryFirstOrDefaultAsync<TotalOutOfOfficeResponseModel>(query);
                    return result ?? new TotalOutOfOfficeResponseModel { TotalOutOfOfficeResponse = 0 };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TotalIncorrectContactResponseModel> GetIncorrectContactsResponse()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                    SELECT Count(Id) AS TotalIncorrectContactResponse
                    FROM SmartLeadsExportedContacts 
                    WHERE ModifiedAt IS NOT NULL 
                        AND ExportedDate >= '2025-01-01'
                        AND HasReply = 1
                        AND MessageHistory REGEXP 'not the right person|no longer working with|no longer work for|not interested|in charge|onshore|remove me|unsubscribe'";
                    var result = await connection.QueryFirstOrDefaultAsync<TotalIncorrectContactResponseModel>(query);
                    return result ?? new TotalIncorrectContactResponseModel { TotalIncorrectContactResponse = 0 };
                    ;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Users?> TestSelectAllUser()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                SELECT *
                FROM users";
                    return await connection.QueryFirstOrDefaultAsync<Users>(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<SmartLeadsExportedContact?> GetByEmail(string email)
        {
            try
            {
                using var connection = this.dbConnectionFactory.GetSqlConnection();
                var query = "SELECT * FROM SmartLeadsExportedContacts WHERE Email = @email";
                return await connection.QueryFirstOrDefaultAsync<SmartLeadsExportedContact>(query, new { email });
            }
            catch (System.Exception ex)
            {
                this.logger.LogError("Database error: {0}", ex.Message);
                throw;
            }
        }

        public async Task UpdateReply(string email, string repliedAt)
        {
            try
            {
                var lead = await this.GetByEmail(email.ToString());
                if (lead == null)
                {
                    throw new ArgumentException("Email not found in leads.");
                }

                DateTime.TryParse(repliedAt, out var formatedRepliedAt);

                using var connection = this.dbConnectionFactory.GetSqlConnection();
                var update = """
                    UPDATE SmartLeadsExportedContacts 
                    SET HasReply = 1, 
                        RepliedAt = @formatedRepliedAt
                    WHERE Email = @email
                """;
                await connection.ExecuteAsync(update, new { email, formatedRepliedAt });
            }
            catch (System.Exception ex)
            {
                this.logger.LogError("Database error: {0}", ex.Message);
                throw;
            }
        }

        internal async Task UpdateLeadCategory(string email, string category)
        {
            try
            {
                var lead = await this.GetByEmail(email);
                if (lead == null)
                {
                    throw new ArgumentException("Email not found in leads.");
                }

                using var connection = this.dbConnectionFactory.GetSqlConnection();
                var update = """
                    UPDATE SmartLeadsExportedContacts 
                    SET SmartLeadsCategory = @category
                    WHERE Email = @email
                """;
                await connection.ExecuteAsync(update, new { email, category });
            }
            catch (System.Exception ex)
            {
                this.logger.LogError("Database error: {0}", ex.Message);
                throw;
            }
        }
    }
}
