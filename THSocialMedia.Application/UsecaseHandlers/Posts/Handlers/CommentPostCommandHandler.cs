using Microsoft.Extensions.Logging;
using THSocialMedia.Application.Services;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class CommentPostCommandHandler : IRequestHandler<CommentPostCommand, Result<Guid>>
    {
        private readonly IPostWriteRepository _postRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CommentPostCommandHandler> _logger;

        public CommentPostCommandHandler(
            IPostWriteRepository postRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork,
            IEventBus eventBus,
            ILogger<CommentPostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CommentPostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetFirstOrDefault(x => x.Id == request.PostId);
            var userId = _identityService.GetUserIdentity();

            var comment = new Comment
            {
                Content = request.Content,
                FileUrl = request.FileUrl,
                UserId = userId
            };

            post.AddComment(comment);

            _postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish event to sync to MongoDB
            try
            {
                var @event = new CommentAddedEvent(request.PostId, comment.Id, userId, request.Content, request.FileUrl);
                await _eventBus.PublishEventAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish CommentAddedEvent for post {PostId}", request.PostId);
            }

            return Result<Guid>.Success(comment.Id);
        }
    }
}
