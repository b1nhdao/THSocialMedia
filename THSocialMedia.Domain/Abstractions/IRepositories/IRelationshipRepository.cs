using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Domain.Abstractions.IRepositories
{
    public interface IRelationshipRepository : IBaseWriteRepository<Relationship>
    {
        Task<IReadOnlyList<Guid>> GetConnectedUserIdsAsync(Guid userId, int? status = null, CancellationToken cancellationToken = default);
    }
}
