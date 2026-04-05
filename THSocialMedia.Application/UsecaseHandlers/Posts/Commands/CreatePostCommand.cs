using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Commands
{
    public class CreatePostCommand : IRequest<Result<Guid>>
    {
        public string Content { get; set; } = string.Empty;
        public int Visibility { get; set; }
        public string? FileUrls { get; set; }
    }
}
