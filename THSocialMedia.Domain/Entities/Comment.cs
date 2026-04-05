using System.Text.Json.Serialization;

namespace THSocialMedia.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; }

        public Guid PostId { get; set; }
        public Guid UserId { get; set; }

        [JsonIgnore]
        public Post Post { get; set; } = new Post();
    }
}
