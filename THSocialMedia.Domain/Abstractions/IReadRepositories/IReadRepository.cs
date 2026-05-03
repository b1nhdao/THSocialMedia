namespace THSocialMedia.Domain.Abstractions.IReadRepositories
{
    public interface IReadRepository<TReadModel>
    {
        Task<TReadModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task CreateAsync(TReadModel entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, TReadModel entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
