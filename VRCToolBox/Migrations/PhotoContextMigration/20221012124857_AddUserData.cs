using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class AddUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorldAuthor",
                table: "Worlds",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "AvatarAuthor",
                table: "Avatars",
                newName: "AuthorId");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    VRChatName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_VRChatName",
                table: "Users",
                column: "VRChatName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Worlds",
                newName: "WorldAuthor");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Avatars",
                newName: "AvatarAuthor");
        }
    }
}
