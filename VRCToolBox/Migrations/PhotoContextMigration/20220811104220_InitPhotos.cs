using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class InitPhotos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avatars",
                columns: table => new
                {
                    AvatarId = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarName = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarAuthor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avatars", x => x.AvatarId);
                });

            migrationBuilder.CreateTable(
                name: "PhotoTags",
                columns: table => new
                {
                    TagId = table.Column<string>(type: "TEXT", nullable: false),
                    TagName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoTags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Tweets",
                columns: table => new
                {
                    TweetId = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    IsTweeted = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tweets", x => x.TweetId);
                });

            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    WorldId = table.Column<string>(type: "TEXT", nullable: false),
                    WorldName = table.Column<string>(type: "TEXT", nullable: false),
                    WorldAuthor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.WorldId);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    PhotoName = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarId = table.Column<string>(type: "TEXT", nullable: true),
                    WorldId = table.Column<string>(type: "TEXT", nullable: true),
                    TweetId = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarDataAvatarId = table.Column<string>(type: "TEXT", nullable: true),
                    WorldDataWorldId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.PhotoName);
                    table.ForeignKey(
                        name: "FK_Photos_Avatars_AvatarDataAvatarId",
                        column: x => x.AvatarDataAvatarId,
                        principalTable: "Avatars",
                        principalColumn: "AvatarId");
                    table.ForeignKey(
                        name: "FK_Photos_Tweets_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweets",
                        principalColumn: "TweetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Photos_Worlds_WorldDataWorldId",
                        column: x => x.WorldDataWorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId");
                });

            migrationBuilder.CreateTable(
                name: "PhotoDataPhotoTag",
                columns: table => new
                {
                    PhotosPhotoName = table.Column<string>(type: "TEXT", nullable: false),
                    TagsTagId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoDataPhotoTag", x => new { x.PhotosPhotoName, x.TagsTagId });
                    table.ForeignKey(
                        name: "FK_PhotoDataPhotoTag_Photos_PhotosPhotoName",
                        column: x => x.PhotosPhotoName,
                        principalTable: "Photos",
                        principalColumn: "PhotoName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoDataPhotoTag_PhotoTags_TagsTagId",
                        column: x => x.TagsTagId,
                        principalTable: "PhotoTags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoDataPhotoTag_TagsTagId",
                table: "PhotoDataPhotoTag",
                column: "TagsTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AvatarDataAvatarId",
                table: "Photos",
                column: "AvatarDataAvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_TweetId",
                table: "Photos",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_WorldDataWorldId",
                table: "Photos",
                column: "WorldDataWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoTags_TagName",
                table: "PhotoTags",
                column: "TagName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoDataPhotoTag");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "PhotoTags");

            migrationBuilder.DropTable(
                name: "Avatars");

            migrationBuilder.DropTable(
                name: "Tweets");

            migrationBuilder.DropTable(
                name: "Worlds");
        }
    }
}
