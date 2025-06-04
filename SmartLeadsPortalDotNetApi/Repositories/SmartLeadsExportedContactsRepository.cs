using System;
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
                    slec.Id,
                    slec.Email,
                    slec.ContactSource,
                    slec.ExportedDate,
                    sles.SentTime [SentAt],
                    CASE 
                        WHEN sles.ReplyTime IS NOT NULL THEN 1
                        ELSE 0
                    END [HasReply],
                    sles.ReplyTime [RepliedAt],
                    slec.HasReviewed,
                    slec.SmartLeadsStatus,
                    slec.SmartLeadsCategory
                From SmartLeadsExportedContacts slec
                LEFT JOIN SmartLeadsEmailStatistics sles ON 
                    sles.LeadEmail = slec.Email AND sles.SequenceNumber = 1
            """;

            var countQuery = """ 
                SELECT COUNT(slec.Id) AS TotalCount
                FROM SmartLeadsExportedContacts slec
                LEFT JOIN SmartLeadsEmailStatistics sles ON 
                    sles.LeadEmail = slec.Email AND sles.SequenceNumber = 1
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
                        case "email":
                            whereClause.Add("slec.Email LIKE @Email");
                            parameters.Add("Email", $"%{filter.Value}%");
                            break;
                        case "hasreply":
                            whereClause.Add("(sles.ReplyTime IS NOT NULL OR sles.ReplyTime = '')");
                            break;
                        case "hasreviewed":
                            whereClause.Add("slec.hHasReview = @HasRevew");
                            parameters.Add("HasReview", filter.Value);
                            break;
                        case "exporteddatefrom":
                            whereClause.Add("slec.ExportedDate >= @ExportedDateFrom");
                            parameters.Add("ExportedDateFrom", filter.Value);
                            break;
                        case "exporteddateto":
                            whereClause.Add("slec.ExportedDate <  DATEADD(day, 1, @ExportedDateTo)");
                            parameters.Add("ExportedDateTo", filter.Value);
                            break;
                        case "category":
                            switch (filter.Value.ToLower())
                            {
                                case "responses-today":
                                    var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                                    var nowInLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);
                                    var startOfDayLocal = nowInLocal.Date;
                                    var endOfDayLocal = startOfDayLocal.AddDays(1).AddTicks(-1);
                                    var startOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDayLocal, localTimeZone);
                                    var endOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDayLocal, localTimeZone);
                                    whereClause.Add("slec.RepliedAt BETWEEN @RepliedAtStartDay AND @RepliedAtEndDay");
                                    parameters.Add("RepliedAtStartDay", startOfDayUtc);
                                    parameters.Add("RepliedAtEndDay", endOfDayUtc);
                                    whereClause.Add("(slec.SmartleadsCategory IS NULL OR slec.SmartleadsCategory = '')");
                                    break;
                                case "positive-response":
                                    whereClause.Add("slec.HasReviewed = 1");
                                    break;
                                case "out-of-office":
                                    whereClause.Add("slec.SmartleadsCategory = 'Out Of Office'");
                                    break;
                                case "incorrect-contact":
                                    whereClause.Add("slec.SmartleadsCategory = 'Wrong Person'");
                                    break;
                                case "email-error":
                                    whereClause.Add("slec.SmartleadsCategory = 'Sender Originated Bounce'");
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
                        baseQuery += $" ORDER BY slec.Id DESC";
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
