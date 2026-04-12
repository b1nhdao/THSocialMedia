using MediatR;
using THSocialMedia.Domain.Abstractions;

namespace THSocialMedia.Domain.Events
{
    public class CommentAddedEvent : IDomainEvent, INotification
    {
        public Guid AggregateId { get; }
        public DateTime CreatedAt { get; }
        public string EventType => nameof(CommentAddedEvent);

        public Guid PostId { get; }
        public Guid CommentId { get; }
        public Guid UserId { get; }
        public string Content { get; }
        public string? FileUrl { get; }

        public CommentAddedEvent(Guid postId, Guid commentId, Guid userId, string content, string? fileUrl)
        {
            PostId = postId;
            AggregateId = postId;
            CommentId = commentId;
            UserId = userId;
            Content = content;
            FileUrl = fileUrl;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
