using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Users.Queries;
using THSocialMedia.Application.UsecaseHandlers.Users.ViewModels;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserViewModel>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<UserViewModel>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();

            var userViewModels = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                AvatarUrl = u.AvatarUrl,
                Bio = u.Bio,
                CreatedAt = u.CreatedAt,
            }).ToList();

            return Result<IEnumerable<UserViewModel>>.Success(userViewModels);
        }
    }
}