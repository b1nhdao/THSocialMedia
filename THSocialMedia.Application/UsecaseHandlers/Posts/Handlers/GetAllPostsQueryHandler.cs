using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Infrastructure.Services.RedisCache;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, Result<IEnumerable<PostViewModel>>>
    {
        private readonly IPostRepository _postRepository;
        private readonly ICacheService _cacheService;
        private readonly IIdentityService _identityService;

        public GetAllPostsQueryHandler(IPostRepository postRepository, ICacheService cacheService, IIdentityService identityService)
        {
            _postRepository = postRepository;
            _cacheService = cacheService;
            _identityService = identityService;
        }

        public async Task<Result<IEnumerable<PostViewModel>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetUserIdentity();

            var cachedPosts = await _cacheService.GetTimelineFanOutOnReadAsync<Post>(
                entityKeyPrefix: "post",
                timelineKeyPrefix: "timeline",
                readCacheKeyPrefix: "feed",
                userId: userId,
                fetchFromSource: async () => await _postRepository.GetAllAsync(
                    predicate: x => x.IsDeleted == false,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    include: x => x.Include(x => x.Reactions).Include(x => x.Comments)),
                getEntityId: p => p.Id,
                getScoreTime: p => new DateTimeOffset(p.CreatedAt, TimeSpan.Zero),
                take: 50,
                readCacheTtl: TimeSpan.FromMinutes(2),
                entityTtl: TimeSpan.FromHours(6));

            var postViewModels = cachedPosts.Select(p => new PostViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                Content = p.Content,
                Visibility = p.Visibility,
                FileUrls = p.FileUrls,
                CreatedAt = p.CreatedAt,
                IsDeleted = p.IsDeleted,
                CommentsCount = p.Comments.Count
            }).ToList();

            return Result<IEnumerable<PostViewModel>>.Success(postViewModels);
        }
    }
}