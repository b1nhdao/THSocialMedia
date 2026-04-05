using System.Text.Json.Serialization;
using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Commands
{
    public class CommentPostCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; } = string.Empty;
    }
}
