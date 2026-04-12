using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class GetAllPostsReadQueryHandler : IRequestHandler<GetAllPostsReadQuery, Result<IEnumerable<PostViewModel>>>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly ILogger<GetAllPostsReadQueryHandler> _logger;

        public GetAllPostsReadQueryHandler(
            IPostReadRepository postReadRepository,
            ILogger<GetAllPostsReadQueryHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<PostViewModel>>> Handle(GetAllPostsReadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var postReadModels = await _postReadRepository.GetAllPostsAsync(cancellationToken);

                var postViewModels = postReadModels.Select(p => new PostViewModel
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Content = p.Content,
                    Visibility = p.Visibility,
                    FileUrls = p.FileUrls,
                    CreatedAt = p.CreatedAt,
                    IsDeleted = false,
                    CommentsCount = p.CommentsCount,
                    ReactionCounts = new List<ReactionCount>
                    {
                        new ReactionCount { Type = 1, Name = "Like", Count = p.ReactionsCount }
                    }
                }).ToList();

                return Result.Success((IEnumerable<PostViewModel>)postViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving posts from read database");
                return Result.Error($"Error retrieving posts: {ex.Message}");
            }
        }
    }
}
