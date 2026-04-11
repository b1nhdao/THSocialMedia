using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Commands
{
    public class CreateUserCommand : IRequest<Result<Guid>>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}