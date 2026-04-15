using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Events;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Result<bool>>
    {
        private readonly IPostWriteRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public DeletePostCommandHandler(
            IPostWriteRepository postRepository,
            IUnitOfWork unitOfWork,
            IEventBus eventBus,
            ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id);

            if (post == null)
                return Result<bool>.NotFound();

            _postRepository.Remove(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish event to sync to MongoDB read database
            try
            {
                var @event = new PostDeletedEvent(post.Id, post.UserId);
                await _eventBus.PublishEventAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish PostDeletedEvent for post {PostId}. Post deleted successfully but read database may not be synced.", post.Id);
            }

            return Result<bool>.Success(true);
        }
    }
}