namespace THSocialMedia.Domain.Entities
{
    public class Reaction : BaseEntity
    {
        public int Type { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
