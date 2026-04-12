namespace THSocialMedia.Application.UsecaseHandlers.Users.VIewModels
{
    public class RelationshipDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid FriendId { get; set; }
        public string FriendName { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
