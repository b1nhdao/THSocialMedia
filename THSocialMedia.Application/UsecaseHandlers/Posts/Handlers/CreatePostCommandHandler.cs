using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Domain.Events;
using THSocialMedia.Infrastructure.Services.RedisCache;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<Guid>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CreatePostCommandHandler> _logger;

        public CreatePostCommandHandler(
            IPostRepository postRepository,
            IRelationshipRepository relationshipRepository,
            IUserRepository userRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IEventBus eventBus,
            ILogger<CreatePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _relationshipRepository = relationshipRepository;
            _userRepository = userRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetUserIdentity();

            var post = new Post
            {
                UserId = userId,
                Content = request.Content,
                Visibility = request.Visibility,
                FileUrls = request.FileUrls,
            };

            _postRepository.Add(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var user = await _userRepository.GetFirstOrDefault(x => x.Id == userId);
            var connectedUserIds = await _relationshipRepository.GetConnectedUserIdsAsync(userId, status: null, cancellationToken);

            try
            {
                await _cacheService.FanOutOnWriteAsync(
                    entityKeyPrefix: "post",
                    timelineKeyPrefix: "timeline",
                    readCacheKeyPrefix: "feed",
                    actorId: userId,
                    entity: post,
                    getEntityId: p => p.Id,
                    getScoreTime: p => new DateTimeOffset(p.CreatedAt, TimeSpan.Zero),
                    recipientIds: connectedUserIds,
                    entityTtl: TimeSpan.FromHours(6),
                    includeActor: true,
                    invalidateReadCache: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cache for new post {PostId}. Post created successfully but cache may be inconsistent.", post.Id);
            }

            // Publish event to sync to MongoDB read database
            try
            {
                var @event = new PostCreatedEvent(post.Id, userId, post.Content, post.Visibility, post.FileUrls, user.Username);
                await _eventBus.PublishEventAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish PostCreatedEvent for post {PostId}. Post created successfully but read database may not be synced.", post.Id);
            }

            return Result<Guid>.Success(post.Id);
        }
    }
}
