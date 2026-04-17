using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.JobSearch.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAppliedStateAndInterviewRounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApplied",
                schema: "jobsearch",
                table: "user_job_states",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RoundNumber",
                schema: "jobsearch",
                table: "job_application_status_entries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_job_states_UserAccountId_IsApplied",
                schema: "jobsearch",
                table: "user_job_states",
                columns: new[] { "UserAccountId", "IsApplied" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_job_states_UserAccountId_IsApplied",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "IsApplied",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "RoundNumber",
                schema: "jobsearch",
                table: "job_application_status_entries");
        }
    }
}
