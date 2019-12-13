using Microsoft.EntityFrameworkCore.Migrations;

namespace MeterDataDashboard.Infra.Migrations.MeterDb
{
    public partial class scadaAttrModify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MeasType",
                table: "ScadaArchiveMeasurements",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MeasType",
                table: "ScadaArchiveMeasurements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250);
        }
    }
}
