using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.Identity.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserAccount = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    UserAvatar = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UserProfile = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UserRole = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValue: "user"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
