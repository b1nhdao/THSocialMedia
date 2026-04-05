namespace THSocialMedia.Domain.Entities
{
    public class Relationship
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }

        public int Status { get; set; }
    }
}
