using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MeterDataDashboard.Infra.Migrations.MeterDb
{
    public partial class scadaNodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ScadaArchiveMeasurements",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'852', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "ScadaNodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    IpAddress = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScadaNodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScadaNodes_IpAddress",
                table: "ScadaNodes",
                column: "IpAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScadaNodes_Name",
                table: "ScadaNodes",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScadaNodes");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ScadaArchiveMeasurements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:IdentitySequenceOptions", "'852', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
