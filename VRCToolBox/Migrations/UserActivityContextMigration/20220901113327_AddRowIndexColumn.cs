using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.UserActivityContextMigration
{
    public partial class AddRowIndexColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserActivities",
                table: "UserActivities");

            migrationBuilder.AddColumn<long>(
                name: "FileRowIndex",
                table: "UserActivities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserActivities",
                table: "UserActivities",
                columns: new[] { "UserName", "ActivityTime", "ActivityType", "FileName", "FileRowIndex" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserActivities",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "FileRowIndex",
                table: "UserActivities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserActivities",
                table: "UserActivities",
                columns: new[] { "UserName", "ActivityTime", "ActivityType", "FileName" });
        }
    }
}
