using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.UsecaseHandlers.Posts.Queries;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result<PostViewModel>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository userRepository;

        public GetPostByIdQueryHandler(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            this.userRepository = userRepository;
        }

        public async Task<Result<PostViewModel>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetFirstOrDefault(x => x.Id == request.Id, include: x => x.Include(x => x.Reactions).Include(x => x.Comments));

            var comments = post.Comments.ToList();
            var reactions = post.Reactions.ToList();

            var listUserIds = comments.Select(c => c.UserId).Union(reactions.Select(r => r.UserId)).ToList();

            var listUsers = await userRepository.GetAllAsync(x => listUserIds.Contains(x.Id));

            if (post == null)
                return Result<PostViewModel>.NotFound();

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                UserId = post.UserId,
                Content = post.Content,
                Visibility = post.Visibility,
                FileUrls = post.FileUrls,
                CreatedAt = post.CreatedAt,
                IsDeleted = post.IsDeleted,
                CommentsCount = post.Comments.Count,
                ReactionCounts = post.Reactions.GroupBy(r => r.Name).Select(g => new ReactionCount
                {
                    Name = g.Key,
                    Count = g.Count()
                }).ToList(),
                Comments = post.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    FileUrl = c.FileUrl,
                    UserName = listUsers.FirstOrDefault(u => u.Id == c.UserId)?.Username ?? "Unknown"
                }).ToList(),
                ReactionViewModels = post.Reactions.Select(r => new ReactionViewModel
                {
                    Id = r.ReactionId,
                    Name = r.Name,
                    UserName = listUsers.FirstOrDefault(u => u.Id == r.UserId)?.Username ?? "Unknown"
                }).ToList(),
            };

            return Result<PostViewModel>.Success(postViewModel);
        }
    }
}