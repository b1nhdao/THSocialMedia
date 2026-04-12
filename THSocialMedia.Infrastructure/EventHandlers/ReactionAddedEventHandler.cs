using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Events;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;
using THSocialMedia.Infrastructure.MongoDb.ReadModels;

namespace THSocialMedia.Infrastructure.EventHandlers
{
    public class ReactionAddedEventHandler : INotificationHandler<ReactionAddedEvent>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ReactionAddedEventHandler> _logger;

        public ReactionAddedEventHandler(
            IPostReadRepository postReadRepository,
            IUserRepository userRepository,
            ILogger<ReactionAddedEventHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(ReactionAddedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postReadRepository.GetPostByIdAsync(notification.PostId, cancellationToken);
                if (post == null)
                {
                    _logger.LogWarning("Post {PostId} not found when adding reaction", notification.PostId);
                    return;
                }

                var user = await _userRepository.GetFirstOrDefault(x => x.Id == notification.UserId);

                var reactionReadModel = new ReactionReadModel
                {
                    Id = Guid.NewGuid(),
                    UserId = notification.UserId,
                    UserName = user?.Username ?? "Unknown",
                    ReactionId = notification.ReactionId,
                    ReactionType = GetReactionTypeName(notification.ReactionId),
                    CreatedAt = notification.CreatedAt
                };

                post.Reactions.Add(reactionReadModel);
                post.ReactionsCount = post.Reactions.Count;
                post.UpdatedAt = DateTime.UtcNow;

                await _postReadRepository.UpdatePostAsync(notification.PostId, post, cancellationToken);
                _logger.LogInformation("Reaction added to post {PostId} in MongoDB. Total reactions: {ReactionCount}", 
                    notification.PostId, post.ReactionsCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding reaction to post {PostId}", notification.PostId);
                throw;
            }
        }

        private string GetReactionTypeName(Guid reactionId)
        {
            // Map reaction IDs to names - adjust based on your reaction types
            return reactionId switch
            {
                _ when reactionId.ToString() == "00000000-0000-0000-0000-000000000001" => "Like",
                _ when reactionId.ToString() == "00000000-0000-0000-0000-000000000002" => "Love",
                _ when reactionId.ToString() == "00000000-0000-0000-0000-000000000003" => "Haha",
                _ when reactionId.ToString() == "00000000-0000-0000-0000-000000000004" => "Wow",
                _ when reactionId.ToString() == "00000000-0000-0000-0000-000000000005" => "Sad",
                _ => "Like" // Default reaction type
            };
        }
    }
}
