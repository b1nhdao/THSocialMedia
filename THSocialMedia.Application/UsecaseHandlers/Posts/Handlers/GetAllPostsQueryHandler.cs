using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, Result<IEnumerable<PostViewModel>>>
    {
        private readonly IPostRepository _postRepository;

        public GetAllPostsQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Result<IEnumerable<PostViewModel>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            var posts = await _postRepository.GetAllAsync(predicate: x => x.IsDeleted == false, include: x => x.Include(x => x.Reactions).Include(x => x.Comments));

            var postViewModels = posts.Select(p => new PostViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                Content = p.Content,
                Visibility = p.Visibility,
                FileUrls = p.FileUrls,
                CreatedAt = p.CreatedAt,
                IsDeleted = p.IsDeleted,
                CommentsCount = p.Comments.Count()
            }).ToList();

            return Result<IEnumerable<PostViewModel>>.Success(postViewModels);
        }
    }
}