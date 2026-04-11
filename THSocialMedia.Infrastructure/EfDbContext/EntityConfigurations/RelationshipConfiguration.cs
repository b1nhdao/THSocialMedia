using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.EntityConfigurations
{
    public class RelationshipConfiguration : IEntityTypeConfiguration<Relationship>
    {
        public void Configure(EntityTypeBuilder<Relationship> builder)
        {
            builder.HasKey(r => new { r.ReceiverId, r.SenderId });
            builder.HasData(
                new Relationship
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    IsDeleted = false,
                    ReceiverId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    SenderId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Status = 1
                });
        }
    }
}
