namespace THSocialMedia.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public int Status { get; set; }
        public bool IsActive { get; set; }
        public string? AvatarUrl { get; set; }

        // Navigation
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
