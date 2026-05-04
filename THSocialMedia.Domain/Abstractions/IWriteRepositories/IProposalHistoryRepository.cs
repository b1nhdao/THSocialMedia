using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Domain.Abstractions.IWriteRepositories
{
    public interface IProposalHistoryRepository
    {
        Task<ProposalHistory> AddAsync(ProposalHistory history);
        Task<IEnumerable<ProposalHistory>> GetByProposalIdAsync(string proposalId);
    }
}
