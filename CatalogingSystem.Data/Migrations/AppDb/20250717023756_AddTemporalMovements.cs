using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogingSystem.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddTemporalMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemporalMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    MovementType = table.Column<int>(type: "integer", nullable: false),
                    Expediente = table.Column<long>(type: "bigint", nullable: false),
                    Applicant_FirstName = table.Column<string>(type: "text", nullable: false),
                    Applicant_LastName = table.Column<string>(type: "text", nullable: false),
                    Applicant_IdentityCard = table.Column<string>(type: "text", nullable: false),
                    Applicant_InstitutionalId = table.Column<string>(type: "text", nullable: false),
                    Applicant_Institution = table.Column<string>(type: "text", nullable: false),
                    Applicant_Address = table.Column<string>(type: "text", nullable: false),
                    Applicant_Locality = table.Column<string>(type: "text", nullable: false),
                    Applicant_Province = table.Column<string>(type: "text", nullable: false),
                    Applicant_Department = table.Column<string>(type: "text", nullable: false),
                    Applicant_Country = table.Column<string>(type: "text", nullable: false),
                    Applicant_PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Applicant_Email = table.Column<string>(type: "text", nullable: false),
                    Applicant_References = table.Column<string>(type: "text", nullable: true),
                    Applicant_Observations = table.Column<string>(type: "text", nullable: true),
                    Representative_FirstName = table.Column<string>(type: "text", nullable: true),
                    Representative_LastName = table.Column<string>(type: "text", nullable: true),
                    Representative_IdentityCard = table.Column<string>(type: "text", nullable: true),
                    Representative_InstitutionalId = table.Column<string>(type: "text", nullable: true),
                    Representative_Institution = table.Column<string>(type: "text", nullable: true),
                    Representative_Address = table.Column<string>(type: "text", nullable: true),
                    Representative_Locality = table.Column<string>(type: "text", nullable: true),
                    Representative_Province = table.Column<string>(type: "text", nullable: true),
                    Representative_Department = table.Column<string>(type: "text", nullable: true),
                    Representative_Country = table.Column<string>(type: "text", nullable: true),
                    Representative_PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Representative_Email = table.Column<string>(type: "text", nullable: true),
                    Representative_References = table.Column<string>(type: "text", nullable: true),
                    Representative_Observations = table.Column<string>(type: "text", nullable: true),
                    Entity = table.Column<string>(type: "text", nullable: true),
                    TransferLocation = table.Column<string>(type: "text", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Document = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Insurer = table.Column<string>(type: "text", nullable: true),
                    Policy = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Departure_Company = table.Column<string>(type: "text", nullable: true),
                    Departure_Location = table.Column<string>(type: "text", nullable: true),
                    Departure_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Departure_Time = table.Column<string>(type: "text", nullable: true),
                    Departure_Notes = table.Column<string>(type: "text", nullable: true),
                    Return_Company = table.Column<string>(type: "text", nullable: true),
                    Return_Location = table.Column<string>(type: "text", nullable: true),
                    Return_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Return_Time = table.Column<string>(type: "text", nullable: true),
                    Return_Notes = table.Column<string>(type: "text", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemporalMovements_ArchivosAdministrativos_Expediente",
                        column: x => x.Expediente,
                        principalTable: "ArchivosAdministrativos",
                        principalColumn: "expediente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemporalMovements_Expediente",
                table: "TemporalMovements",
                column: "Expediente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporalMovements");
        }
    }
}
