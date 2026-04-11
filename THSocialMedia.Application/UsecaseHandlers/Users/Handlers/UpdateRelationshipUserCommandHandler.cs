using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.UsecaseHandlers.Users.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class UpdateRelationshipUserCommandHandler : IRequestHandler<UpdateRelationshipUserCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRelationshipUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateRelationshipUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstOrDefault(x => x.Id == request.TargetUserId, include: x => x.Include(x => x.Relationships));
            var relationship = user.Relationships.FirstOrDefault(r => (r.SenderId == request.SenderId && r.ReceiverId == request.TargetUserId) || (r.SenderId == request.TargetUserId && r.ReceiverId == request.SenderId));
            relationship.UpdateStatus(request.Status);
            await _unitOfWork.SaveChangesAsync();

            return new Result<bool>(true);
        }
    }
}
