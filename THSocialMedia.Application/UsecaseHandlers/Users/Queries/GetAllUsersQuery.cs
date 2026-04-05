using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Users.ViewModels;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Queries
{
    public class GetAllUsersQuery : IRequest<Result<IEnumerable<UserViewModel>>>
    {
    }
}