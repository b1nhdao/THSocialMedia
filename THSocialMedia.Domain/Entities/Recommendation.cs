using System.Text.Json.Serialization;
using THSocialMedia.Domain.Enums;

namespace THSocialMedia.Domain.Entities
{
    public class Recommendation : BaseEntity
    {
        public Guid ProposalEntityId { get; set; }
        [JsonIgnore]
        public Proposal Proposal { get; set; } = new();
        public string RecommendationId { get; set; } = string.Empty;
        public RecommendationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string ActionItems { get; set; } = string.Empty;
    }
}
