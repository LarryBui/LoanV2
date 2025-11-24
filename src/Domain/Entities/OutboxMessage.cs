using System.Text.Json;
using OaigLoan.Domain.Common;

namespace OaigLoan.Domain.Entities;

public class OutboxMessage
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private OutboxMessage()
    {
    }

    private OutboxMessage(Guid id, string type, string content, DateTime occurredOnUtc)
    {
        Id = id;
        Type = type;
        Content = content;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public static OutboxMessage FromDomainEvent(IDomainEvent domainEvent)
    {
        var type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name;
        var content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), SerializerOptions);
        return new OutboxMessage(Guid.NewGuid(), type, content, domainEvent.OccurredOnUtc);
    }

    public void MarkProcessed(DateTime processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
    }

    public void MarkFailed(string error)
    {
        Error = error;
    }
}
