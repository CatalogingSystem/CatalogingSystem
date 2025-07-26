using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogingSystem.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class ConservationCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Expediente = table.Column<long>(type: "bigint", nullable: false),
                    Inventory = table.Column<long>(type: "bigint", nullable: false),
                    AffectedArea = table.Column<string>(type: "text", nullable: true),
                    Length = table.Column<string>(type: "text", nullable: true),
                    Width = table.Column<string>(type: "text", nullable: true),
                    Depth = table.Column<string>(type: "text", nullable: true),
                    Reports = table.Column<string>(type: "text", nullable: true),
                    AnalysisTypes = table.Column<string>(type: "text", nullable: true),
                    Results = table.Column<string>(type: "text", nullable: true),
                    TreatmentType = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SpecialConditions = table.Column<string>(type: "text", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conservations_ArchivosAdministrativos_Expediente",
                        column: x => x.Expediente,
                        principalTable: "ArchivosAdministrativos",
                        principalColumn: "expediente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conservations_Expediente",
                table: "Conservations",
                column: "Expediente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conservations");
        }
    }
}
