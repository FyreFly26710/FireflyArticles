using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.AI.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSessionNameType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SessionName",
                schema: "AI",
                table: "Session",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldDefaultValue: "New Session");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "AI",
                table: "ChatRound",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "AI",
                table: "ChatRound");

            migrationBuilder.AlterColumn<string>(
                name: "SessionName",
                schema: "AI",
                table: "Session",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "New Session",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
