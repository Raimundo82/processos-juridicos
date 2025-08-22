using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class StateTransitionMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "State_transitions",
            columns: table => new
            {
                FromStateId = table.Column<int>(type: "int", nullable: false),
                ToStateId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_State_transitions", x => new { x.FromStateId, x.ToStateId });
                table.ForeignKey(
                    name: "FK_State_transitions_Process_states_FromStateId",
                    column: x => x.FromStateId,
                    principalTable: "Process_states",
                    principalColumn: "state_id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_State_transitions_Process_states_ToStateId",
                    column: x => x.ToStateId,
                    principalTable: "Process_states",
                    principalColumn: "state_id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_State_transitions_ToStateId",
            table: "State_transitions",
            column: "ToStateId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "State_transitions");
    }
}
