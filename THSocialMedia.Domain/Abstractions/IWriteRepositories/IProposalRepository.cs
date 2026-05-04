using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Domain.Abstractions.IWriteRepositories
{
    public interface IProposalRepository
    {
        Task<Proposal> AddAsync(Proposal proposal);
        Task<Proposal?> GetByProposalIdAsync(string proposalId);
        Task<IEnumerable<Proposal>> GetPendingAsync();
        Task<Proposal> UpdateAsync(Proposal proposal);
    }
}
