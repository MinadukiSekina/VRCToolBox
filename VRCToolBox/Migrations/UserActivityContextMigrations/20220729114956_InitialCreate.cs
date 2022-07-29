using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRCToolBox.Migrations.UserActivityContextMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorldVisits",
                columns: table => new
                {
                    WorldVisitId = table.Column<string>(type: "TEXT", nullable: false),
                    WorldName = table.Column<string>(type: "TEXT", nullable: false),
                    VisitTime = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldVisits", x => x.WorldVisitId);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    ActivityTime = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityType = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    WorldVisitId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => new { x.UserName, x.ActivityTime, x.ActivityType, x.FileName });
                    table.ForeignKey(
                        name: "FK_UserActivities_WorldVisits_WorldVisitId",
                        column: x => x.WorldVisitId,
                        principalTable: "WorldVisits",
                        principalColumn: "WorldVisitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserName",
                table: "UserActivities",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_WorldVisitId",
                table: "UserActivities",
                column: "WorldVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldVisits_VisitTime",
                table: "WorldVisits",
                column: "VisitTime");

            migrationBuilder.CreateIndex(
                name: "IX_WorldVisits_WorldName",
                table: "WorldVisits",
                column: "WorldName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "WorldVisits");
        }
    }
}
