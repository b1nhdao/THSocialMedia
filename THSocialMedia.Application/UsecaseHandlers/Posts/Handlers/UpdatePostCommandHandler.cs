using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Posts.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Handlers
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result<Guid>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id);

            if (post == null)
                return Result<Guid>.NotFound();

            if (!string.IsNullOrEmpty(request.Content))
                post.Content = request.Content;
            if (request.Visibility.HasValue)
                post.Visibility = request.Visibility.Value;
            if (!string.IsNullOrEmpty(request.FileUrls))
                post.FileUrls = request.FileUrls;

            _postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(post.Id);
        }
    }
}