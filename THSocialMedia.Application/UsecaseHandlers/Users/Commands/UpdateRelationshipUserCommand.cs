using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Commands
{
    public class UpdateRelationshipUserCommand : IRequest<Result<bool>>
    {
        public Guid SenderId { get; set; }
        public Guid TargetUserId { get; set; }
        public int Status { get; set; }
    }
}
