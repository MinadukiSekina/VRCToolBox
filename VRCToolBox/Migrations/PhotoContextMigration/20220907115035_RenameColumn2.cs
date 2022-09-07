using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class RenameColumn2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Worlds_WorldDataWorldId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_WorldDataWorldId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "WorldDataWorldId",
                table: "Photos");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_WorldId",
                table: "Photos",
                column: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Worlds_WorldId",
                table: "Photos",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Worlds_WorldId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_WorldId",
                table: "Photos");

            migrationBuilder.AddColumn<string>(
                name: "WorldDataWorldId",
                table: "Photos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_WorldDataWorldId",
                table: "Photos",
                column: "WorldDataWorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Worlds_WorldDataWorldId",
                table: "Photos",
                column: "WorldDataWorldId",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }
    }
}
