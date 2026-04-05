using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class UserRepository : BaseWriteRepository<User>, IUserRepository
    {
        public UserRepository(WriteDbContext dbContext) : base(dbContext)
        {
        }
    }
}
