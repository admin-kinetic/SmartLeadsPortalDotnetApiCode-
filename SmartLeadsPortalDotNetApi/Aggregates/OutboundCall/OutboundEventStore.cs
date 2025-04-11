using System;
using System.Threading.Tasks;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;

public class OutboundEventStore
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly OutboundCallEventParser outboundCallEventParser;

    public OutboundEventStore(DbConnectionFactory dbConnectionFactory, OutboundCallEventParser outboundCallEventParser)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.outboundCallEventParser = outboundCallEventParser;
    }

    private readonly Dictionary<string, List<IOutboundCallEvent>> _eventStore = new Dictionary<string, List<IOutboundCallEvent>>();

    public void StoreEvent(IOutboundCallEvent @event)
    {
        if (!_eventStore.TryGetValue(@event.UniqueCallId, out var events))
        {
            events = new List<IOutboundCallEvent>();
            _eventStore[@event.UniqueCallId] = events;
        }
        events.Add(@event);
    }

    public async Task<OutboundCallAggregate> GetOutboundCallAggregate(string uniqueCallId)
    {
        using( var connection = this.dbConnectionFactory.GetSqlConnection()){
            var query = """
                SELECT Request FROM VoiplineWebhooks WHERE UniqueCallId = @uniqueCallId
            """;
            var result = await connection.QueryAsync<string>(query, new { uniqueCallId });
            var callEvents = result.Select(this.outboundCallEventParser.ParseEvent);
            return OutboundCallAggregate.Rebuild(uniqueCallId, callEvents);
        }
    }
}
