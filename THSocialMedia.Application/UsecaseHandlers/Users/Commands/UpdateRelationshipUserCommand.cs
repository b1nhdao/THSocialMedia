using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Commands
{
    public class UpdateRelationshipUserCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 1 = accepted, 2 = rejected
        /// </summary>
        public int Status { get; set; }
    }
}
