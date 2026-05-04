using THSocialMedia.Domain.Enums;

namespace THSocialMedia.Domain.Entities
{
    public class Proposal : BaseEntity
    {
        public string ProposalId { get; set; } = string.Empty;
        public string AnalysisVersion { get; set; } = string.Empty;
        public ProposalStatus Status { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public AnalysisSummary Summary { get; set; } = new();
        public ICollection<ProposalDetail> Details { get; set; } = new List<ProposalDetail>();
        public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
    }
}
