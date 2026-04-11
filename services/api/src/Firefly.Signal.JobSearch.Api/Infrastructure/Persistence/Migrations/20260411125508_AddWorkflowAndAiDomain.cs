using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.JobSearch.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowAndAiDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_analysis_runs",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    RequestedByUserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Mode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    JobCount = table.Column<int>(type: "integer", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_analysis_runs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "job_applications",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    JobPostingId = table.Column<long>(type: "bigint", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SubmittedCvDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    SubmittedCoverLetterDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    RejectionAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_job_applications_jobs_JobPostingId",
                        column: x => x.JobPostingId,
                        principalSchema: "jobsearch",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "postcode_lookups",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Postcode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Source = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LastVerifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postcode_lookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_job_states",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    JobPostingId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SavedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AppliedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_job_states", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_job_states_jobs_JobPostingId",
                        column: x => x.JobPostingId,
                        principalSchema: "jobsearch",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_job_ai_insights",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    JobPostingId = table.Column<long>(type: "bigint", nullable: false),
                    GeneratedByUserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    AiAnalysisRunId = table.Column<long>(type: "bigint", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DetailedExplanation = table.Column<string>(type: "character varying(12000)", maxLength: 12000, nullable: true),
                    CvImprovementSuggestions = table.Column<string>(type: "character varying(12000)", maxLength: 12000, nullable: true),
                    PromptVersion = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    GeneratedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_job_ai_insights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_job_ai_insights_ai_analysis_runs_AiAnalysisRunId",
                        column: x => x.AiAnalysisRunId,
                        principalSchema: "jobsearch",
                        principalTable: "ai_analysis_runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_user_job_ai_insights_jobs_JobPostingId",
                        column: x => x.JobPostingId,
                        principalSchema: "jobsearch",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_document_links",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    UserDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    LinkType = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_document_links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_application_document_links_job_applications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "jobsearch",
                        principalTable: "job_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_notes",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "IX_jobs_JobRefreshRunId",
                schema: "jobsearch",
                table: "jobs",
                column: "JobRefreshRunId");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_SourceName_SourceJobId",
                schema: "jobsearch",
                table: "jobs",
                columns: new[] { "SourceName", "SourceJobId" },
                unique: true,
                filter: "\"SourceJobId\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ai_analysis_runs_StartedAtUtc",
                schema: "jobsearch",
                table: "ai_analysis_runs",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ai_analysis_runs_Status",
                schema: "jobsearch",
                table: "ai_analysis_runs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_application_document_links_JobApplicationId",
                schema: "jobsearch",
                table: "application_document_links",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_application_document_links_JobApplicationId_UserDocumentId_~",
                schema: "jobsearch",
                table: "application_document_links",
                columns: new[] { "JobApplicationId", "UserDocumentId", "LinkType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_application_notes_JobApplicationId",
                schema: "jobsearch",
                table: "application_notes",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_JobPostingId",
                schema: "jobsearch",
                table: "job_applications",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_UserAccountId_JobPostingId",
                schema: "jobsearch",
                table: "job_applications",
                columns: new[] { "UserAccountId", "JobPostingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_UserAccountId_Status",
                schema: "jobsearch",
                table: "job_applications",
                columns: new[] { "UserAccountId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_postcode_lookups_Postcode",
                schema: "jobsearch",
                table: "postcode_lookups",
                column: "Postcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_job_ai_insights_AiAnalysisRunId",
                schema: "jobsearch",
                table: "user_job_ai_insights",
                column: "AiAnalysisRunId");

            migrationBuilder.CreateIndex(
                name: "IX_user_job_ai_insights_JobPostingId",
                schema: "jobsearch",
                table: "user_job_ai_insights",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_user_job_ai_insights_UserAccountId_JobPostingId_GeneratedAt~",
                schema: "jobsearch",
                table: "user_job_ai_insights",
                columns: new[] { "UserAccountId", "JobPostingId", "GeneratedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_user_job_states_JobPostingId",
                schema: "jobsearch",
                table: "user_job_states",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_user_job_states_UserAccountId_JobPostingId",
                schema: "jobsearch",
                table: "user_job_states",
                columns: new[] { "UserAccountId", "JobPostingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_job_states_UserAccountId_State",
                schema: "jobsearch",
                table: "user_job_states",
                columns: new[] { "UserAccountId", "State" });

            migrationBuilder.AddForeignKey(
                name: "FK_jobs_job_refresh_runs_JobRefreshRunId",
                schema: "jobsearch",
                table: "jobs",
                column: "JobRefreshRunId",
                principalSchema: "jobsearch",
                principalTable: "job_refresh_runs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobs_job_refresh_runs_JobRefreshRunId",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropTable(
                name: "application_document_links",
                schema: "jobsearch");

            migrationBuilder.DropTable(
                name: "application_notes",
                schema: "jobsearch");

            migrationBuilder.DropTable(
                name: "postcode_lookups",
                schema: "jobsearch");

            migrationBuilder.DropTable(
                name: "user_job_ai_insights",
                schema: "jobsearch");

            migrationBuilder.DropTable(
                name: "user_job_states",
                schema: "jobsearch");

            migrationBuilder.DropTable(
                name: "job_applications",
                schema: "jobsearch");

            migrationBuilder.DropTable(
                name: "ai_analysis_runs",
                schema: "jobsearch");

            migrationBuilder.DropIndex(
                name: "IX_jobs_JobRefreshRunId",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_jobs_SourceName_SourceJobId",
                schema: "jobsearch",
                table: "jobs");
        }
    }
}
