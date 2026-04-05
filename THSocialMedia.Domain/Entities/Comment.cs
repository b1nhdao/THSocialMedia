namespace THSocialMedia.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int FileUrl { get; set; }

        public Guid PostsId { get; set; }

        public Post? Post { get; set; }
    }
}
