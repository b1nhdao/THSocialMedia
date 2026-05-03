using MongoDB.Driver;
using MongoDB.Driver;
using THSocialMedia.Domain.Abstractions.IReadRepositories;
using THSocialMedia.Domain.Abstractions.IReadRepositories.ReadModels;

namespace THSocialMedia.Infrastructure.MongoDb.Repositories
{
    public class PostReadRepository : ReadRepository<PostReadModel>, IPostReadRepository
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

        public async Task<PostReadModel> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(postId, cancellationToken);
        }

        public async Task<IEnumerable<PostReadModel>> GetPostsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<PostReadModel>.Filter.Eq(x => x.UserId, userId);
            var sort = Builders<PostReadModel>.Sort.Descending(x => x.CreatedAt);
            return await Collection.Find(filter).Sort(sort).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PostReadModel>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            var sort = Builders<PostReadModel>.Sort.Descending(x => x.CreatedAt);
            return await Collection.Find(_ => true).Sort(sort).ToListAsync(cancellationToken);
        }

        public async Task CreatePostAsync(PostReadModel post, CancellationToken cancellationToken = default)
        {
            await CreateAsync(post, cancellationToken);
        }

        public async Task UpdatePostAsync(Guid postId, PostReadModel post, CancellationToken cancellationToken = default)
        {
            await UpdateAsync(postId, post, cancellationToken);
        }

        public async Task DeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(postId, cancellationToken);
        }

        public override async Task<IEnumerable<PostReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllPostsAsync(cancellationToken);
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
