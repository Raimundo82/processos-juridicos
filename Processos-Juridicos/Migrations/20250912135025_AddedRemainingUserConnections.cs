using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class AddedRemainingUserConnections : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "modified_by",
            table: "Processes",
            newName: "modified_by_nii");

        migrationBuilder.RenameColumn(
            name: "created_by",
            table: "Processes",
            newName: "modified_by_name");

        migrationBuilder.AddColumn<string>(
            name: "created_by_name",
            table: "Processes",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "created_by_nii",
            table: "Processes",
            type: "nvarchar(450)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Processes_created_by_nii",
            table: "Processes",
            column: "created_by_nii");

        migrationBuilder.AddForeignKey(
            name: "FK_Processes_Users_created_by_nii",
            table: "Processes",
            column: "created_by_nii",
            principalTable: "Users",
            principalColumn: "user_nii");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Processes_Users_created_by_nii",
            table: "Processes");

        migrationBuilder.DropIndex(
            name: "IX_Processes_created_by_nii",
            table: "Processes");

        migrationBuilder.DropColumn(
            name: "created_by_name",
            table: "Processes");

        migrationBuilder.DropColumn(
            name: "created_by_nii",
            table: "Processes");

        migrationBuilder.RenameColumn(
            name: "modified_by_nii",
            table: "Processes",
            newName: "modified_by");

        migrationBuilder.RenameColumn(
            name: "modified_by_name",
            table: "Processes",
            newName: "created_by");
    }
}
