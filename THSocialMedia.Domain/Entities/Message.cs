namespace THSocialMedia.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string? FileUrls { get; set; }

        public Guid? ReplyMsgId { get; set; }

        public Guid ConversationsId { get; set; }
        public Guid UsersId { get; set; }

        public Conversation? Conversation { get; set; }
        public User? User { get; set; }
    }
}
