using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class GetPostByIdReadQueryHandler : IRequestHandler<GetPostByIdReadQuery, Result<PostViewModel>>
    {
        private readonly IPostReadRepository _postReadRepository;
        private readonly ILogger<GetPostByIdReadQueryHandler> _logger;

        public GetPostByIdReadQueryHandler(
            IPostReadRepository postReadRepository,
            ILogger<GetPostByIdReadQueryHandler> logger)
        {
            _postReadRepository = postReadRepository;
            _logger = logger;
        }

        public async Task<Result<PostViewModel>> Handle(GetPostByIdReadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var postReadModel = await _postReadRepository.GetPostByIdAsync(request.Id, cancellationToken);

                if (postReadModel == null)
                {
                    return Result.NotFound($"Post with ID {request.Id} not found");
                }

                var postViewModel = new PostViewModel
                {
                    Id = postReadModel.Id,
                    UserId = postReadModel.UserId,
                    Content = postReadModel.Content,
                    Visibility = postReadModel.Visibility,
                    FileUrls = postReadModel.FileUrls,
                    CreatedAt = postReadModel.CreatedAt,
                    IsDeleted = false,
                    CommentsCount = postReadModel.CommentsCount,
                    ReactionCounts = new List<ReactionCount>
                    {
                        new ReactionCount { Type = 1, Name = "Like", Count = postReadModel.ReactionsCount }
                    }
                };

                return Result.Success(postViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving post {PostId} from read database", request.Id);
                return Result.Error($"Error retrieving post: {ex.Message}");
            }
        }
    }
}
