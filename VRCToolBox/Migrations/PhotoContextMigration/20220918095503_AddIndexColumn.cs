using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    public partial class AddIndexColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "Photos");
        }
    }
}
