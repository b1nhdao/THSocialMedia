using MediatR;
using THSocialMedia.Domain.Abstractions;

namespace THSocialMedia.Domain.Events
{
    public class PostDeletedEvent : IDomainEvent, INotification
    {
        public Guid AggregateId { get; }
        public DateTime CreatedAt { get; }
        public string EventType => nameof(PostDeletedEvent);

        public Guid PostId { get; }
        public Guid UserId { get; }

        public PostDeletedEvent(Guid postId, Guid userId)
        {
            PostId = postId;
            AggregateId = postId;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
