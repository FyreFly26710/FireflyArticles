using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.Contents.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Contents");

            migrationBuilder.CreateTable(
                name: "Article",
                schema: "Contents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: ""),
                    Content = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Abstract = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false, defaultValue: ""),
                    ArticleType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Article"),
                    ParentArticleId = table.Column<long>(type: "bigint", nullable: true, defaultValue: 0L),
                    UserId = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    TopicId = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    SortNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsHidden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<int>(type: "integer", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTag",
                schema: "Contents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ArticleId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                schema: "Contents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TagName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TagGroup = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TagColour = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                schema: "Contents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: ""),
                    Abstract = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false, defaultValue: ""),
                    Category = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: ""),
                    TopicImage = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false, defaultValue: ""),
                    UserId = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    SortNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsHidden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<int>(type: "integer", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_ParentArticleId",
                schema: "Contents",
                table: "Article",
                column: "ParentArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_TopicId_SortNumber",
                schema: "Contents",
                table: "Article",
                columns: new[] { "TopicId", "SortNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Article",
                schema: "Contents");

            migrationBuilder.DropTable(
                name: "ArticleTag",
                schema: "Contents");

            migrationBuilder.DropTable(
                name: "Tag",
                schema: "Contents");

            migrationBuilder.DropTable(
                name: "Topic",
                schema: "Contents");
        }
    }
}
