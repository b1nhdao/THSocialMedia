namespace THSocialMedia.Domain.Entities
{
    public class ChatMember
    {
        public Guid ConversationsId { get; set; }
        public Guid UsersId { get; set; }

        public Conversation? Conversation { get; set; }
        public User? User { get; set; }
    }
}
