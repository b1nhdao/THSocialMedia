using Microsoft.EntityFrameworkCore;
using THSocialMedia.Domain.Abstractions.IWriteRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class ProposalHistoryRepository : IProposalHistoryRepository
    {
        private readonly WriteDbContext _dbContext;

        public ProposalHistoryRepository(WriteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProposalHistory> AddAsync(ProposalHistory history)
        {
            await _dbContext.ProposalHistories.AddAsync(history);
            await _dbContext.SaveChangesAsync();
            return history;
        }

        public async Task<IEnumerable<ProposalHistory>> GetByProposalIdAsync(string proposalId)
        {
            return await _dbContext.ProposalHistories
                .Where(h => h.ProposalId == proposalId)
                .ToListAsync();
        }
    }
}
