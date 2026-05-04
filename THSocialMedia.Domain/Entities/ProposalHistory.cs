using System.Text.Json.Serialization;
using THSocialMedia.Domain.Enums;

namespace THSocialMedia.Domain.Entities
{
    public class ProposalHistory : BaseEntity
    {
        public Guid ProposalEntityId { get; set; }
        [JsonIgnore]
        public Proposal Proposal { get; set; } = new();
        public string HistoryId { get; set; } = string.Empty;
        public string ProposalId { get; set; } = string.Empty;
        public ProposalStatus Status { get; set; }
        public string ChangeDescription { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public string? ManagerRationale { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
