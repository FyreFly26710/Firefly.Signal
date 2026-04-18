using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.JobSearch.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_profiles",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PreferredTitle = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    PrimaryLocationPostcode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    GitHubUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PortfolioUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Summary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    SkillsText = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ExperienceText = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    PreferencesText = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_UserAccountId",
                schema: "jobsearch",
                table: "user_profiles",
                column: "UserAccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_profiles",
                schema: "jobsearch");
        }
    }
}
