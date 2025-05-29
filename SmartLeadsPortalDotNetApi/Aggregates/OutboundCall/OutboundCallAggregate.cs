using System;

namespace SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;

public class OutboundCallAggregate
{
    public string? UniqueCallId { get; private set; }
    public string? CallerId { get; private set; }
    public string? UserName { get; private set; }
    public string? UserNumber { get; private set; }
    public string? DestNumber { get; private set; }
    public DateTime? CallStartAt { get; private set; }
    public DateTime? ConnectedAt { get; private set; }
    public int? CallDuration { get; private set; }
    public int? ConversationDuration { get; private set; }
    public DateTime? RecordedAt { get; private set; }
    public string? Emails { get; private set; }
    public string? EmailSubject { get; private set; }
    public string? EmailMessage { get; private set; }
    public string? CallRecordingLink { get; private set; }
    public string? LastEventType { get; private set; }

    private readonly List<IOutboundCallEvent> _events = new List<IOutboundCallEvent>();

    public IReadOnlyCollection<IOutboundCallEvent> Events => _events.AsReadOnly();

    public OutboundCallAggregate()
    {
    }

    public OutboundCallAggregate(string uniqueCallId)
    {
        UniqueCallId = uniqueCallId;
    }

    public void ApplyEvent(IOutboundCallEvent @event)
    {
        if (@event.UniqueCallId != UniqueCallId)
            throw new ArgumentException("Event does not belong to this aggregate");

        _events.Add(@event);
        LastEventType = @event.Type;

        switch (@event)
        {
            case UserOutboundEvent e:
                Apply(e);
                break;
            case UserOutboundAnsweredEvent e:
                Apply(e);
                break;
            case UserOutboundCompletedEvent e:
                Apply(e);
                break;
            case RecordingOutboundEvent e:
                Apply(e);
                break;
        }
    }

    private void Apply(UserOutboundEvent @event)
    {
        CallerId = @event.CallerId;
        UserName = @event.UserName;
        UserNumber = @event.UserNumber;
        DestNumber = @event.DestNumber;
        CallStartAt = @event.CallStartAt;
    }

    private void Apply(UserOutboundAnsweredEvent @event)
    {
        ConnectedAt = @event.ConnectedAt;
    }

    private void Apply(UserOutboundCompletedEvent @event)
    {
        CallDuration = @event.CallDuration;
        ConversationDuration = @event.ConversationDuration;
    }

    private void Apply(RecordingOutboundEvent @event)
    {
        RecordedAt = @event.RecordedAt;
        Emails = @event.Emails;
        EmailSubject = @event.EmailSubject;
        EmailMessage = @event.EmailMessage;
        CallRecordingLink = @event.CallRecordingLink;
    }

    public static OutboundCallAggregate Rebuild(string uniqueCallId, IEnumerable<IOutboundCallEvent> events)
    {
        var aggregate = new OutboundCallAggregate(uniqueCallId);
        foreach (var @event in events)
        {
            aggregate.ApplyEvent(@event);
        }
        return aggregate;
    }
}
