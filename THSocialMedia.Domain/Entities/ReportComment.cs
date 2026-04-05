namespace THSocialMedia.Domain.Entities
{
    public class ReportComment
    {
        public Guid ReportsId { get; set; }
        public Guid CommentsId { get; set; }

        public Report? Report { get; set; }
        public Comment? Comment { get; set; }
    }


}
