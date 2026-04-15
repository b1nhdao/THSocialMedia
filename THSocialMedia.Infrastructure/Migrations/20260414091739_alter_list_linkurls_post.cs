using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THSocialMedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alter_list_linkurls_post : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrls",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrls",
                table: "Posts",
                type: "text",
                nullable: true);
        }
    }
}
