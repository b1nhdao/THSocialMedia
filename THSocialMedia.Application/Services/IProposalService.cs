using THSocialMedia.Application.Models.Analysis;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Application.Services
{
    public interface IProposalService
    {
        Task<Proposal> RunAnalysisAsync(DataAnalysisDto data);
    }
}
