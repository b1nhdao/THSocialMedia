using MediatR;
using THSocialMedia.Domain.Abstractions;

namespace THSocialMedia.Domain.Events
{
    public class ReactionAddedEvent : IDomainEvent, INotification
    {
        public Guid AggregateId { get; }
        public DateTime CreatedAt { get; }
        public string EventType => nameof(ReactionAddedEvent);

        public Guid PostId { get; }
        public Guid UserId { get; }
        public Guid ReactionId { get; }

        public ReactionAddedEvent(Guid postId, Guid userId, Guid reactionId)
        {
            PostId = postId;
            AggregateId = postId;
            UserId = userId;
            ReactionId = reactionId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
