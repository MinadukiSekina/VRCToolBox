using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class AddColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoDirPath",
                table: "Photos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoDirPath",
                table: "Photos");
        }
    }
}
