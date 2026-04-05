using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class PostRepository : BaseWriteRepository<Post>, IPostRepository
    {
        public PostRepository(WriteDbContext dbContext) : base(dbContext)
        {
        }
    }
}
