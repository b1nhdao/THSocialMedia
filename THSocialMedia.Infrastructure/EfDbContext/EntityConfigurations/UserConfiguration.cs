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
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    PasswordHash = "1234",
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
                    UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(),
                    PasswordHash = "1234",
                },
                new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), AvatarUrl = "", Bio = "Thích chụp ảnh", Email = "lan.nguyen@gmail.com", FullName = "Nguyễn Ngọc Lan", IsActive = true, Username = "ngoclan", CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), PasswordHash = "1234" },
                new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), AvatarUrl = "", Bio = "Đam mê code", Email = "minh.tran@gmail.com", FullName = "Trần Đức Minh", IsActive = true, Username = "ducminh", CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), PasswordHash = "1234" },
                new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), AvatarUrl = "", Bio = "Food reviewer", Email = "hoa.le@gmail.com", FullName = "Lê Thanh Hoa", IsActive = true, Username = "thanhhoa", CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), PasswordHash = "1234" },
                new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), AvatarUrl = "", Bio = "Thích đi phượt", Email = "tuan.pham@gmail.com", FullName = "Phạm Anh Tuấn", IsActive = true, Username = "anhtuan", CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), PasswordHash = "1234" },
                new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), AvatarUrl = "", Bio = "Yêu âm nhạc", Email = "mai.vu@gmail.com", FullName = "Vũ Xuân Mai", IsActive = true, Username = "xuanmai", CreatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), UpdatedAt = DateTime.Parse("2026-04-06T00:00:00").ToUniversalTime(), PasswordHash = "1234" }
                
                );
        }
    }
}
