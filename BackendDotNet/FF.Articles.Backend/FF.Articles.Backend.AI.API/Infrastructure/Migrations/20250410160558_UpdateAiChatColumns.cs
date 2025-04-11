using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.AI.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAiChatColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "AI",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "AI",
                table: "ChatRound");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                schema: "AI",
                table: "Session",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TimeStamp",
                schema: "AI",
                table: "Session",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                schema: "AI",
                table: "Session",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                schema: "AI",
                table: "ChatRound",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TimeStamp",
                schema: "AI",
                table: "ChatRound",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                schema: "AI",
                table: "ChatRound",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                schema: "AI",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                schema: "AI",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                schema: "AI",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                schema: "AI",
                table: "ChatRound");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                schema: "AI",
                table: "ChatRound");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                schema: "AI",
                table: "ChatRound");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "AI",
                table: "Session",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 6, 14, 17, 6, 979, DateTimeKind.Utc).AddTicks(5090));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "AI",
                table: "ChatRound",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
