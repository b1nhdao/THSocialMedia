using MediatR;
using THSocialMedia.Domain.Abstractions;

namespace THSocialMedia.Domain.Events;

public class CommentDeletedEvent : IDomainEvent, INotification
{
    public Guid AggregateId { get; }
    public DateTime CreatedAt { get; }
    public string EventType => nameof(CommentDeletedEvent);

    public Guid PostId { get; }
    public Guid CommentId { get; }

    public CommentDeletedEvent(Guid postId, Guid commentId)
    {
        PostId = postId;
        AggregateId = postId;
        CommentId = commentId;
        CreatedAt = DateTime.UtcNow;
    }
}
