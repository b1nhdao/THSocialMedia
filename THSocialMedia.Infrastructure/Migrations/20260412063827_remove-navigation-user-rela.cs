using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THSocialMedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removenavigationuserrela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Users_UserId",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_UserId",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Relationships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Relationships",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Relationships",
                keyColumns: new[] { "ReceiverId", "SenderId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002") },
                column: "UserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_UserId",
                table: "Relationships",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Users_UserId",
                table: "Relationships",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
