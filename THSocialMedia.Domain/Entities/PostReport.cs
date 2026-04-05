namespace THSocialMedia.Domain.Entities
{
    public class PostReport
    {
        public Guid PostsId { get; set; }
        public Guid ReportsId { get; set; }

        public Post? Post { get; set; }
        public Report? Report { get; set; }
    }


}
