using System;

namespace SmartLeadsPortalDotNetApi.Aggregates.InboundCall;

public interface IInboundCallEvent
{
    string Type { get; }
    string? UniqueCallId { get; }
    DateTime? Timestamp { get; }
}
