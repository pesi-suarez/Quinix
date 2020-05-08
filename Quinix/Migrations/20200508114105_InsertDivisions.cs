using Microsoft.EntityFrameworkCore.Migrations;

namespace Quinix.Migrations
{
    public partial class InsertDivisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Divisions(Name, Number) VALUES ('primera', 1), ('segunda', 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Divisions WHERE Name IN ('primera', 'segunda')");
        }
    }
}
