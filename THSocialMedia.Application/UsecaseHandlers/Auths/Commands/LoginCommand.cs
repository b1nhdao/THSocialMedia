using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Auths.Commands
{
    public class LoginCommand : IRequest<Result<string>>
    {
        public string UserName { get; set; } = string.Empty;
        // tam thoi de ko hash
        public string Password { get; set; } = string.Empty;
    }
}
