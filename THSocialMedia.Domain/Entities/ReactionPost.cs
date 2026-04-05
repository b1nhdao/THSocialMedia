using System.Text.Json.Serialization;

namespace THSocialMedia.Domain.Entities
{
    public class ReactionPost
    {
        public Guid ReactionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; } = new User();
        [JsonIgnore]
        public Reaction Reaction { get; set; } = new Reaction();
        [JsonIgnore]
        public Post Post { get; set; } = new Post();
    }
}
