using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Commands
{
    public class UpdatePostCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public int? Visibility { get; set; }
        public List<string>? FileUrls { get; set; }
    }
}