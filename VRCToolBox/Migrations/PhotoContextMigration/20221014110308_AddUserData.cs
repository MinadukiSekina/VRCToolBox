using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class AddUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorldAuthor",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "AvatarAuthor",
                table: "Avatars");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Worlds",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserDataUserId",
                table: "Tweets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Avatars",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    VRChatName = table.Column<string>(type: "TEXT", nullable: true),
                    TwitterId = table.Column<string>(type: "TEXT", nullable: true),
                    TwitterName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_AuthorId",
                table: "Worlds",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_UserDataUserId",
                table: "Tweets",
                column: "UserDataUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Avatars_AuthorId",
                table: "Avatars",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_VRChatName",
                table: "Users",
                column: "VRChatName");

            migrationBuilder.AddForeignKey(
                name: "FK_Avatars_Users_AuthorId",
                table: "Avatars",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_Users_UserDataUserId",
                table: "Tweets",
                column: "UserDataUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_Users_AuthorId",
                table: "Worlds",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avatars_Users_AuthorId",
                table: "Avatars");

            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_Users_UserDataUserId",
                table: "Tweets");

            migrationBuilder.DropForeignKey(
                name: "FK_Worlds_Users_AuthorId",
                table: "Worlds");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Worlds_AuthorId",
                table: "Worlds");

            migrationBuilder.DropIndex(
                name: "IX_Tweets_UserDataUserId",
                table: "Tweets");

            migrationBuilder.DropIndex(
                name: "IX_Avatars_AuthorId",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "UserDataUserId",
                table: "Tweets");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Avatars");

            migrationBuilder.AddColumn<string>(
                name: "WorldAuthor",
                table: "Worlds",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarAuthor",
                table: "Avatars",
                type: "TEXT",
                nullable: true);
        }
    }
}
