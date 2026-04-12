using THSocialMedia.Application.UsecaseHandlers.Users.VIewModels;

namespace THSocialMedia.Application.UsecaseHandlers.Users.Queries
{
    public class GetAllRelationshipsQuery : IRequest<Result<List<RelationshipDto>>>
    {
        public Guid UserId { get; set; }
    }
}
