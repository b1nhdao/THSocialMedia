using MongoDB.Driver;
using THSocialMedia.Domain.Abstractions.IReadRepositories;

namespace THSocialMedia.Infrastructure.ReadRepositories.Repositories
{
    public class ReadRepository<TReadModel> : IReadRepository<TReadModel>
    {
        protected readonly IMongoCollection<TReadModel> Collection;

        public ReadRepository(IMongoDatabase mongoDatabase, string collectionName)
        {
            Collection = mongoDatabase.GetCollection<TReadModel>(collectionName);
        }

        public virtual async Task<TReadModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TReadModel>.Filter.Eq("Id", id);
            return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public virtual async Task CreateAsync(TReadModel entity, CancellationToken cancellationToken = default)
        {
            await Collection.InsertOneAsync(entity, null, cancellationToken);
        }

        public virtual async Task UpdateAsync(Guid id, TReadModel entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TReadModel>.Filter.Eq("Id", id);
            await Collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TReadModel>.Filter.Eq("Id", id);
            await Collection.DeleteOneAsync(filter, null, cancellationToken);
        }
    }
}
