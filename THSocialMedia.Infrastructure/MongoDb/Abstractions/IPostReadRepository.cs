using THSocialMedia.Infrastructure.MongoDb.ReadModels;

namespace THSocialMedia.Infrastructure.MongoDb.Abstractions
{
    public interface IPostReadRepository
    {
        Task<PostReadModel> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostReadModel>> GetPostsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostReadModel>> GetAllPostsAsync(CancellationToken cancellationToken = default);
        Task CreatePostAsync(PostReadModel post, CancellationToken cancellationToken = default);
        Task UpdatePostAsync(Guid postId, PostReadModel post, CancellationToken cancellationToken = default);
        Task DeletePostAsync(Guid postId, CancellationToken cancellationToken = default);
    }
}
