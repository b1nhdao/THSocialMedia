using Microsoft.Extensions.Logging;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result<Guid>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly ILogger<UpdatePostCommandHandler> _logger;

        public UpdatePostCommandHandler(
            IPostRepository postRepository,
            IUnitOfWork unitOfWork,
            IEventBus eventBus,
            ILogger<UpdatePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id);

            if (post == null)
                return Result<Guid>.NotFound();

            if (!string.IsNullOrEmpty(request.Content))
                post.Content = request.Content;
            if (request.Visibility.HasValue)
                post.Visibility = request.Visibility.Value;
            if (request.FileUrls != null)
                post.FileUrls = request.FileUrls;

            _postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish event to sync to MongoDB read database
            try
            {
                var @event = new PostUpdatedEvent(post.Id, post.UserId, post.Content, post.Visibility, post.FileUrls);
                await _eventBus.PublishEventAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish PostUpdatedEvent for post {PostId}. Post updated successfully but read database may not be synced.", post.Id);
            }

            return Result<Guid>.Success(post.Id);
        }
    }
}