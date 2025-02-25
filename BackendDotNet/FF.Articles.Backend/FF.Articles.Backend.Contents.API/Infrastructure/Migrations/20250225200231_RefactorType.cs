using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.Contents.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleGroup",
                schema: "Contents");

            migrationBuilder.AddColumn<int>(
                name: "ParentArticleId",
                schema: "Contents",
                table: "Article",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentArticleId",
                schema: "Contents",
                table: "Article");

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
    }
}
