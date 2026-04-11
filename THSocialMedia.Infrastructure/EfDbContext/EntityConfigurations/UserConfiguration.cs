using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasData(
                new User
                {
                    AvatarUrl = "",
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Bio = "Tôi là admin",
                    Email = "admin@gmail.com",
                    FullName = "Nguyễn Admin",
                    IsActive = true,
                    Username = "admin",
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()
                },
                new User
                {
                    AvatarUrl = "",
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Bio = "Tôi bạn là admin",
                    Email = "banadmin@gmail.com",
                    FullName = "Nguyễn Bạn Admin",
                    IsActive = true,
                    Username = "banadmin",
                    CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime()

                });
        }
    }
}
