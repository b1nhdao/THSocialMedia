using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.EntityConfigurations
{
    public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasData(
                new Reaction
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Like",
                    Type = 1,
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()
                },
                new Reaction
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Love",
                    Type = 2,
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()
                },
                new Reaction
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Name = "Angry",
                    Type = 3,
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()

                },
                new Reaction
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Name = "Sad",
                    Type = 4,
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()

                },
                new Reaction
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                    Name = "Wow",
                    Type = 5,
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()

                },
                new Reaction
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                    Name = "Haha",
                    Type = 6,
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()
                }
                );
        }
    }
}
