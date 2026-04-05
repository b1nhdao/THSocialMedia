using System.Text.Json.Serialization;

namespace THSocialMedia.Domain.Entities
{
    public class ChatMember
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }

        [JsonIgnore]
        public Conversation? Conversation { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
    }
}
