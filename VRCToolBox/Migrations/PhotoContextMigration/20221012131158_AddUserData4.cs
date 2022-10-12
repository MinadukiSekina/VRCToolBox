using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class AddUserData4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwitterId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterName",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserDataUserId",
                table: "Tweets",
                type: "TEXT",
                nullable: true);

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
                name: "TwitterId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwitterName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserDataUserId",
                table: "Tweets");
        }
    }
}
