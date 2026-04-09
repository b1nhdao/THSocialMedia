using Microsoft.EntityFrameworkCore;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class RelationshipRepository : BaseWriteRepository<Relationship>, IRelationshipRepository
    {
        public RelationshipRepository(WriteDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Guid>> GetConnectedUserIdsAsync(Guid userId, int? status = null, CancellationToken cancellationToken = default)
        {
            // Relationship is a connection between 2 people. If connected, they can see each other's posts.
            // Connected users for userId are the "other" end of any relationship edge.
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
