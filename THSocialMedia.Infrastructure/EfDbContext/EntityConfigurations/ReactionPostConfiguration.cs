using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.EntityConfigurations
{
    public class ReactionPostConfiguration : IEntityTypeConfiguration<ReactionPost>
    {
        public void Configure(EntityTypeBuilder<ReactionPost> builder)
        {
            builder.HasKey(rp => new { rp.PostsId, rp.UserId });
        }
    }
}
