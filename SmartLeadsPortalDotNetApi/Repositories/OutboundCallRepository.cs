using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class OutboundCallRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public OutboundCallRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<OutboundCallAggregate> GetOutboundCallAggregate(string uniqueCallId)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        const string sql = """
            SELECT * FROM OutboundCalls WHERE UniqueCallId = @uniqueCallId
        """;

        var result = await connection.QuerySingleOrDefaultAsync<OutboundCallAggregate>(sql, new { uniqueCallId });
        return result ?? new OutboundCallAggregate(uniqueCallId);
    }

    public async Task UpsertOutboundCallAggregate(OutboundCallAggregate aggregate)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        const string sql = """
            MERGE INTO OutboundCalls AS target
            USING (VALUES (
                @UniqueCallId, @CallerId, @UserName, @UserNumber, @DestNumber,
                @CallStartAt, @ConnectedAt, @CallDuration, @ConversationDuration, @RecordedAt,
                @Emails, @EmailSubject, @EmailMessage, @CallRecordingLink, @LastEventType
            )) AS source (
                UniqueCallId, CallerId, UserName, UserNumber, DestNumber,
                CallStartAt, ConnectedAt, CallDuration, ConversationDuration, RecordedAt,
                Emails, EmailSubject, EmailMessage, CallRecordingLink, LastEventType
            )
            ON target.UniqueCallId = source.UniqueCallId
            WHEN MATCHED THEN
                UPDATE SET
                    UniqueCallId = source.UniqueCallId,
                    CallerId = source.CallerId,
                    UserName = source.UserName,
                    UserNumber = source.UserNumber,
                    DestNumber = source.DestNumber,
                    CallStartAt = source.CallStartAt,
                    ConnectedAt = source.ConnectedAt,
                    CallDuration = source.CallDuration,
                    ConversationDuration = source.ConversationDuration,
                    RecordedAt = source.RecordedAt,
                    Emails = source.Emails,
                    EmailSubject = source.EmailSubject,
                    EmailMessage = source.EmailMessage,
                    CallRecordingLink = source.CallRecordingLink,
                    LastEventType = source.LastEventType
            WHEN NOT MATCHED THEN
                INSERT (
                    UniqueCallId, CallerId, UserName, UserNumber, DestNumber,
                    CallStartAt, ConnectedAt, CallDuration, ConversationDuration, RecordedAt,
                    Emails, EmailSubject, EmailMessage, CallRecordingLink, LastEventType
                ) VALUES (
                    @UniqueCallId, @CallerId, @UserName, @UserNumber, @DestNumber,
                    @CallStartAt, @ConnectedAt, @CallDuration, @ConversationDuration, @RecordedAt,
                    @Emails, @EmailSubject, @EmailMessage, @CallRecordingLink, @LastEventType
                );
            """;


        await connection.ExecuteAsync(sql, aggregate);
    }

    internal async Task UpdateAzureStorageRecordingLik(string uniqueCallId, string uri)
    {
        await using var connection = await this.dbConnectionFactory.GetSqlConnectionAsync();
        const string sql = """
            UPDATE OutboundCalls
            SET AzureStorageCallRecordingLink = @uri
            WHERE UniqueCallId = @uniqueCallId
        """;

        await connection.ExecuteAsync(sql, new { uniqueCallId, uri });
    }
}
