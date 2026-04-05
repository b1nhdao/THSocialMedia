namespace THSocialMedia.Domain.Entities
{
    public class Report : BaseEntity
    {
        public string Reason { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
