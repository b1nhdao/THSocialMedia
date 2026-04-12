namespace THSocialMedia.Domain.Abstractions
{
    public interface IDomainEvent
    {
        Guid AggregateId { get; }
        DateTime CreatedAt { get; }
        string EventType { get; }
    }
}
