using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class ManyToManyInfringements : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Processes_Infringements_infringement_id",
            table: "Processes");

        migrationBuilder.DropIndex(
            name: "IX_Processes_infringement_id",
            table: "Processes");

        migrationBuilder.DropColumn(
            name: "infringement_id",
            table: "Processes");

        migrationBuilder.CreateTable(
            name: "InfringementProcess",
            columns: table => new
            {
                InfringementsInfringementId = table.Column<int>(type: "int", nullable: false),
                ProcessesProcessId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InfringementProcess", x => new { x.InfringementsInfringementId, x.ProcessesProcessId });
                table.ForeignKey(
                    name: "FK_InfringementProcess_Infringements_InfringementsInfringementId",
                    column: x => x.InfringementsInfringementId,
                    principalTable: "Infringements",
                    principalColumn: "infringement_id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_InfringementProcess_Processes_ProcessesProcessId",
                    column: x => x.ProcessesProcessId,
                    principalTable: "Processes",
                    principalColumn: "process_id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InfringementProcess_ProcessesProcessId",
            table: "InfringementProcess",
            column: "ProcessesProcessId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InfringementProcess");

        migrationBuilder.AddColumn<int>(
            name: "infringement_id",
            table: "Processes",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Processes_infringement_id",
            table: "Processes",
            column: "infringement_id");

        migrationBuilder.AddForeignKey(
            name: "FK_Processes_Infringements_infringement_id",
            table: "Processes",
            column: "infringement_id",
            principalTable: "Infringements",
            principalColumn: "infringement_id");
    }
}
