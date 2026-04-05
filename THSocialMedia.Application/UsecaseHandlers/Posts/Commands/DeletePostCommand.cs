using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Commands
{
    public class DeletePostCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}