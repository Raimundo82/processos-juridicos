using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class MandatoryDeclaration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Accident_types",
            columns: table => new
            {
                accident_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                accident_type = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accident_types", x => x.accident_id);
            });

        migrationBuilder.CreateTable(
            name: "Crime_types",
            columns: table => new
            {
                crime_type_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                crime_type_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Crime_types", x => x.crime_type_id);
            });

        migrationBuilder.CreateTable(
            name: "Harmed_or_casualties",
            columns: table => new
            {
                casualties_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                casualties_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Harmed_or_casualties", x => x.casualties_id);
            });

        migrationBuilder.CreateTable(
            name: "Infringements",
            columns: table => new
            {
                infringement_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                infringement_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Infringements", x => x.infringement_id);
            });

        migrationBuilder.CreateTable(
            name: "Military_securities",
            columns: table => new
            {
                military_security_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                military_security_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Military_securities", x => x.military_security_id);
            });

        migrationBuilder.CreateTable(
            name: "Process_states",
            columns: table => new
            {
                state_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                state_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Process_states", x => x.state_id);
            });

        migrationBuilder.CreateTable(
            name: "Process_types",
            columns: table => new
            {
                process_type_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                process_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                deadline = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Process_types", x => x.process_type_id);
            });

        migrationBuilder.CreateTable(
            name: "Roles",
            columns: table => new
            {
                role_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                role_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.role_id);
            });

        migrationBuilder.CreateTable(
            name: "Sentences",
            columns: table => new
            {
                sentence_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                sentence_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sentences", x => x.sentence_id);
            });

        migrationBuilder.CreateTable(
            name: "Units",
            columns: table => new
            {
                unit_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                unit_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                unit_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                unit_acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                enable = table.Column<bool>(type: "bit", nullable: false),
                can_compensate = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Units", x => x.unit_id);
            });

        migrationBuilder.CreateTable(
            name: "State_transitions",
            columns: table => new
            {
                state_transition_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                from_state_id = table.Column<int>(type: "int", nullable: false),
                to_state_id = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_State_transitions", x => x.state_transition_id);
                table.ForeignKey(
                    name: "FK_State_transitions_Process_states_from_state_id",
                    column: x => x.from_state_id,
                    principalTable: "Process_states",
                    principalColumn: "state_id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_State_transitions_Process_states_to_state_id",
                    column: x => x.to_state_id,
                    principalTable: "Process_states",
                    principalColumn: "state_id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                user_nii = table.Column<string>(type: "nvarchar(450)", nullable: false),
                user_role = table.Column<int>(type: "int", nullable: true),
                user_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                is_manually_set = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.user_nii);
                table.ForeignKey(
                    name: "FK_Users_Roles_user_role",
                    column: x => x.user_role,
                    principalTable: "Roles",
                    principalColumn: "role_id");
            });

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

        migrationBuilder.CreateTable(
            name: "Unit_commanders",
            columns: table => new
            {
                unit_id = table.Column<int>(type: "int", nullable: false),
                user_nii = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Unit_commanders", x => new { x.unit_id, x.user_nii });
                table.ForeignKey(
                    name: "FK_Unit_commanders_Units_unit_id",
                    column: x => x.unit_id,
                    principalTable: "Units",
                    principalColumn: "unit_id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Unit_commanders_Users_user_nii",
                    column: x => x.user_nii,
                    principalTable: "Users",
                    principalColumn: "user_nii",
                    onDelete: ReferentialAction.Cascade);
            });

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
            });

        migrationBuilder.CreateTable(
            name: "Process_files",
            columns: table => new
            {
                process_file_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                process_file_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                process_file_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                process_file_content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                process_id = table.Column<int>(type: "int", nullable: false),
                process_file_trusted_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Process_files", x => x.process_file_id);
            });

        migrationBuilder.CreateTable(
            name: "Processes",
            columns: table => new
            {
                process_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                nuipm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                process_type_id = table.Column<int>(type: "int", nullable: true),
                unit_id = table.Column<int>(type: "int", nullable: true),
                oficial_inst_telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                oficial_inst_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                oficial_inst_nii = table.Column<string>(type: "nvarchar(450)", nullable: true),
                compensating_unit_id = table.Column<int>(type: "int", nullable: true),
                investigated_uncertain = table.Column<bool>(type: "bit", nullable: false),
                investigated_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                investigated_gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                occurrence_date = table.Column<DateOnly>(type: "date", nullable: true),
                dispatch_date = table.Column<DateOnly>(type: "date", nullable: true),
                description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                deadline_date = table.Column<DateOnly>(type: "date", nullable: true),
                final_dispatch_date = table.Column<DateOnly>(type: "date", nullable: true),
                sentence_id = table.Column<int>(type: "int", nullable: true),
                state_id = table.Column<int>(type: "int", nullable: false),
                created_at = table.Column<DateOnly>(type: "date", nullable: true),
                created_by_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                created_by_nii = table.Column<string>(type: "nvarchar(450)", nullable: true),
                modified_at = table.Column<DateOnly>(type: "date", nullable: true),
                modified_by_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                modified_by_nii = table.Column<string>(type: "nvarchar(450)", nullable: true),
                service_accident_id = table.Column<int>(type: "int", nullable: true),
                harmed_or_casualties_id = table.Column<int>(type: "int", nullable: true),
                third_party_compensation = table.Column<double>(type: "float", nullable: true),
                reimbursement = table.Column<double>(type: "float", nullable: true),
                crime_type_id = table.Column<int>(type: "int", nullable: true),
                compensation_paid_by_unit = table.Column<bool>(type: "bit", nullable: false),
                comunicated_pjm = table.Column<bool>(type: "bit", nullable: false),
                pjm_comunication_date = table.Column<DateOnly>(type: "date", nullable: true),
                military_security_id = table.Column<int>(type: "int", nullable: true),
                interest_conflict_declaration_id = table.Column<int>(type: "int", nullable: true),
                jurist_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Processes", x => x.process_id);
                table.ForeignKey(
                    name: "FK_Processes_Accident_types_service_accident_id",
                    column: x => x.service_accident_id,
                    principalTable: "Accident_types",
                    principalColumn: "accident_id");
                table.ForeignKey(
                    name: "FK_Processes_Crime_types_crime_type_id",
                    column: x => x.crime_type_id,
                    principalTable: "Crime_types",
                    principalColumn: "crime_type_id");
                table.ForeignKey(
                    name: "FK_Processes_Harmed_or_casualties_harmed_or_casualties_id",
                    column: x => x.harmed_or_casualties_id,
                    principalTable: "Harmed_or_casualties",
                    principalColumn: "casualties_id");
                table.ForeignKey(
                    name: "FK_Processes_Military_securities_military_security_id",
                    column: x => x.military_security_id,
                    principalTable: "Military_securities",
                    principalColumn: "military_security_id");
                table.ForeignKey(
                    name: "FK_Processes_Process_files_interest_conflict_declaration_id",
                    column: x => x.interest_conflict_declaration_id,
                    principalTable: "Process_files",
                    principalColumn: "process_file_id");
                table.ForeignKey(
                    name: "FK_Processes_Process_states_state_id",
                    column: x => x.state_id,
                    principalTable: "Process_states",
                    principalColumn: "state_id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Processes_Process_types_process_type_id",
                    column: x => x.process_type_id,
                    principalTable: "Process_types",
                    principalColumn: "process_type_id");
                table.ForeignKey(
                    name: "FK_Processes_Sentences_sentence_id",
                    column: x => x.sentence_id,
                    principalTable: "Sentences",
                    principalColumn: "sentence_id");
                table.ForeignKey(
                    name: "FK_Processes_Units_compensating_unit_id",
                    column: x => x.compensating_unit_id,
                    principalTable: "Units",
                    principalColumn: "unit_id");
                table.ForeignKey(
                    name: "FK_Processes_Units_unit_id",
                    column: x => x.unit_id,
                    principalTable: "Units",
                    principalColumn: "unit_id");
                table.ForeignKey(
                    name: "FK_Processes_Users_created_by_nii",
                    column: x => x.created_by_nii,
                    principalTable: "Users",
                    principalColumn: "user_nii");
                table.ForeignKey(
                    name: "FK_Processes_Users_modified_by_nii",
                    column: x => x.modified_by_nii,
                    principalTable: "Users",
                    principalColumn: "user_nii");
                table.ForeignKey(
                    name: "FK_Processes_Users_oficial_inst_nii",
                    column: x => x.oficial_inst_nii,
                    principalTable: "Users",
                    principalColumn: "user_nii");
            });

        migrationBuilder.CreateIndex(
            name: "IX_InfringementProcess_ProcessesProcessId",
            table: "InfringementProcess",
            column: "ProcessesProcessId");

        migrationBuilder.CreateIndex(
            name: "IX_Process_files_process_id",
            table: "Process_files",
            column: "process_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_compensating_unit_id",
            table: "Processes",
            column: "compensating_unit_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_created_by_nii",
            table: "Processes",
            column: "created_by_nii");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_crime_type_id",
            table: "Processes",
            column: "crime_type_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_harmed_or_casualties_id",
            table: "Processes",
            column: "harmed_or_casualties_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_interest_conflict_declaration_id",
            table: "Processes",
            column: "interest_conflict_declaration_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_military_security_id",
            table: "Processes",
            column: "military_security_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_modified_by_nii",
            table: "Processes",
            column: "modified_by_nii");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_oficial_inst_nii",
            table: "Processes",
            column: "oficial_inst_nii");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_process_type_id",
            table: "Processes",
            column: "process_type_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_sentence_id",
            table: "Processes",
            column: "sentence_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_service_accident_id",
            table: "Processes",
            column: "service_accident_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_state_id",
            table: "Processes",
            column: "state_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_unit_id",
            table: "Processes",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "IX_State_transition_roles_role_id",
            table: "State_transition_roles",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "IX_State_transitions_from_state_id_to_state_id",
            table: "State_transitions",
            columns: ["from_state_id", "to_state_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_State_transitions_to_state_id",
            table: "State_transitions",
            column: "to_state_id");

        migrationBuilder.CreateIndex(
            name: "IX_Unit_commanders_user_nii",
            table: "Unit_commanders",
            column: "user_nii");

        migrationBuilder.CreateIndex(
            name: "IX_Users_user_role",
            table: "Users",
            column: "user_role");

        migrationBuilder.AddForeignKey(
            name: "FK_InfringementProcess_Processes_ProcessesProcessId",
            table: "InfringementProcess",
            column: "ProcessesProcessId",
            principalTable: "Processes",
            principalColumn: "process_id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Process_files_Processes_process_id",
            table: "Process_files",
            column: "process_id",
            principalTable: "Processes",
            principalColumn: "process_id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Process_files_Processes_process_id",
            table: "Process_files");

        migrationBuilder.DropTable(
            name: "InfringementProcess");

        migrationBuilder.DropTable(
            name: "State_transition_roles");

        migrationBuilder.DropTable(
            name: "Unit_commanders");

        migrationBuilder.DropTable(
            name: "Infringements");

        migrationBuilder.DropTable(
            name: "State_transitions");

        migrationBuilder.DropTable(
            name: "Processes");

        migrationBuilder.DropTable(
            name: "Accident_types");

        migrationBuilder.DropTable(
            name: "Crime_types");

        migrationBuilder.DropTable(
            name: "Harmed_or_casualties");

        migrationBuilder.DropTable(
            name: "Military_securities");

        migrationBuilder.DropTable(
            name: "Process_files");

        migrationBuilder.DropTable(
            name: "Process_states");

        migrationBuilder.DropTable(
            name: "Process_types");

        migrationBuilder.DropTable(
            name: "Sentences");

        migrationBuilder.DropTable(
            name: "Units");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Roles");
    }
}
