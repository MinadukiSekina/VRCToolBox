using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class AddUserData6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_Users_UserDataUserId",
                table: "Tweets");

            migrationBuilder.DropIndex(
                name: "IX_Tweets_UserDataUserId",
                table: "Tweets");

            migrationBuilder.DropColumn(
                name: "UserDataUserId",
                table: "Tweets");

            migrationBuilder.CreateTable(
                name: "TweetUserData",
                columns: table => new
                {
                    TweetsTweetId = table.Column<string>(type: "TEXT", nullable: false),
                    UsersUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TweetUserData", x => new { x.TweetsTweetId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_TweetUserData_Tweets_TweetsTweetId",
                        column: x => x.TweetsTweetId,
                        principalTable: "Tweets",
                        principalColumn: "TweetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TweetUserData_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TweetUserData_UsersUserId",
                table: "TweetUserData",
                column: "UsersUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TweetUserData");

            migrationBuilder.AddColumn<string>(
                name: "UserDataUserId",
                table: "Tweets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_UserDataUserId",
                table: "Tweets",
                column: "UserDataUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_Users_UserDataUserId",
                table: "Tweets",
                column: "UserDataUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
