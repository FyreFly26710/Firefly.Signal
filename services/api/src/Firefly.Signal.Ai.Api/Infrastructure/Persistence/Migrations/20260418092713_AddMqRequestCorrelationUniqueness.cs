using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firefly.Signal.Ai.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMqRequestCorrelationUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ai_requests_CorrelationId",
                schema: "ai",
                table: "ai_requests");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_CorrelationId",
                schema: "ai",
                table: "ai_requests",
                column: "CorrelationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ai_requests_CorrelationId",
                schema: "ai",
                table: "ai_requests");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_CorrelationId",
                schema: "ai",
                table: "ai_requests",
                column: "CorrelationId");
        }
    }
}
