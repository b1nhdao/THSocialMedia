using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;
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

        public CreatePostCommandHandler(
            IPostRepository postRepository,
            IRelationshipRepository relationshipRepository,
            IUserRepository userRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _postRepository = postRepository;
            _relationshipRepository = relationshipRepository;
            _userRepository = userRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
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

            var user = await _userRepository.GetFirstOrDefault(x => x.Id == userId, include: x => x.Include(x => x.Relationships));
            var connectionFollowed = user.Relationships.Where(r => r.SenderId == userId).Select(r => r.ReceiverId)
                .Union(user.Relationships.Where(r => r.ReceiverId == userId).Select(r => r.SenderId))
                .Distinct()
                .ToList();

            var connectedUserIds = await _relationshipRepository.GetConnectedUserIdsAsync(userId, status: null, cancellationToken);

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

            return Result<Guid>.Success(post.Id);
        }
    }
}
