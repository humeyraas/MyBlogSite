using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlogSite.Migrations
{
    /// <inheritdoc />
    public partial class AddRepostsTableFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reposts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reposts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reposts_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reposts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);

                });

            migrationBuilder.CreateIndex(
                name: "IX_Reposts_BlogId",
                table: "Reposts",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Reposts_UserId",
                table: "Reposts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reposts");
        }
    }
}
