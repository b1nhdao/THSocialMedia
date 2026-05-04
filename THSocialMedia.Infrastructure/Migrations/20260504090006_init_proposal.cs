using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THSocialMedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init_proposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalId = table.Column<string>(type: "text", nullable: false),
                    AnalysisVersion = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Summary_TotalPostsAnalyzed = table.Column<int>(type: "integer", nullable: false),
                    Summary_TotalCommentsAnalyzed = table.Column<int>(type: "integer", nullable: false),
                    Summary_TotalReactionsAnalyzed = table.Column<int>(type: "integer", nullable: false),
                    Summary_TotalReportsAnalyzed = table.Column<int>(type: "integer", nullable: false),
                    Summary_AverageEngagementRate = table.Column<double>(type: "double precision", nullable: false),
                    Summary_AverageCommentPerPost = table.Column<double>(type: "double precision", nullable: false),
                    Summary_AverageReactionPerPost = table.Column<double>(type: "double precision", nullable: false),
                    Summary_ProblematicPostsCount = table.Column<int>(type: "integer", nullable: false),
                    Summary_ProblematicCommentsCount = table.Column<int>(type: "integer", nullable: false),
                    Summary_CommunityHealthScore = table.Column<double>(type: "double precision", nullable: false),
                    Summary_AnalysisStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Summary_AnalysisEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProposalDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetailId = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    ValueJson = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalDetails_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    HistoryId = table.Column<string>(type: "text", nullable: false),
                    ProposalId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ChangeDescription = table.Column<string>(type: "text", nullable: false),
                    ChangedBy = table.Column<string>(type: "text", nullable: false),
                    ManagerRationale = table.Column<string>(type: "text", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalHistories_Proposals_ProposalId1",
                        column: x => x.ProposalId1,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecommendationId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    ActionItems = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recommendations_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProposalDetails_ProposalId",
                table: "ProposalDetails",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalHistories_ProposalId1",
                table: "ProposalHistories",
                column: "ProposalId1");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_ProposalId",
                table: "Recommendations",
                column: "ProposalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProposalDetails");

            migrationBuilder.DropTable(
                name: "ProposalHistories");

            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.DropTable(
                name: "Proposals");
        }
    }
}
