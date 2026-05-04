using Microsoft.EntityFrameworkCore;
using THSocialMedia.Domain.Abstractions.IWriteRepositories;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Domain.Enums;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class ProposalRepository : IProposalRepository
    {
        private readonly WriteDbContext _dbContext;

        public ProposalRepository(WriteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Proposal> AddAsync(Proposal proposal)
        {
            await _dbContext.Proposals.AddAsync(proposal);
            await _dbContext.SaveChangesAsync();
            return proposal;
        }

        public async Task<Proposal?> GetByProposalIdAsync(string proposalId)
        {
            return await _dbContext.Proposals
                .Include(p => p.Details)
                .Include(p => p.Recommendations)
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);
        }

        public async Task<IEnumerable<Proposal>> GetPendingAsync()
        {
            return await _dbContext.Proposals
                .Where(p => p.Status == ProposalStatus.PendingReview)
                .ToListAsync();
        }

        public async Task<Proposal> UpdateAsync(Proposal proposal)
        {
            _dbContext.Proposals.Update(proposal);
            await _dbContext.SaveChangesAsync();
            return proposal;
        }
    }
}
