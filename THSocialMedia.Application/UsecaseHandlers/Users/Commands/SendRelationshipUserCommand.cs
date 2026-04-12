using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Commands
{
    public class SendRelationshipUserCommand : IRequest<Result<Guid>>
    {
        public Guid SenderId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}
