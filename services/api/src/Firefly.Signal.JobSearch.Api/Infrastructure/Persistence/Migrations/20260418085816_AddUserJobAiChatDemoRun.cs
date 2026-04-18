using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.JobSearch.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserJobAiChatDemoRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_job_ai_chat_demo_runs",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    JobPostingId = table.Column<long>(type: "bigint", nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Provider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Model = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Prompt = table.Column<string>(type: "character varying(12000)", maxLength: 12000, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AiResponseId = table.Column<long>(type: "bigint", nullable: true),
                    AiResponseContent = table.Column<string>(type: "character varying(12000)", maxLength: 12000, nullable: true),
                    RequestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_job_ai_chat_demo_runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_job_ai_chat_demo_runs_jobs_JobPostingId",
                        column: x => x.JobPostingId,
                        principalSchema: "jobsearch",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_job_ai_chat_demo_runs_CorrelationId",
                schema: "jobsearch",
                table: "user_job_ai_chat_demo_runs",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_job_ai_chat_demo_runs_JobPostingId",
                schema: "jobsearch",
                table: "user_job_ai_chat_demo_runs",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_user_job_ai_chat_demo_runs_UserAccountId_JobPostingId_Reque~",
                schema: "jobsearch",
                table: "user_job_ai_chat_demo_runs",
                columns: new[] { "UserAccountId", "JobPostingId", "RequestedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_job_ai_chat_demo_runs",
                schema: "jobsearch");
        }
    }
}
