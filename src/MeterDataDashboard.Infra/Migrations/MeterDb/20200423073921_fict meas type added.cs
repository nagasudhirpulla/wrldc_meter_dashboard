using Microsoft.EntityFrameworkCore.Migrations;

namespace MeterDataDashboard.Infra.Migrations.MeterDb
{
    public partial class fictmeastypeadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeasType",
                table: "FictMeasurements",
                nullable: false,
                defaultValue: "Fict");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasType",
                table: "FictMeasurements");
        }
    }
}
