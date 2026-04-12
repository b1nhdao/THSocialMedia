using MediatR;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Events;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;
using THSocialMedia.Infrastructure.MongoDb.ReadModels;

namespace THSocialMedia.Infrastructure.EventHandlers
{
    public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PostCreatedEventHandler> _logger;

        public PostCreatedEventHandler(
            IPostReadRepository postReadRepository,
            IUserRepository userRepository,
            ILogger<PostCreatedEventHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetFirstOrDefault(x => x.Id == notification.UserId);

                var postReadModel = new PostReadModel
                {
                    Id = notification.PostId,
                    UserId = notification.UserId,
                    UserName = user?.Username ?? "Unknown",
                    UserAvatar = user?.AvatarUrl ?? string.Empty,
                    Content = notification.Content,
                    Visibility = notification.Visibility,
                    FileUrls = notification.FileUrls,
                    CreatedAt = notification.CreatedAt,
                    UpdatedAt = notification.CreatedAt,
                    ReactionsCount = 0,
                    CommentsCount = 0
                };

                await _postReadRepository.CreatePostAsync(postReadModel, cancellationToken);
                _logger.LogInformation("Post {PostId} synced to MongoDB read database", notification.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing post {PostId} to MongoDB", notification.PostId);
                throw;
            }
        }
    }
}
