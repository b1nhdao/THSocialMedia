using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class CommentPostCommandHandler : IRequestHandler<CommentPostCommand, Result<Guid>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public CommentPostCommandHandler(IPostRepository postRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CommentPostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetFirstOrDefault(x => x.Id == request.PostId);
            var userId = _identityService.GetUserIdentity();

            var comment = new Comment
            {
                Content = request.Content,
                FileUrl = request.FileUrl,
                UserId = userId
            };

            post.AddComment(comment);

            _postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(comment.Id);
        }
    }
}
