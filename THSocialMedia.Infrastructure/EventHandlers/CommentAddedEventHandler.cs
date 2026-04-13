using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Abstractions.IReadRepositories.ReadModels;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Infrastructure.EventHandlers
{
    public class CommentAddedEventHandler : INotificationHandler<CommentAddedEvent>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CommentAddedEventHandler> _logger;

        public CommentAddedEventHandler(
            IPostReadRepository postReadRepository,
            IUserRepository userRepository,
            ILogger<CommentAddedEventHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postReadRepository.GetPostByIdAsync(notification.PostId, cancellationToken);
                if (post == null)
                {
                    _logger.LogWarning("Post {PostId} not found when adding comment", notification.PostId);
                    return;
                }

                var user = await _userRepository.GetFirstOrDefault(x => x.Id == notification.UserId);

                var commentReadModel = new CommentReadModel
                {
                    Id = notification.CommentId,
                    UserId = notification.UserId,
                    UserName = user?.Username ?? "Unknown",
                    Content = notification.Content,
                    FileUrl = notification.FileUrl,
                    CreatedAt = notification.CreatedAt
                };

                post.Comments.Add(commentReadModel);
                post.CommentsCount = post.Comments.Count;
                post.UpdatedAt = DateTime.UtcNow;

                await _postReadRepository.UpdatePostAsync(notification.PostId, post, cancellationToken);
                _logger.LogInformation("Comment {CommentId} added to post {PostId} in MongoDB", 
                    notification.CommentId, notification.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment {CommentId} to post {PostId}", 
                    notification.CommentId, notification.PostId);
                throw;
            }
        }
    }
}
