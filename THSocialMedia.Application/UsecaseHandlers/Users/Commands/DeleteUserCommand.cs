using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}