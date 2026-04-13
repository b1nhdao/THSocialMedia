using THSocialMedia.Application.Services;
using THSocialMedia.Application.UsecaseHandlers.Users.Queries;
using THSocialMedia.Application.UsecaseHandlers.Users.VIewModels;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Handlers
{
    public class GetAllRelationshipsQueryHandler : IRequestHandler<GetAllRelationshipsQuery, Result<List<RelationshipDto>>>
    {
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public GetAllRelationshipsQueryHandler(IRelationshipRepository relationshipRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _relationshipRepository = relationshipRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<List<RelationshipDto>>> Handle(GetAllRelationshipsQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            
            var relationShips = await _relationshipRepository.GetAllAsync(x => x.ReceiverId == userId);


            var list = new List<RelationshipDto>();
            foreach (var item in relationShips)
            {
                var user = await _userRepository.GetByIdAsync(item.SenderId);
                var friend = await _userRepository.GetByIdAsync(item.ReceiverId);
                 var relationshipDto = new RelationshipDto
                {
                    Id = item.Id,
                    UserId = user.Id,
                    UserName = user.Username,
                    FriendId = friend.Id,
                    FriendName = friend.Username,
                    Status = item.Status
                };
                list.Add(relationshipDto);
            }
            
            return Result<List<RelationshipDto>>.Success(list); 
        }
    }
}
