using System.Text.Json.Serialization;
using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Commands;

public class DeleteCommentCommand : IRequest<Result<bool>>
{
    [JsonIgnore]
    public Guid PostId { get; set; }

    [JsonIgnore]
    public Guid CommentId { get; set; }
}
