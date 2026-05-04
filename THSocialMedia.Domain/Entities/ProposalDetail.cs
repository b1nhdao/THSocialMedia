using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using THSocialMedia.Domain.Enums;

namespace THSocialMedia.Domain.Entities
{
    public class ProposalDetail : BaseEntity
    {
        public Guid ProposalEntityId { get; set; }
        [JsonIgnore]
        public Proposal Proposal { get; set; } = new();
        public string DetailId { get; set; } = string.Empty;
        public AnalysisCategory Category { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? ValueJson { get; set; }
        public string Description { get; set; } = string.Empty;

        [NotMapped]
        public object? Value
        {
            get
            {
                if (_value != null)
                {
                    return _value;
                }

                if (string.IsNullOrWhiteSpace(ValueJson))
                {
                    return null;
                }

                _value = JsonSerializer.Deserialize<object>(ValueJson);
                return _value;
            }
            set
            {
                _value = value;
                ValueJson = value == null ? null : JsonSerializer.Serialize(value);
            }
        }

        [NotMapped]
        private object? _value;
    }
}
