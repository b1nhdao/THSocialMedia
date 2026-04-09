namespace THSocialMedia.Domain.Entities
{
    public class Relationship : BaseEntity
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }

        public int Status { get; set; }
    }
}
