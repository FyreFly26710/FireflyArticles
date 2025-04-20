using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.AI.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AI");

            migrationBuilder.CreateTable(
                name: "ChatRound",
                schema: "AI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SessionId = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    SystemMessageId = table.Column<long>(type: "bigint", nullable: true),
                    UserMessage = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    AssistantMessage = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Model = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeTaken = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PromptTokens = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CompletionTokens = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRound", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                schema: "AI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    SessionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: "New Session")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemMessage",
                schema: "AI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMessage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatRound_SessionId",
                schema: "AI",
                table: "ChatRound",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_UserId",
                schema: "AI",
                table: "Session",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRound",
                schema: "AI");

            migrationBuilder.DropTable(
                name: "Session",
                schema: "AI");

            migrationBuilder.DropTable(
                name: "SystemMessage",
                schema: "AI");
        }
    }
}
