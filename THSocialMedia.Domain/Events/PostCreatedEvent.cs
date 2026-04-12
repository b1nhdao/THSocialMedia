using MediatR;
using THSocialMedia.Domain.Abstractions;

namespace THSocialMedia.Domain.Events
{
    public class PostCreatedEvent : IDomainEvent, INotification
    {
        public Guid AggregateId { get; }
        public DateTime CreatedAt { get; }
        public string EventType => nameof(PostCreatedEvent);

        public Guid PostId { get; }
        public Guid UserId { get; }
        public string Content { get; }
        public int Visibility { get; }
        public string? FileUrls { get; }

        public PostCreatedEvent(Guid postId, Guid userId, string content, int visibility, string? fileUrls)
        {
            PostId = postId;
            AggregateId = postId;
            UserId = userId;
            Content = content;
            Visibility = visibility;
            FileUrls = fileUrls;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
