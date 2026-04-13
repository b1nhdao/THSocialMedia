using THSocialMedia.Application.UsecaseHandlers.Users.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class UpdateRelationshipUserCommandHandler : IRequestHandler<UpdateRelationshipUserCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRelationshipRepository _relationshipRepository;

        public UpdateRelationshipUserCommandHandler(IUnitOfWork unitOfWork, IRelationshipRepository relationshipRepository)
        {
            _unitOfWork = unitOfWork;
            _relationshipRepository = relationshipRepository;
        }

        public async Task<Result<bool>> Handle(UpdateRelationshipUserCommand request, CancellationToken cancellationToken)
        {
            var relationship = await _relationshipRepository.GetFirstOrDefault(x => x.Id == request.Id);

            relationship.Status = request.Status;
            _relationshipRepository.Update(relationship);
            await _unitOfWork.SaveChangesAsync();
            return new Result<bool>(true);
        }
    }
}
