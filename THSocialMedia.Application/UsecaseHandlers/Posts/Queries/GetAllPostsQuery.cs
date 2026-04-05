using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;

namespace THSocialMedia.Application.UsecaseHandlers.Posts.Queries
{
    public class GetAllPostsQuery : IRequest<Result<IEnumerable<PostViewModel>>>
    {
    }
}