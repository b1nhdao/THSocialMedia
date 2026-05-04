using MongoDB.Driver;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Abstractions.IReadRepositories.ReadModels;

namespace THSocialMedia.Infrastructure.ReadRepositories.Repositories
{
    public class PostReadRepository : ReadRepository<PostReadModel>, IBasePostReadRepository
    {
        public PostReadRepository(IMongoDatabase mongoDatabase)
            : base(mongoDatabase, "Posts")
        {
            CreateIndexesAsync().Wait();
        }

        private async Task CreateIndexesAsync()
        {
            var indexKeysDefinition = Builders<PostReadModel>.IndexKeys.Ascending(x => x.UserId);
            try
            {
                await Collection.Indexes.CreateOneAsync(
                    new CreateIndexModel<PostReadModel>(indexKeysDefinition));
            }
            catch
            {
                // Index might already exist
            }
        }

        public async Task<IEnumerable<PostReadModel>> GetPostsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<PostReadModel>.Filter.Eq(x => x.UserId, userId);
            var sort = Builders<PostReadModel>.Sort.Descending(x => x.CreatedAt);
            return await Collection.Find(filter).Sort(sort).ToListAsync(cancellationToken);
        }

        public override async Task<IEnumerable<PostReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var sort = Builders<PostReadModel>.Sort.Descending(x => x.CreatedAt);
            return await Collection.Find(_ => true).Sort(sort).ToListAsync(cancellationToken);
        }

        public override async Task CreateAsync(PostReadModel post, CancellationToken cancellationToken = default)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            await base.CreateAsync(post, cancellationToken);
        }

        public override async Task UpdateAsync(Guid id, PostReadModel post, CancellationToken cancellationToken = default)
        {
            post.UpdatedAt = DateTime.UtcNow;
            await base.UpdateAsync(id, post, cancellationToken);
        }
    }
}
