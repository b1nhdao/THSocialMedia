using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Infrastructure.EventHandlers;

public class CommentUpdatedEventHandler : INotificationHandler<CommentUpdatedEvent>
{
    private readonly IPostReadRepository _postReadRepository;
    private readonly ILogger<CommentUpdatedEventHandler> _logger;

    public CommentUpdatedEventHandler(
        IPostReadRepository postReadRepository,
        ILogger<CommentUpdatedEventHandler> logger)
    {
        _postReadRepository = postReadRepository;
        _logger = logger;
    }

    public async Task Handle(CommentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _postReadRepository.GetPostByIdAsync(notification.PostId, cancellationToken);
            if (post == null)
            {
                _logger.LogWarning("Post {PostId} not found when updating comment", notification.PostId);
                return;
            }

            var existing = post.Comments.FirstOrDefault(c => c.Id == notification.CommentId);
            if (existing == null)
            {
                _logger.LogWarning("Comment {CommentId} not found in post {PostId} when updating", notification.CommentId, notification.PostId);
                return;
            }

            existing.Content = notification.Content;
            existing.FileUrl = notification.FileUrl;
            post.UpdatedAt = DateTime.UtcNow;

            await _postReadRepository.UpdatePostAsync(notification.PostId, post, cancellationToken);
            _logger.LogInformation("Comment {CommentId} updated in post {PostId} in MongoDB", notification.CommentId, notification.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId} in post {PostId}", notification.CommentId, notification.PostId);
            throw;
        }
    }
}
