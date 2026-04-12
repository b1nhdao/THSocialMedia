using MongoDB.Driver;
using MongoDB.Driver;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;
using THSocialMedia.Infrastructure.MongoDb.ReadModels;

namespace THSocialMedia.Infrastructure.MongoDb.Repositories
{
    public class PostReadRepository : IPostReadRepository
    {
        private readonly IMongoCollection<PostReadModel> _collection;

        public PostReadRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<PostReadModel>("Posts");
            CreateIndexesAsync().Wait();
        }

        private async Task CreateIndexesAsync()
        {
            var indexKeysDefinition = Builders<PostReadModel>.IndexKeys.Ascending(x => x.UserId);
            try
            {
                await _collection.Indexes.CreateOneAsync(
                    new CreateIndexModel<PostReadModel>(indexKeysDefinition));
            }
            catch
            {
                // Index might already exist
            }
        }

        public async Task<PostReadModel> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<PostReadModel>.Filter.Eq(x => x.Id, postId);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<PostReadModel>> GetPostsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<PostReadModel>.Filter.Eq(x => x.UserId, userId);
            var sort = Builders<PostReadModel>.Sort.Descending(x => x.CreatedAt);
            return await _collection.Find(filter).Sort(sort).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PostReadModel>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            var sort = Builders<PostReadModel>.Sort.Descending(x => x.CreatedAt);
            return await _collection.Find(_ => true).Sort(sort).ToListAsync(cancellationToken);
        }

        public async Task CreatePostAsync(PostReadModel post, CancellationToken cancellationToken = default)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(post, null, cancellationToken);
        }

        public async Task UpdatePostAsync(Guid postId, PostReadModel post, CancellationToken cancellationToken = default)
        {
            post.UpdatedAt = DateTime.UtcNow;
            var filter = Builders<PostReadModel>.Filter.Eq(x => x.Id, postId);

            // Replace entire document to ensure all fields are updated (including Comments and Reactions)
            await _collection.ReplaceOneAsync(filter, post, new ReplaceOptions { IsUpsert = false }, cancellationToken);
        }

        public async Task DeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<PostReadModel>.Filter.Eq(x => x.Id, postId);
            await _collection.DeleteOneAsync(filter, null, cancellationToken);
        }
    }
}
