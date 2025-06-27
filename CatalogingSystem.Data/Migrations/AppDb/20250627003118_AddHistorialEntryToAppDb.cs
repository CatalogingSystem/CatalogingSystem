using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogingSystem.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddHistorialEntryToAppDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreated",
                table: "GraphicDocumentations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsModified",
                table: "GraphicDocumentations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "GraphicDocumentations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "GraphicDocumentations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreated",
                table: "ArchivosAdministrativos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsModified",
                table: "ArchivosAdministrativos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "ArchivosAdministrativos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ArchivosAdministrativos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArchivoAdministrativoId = table.Column<Guid>(type: "uuid", nullable: true),
                    GraphicDocumentationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEntries_ArchivosAdministrativos_ArchivoAdministrat~",
                        column: x => x.ArchivoAdministrativoId,
                        principalTable: "ArchivosAdministrativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialEntries_AuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialEntries_GraphicDocumentations_GraphicDocumentation~",
                        column: x => x.GraphicDocumentationId,
                        principalTable: "GraphicDocumentations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEntries_ArchivoAdministrativoId",
                table: "HistorialEntries",
                column: "ArchivoAdministrativoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEntries_AuditLogId",
                table: "HistorialEntries",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEntries_GraphicDocumentationId",
                table: "HistorialEntries",
                column: "GraphicDocumentationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialEntries");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsCreated",
                table: "GraphicDocumentations");

            migrationBuilder.DropColumn(
                name: "IsModified",
                table: "GraphicDocumentations");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "GraphicDocumentations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "GraphicDocumentations");

            migrationBuilder.DropColumn(
                name: "IsCreated",
                table: "ArchivosAdministrativos");

            migrationBuilder.DropColumn(
                name: "IsModified",
                table: "ArchivosAdministrativos");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "ArchivosAdministrativos");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ArchivosAdministrativos");
        }
    }
}
