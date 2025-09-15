using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class OfInstNiiFKAdded : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "oficial_inst_nii",
            table: "Processes",
            type: "nvarchar(450)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Processes_oficial_inst_nii",
            table: "Processes",
            column: "oficial_inst_nii");

        migrationBuilder.AddForeignKey(
            name: "FK_Processes_Users_oficial_inst_nii",
            table: "Processes",
            column: "oficial_inst_nii",
            principalTable: "Users",
            principalColumn: "user_nii");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Processes_Users_oficial_inst_nii",
            table: "Processes");

        migrationBuilder.DropIndex(
            name: "IX_Processes_oficial_inst_nii",
            table: "Processes");

        migrationBuilder.AlterColumn<string>(
            name: "oficial_inst_nii",
            table: "Processes",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)",
            oldNullable: true);
    }
}
