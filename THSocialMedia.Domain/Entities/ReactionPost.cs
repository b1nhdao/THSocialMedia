namespace THSocialMedia.Domain.Entities
{
    public class ReactionPost
    {
        public Guid ReactionsId { get; set; }
        public Guid PostsId { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Reaction? Reaction { get; set; }
        public Post? Post { get; set; }
    }
}
