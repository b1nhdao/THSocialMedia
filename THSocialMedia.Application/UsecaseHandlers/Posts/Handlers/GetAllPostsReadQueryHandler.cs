using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;
using THSocialMedia.Infrastructure.MongoDb.ReadModels;
using THSocialMedia.Infrastructure.Services.RedisCache;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class GetAllPostsReadQueryHandler : IRequestHandler<GetAllPostsReadQuery, Result<IEnumerable<PostViewModel>>>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetAllPostsReadQueryHandler> _logger;

        public GetAllPostsReadQueryHandler(
            IPostReadRepository postReadRepository,
            ICacheService cacheService,
            ILogger<GetAllPostsReadQueryHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<PostViewModel>>> Handle(GetAllPostsReadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Use Fan-Out on Read with cache
                var cachedPosts = await _cacheService.GetTimelineFanOutOnReadAsync<PostReadModel>(
                    entityKeyPrefix: "post",
                    timelineKeyPrefix: "timeline",
                    readCacheKeyPrefix: "feed",
                    userId: Guid.Empty, // Use empty GUID as key for "all posts" feed
                    fetchFromSource: async () => await _postReadRepository.GetAllPostsAsync(cancellationToken),
                    getEntityId: p => p.Id,
                    getScoreTime: p => new DateTimeOffset(p.CreatedAt, TimeSpan.Zero),
                    take: 50,
                    readCacheTtl: TimeSpan.FromHours(1),
                    entityTtl: TimeSpan.FromHours(6)
                );

                var postViewModels = cachedPosts.Select(p => new PostViewModel
                {
                    Id = p.Id,
                    Username = p.UserName,
                    UserId = p.UserId,
                    Content = p.Content,
                    Visibility = p.Visibility,
                    FileUrls = p.FileUrls,
                    CreatedAt = p.CreatedAt,
                    IsDeleted = false,
                    CommentsCount = p.CommentsCount,
                    Comments = p.Comments.Select(c => new CommentViewModel
                    {
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UserName = c.UserName,
                        Id = c.Id
                    })
                    .ToList(),
                    ReactionCounts = new List<ReactionCount>
                    {
                        new ReactionCount { Type = 1, Name = "Like", Count = p.ReactionsCount }
                    }
                }).ToList();

                _logger.LogInformation("Retrieved {PostCount} posts from cache/database", postViewModels.Count);
                return Result.Success((IEnumerable<PostViewModel>)postViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving posts from cache/read database");
                return Result.Error($"Error retrieving posts: {ex.Message}");
            }
        }
    }
}
