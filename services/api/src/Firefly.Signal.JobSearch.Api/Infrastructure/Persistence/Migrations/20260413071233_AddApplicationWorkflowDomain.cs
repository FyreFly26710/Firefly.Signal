using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.JobSearch.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationWorkflowDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_notes",
                schema: "jobsearch");

            migrationBuilder.DropIndex(
                name: "IX_user_job_states_UserAccountId_State",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_UserAccountId_Status",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "AppliedAtUtc",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "RejectedAtUtc",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "SavedAtUtc",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "State",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "AppliedAtUtc",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "RejectionAtUtc",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "SubmittedCoverLetterDocumentId",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "SubmittedCvDocumentId",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                schema: "jobsearch",
                table: "user_job_states",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSaved",
                schema: "jobsearch",
                table: "user_job_states",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                schema: "jobsearch",
                table: "job_applications",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "job_application_status_entries",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    StatusAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_application_status_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_job_application_status_entries_job_applications_JobApplicat~",
                        column: x => x.JobApplicationId,
                        principalSchema: "jobsearch",
                        principalTable: "job_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_job_states_UserAccountId_IsHidden",
                schema: "jobsearch",
                table: "user_job_states",
                columns: new[] { "UserAccountId", "IsHidden" });

            migrationBuilder.CreateIndex(
                name: "IX_user_job_states_UserAccountId_IsSaved",
                schema: "jobsearch",
                table: "user_job_states",
                columns: new[] { "UserAccountId", "IsSaved" });

            migrationBuilder.CreateIndex(
                name: "IX_job_application_status_entries_JobApplicationId",
                schema: "jobsearch",
                table: "job_application_status_entries",
                column: "JobApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_application_status_entries",
                schema: "jobsearch");

            migrationBuilder.DropIndex(
                name: "IX_user_job_states_UserAccountId_IsHidden",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropIndex(
                name: "IX_user_job_states_UserAccountId_IsSaved",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "IsSaved",
                schema: "jobsearch",
                table: "user_job_states");

            migrationBuilder.DropColumn(
                name: "Note",
                schema: "jobsearch",
                table: "job_applications");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedAtUtc",
                schema: "jobsearch",
                table: "user_job_states",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAtUtc",
                schema: "jobsearch",
                table: "user_job_states",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SavedAtUtc",
                schema: "jobsearch",
                table: "user_job_states",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                schema: "jobsearch",
                table: "user_job_states",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedAtUtc",
                schema: "jobsearch",
                table: "job_applications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectionAtUtc",
                schema: "jobsearch",
                table: "job_applications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                schema: "jobsearch",
                table: "job_applications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "jobsearch",
                table: "job_applications",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SubmittedCoverLetterDocumentId",
                schema: "jobsearch",
                table: "job_applications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubmittedCvDocumentId",
                schema: "jobsearch",
                table: "job_applications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "application_notes",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_application_notes_job_applications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "jobsearch",
                        principalTable: "job_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_job_states_UserAccountId_State",
                schema: "jobsearch",
                table: "user_job_states",
                columns: new[] { "UserAccountId", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_UserAccountId_Status",
                schema: "jobsearch",
                table: "job_applications",
                columns: new[] { "UserAccountId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_application_notes_JobApplicationId",
                schema: "jobsearch",
                table: "application_notes",
                column: "JobApplicationId");
        }
    }
}
