using System;

namespace SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;

public interface IOutboundCallEvent
{
    string Type { get; }
    string UniqueCallId { get; }
    DateTime? Timestamp { get; }
}
