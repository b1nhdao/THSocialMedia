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

        public Relationship(Guid receiverId, Guid senderId, int status)
        {
            ReceiverId = receiverId;
            SenderId = senderId;
            Status = status;
        }

        public Relationship()
        {
        }

        public void UpdateStatus(int newStatus)
        {
            Status = newStatus;
        }
    }
}
