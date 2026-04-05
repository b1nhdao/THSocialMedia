using System.Text.Json.Serialization;

namespace THSocialMedia.Domain.Entities
{
    public class Conversation : BaseEntity
    {
        public string? Name { get; set; }
        public bool IsGroup { get; set; }

        [JsonIgnore]
        public ICollection<ChatMember> Members { get; set; } = [];
        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = [];
    }
}
