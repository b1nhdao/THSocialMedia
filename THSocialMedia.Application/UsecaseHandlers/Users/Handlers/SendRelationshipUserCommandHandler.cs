using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Application.UsecaseHandlers.Users.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class SendRelationshipUserCommandHandler : IRequestHandler<SendRelationshipUserCommand, Result<Guid>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRelationshipRepository _relationshipRepository;

        public SendRelationshipUserCommandHandler(IIdentityService identityService, IUnitOfWork unitOfWork, IRelationshipRepository relationshipRepository)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _relationshipRepository = relationshipRepository;
        }

        public async Task<Result<Guid>> Handle(SendRelationshipUserCommand request, CancellationToken cancellationToken)
        {
            var userSentId = request.SenderId == Guid.Empty ? _identityService.GetUserIdentity() : request.SenderId;

            var relationship = new Relationship
            {
                Id = Guid.CreateVersion7(),
                SenderId = userSentId,
                ReceiverId = request.TargetUserId,
                Status = 0, // pending
            };

            _relationshipRepository.Add(relationship);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(relationship.Id);
        }
    }
}
