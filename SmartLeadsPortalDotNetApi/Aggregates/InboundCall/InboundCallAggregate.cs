using System;

namespace SmartLeadsPortalDotNetApi.Aggregates.InboundCall;

public class InboundCallAggregate
{
    public string? UniqueCallId { get; private set; }
    public string? CallerId { get; private set; }
    public string? CallerName { get; private set; }
    public string? UserName { get; private set; }
    public string? UserNumber { get; private set; }
    public string? DestNumber { get; private set; }
    public DateTime? CallStartAt { get; private set; }
    public string? QueueName { get; set; }
    public string? Status { get; set; }
    public string? RingGroupName { get; set; }
    public DateTime? ConnectedAt { get; private set; }
    public int? CallDuration { get; private set; }
    public int? ConversationDuration { get; private set; }
    public DateTime? RecordedAt { get; private set; }
    public string? Emails { get; private set; }
    public string? EmailSubject { get; private set; }
    public string? EmailMessage { get; private set; }
    public string? CallRecordingLink { get; private set; }
    public List<string>? AudioFile { get; private set; }
    public string? LastEventType { get; private set; }

    private readonly List<IInboundCallEvent> _events = new List<IInboundCallEvent>();

    public IReadOnlyCollection<IInboundCallEvent> Events => _events.AsReadOnly();

    public InboundCallAggregate()
    {
    }

    public InboundCallAggregate(string uniqueCallId)
    {
        UniqueCallId = uniqueCallId;
    }

    public void ApplyEvent(IInboundCallEvent @event)
    {
        if (@event.UniqueCallId != UniqueCallId)
            throw new ArgumentException("Event does not belong to this aggregate");

        _events.Add(@event);
        LastEventType = @event.Type;

        switch (@event)
        {
            case UserInboundEvent e:
                Apply(e);
                break;
            case UserInboundAnsweredEvent e:
                Apply(e);
                break;
            case UserInboundCompletedEvent e:
                Apply(e);
                break;
            case QueueCallEvent e:
                Apply(e);
                break;
            case RingGroupCallEvent e:
                Apply(e);
                break;
            case VoicemailEvent e:
                Apply(e);
                break;
            case RecordingInboundEvent e:
                Apply(e);
                break;
        }
    }

    private void Apply(UserInboundEvent @event)
    {
        CallerId = @event.CallerId;
        UserName = @event.UserName;
        UserNumber = @event.UserNumber;
        DestNumber = @event.DestNumber;
        CallStartAt = @event.CallStartAt;
        QueueName = @event.QueueName;
        RingGroupName = @event.RingGroupName;
    }

    private void Apply(UserInboundAnsweredEvent @event)
    {
        ConnectedAt = @event.ConnectedAt;
        QueueName = @event.QueueName;
        RingGroupName = @event.RingGroupName;
    }

    private void Apply(UserInboundCompletedEvent @event)
    {
        CallDuration = @event.CallDuration;
        ConversationDuration = @event.ConversationDuration;
        QueueName = @event.QueueName;
        RingGroupName = @event.RingGroupName;
    }

    private void Apply(QueueCallEvent @event)
    {
        CallerName = @event.CallerName;
        QueueName = @event.QueueName;
        Status = @event.Status;
    }

    private void Apply(RingGroupCallEvent @event)
    {
        CallerName = @event.CallerName;
        RingGroupName = @event.RingGroupName;
        Status = @event.Status;
    }

    private void Apply(VoicemailEvent @event)
    {
        RecordedAt = @event.RecordedAt;
        Emails = @event.Emails;
        EmailSubject = @event.EmailSubject;
        EmailMessage = @event.EmailMessage;
        AudioFile = @event.AudioFile;
    }

    private void Apply(RecordingInboundEvent @event)
    {
        RecordedAt = @event.RecordedAt;
        Emails = @event.Emails;
        EmailSubject = @event.EmailSubject;
        EmailMessage = @event.EmailMessage;
        CallRecordingLink = @event.CallRecordingLink;
    }

    public static InboundCallAggregate Rebuild(string uniqueCallId, IEnumerable<IInboundCallEvent> events)
    {
        var aggregate = new InboundCallAggregate(uniqueCallId);
        foreach (var @event in events)
        {
            aggregate.ApplyEvent(@event);
        }
        return aggregate;
    }
}
