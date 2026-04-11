using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;
using THSocialMedia.Application.UsecaseHandlers.Users.Queries;
using THSocialMedia.Application.UsecaseHandlers.Users.ViewModels;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserViewModel>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserViewModel>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstOrDefault(x => x.Id == request.Id, include: x => x.Include(x => x.Posts));

            if (user == null)
                return Result<UserViewModel>.NotFound();

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                CreatedAt = user.CreatedAt,
            };

            return Result<UserViewModel>.Success(userViewModel);
        }
    }
}