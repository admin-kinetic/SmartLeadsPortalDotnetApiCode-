using System;
using System.Threading.Tasks;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Aggregates.InboundCall;

public class InboundEventStore
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly InboundCallEventParser outboundCallEventParser;

    public InboundEventStore(DbConnectionFactory dbConnectionFactory, InboundCallEventParser outboundCallEventParser)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.outboundCallEventParser = outboundCallEventParser;
    }

    private readonly Dictionary<string, List<IInboundCallEvent>> _eventStore = new Dictionary<string, List<IInboundCallEvent>>();

    public void StoreEvent(IInboundCallEvent @event)
    {
        if (!_eventStore.TryGetValue(@event.UniqueCallId, out var events))
        {
            events = new List<IInboundCallEvent>();
            _eventStore[@event.UniqueCallId] = events;
        }
        events.Add(@event);
    }

    public async Task<InboundCallAggregate> GetOutboundCallAggregate(string uniqueCallId)
    {
        using( var connection = this.dbConnectionFactory.GetSqlConnection()){
            var query = """
                SELECT Request FROM VoiplineWebhooks WHERE UniqueCallId = @uniqueCallId
            """;
            var result = await connection.QueryAsync<string>(query, new { uniqueCallId });
            var callEvents = result.Select(this.outboundCallEventParser.ParseEvent);
            return InboundCallAggregate.Rebuild(uniqueCallId, callEvents);
        }
    }
}
