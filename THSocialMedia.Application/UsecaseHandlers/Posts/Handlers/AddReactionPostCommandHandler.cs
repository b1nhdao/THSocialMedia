using Microsoft.Extensions.Logging;
using THSocialMedia.Application.Services;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class AddReactionPostCommandHandler : IRequestHandler<AddReactionPostCommand, Result<Guid>>
    {
        private readonly IPostWriteRepository _postRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly ILogger<AddReactionPostCommandHandler> _logger;

        public AddReactionPostCommandHandler(
            IPostWriteRepository postRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork,
            IEventBus eventBus,
            ILogger<AddReactionPostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(AddReactionPostCommand request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetUserIdentity();
            var post = await _postRepository.GetFirstOrDefault(x => x.Id == request.PostId);

            var reaction = new ReactionPost
            {
                UserId = userId,
                PostId = request.PostId,
                ReactionId = request.ReactionId
            };

            post.AddReaction(reaction);
            _postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish event to sync to MongoDB
            try
            {
                var @event = new ReactionAddedEvent(request.PostId, userId, request.ReactionId);
                await _eventBus.PublishEventAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish ReactionAddedEvent for post {PostId}", request.PostId);
            }

            return Result.Success(reaction.ReactionId);
        }
    }
}
