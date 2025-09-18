using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class AddStateTransitionRolesAndRelations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_State_transitions_Process_states_FromStateId",
            table: "State_transitions");

        migrationBuilder.DropForeignKey(
            name: "FK_State_transitions_Process_states_ToStateId",
            table: "State_transitions");

        migrationBuilder.DropPrimaryKey(
            name: "PK_State_transitions",
            table: "State_transitions");

        migrationBuilder.RenameColumn(
            name: "ToStateId",
            table: "State_transitions",
            newName: "to_state_id");

        migrationBuilder.RenameColumn(
            name: "FromStateId",
            table: "State_transitions",
            newName: "from_state_id");

        migrationBuilder.RenameIndex(
            name: "IX_State_transitions_ToStateId",
            table: "State_transitions",
            newName: "IX_State_transitions_to_state_id");

        migrationBuilder.AddColumn<int>(
            name: "state_transition_id",
            table: "State_transitions",
            type: "int",
            nullable: false,
            defaultValue: 0)
            .Annotation("SqlServer:Identity", "1, 1");

        migrationBuilder.AlterColumn<string>(
            name: "modified_by_nii",
            table: "Processes",
            type: "nvarchar(450)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_State_transitions",
            table: "State_transitions",
            column: "state_transition_id");

        migrationBuilder.CreateTable(
            name: "State_transition_roles",
            columns: table => new
            {
                state_transition_id = table.Column<int>(type: "int", nullable: false),
                role_id = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_State_transition_roles", x => new { x.state_transition_id, x.role_id });
                table.ForeignKey(
                    name: "FK_State_transition_roles_Roles_role_id",
                    column: x => x.role_id,
                    principalTable: "Roles",
                    principalColumn: "role_id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_State_transition_roles_State_transitions_state_transition_id",
                    column: x => x.state_transition_id,
                    principalTable: "State_transitions",
                    principalColumn: "state_transition_id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_State_transitions_from_state_id_to_state_id",
            table: "State_transitions",
            columns: ["from_state_id", "to_state_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Processes_modified_by_nii",
            table: "Processes",
            column: "modified_by_nii");

        migrationBuilder.CreateIndex(
            name: "IX_State_transition_roles_role_id",
            table: "State_transition_roles",
            column: "role_id");

        migrationBuilder.AddForeignKey(
            name: "FK_Processes_Users_modified_by_nii",
            table: "Processes",
            column: "modified_by_nii",
            principalTable: "Users",
            principalColumn: "user_nii");

        migrationBuilder.AddForeignKey(
            name: "FK_State_transitions_Process_states_from_state_id",
            table: "State_transitions",
            column: "from_state_id",
            principalTable: "Process_states",
            principalColumn: "state_id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_State_transitions_Process_states_to_state_id",
            table: "State_transitions",
            column: "to_state_id",
            principalTable: "Process_states",
            principalColumn: "state_id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Processes_Users_modified_by_nii",
            table: "Processes");

        migrationBuilder.DropForeignKey(
            name: "FK_State_transitions_Process_states_from_state_id",
            table: "State_transitions");

        migrationBuilder.DropForeignKey(
            name: "FK_State_transitions_Process_states_to_state_id",
            table: "State_transitions");

        migrationBuilder.DropTable(
            name: "State_transition_roles");

        migrationBuilder.DropPrimaryKey(
            name: "PK_State_transitions",
            table: "State_transitions");

        migrationBuilder.DropIndex(
            name: "IX_State_transitions_from_state_id_to_state_id",
            table: "State_transitions");

        migrationBuilder.DropIndex(
            name: "IX_Processes_modified_by_nii",
            table: "Processes");

        migrationBuilder.DropColumn(
            name: "state_transition_id",
            table: "State_transitions");

        migrationBuilder.RenameColumn(
            name: "to_state_id",
            table: "State_transitions",
            newName: "ToStateId");

        migrationBuilder.RenameColumn(
            name: "from_state_id",
            table: "State_transitions",
            newName: "FromStateId");

        migrationBuilder.RenameIndex(
            name: "IX_State_transitions_to_state_id",
            table: "State_transitions",
            newName: "IX_State_transitions_ToStateId");

        migrationBuilder.AlterColumn<string>(
            name: "modified_by_nii",
            table: "Processes",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)",
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_State_transitions",
            table: "State_transitions",
            columns: ["FromStateId", "ToStateId"]);

        migrationBuilder.AddForeignKey(
            name: "FK_State_transitions_Process_states_FromStateId",
            table: "State_transitions",
            column: "FromStateId",
            principalTable: "Process_states",
            principalColumn: "state_id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_State_transitions_Process_states_ToStateId",
            table: "State_transitions",
            column: "ToStateId",
            principalTable: "Process_states",
            principalColumn: "state_id",
            onDelete: ReferentialAction.Cascade);
    }
}
