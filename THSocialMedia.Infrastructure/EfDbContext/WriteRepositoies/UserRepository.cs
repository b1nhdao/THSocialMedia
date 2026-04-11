using Microsoft.EntityFrameworkCore;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class UserRepository : BaseWriteRepository<User>, IUserRepository
    {
        public UserRepository(WriteDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Guid>> GetConnectedUserIdsAsync(Guid userId, int? status = null, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Relationships.AsNoTracking().Where(r => r.SenderId == userId || r.ReceiverId == userId);

            if (status is not null)
                query = query.Where(r => r.Status == status);

            return await query
                .Select(r => r.SenderId == userId ? r.ReceiverId : r.SenderId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
