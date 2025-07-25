using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogingSystem.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class Dating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Departure_IsPresent",
                table: "TemporalMovements",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Return_IsPresent",
                table: "TemporalMovements",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Datings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Expediente = table.Column<long>(type: "bigint", nullable: false),
                    Inventory = table.Column<long>(type: "bigint", nullable: false),
                    SimpleDate_IsPresent = table.Column<bool>(type: "boolean", nullable: true),
                    SimpleDate_Exact = table.Column<string>(type: "text", nullable: true),
                    SimpleDate_Approximate = table.Column<string>(type: "text", nullable: true),
                    SimpleDate_Probable = table.Column<string>(type: "text", nullable: true),
                    SimpleDate_BC = table.Column<long>(type: "bigint", nullable: true),
                    SimpleDate_Year = table.Column<long>(type: "bigint", nullable: true),
                    SimpleDate_Month = table.Column<long>(type: "bigint", nullable: true),
                    SimpleDate_Day = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_IsPresent = table.Column<bool>(type: "boolean", nullable: true),
                    DateRange_From_IsPresent = table.Column<bool>(type: "boolean", nullable: true),
                    DateRange_From_Exact = table.Column<string>(type: "text", nullable: true),
                    DateRange_From_Approximate = table.Column<string>(type: "text", nullable: true),
                    DateRange_From_Probable = table.Column<string>(type: "text", nullable: true),
                    DateRange_From_BC = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_From_Year = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_From_Month = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_From_Day = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_To_IsPresent = table.Column<bool>(type: "boolean", nullable: true),
                    DateRange_To_Exact = table.Column<string>(type: "text", nullable: true),
                    DateRange_To_Approximate = table.Column<string>(type: "text", nullable: true),
                    DateRange_To_Probable = table.Column<string>(type: "text", nullable: true),
                    DateRange_To_BC = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_To_Year = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_To_Month = table.Column<long>(type: "bigint", nullable: true),
                    DateRange_To_Day = table.Column<long>(type: "bigint", nullable: true),
                    ApproximateDating_IsPresent = table.Column<bool>(type: "boolean", nullable: true),
                    ApproximateDating_FromCentury = table.Column<int>(type: "integer", nullable: true),
                    ApproximateDating_ToCentury = table.Column<int>(type: "integer", nullable: true),
                    Notes_IsPresent = table.Column<bool>(type: "boolean", nullable: true),
                    Notes_TextualDate = table.Column<string>(type: "text", nullable: true),
                    Notes_InitialDateNotes = table.Column<string>(type: "text", nullable: true),
                    Notes_FinalDateNotes = table.Column<string>(type: "text", nullable: true),
                    Notes_Observations = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Datings_ArchivosAdministrativos_Expediente",
                        column: x => x.Expediente,
                        principalTable: "ArchivosAdministrativos",
                        principalColumn: "expediente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Datings_Expediente",
                table: "Datings",
                column: "Expediente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Datings");

            migrationBuilder.DropColumn(
                name: "Departure_IsPresent",
                table: "TemporalMovements");

            migrationBuilder.DropColumn(
                name: "Return_IsPresent",
                table: "TemporalMovements");
        }
    }
}
