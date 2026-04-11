using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THSocialMedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alter_relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Relationships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Relationships",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Relationships",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Relationships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Relationships",
                type: "uuid",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Users_UserId",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_UserId",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Relationships");
        }
    }
}
