using MediatR;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Infrastructure.EventHandlers
{
    public class PostDeletedEventHandler : INotificationHandler<PostDeletedEvent>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly ILogger<PostDeletedEventHandler> _logger;

        public PostDeletedEventHandler(
            IPostReadRepository postReadRepository,
            ILogger<PostDeletedEventHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _logger = logger;
        }

        public async Task Handle(PostDeletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _postReadRepository.DeletePostAsync(notification.PostId, cancellationToken);
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
