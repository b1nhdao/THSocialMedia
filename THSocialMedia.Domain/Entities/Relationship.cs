namespace THSocialMedia.Domain.Entities
{
    public class Relationship : BaseEntity
    {
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }

        /// <summary>
        /// 0 = pending, 1 = accepted, 2 = rejected
        /// </summary>
        public int Status { get; set; }

        public void UpdateStatus(int newStatus)
        {
            Status = newStatus;
        }
    }
}
