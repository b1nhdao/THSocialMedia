using MediatR;
using MediatR;
using THSocialMedia.Domain.Abstractions;

namespace THSocialMedia.Infrastructure.EventBus
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IMediator _mediator;

        public InMemoryEventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishEventAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
        {
            // Publish the domain event as a MediatR notification
            // Domain events implement INotification, so MediatR will handle this
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
