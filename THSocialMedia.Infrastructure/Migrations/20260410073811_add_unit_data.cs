using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THSocialMedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_unit_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Relationships",
                columns: new[] { "ReceiverId", "SenderId", "CreatedAt", "Id", "IsDeleted", "Status", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), new Guid("00000000-0000-0000-0000-000000000001"), false, 1, new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Bio", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "PasswordHash", "Status", "UpdatedAt", "Username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), "", "Tôi bạn là admin", new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "banadmin@gmail.com", "Nguyễn Bạn Admin", true, false, "", 0, new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "banadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Relationships",
                keyColumns: new[] { "ReceiverId", "SenderId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));
        }
    }
}
