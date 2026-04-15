using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class PostWriteRepository : BaseWriteRepository<Post>, IPostWriteRepository
    {
        public PostWriteRepository(WriteDbContext dbContext) : base(dbContext)
        {
        }
    }
}
