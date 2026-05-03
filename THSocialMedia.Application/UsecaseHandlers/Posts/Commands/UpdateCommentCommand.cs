using System.Text.Json.Serialization;
using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Commands;

public class UpdateCommentCommand : IRequest<Result<Guid>>
{
    [JsonIgnore]
    public Guid PostId { get; set; }

    [JsonIgnore]
    public Guid CommentId { get; set; }

    public string Content { get; set; } = string.Empty;

    public string? FileUrl { get; set; }
}
