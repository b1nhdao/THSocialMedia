using System.Text.Json.Serialization;

namespace THSocialMedia.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public string? AvatarUrl { get; set; }

        // Navigation
        [JsonIgnore]
        public ICollection<Post> Posts { get; set; } = [];
        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = [];
        [JsonIgnore]
        public ICollection<Relationship> Relationships { get; set; } = [];

        public void AddRelationShip(Relationship relationship)
        {
            Relationships.Add(relationship);
        }
    }
}
