namespace THSocialMedia.Domain.Entities
{
    public class Conversation : BaseEntity
    {
        public string? Name { get; set; }
        public bool IsGroup { get; set; }

        public ICollection<ChatMember>? Members { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
