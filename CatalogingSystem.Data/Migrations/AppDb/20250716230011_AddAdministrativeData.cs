using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogingSystem.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddAdministrativeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdministrativeData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    FileNumber = table.Column<long>(type: "bigint", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntryForm = table.Column<string>(type: "text", nullable: true),
                    EntrySource = table.Column<string>(type: "text", nullable: true),
                    CollectionType = table.Column<string>(type: "text", nullable: true),
                    CopiesReproductions_Author = table.Column<string>(type: "text", nullable: true),
                    CopiesReproductions_OriginalTitle = table.Column<string>(type: "text", nullable: true),
                    CopiesReproductions_Method = table.Column<string>(type: "text", nullable: true),
                    CopiesReproductions_Format = table.Column<string>(type: "text", nullable: true),
                    CopiesReproductions_OriginalDestination = table.Column<string>(type: "text", nullable: true),
                    CopiesReproductions_Location = table.Column<string>(type: "text", nullable: true),
                    CopiesReproductions_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CopiesReproductions_Notes = table.Column<string>(type: "text", nullable: true),
                    Valuation_Value = table.Column<string>(type: "text", nullable: true),
                    Valuation_Appraiser = table.Column<string>(type: "text", nullable: true),
                    Valuation_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Valuation_Notes = table.Column<string>(type: "text", nullable: true),
                    Cataloger_FirstName = table.Column<string>(type: "text", nullable: true),
                    Cataloger_LastName = table.Column<string>(type: "text", nullable: true),
                    Cataloger_IdentityCard = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Institution = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Address = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Locality = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Province = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Department = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Country = table.Column<string>(type: "text", nullable: true),
                    Cataloger_PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Email = table.Column<string>(type: "text", nullable: true),
                    Cataloger_References = table.Column<string>(type: "text", nullable: true),
                    Cataloger_Observations = table.Column<string>(type: "text", nullable: true),
                    CatalogingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativeData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdministrativeData_ArchivosAdministrativos_FileNumber",
                        column: x => x.FileNumber,
                        principalTable: "ArchivosAdministrativos",
                        principalColumn: "expediente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministrativeData_FileNumber",
                table: "AdministrativeData",
                column: "FileNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdministrativeData");
        }
    }
}
