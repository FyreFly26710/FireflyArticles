using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FF.Articles.Backend.Contents.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorArticleType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HierachyType",
                schema: "Contents",
                table: "Article",
                newName: "ArticleType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArticleType",
                schema: "Contents",
                table: "Article",
                newName: "HierachyType");
        }
    }
}
