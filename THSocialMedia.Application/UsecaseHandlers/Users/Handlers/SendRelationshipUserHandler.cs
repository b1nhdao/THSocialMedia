using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Application.UsecaseHandlers.Users.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class SendRelationshipUserHandler : IRequestHandler<SendRelationshipUserCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public SendRelationshipUserHandler(IUserRepository userRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SendRelationshipUserCommand request, CancellationToken cancellationToken)
        {
            var userSentId = _identityService.GetUserIdentity();

            var user = await _userRepository.GetFirstOrDefault(x => x.Id == userSentId, include: x => x.Include(x => x.Relationships));

            user.AddRelationShip(new Domain.Entities.Relationship
            {
                SenderId = userSentId,
                ReceiverId = request.TargetUserId,
                Status = 0, // pending
            });

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(true);
        }
    }
}
