namespace THSocialMedia.Domain.Entities
{
    public class Post : BaseEntity
    {
        public Guid UsersId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Visibility { get; set; }
        public string? FileUrls { get; set; }

        public User? User { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
