using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartLeadsExportedContactsRepository
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

    public SmartLeadsExportedContactsRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }


    public async Task<TableResponse<SmartLeadsExportedContact>> Find(TableRequest request)
    {
        using (var connection = dbConnectionFactory.GetSqlConnection())
        {
            var baseQuery = """ 
                Select
                TRY_CAST(sla.LeadId AS INT) AS Id,
                sla.Email,
                slec.ContactSource,
                sla.CreatedAt AS ExportedDate,
                ses.SentTime AS [SentAt],
                CASE 
                    WHEN ses.ReplyTime IS NOT NULL THEN 1
                    ELSE 0
                END AS [HasReply],
                ses.ReplyTime AS [RepliedAt],
                slec.HasReviewed,
                sla.LeadStatus AS SmartLeadsStatus,
                sla.SmartleadCategory AS SmartLeadsCategory
            From SmartLeadAllLeads sla
            LEFT JOIN SmartLeadsEmailStatistics ses ON sla.Email = ses.LeadEmail AND ses.SequenceNumber = 1
            LEFT JOIN SmartLeadsExportedContacts slec ON sla.Email = slec.Email
            """;

            var countQuery = """ 
                SELECT COUNT(sla.Id) AS TotalCount
                    From SmartLeadAllLeads sla
                LEFT JOIN SmartLeadsEmailStatistics ses ON sla.Email = ses.LeadEmail AND ses.SequenceNumber = 1
                LEFT JOIN SmartLeadsExportedContacts slec ON sla.Email = slec.Email
            """;

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
                        case "emailaddress":
                            whereClause.Add("sla.Email LIKE @Email");
                            parameters.Add("Email", $"%{filter.Value}%");
                            break;
                        case "hasreply":
                            whereClause.Add("(ses.ReplyTime IS NOT NULL OR ses.ReplyTime = '')");
                            break;
                        case "hasreview":
                            whereClause.Add("slec.HasReviewed = @HasReview");
                            parameters.Add("HasReview", filter.Value);
                            break;
                        case "fromdate":
                            whereClause.Add("sla.CreatedAt >= @ExportedDateFrom");
                            parameters.Add("ExportedDateFrom", filter.Value);
                            break;
                        case "todate":
                            whereClause.Add("sla.CreatedAt <  DATEADD(day, 1, @ExportedDateTo)");
                            parameters.Add("ExportedDateTo", filter.Value);
                            break;
                        case "category":
                            switch (filter?.Value?.ToLower())
                            {
                                case "responses-today":
                                    var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                                    var nowInLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);
                                    var startOfDayLocal = nowInLocal.Date;
                                    var endOfDayLocal = startOfDayLocal.AddDays(1).AddTicks(-1);
                                    var startOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDayLocal, localTimeZone);
                                    var endOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDayLocal, localTimeZone);
                                    whereClause.Add("ses.ReplyTime BETWEEN @RepliedAtStartDay AND @RepliedAtEndDay");
                                    parameters.Add("RepliedAtStartDay", startOfDayUtc);
                                    parameters.Add("RepliedAtEndDay", endOfDayUtc);
                                    whereClause.Add("(sla.SmartleadCategory IS NULL OR sla.SmartleadCategory = '')");
                                    break;
                                case "positive-response":
                                    //whereClause.Add("slec.HasReviewed = 1");
                                    whereClause.Add("sla.SmartleadCategory = 'Interested' OR sla.SmartleadCategory = 'Information Request' OR sla.SmartleadCategory = 'Meeting Request'");
                                    break;
                                case "out-of-office":
                                    whereClause.Add("sla.SmartleadCategory = 'Out Of Office'");
                                    break;
                                case "incorrect-contact":
                                    whereClause.Add("sla.SmartleadCategory = 'Wrong Person'");
                                    break;
                                case "email-error":
                                    whereClause.Add("sla.SmartleadCategory = 'Sender Originated Bounce' OR sla.SmartleadCategory = 'Bounced'");
                                    break;
                                case "open-email":
                                    whereClause.Add("ses.OpenTime IS NOT NULL OR ses.OpenTime <> ''");
                                    break;
                                default:
                                    break;
                            }
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

            // Sorting to follow after where
            if (request.sorting != null)
            {
                switch (request.sorting.column.ToLower())
                {
                    default:
                        baseQuery += $" ORDER BY TRY_CAST(sla.LeadId AS INT) DESC";
                        break;
                }
            }

            // Add ORDER BY and pagination
            baseQuery += """
                OFFSET (@PageNumber - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
            """;


            var items = await connection.QueryAsync<SmartLeadsExportedContact>(baseQuery, parameters);
            var count = await connection.QueryFirstAsync<int>(countQuery, parameters);

            var response = new TableResponse<SmartLeadsExportedContact>
            {
                Items = items.ToList(),
                Total = count
            };
            return response;
        }
    }

    public async Task SaveExportedContacts(List<ExportedContactsPayload>? exportedContactsPayload)
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    await connection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts ON;", transaction: transaction);

                    // var insert = """
                    //     INSERT INTO SmartLeadsExportedContacts (Id, ExportedDate, Email, ContactSource, Rate)
                    //     VALUES (@id, @exportedDate, @email, @contactSource, @rate)
                    // """;

                    var upsert = """
                        MERGE INTO SmartLeadsExportedContacts AS target
                        USING (SELECT @id AS Id, @exportedDate AS ExportedDate, @email AS Email, 
                                    @contactSource AS ContactSource, @rate AS Rate) AS source
                        ON (target.Id = source.Id)
                        WHEN MATCHED THEN
                            UPDATE SET 
                                target.ExportedDate = source.ExportedDate,
                                target.Email = source.Email,
                                target.ContactSource = source.ContactSource,
                                target.Rate = source.Rate
                        WHEN NOT MATCHED THEN
                            INSERT (Id, ExportedDate, Email, ContactSource, Rate)
                            VALUES (source.Id, source.ExportedDate, source.Email, 
                                    source.ContactSource, source.Rate);
                    """;

                    await connection.ExecuteAsync(upsert, exportedContactsPayload, transaction);
                    await connection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts OFF;", transaction: transaction);

                    transaction.Commit();
                    Console.WriteLine($"Save {exportedContactsPayload.Count()} exported contacts");

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error saving exported contacts: {ex.Message}");
                    throw ex;
                }
            }
        }
    }
}
