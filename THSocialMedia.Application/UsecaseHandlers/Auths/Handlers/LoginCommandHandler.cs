using System.Security.Claims;
using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Auths.Commands;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Infrastructure.Services.Jwt;

namespace THSocialMedia.Application.UsecaseHandlers.Auths.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenProvider _jwtTokenProvider;

        public LoginCommandHandler(IUserRepository userRepository, IJwtTokenProvider jwtTokenProvider)
        {
            _userRepository = userRepository;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // tam thoi de ko hash: compare plain string with PasswordHash
            var user = await _userRepository.GetFirstOrDefault(
                x => x.Username == request.UserName && x.IsDeleted == false);

            if (user is null)
                return Result<string>.Unauthorized();

            if (!string.Equals(user.PasswordHash ?? string.Empty, request.Password ?? string.Empty, StringComparison.Ordinal))
                return Result<string>.Unauthorized();

            var claims = new List<Claim>
            {
                new("UserGuid", user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
            };

            var token = _jwtTokenProvider.CreateAccessToken(claims);
            return Result<string>.Success(token);
        }
    }
}
