using Ardalis.Result;
using MediatR;
using THSocialMedia.Application.UsecaseHandlers.Users.ViewModels;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result<UserViewModel>>
    {
        public Guid Id { get; set; }
    }
}