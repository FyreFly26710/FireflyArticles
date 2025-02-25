using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.Contents.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                schema: "Contents",
                table: "Topic");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "Contents",
                table: "Topic",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HierachyType",
                schema: "Contents",
                table: "Article",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ArticleGroup",
                schema: "Contents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    ParentArticleId = table.Column<int>(type: "int", nullable: false),
                    SortNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleGroup", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleGroup",
                schema: "Contents");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "Contents",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "HierachyType",
                schema: "Contents",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                schema: "Contents",
                table: "Topic",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "");
        }
    }
}
