using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Queries
{
    public class GetAllPostsReadQuery : IRequest<Result<IEnumerable<PostViewModel>>>
    {
    }
}
