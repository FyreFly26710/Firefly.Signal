using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.Ai.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ai");

            migrationBuilder.CreateTable(
                name: "ai_messages",
                schema: "ai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ai_requests",
                schema: "ai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SystemPromptMessageId = table.Column<long>(type: "bigint", nullable: true),
                    UserPromptMessageId = table.Column<long>(type: "bigint", nullable: true),
                    Source = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Model = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CallerService = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ReplyEventType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessingStartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ai_requests_ai_messages_SystemPromptMessageId",
                        column: x => x.SystemPromptMessageId,
                        principalSchema: "ai",
                        principalTable: "ai_messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ai_requests_ai_messages_UserPromptMessageId",
                        column: x => x.UserPromptMessageId,
                        principalSchema: "ai",
                        principalTable: "ai_messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ai_responses",
                schema: "ai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    AiRequestId = table.Column<long>(type: "bigint", nullable: false),
                    AiResponseMessageId = table.Column<long>(type: "bigint", nullable: false),
                    PromptTokens = table.Column<int>(type: "integer", nullable: true),
                    CompletionTokens = table.Column<int>(type: "integer", nullable: true),
                    GeneratedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ai_responses_ai_messages_AiResponseMessageId",
                        column: x => x.AiResponseMessageId,
                        principalSchema: "ai",
                        principalTable: "ai_messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ai_responses_ai_requests_AiRequestId",
                        column: x => x.AiRequestId,
                        principalSchema: "ai",
                        principalTable: "ai_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_messages_CreatedAtUtc",
                schema: "ai",
                table: "ai_messages",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_CorrelationId",
                schema: "ai",
                table: "ai_requests",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_Source",
                schema: "ai",
                table: "ai_requests",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_Status",
                schema: "ai",
                table: "ai_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_SystemPromptMessageId",
                schema: "ai",
                table: "ai_requests",
                column: "SystemPromptMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_UserPromptMessageId",
                schema: "ai",
                table: "ai_requests",
                column: "UserPromptMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ai_responses_AiRequestId",
                schema: "ai",
                table: "ai_responses",
                column: "AiRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ai_responses_AiResponseMessageId",
                schema: "ai",
                table: "ai_responses",
                column: "AiResponseMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_responses",
                schema: "ai");

            migrationBuilder.DropTable(
                name: "ai_requests",
                schema: "ai");

            migrationBuilder.DropTable(
                name: "ai_messages",
                schema: "ai");
        }
    }
}
