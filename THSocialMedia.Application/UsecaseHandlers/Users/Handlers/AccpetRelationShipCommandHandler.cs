using THSocialMedia.Application.UsecaseHandlers.Users.Commands;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class AccpetRelationShipCommandHandler : IRequestHandler<AccpetRelationShipCommand, Result<bool>>
    {
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AccpetRelationShipCommandHandler(IRelationshipRepository relationshipRepository, IUnitOfWork unitOfWork)
        {
            _relationshipRepository = relationshipRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(AccpetRelationShipCommand request, CancellationToken cancellationToken)
        {
            var relationship = await _relationshipRepository.GetByIdAsync(request.Id);
            if (relationship == null)
            {
                return Result.NotFound();
            }

            relationship.Status = 1; // accepted
            _relationshipRepository.Update(relationship);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(true);
        }
    }
}
