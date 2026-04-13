using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Abstractions.IReadRepositories.ReadModels;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Infrastructure.EventHandlers
{
    public class PostUpdatedEventHandler : INotificationHandler<PostUpdatedEvent>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly ILogger<PostUpdatedEventHandler> _logger;

        public PostUpdatedEventHandler(
            IPostReadRepository postReadRepository,
            ILogger<PostUpdatedEventHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _logger = logger;
        }

        public async Task Handle(PostUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var existingPost = await _postReadRepository.GetPostByIdAsync(notification.PostId, cancellationToken);
                if (existingPost == null)
                {
                    _logger.LogWarning("Post {PostId} not found in MongoDB when updating", notification.PostId);
                    return;
                }

                var updatedPost = new PostReadModel
                {
                    Id = notification.PostId,
                    UserId = notification.UserId,
                    UserName = existingPost.UserName,
                    UserAvatar = existingPost.UserAvatar,
                    Content = notification.Content,
                    Visibility = notification.Visibility,
                    FileUrls = notification.FileUrls,
                    CreatedAt = existingPost.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    ReactionsCount = existingPost.ReactionsCount,
                    CommentsCount = existingPost.CommentsCount
                };

                await _postReadRepository.UpdatePostAsync(notification.PostId, updatedPost, cancellationToken);
                _logger.LogInformation("Post {PostId} updated in MongoDB read database", notification.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post {PostId} in MongoDB", notification.PostId);
                throw;
            }
        }
    }
}
