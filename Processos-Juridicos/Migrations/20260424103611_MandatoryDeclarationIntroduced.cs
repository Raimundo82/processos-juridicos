using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class MandatoryDeclarationIntroduced : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "interest_conflict_declaration_id",
            table: "Processes",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Processes_interest_conflict_declaration_id",
            table: "Processes",
            column: "interest_conflict_declaration_id");

        migrationBuilder.AddForeignKey(
            name: "FK_Processes_Process_files_interest_conflict_declaration_id",
            table: "Processes",
            column: "interest_conflict_declaration_id",
            principalTable: "Process_files",
            principalColumn: "process_file_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Processes_Process_files_interest_conflict_declaration_id",
            table: "Processes");

        migrationBuilder.DropIndex(
            name: "IX_Processes_interest_conflict_declaration_id",
            table: "Processes");

        migrationBuilder.DropColumn(
            name: "interest_conflict_declaration_id",
            table: "Processes");
    }
}
