using MediatR;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.EventHandlers
{
    public class PostDeletedEventHandler : INotificationHandler<PostDeletedEvent>
    {
        private readonly IBasePostReadRepository _postReadRepository;
        private readonly ILogger<PostDeletedEventHandler> _logger;

        public PostDeletedEventHandler(
            IBasePostReadRepository postReadRepository,
            ILogger<PostDeletedEventHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _logger = logger;
        }

        public async Task Handle(PostDeletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _postReadRepository.DeleteAsync(notification.PostId, cancellationToken);
                _logger.LogInformation("Post {PostId} deleted from MongoDB read database", notification.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId} from MongoDB", notification.PostId);
                throw;
            }
        }
    }
}
