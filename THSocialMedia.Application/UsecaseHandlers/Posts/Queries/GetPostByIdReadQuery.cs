using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Queries
{
    public class GetPostByIdReadQuery : IRequest<Result<PostViewModel>>
    {
        public Guid PostId { get; set; }

        public GetPostByIdReadQuery(Guid postId)
        {
            PostId = postId;
        }
    }
}
