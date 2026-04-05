using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public class ConversationRepository : BaseWriteRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(WriteDbContext dbContext) : base(dbContext)
        {
        }
    }
}
