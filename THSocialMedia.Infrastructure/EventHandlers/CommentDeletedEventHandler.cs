using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Infrastructure.EventHandlers;

public class CommentDeletedEventHandler : INotificationHandler<CommentDeletedEvent>
{
    private readonly IPostReadRepository _postReadRepository;
    private readonly ILogger<CommentDeletedEventHandler> _logger;

    public CommentDeletedEventHandler(
        IPostReadRepository postReadRepository,
        ILogger<CommentDeletedEventHandler> logger)
    {
        _postReadRepository = postReadRepository;
        _logger = logger;
    }

    public async Task Handle(CommentDeletedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _postReadRepository.GetPostByIdAsync(notification.PostId, cancellationToken);
            if (post == null)
            {
                _logger.LogWarning("Post {PostId} not found when deleting comment", notification.PostId);
                return;
            }

            var removed = post.Comments.RemoveAll(c => c.Id == notification.CommentId);
            if (removed == 0)
            {
                _logger.LogWarning("Comment {CommentId} not found in post {PostId} when deleting", notification.CommentId, notification.PostId);
                return;
            }

            post.CommentsCount = post.Comments.Count;
            post.UpdatedAt = DateTime.UtcNow;

            await _postReadRepository.UpdatePostAsync(notification.PostId, post, cancellationToken);
            _logger.LogInformation("Comment {CommentId} deleted from post {PostId} in MongoDB", notification.CommentId, notification.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId} from post {PostId}", notification.CommentId, notification.PostId);
            throw;
        }
    }
}
