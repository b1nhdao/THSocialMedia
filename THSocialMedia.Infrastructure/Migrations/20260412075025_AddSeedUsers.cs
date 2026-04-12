using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace THSocialMedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Bio", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "PasswordHash", "Status", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000003"), "", "Thích chụp ảnh", new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "lan.nguyen@gmail.com", "Nguyễn Ngọc Lan", true, false, "1234", 0, new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "ngoclan" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "", "Đam mê code", new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "minh.tran@gmail.com", "Trần Đức Minh", true, false, "1234", 0, new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "ducminh" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "", "Food reviewer", new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "hoa.le@gmail.com", "Lê Thanh Hoa", true, false, "1234", 0, new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "thanhhoa" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "", "Thích đi phượt", new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "tuan.pham@gmail.com", "Phạm Anh Tuấn", true, false, "1234", 0, new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "anhtuan" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "", "Yêu âm nhạc", new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "mai.vu@gmail.com", "Vũ Xuân Mai", true, false, "1234", 0, new DateTime(2026, 4, 5, 17, 0, 0, 0, DateTimeKind.Utc), "xuanmai" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"));
        }
    }
}
