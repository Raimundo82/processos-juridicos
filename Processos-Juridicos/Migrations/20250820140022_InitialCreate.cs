using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
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
                state_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                unit_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                unit_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                unit_acronym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                enable = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Units", x => x.unit_id);
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
                compensating_unit_id = table.Column<int>(type: "int", nullable: true),
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
                created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                modified_at = table.Column<DateOnly>(type: "date", nullable: true),
                modified_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                service_accident_id = table.Column<int>(type: "int", nullable: true),
                harmed_or_casualties_id = table.Column<int>(type: "int", nullable: true),
                third_party_compensation = table.Column<double>(type: "float", nullable: true),
                reimbursement = table.Column<double>(type: "float", nullable: true),
                infringement_id = table.Column<int>(type: "int", nullable: true),
                crime_type_id = table.Column<int>(type: "int", nullable: true),
                compensation_paid_by_unit = table.Column<bool>(type: "bit", nullable: false),
                comunicated_pjm = table.Column<bool>(type: "bit", nullable: false),
                pjm_comunication_date = table.Column<DateOnly>(type: "date", nullable: true),
                military_security_id = table.Column<int>(type: "int", nullable: true)
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
                    name: "FK_Processes_Infringements_infringement_id",
                    column: x => x.infringement_id,
                    principalTable: "Infringements",
                    principalColumn: "infringement_id");
                table.ForeignKey(
                    name: "FK_Processes_Military_securities_military_security_id",
                    column: x => x.military_security_id,
                    principalTable: "Military_securities",
                    principalColumn: "military_security_id");
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
                table.ForeignKey(
                    name: "FK_Process_files_Processes_process_id",
                    column: x => x.process_id,
                    principalTable: "Processes",
                    principalColumn: "process_id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Process_files_process_id",
            table: "Process_files",
            column: "process_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_compensating_unit_id",
            table: "Processes",
            column: "compensating_unit_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_crime_type_id",
            table: "Processes",
            column: "crime_type_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_harmed_or_casualties_id",
            table: "Processes",
            column: "harmed_or_casualties_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_infringement_id",
            table: "Processes",
            column: "infringement_id");

        migrationBuilder.CreateIndex(
            name: "IX_Processes_military_security_id",
            table: "Processes",
            column: "military_security_id");

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
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Process_files");

        migrationBuilder.DropTable(
            name: "Processes");

        migrationBuilder.DropTable(
            name: "Accident_types");

        migrationBuilder.DropTable(
            name: "Crime_types");

        migrationBuilder.DropTable(
            name: "Harmed_or_casualties");

        migrationBuilder.DropTable(
            name: "Infringements");

        migrationBuilder.DropTable(
            name: "Military_securities");

        migrationBuilder.DropTable(
            name: "Process_states");

        migrationBuilder.DropTable(
            name: "Process_types");

        migrationBuilder.DropTable(
            name: "Sentences");

        migrationBuilder.DropTable(
            name: "Units");
    }
}
