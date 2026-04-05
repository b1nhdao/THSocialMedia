using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<Guid>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePostCommandHandler(IPostRepository postRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(post.Id);
        }
    }
}
