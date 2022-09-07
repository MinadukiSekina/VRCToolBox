using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class RenameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Avatars_AvatarDataAvatarId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_AvatarDataAvatarId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "AvatarDataAvatarId",
                table: "Photos");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AvatarId",
                table: "Photos",
                column: "AvatarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Avatars_AvatarId",
                table: "Photos",
                column: "AvatarId",
                principalTable: "Avatars",
                principalColumn: "AvatarId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Avatars_AvatarId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_AvatarId",
                table: "Photos");

            migrationBuilder.AddColumn<string>(
                name: "AvatarDataAvatarId",
                table: "Photos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AvatarDataAvatarId",
                table: "Photos",
                column: "AvatarDataAvatarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Avatars_AvatarDataAvatarId",
                table: "Photos",
                column: "AvatarDataAvatarId",
                principalTable: "Avatars",
                principalColumn: "AvatarId");
        }
    }
}
