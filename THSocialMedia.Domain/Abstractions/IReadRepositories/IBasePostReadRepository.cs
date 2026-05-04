using THSocialMedia.Domain.Abstractions.IReadRepositories.ReadModels;

namespace THSocialMedia.Domain.Abstractions.IReadRepositories
{
    public interface IBasePostReadRepository : IReadRepository<PostReadModel>
    {
        Task<IEnumerable<PostReadModel>> GetPostsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
