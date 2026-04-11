using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Commands
{
    public class SendRelationshipUserCommand : IRequest<Result<bool>>
    {
        public Guid TargetUserId { get; set; }
    }
}
