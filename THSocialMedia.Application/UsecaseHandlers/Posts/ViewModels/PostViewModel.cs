namespace THSocialMedia.Application.UsecaseHandlers.Posts.ViewModels
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Visibility { get; set; }
        public List<string>? FileUrls { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int CommentsCount { get; set; }
        public List<CommentViewModel> Comments { get; set; } = [];
        public List<ReactionCount> ReactionCounts { get; set; } = [];
        public List<ReactionViewModel> ReactionViewModels { get; set; } = [];
    }

    public class ReactionCount
    {
        public int Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ReactionViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}