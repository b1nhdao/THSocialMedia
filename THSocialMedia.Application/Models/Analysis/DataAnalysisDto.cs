namespace THSocialMedia.Application.Models.Analysis
{
    public class DataAnalysisDto
    {
        public List<Post> Posts { get; set; } = new();
        public List<Report> Reports { get; set; } = new();
    }

    public class Post
    {
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public List<Comment>? Comments { get; set; }
        public List<Reaction>? Reactions { get; set; }
    }

    public class Comment
    {
        public string CommentId { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorId { get; set; } = string.Empty;
    }

    public class Reaction
    {
        public string ReactionId { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public ReactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Report
    {
        public string ReportId { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public ReportType Type { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public enum ReactionType
    {
        Like,
        Love,
        Haha,
        Wow,
        Sad,
        Angry
    }

    public enum ReportType
    {
        Spam,
        Hate,
        Violence,
        Misinformation
    }

    public enum ReportStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
