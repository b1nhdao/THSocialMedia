namespace THSocialMedia.Domain.Abstractions
{
    public interface IEventBus
    {
        Task PublishEventAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;
    }
}
