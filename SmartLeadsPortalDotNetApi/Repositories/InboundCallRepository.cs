using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Aggregates.InboundCall;
using SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class InboundCallRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public InboundCallRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<InboundCallAggregate> GetInboundCallAggregate(string uniqueCallId)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        const string sql = """
            SELECT * FROM InboundCalls WHERE UniqueCallId = @uniqueCallId
        """;

        var result = await connection.QuerySingleOrDefaultAsync<InboundCallAggregate>(sql, new { uniqueCallId });
        return result ?? new InboundCallAggregate(uniqueCallId);
    }

    public async Task UpsertInboundCallAggregate(InboundCallAggregate aggregate)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        const string sql = """
            MERGE INTO InboundCalls AS target
            USING (VALUES (
                @UniqueCallId, @CallerId, @CallerName, @UserName, @UserNumber, @DestNumber,
                @CallStartAt, @QueueName, @Status, @RingGroupName, @ConnectedAt, @CallDuration, @ConversationDuration, @RecordedAt,
                @Emails, @EmailSubject, @EmailMessage, @CallRecordingLink, @LastEventType
            )) AS source (
                UniqueCallId, CallerId, CallerName, UserName, UserNumber, DestNumber,
                CallStartAt, QueueName, Status, RingGroupName, ConnectedAt, CallDuration, ConversationDuration, RecordedAt,
                Emails, EmailSubject, EmailMessage, CallRecordingLink, LastEventType
            )
            ON target.UniqueCallId = source.UniqueCallId
            WHEN MATCHED THEN
                UPDATE SET
                    UniqueCallId = source.UniqueCallId,
                    CallerId = source.CallerId,
                    CallerName = source.CallerName,
                    UserName = source.UserName,
                    UserNumber = source.UserNumber,
                    DestNumber = source.DestNumber,
                    CallStartAt = source.CallStartAt,
                    QueueName = source.QueueName,
                    Status = source.Status,
                    RingGroupName = source.RingGroupName,
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
                    UniqueCallId, CallerId, CallerName, UserName, UserNumber, DestNumber,
                    CallStartAt, QueueName, Status, RingGroupName, ConnectedAt, CallDuration, ConversationDuration, RecordedAt,
                    Emails, EmailSubject, EmailMessage, CallRecordingLink, LastEventType
                ) VALUES (
                    @UniqueCallId, @CallerId, @CallerNAme, @UserName, @UserNumber, @DestNumber,
                    @CallStartAt, @QueueName, @Status, @RingGroupName, @ConnectedAt, @CallDuration, @ConversationDuration, @RecordedAt,
                    @Emails, @EmailSubject, @EmailMessage, @CallRecordingLink, @LastEventType
                );
            """;


        await connection.ExecuteAsync(sql, aggregate);
    }

    internal async Task UpdateAzureStorageRecordingLik(string uniqueCallId, string uri)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        const string sql = """
            UPDATE OutboundCalls
            SET AzureStorageCallRecordingLink = @uri
            WHERE UniqueCallId = @uniqueCallId
        """;

        await connection.ExecuteAsync(sql, new { uniqueCallId, uri });
    }
}
