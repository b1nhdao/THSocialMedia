using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace THSocialMedia.Domain.Entities
{
    public class Post : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Visibility { get; set; }
        public List<string>? FileUrls { get; set; }

        [JsonIgnore]
        public List<ReactionPost> Reactions { get; set; } = new List<ReactionPost>();
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public void AddComment(Comment comment)
        {
            Comments.Add(comment);
        }

        public void DeleteComment(Comment comment)
        {
            Comments.Remove(comment);
        }

        public void UpdateComment(Comment comment)
        {
            var existingComment = Comments.FirstOrDefault(c => c.Id == comment.Id);
            if (existingComment != null)
            {
                existingComment.Content = comment.Content;
                existingComment.FileUrl = comment.FileUrl;
            }
        }

        public void AddReaction(ReactionPost reaction)
        {
            Reactions.Add(reaction);
        }

        public void RemoveReaction(ReactionPost reaction)
        {
            Reactions.Remove(reaction);
        }
    }
}
