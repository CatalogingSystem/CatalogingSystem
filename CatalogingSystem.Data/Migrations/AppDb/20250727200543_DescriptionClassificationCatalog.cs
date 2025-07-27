using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogingSystem.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class DescriptionClassificationCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DescriptionClassifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Expediente = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Decoration_Location = table.Column<string>(type: "text", nullable: true),
                    Decoration_MotifOrIconIdentification = table.Column<string>(type: "text", nullable: true),
                    Decoration_Description = table.Column<string>(type: "text", nullable: true),
                    Decoration_Notes = table.Column<string>(type: "text", nullable: true),
                    TechnicalCharacteristics_DescribedPart = table.Column<string>(type: "text", nullable: true),
                    TechnicalCharacteristics_Characteristics = table.Column<string>(type: "text", nullable: true),
                    TechnicalCharacteristics_Description = table.Column<string>(type: "text", nullable: true),
                    TechnicalCharacteristics_Notes = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_Length = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_Width = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_Depth = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_Thickness = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_MouthCircumference = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_BodyCircumference = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_BaseCircumference = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_ObjectTypeSpecifications = table.Column<string>(type: "text", nullable: true),
                    DescriptionDimensions_Notes = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Location = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Method = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Author = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Script = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Language = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_CharacterType = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SignaturesAndMarks_Transcription = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Translation = table.Column<string>(type: "text", nullable: true),
                    SignaturesAndMarks_Notes = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Location = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Method = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Author = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Script = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Language = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_CharacterType = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Inscriptions_Transcription = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Translation = table.Column<string>(type: "text", nullable: true),
                    Inscriptions_Notes = table.Column<string>(type: "text", nullable: true),
                    PlaceOfElaboration_SpecificPlace = table.Column<string>(type: "text", nullable: true),
                    PlaceOfElaboration_Canton = table.Column<string>(type: "text", nullable: true),
                    PlaceOfElaboration_Province = table.Column<string>(type: "text", nullable: true),
                    PlaceOfElaboration_Department = table.Column<string>(type: "text", nullable: true),
                    PlaceOfElaboration_Country = table.Column<string>(type: "text", nullable: true),
                    PlaceOfElaboration_Notes = table.Column<string>(type: "text", nullable: true),
                    CulturalContext_DescribedPart = table.Column<string>(type: "text", nullable: true),
                    CulturalContext_RelatedCulturalElements = table.Column<string>(type: "text", nullable: true),
                    CollectionProvenance_SpecificPlace = table.Column<string>(type: "text", nullable: true),
                    CollectionProvenance_Canton = table.Column<string>(type: "text", nullable: true),
                    CollectionProvenance_Province = table.Column<string>(type: "text", nullable: true),
                    CollectionProvenance_Department = table.Column<string>(type: "text", nullable: true),
                    CollectionProvenance_Country = table.Column<string>(type: "text", nullable: true),
                    CollectionProvenance_Notes = table.Column<string>(type: "text", nullable: true),
                    ReasonedClassification_Classification = table.Column<string>(type: "text", nullable: true),
                    ReasonedClassification_Notes = table.Column<string>(type: "text", nullable: true),
                    Bibliography_Title = table.Column<string>(type: "text", nullable: true),
                    Bibliography_RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    Bibliography_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Bibliography_Author = table.Column<string>(type: "text", nullable: true),
                    Bibliography_SourceDocument = table.Column<string>(type: "text", nullable: true),
                    Bibliography_Page = table.Column<string>(type: "text", nullable: true),
                    Bibliography_TextualCitation = table.Column<string>(type: "text", nullable: true),
                    Bibliography_Notes = table.Column<string>(type: "text", nullable: true),
                    ObjectHistory_GeneralHistory = table.Column<string>(type: "text", nullable: true),
                    ObjectHistory_ObjectHistoryDetails = table.Column<string>(type: "text", nullable: true),
                    ObjectHistory_Notes = table.Column<string>(type: "text", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescriptionClassifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DescriptionClassifications_ArchivosAdministrativos_Expedien~",
                        column: x => x.Expediente,
                        principalTable: "ArchivosAdministrativos",
                        principalColumn: "expediente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionClassifications_Expediente",
                table: "DescriptionClassifications",
                column: "Expediente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DescriptionClassifications");
        }
    }
}
