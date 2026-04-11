using THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels;

namespace THSocialMedia.Application.UsecaseHandlers.Users.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}