using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.JobSearch.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDenormalizedJobImportDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "CategoryLabel",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryTag",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCanonicalName",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyDisplayName",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractTime",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractType",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(8000)",
                maxLength: 8000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportedAtUtc",
                schema: "jobsearch",
                table: "jobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsContract",
                schema: "jobsearch",
                table: "jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFullTime",
                schema: "jobsearch",
                table: "jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                schema: "jobsearch",
                table: "jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartTime",
                schema: "jobsearch",
                table: "jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                schema: "jobsearch",
                table: "jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "JobRefreshRunId",
                schema: "jobsearch",
                table: "jobs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenAtUtc",
                schema: "jobsearch",
                table: "jobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                schema: "jobsearch",
                table: "jobs",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationAreaJson",
                schema: "jobsearch",
                table: "jobs",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationDisplayName",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                schema: "jobsearch",
                table: "jobs",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawPayloadJson",
                schema: "jobsearch",
                table: "jobs",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryCurrency",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SalaryIsPredicted",
                schema: "jobsearch",
                table: "jobs",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryMax",
                schema: "jobsearch",
                table: "jobs",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryMin",
                schema: "jobsearch",
                table: "jobs",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceAdReference",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceJobId",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "job_refresh_runs",
                schema: "jobsearch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    RequestFiltersJson = table.Column<string>(type: "jsonb", nullable: false),
                    RequestedPageSize = table.Column<int>(type: "integer", nullable: false),
                    RequestedMaxPages = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PagesRequested = table.Column<int>(type: "integer", nullable: false),
                    PagesCompleted = table.Column<int>(type: "integer", nullable: false),
                    RecordsReceived = table.Column<int>(type: "integer", nullable: false),
                    RecordsInserted = table.Column<int>(type: "integer", nullable: false),
                    RecordsHidden = table.Column<int>(type: "integer", nullable: false),
                    RecordsFailed = table.Column<int>(type: "integer", nullable: false),
                    FailureMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_refresh_runs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobs_IsHidden",
                schema: "jobsearch",
                table: "jobs",
                column: "IsHidden");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_SourceName",
                schema: "jobsearch",
                table: "jobs",
                column: "SourceName");

            migrationBuilder.CreateIndex(
                name: "IX_job_refresh_runs_StartedAtUtc",
                schema: "jobsearch",
                table: "job_refresh_runs",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_job_refresh_runs_Status",
                schema: "jobsearch",
                table: "job_refresh_runs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_refresh_runs",
                schema: "jobsearch");

            migrationBuilder.DropIndex(
                name: "IX_jobs_IsHidden",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_jobs_SourceName",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "CategoryLabel",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "CategoryTag",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "CompanyCanonicalName",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "CompanyDisplayName",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "ContractTime",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "ContractType",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "ImportedAtUtc",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "IsContract",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "IsFullTime",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "IsPartTime",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "IsPermanent",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "JobRefreshRunId",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "LastSeenAtUtc",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "LocationAreaJson",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "LocationDisplayName",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "RawPayloadJson",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "SalaryCurrency",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "SalaryIsPredicted",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "SalaryMax",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "SalaryMin",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "SourceAdReference",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "SourceJobId",
                schema: "jobsearch",
                table: "jobs");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "jobsearch",
                table: "jobs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);
        }
    }
}
