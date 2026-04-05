using System.Text.Json.Serialization;
using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Commands
{
    public class AddReactionPostCommand : IRequest<Result<Guid>>
    {
        public Guid ReactionId { get; set; }
        [JsonIgnore]
        public Guid PostId { get; set; }
    }
}
