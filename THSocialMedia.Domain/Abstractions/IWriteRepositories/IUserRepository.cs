using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Domain.Abstractions.IWriteRepositories
{
    public interface IUserRepository : IBaseWriteRepository<User>
    {
        Task<IReadOnlyList<Guid>> GetConnectedUserIdsAsync(Guid userId, int? status = null, CancellationToken cancellationToken = default);

    }
}
