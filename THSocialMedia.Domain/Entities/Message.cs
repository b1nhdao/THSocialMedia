using System.Text.Json.Serialization;

namespace THSocialMedia.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string? FileUrls { get; set; }

        public Guid? ReplyMsgId { get; set; }

        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Conversation Conversation { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
