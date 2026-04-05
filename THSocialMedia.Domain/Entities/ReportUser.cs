namespace THSocialMedia.Domain.Entities
{
    public class ReportUser
    {
        public Guid UsersId { get; set; }
        public Guid ReportsId { get; set; }

        public User? User { get; set; }
        public Report? Report { get; set; }
    }
}
