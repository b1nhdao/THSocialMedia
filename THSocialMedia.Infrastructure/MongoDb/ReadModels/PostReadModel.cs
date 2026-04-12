namespace THSocialMedia.Infrastructure.MongoDb.ReadModels
{
    public class PostReadModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Visibility { get; set; }
        public string? FileUrls { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ReactionsCount { get; set; }
        public int CommentsCount { get; set; }
        public List<CommentReadModel> Comments { get; set; } = new();
        public List<ReactionReadModel> Reactions { get; set; } = new();
    }

    public class CommentReadModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReactionReadModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid ReactionId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
