using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class AddUsersResponsibleForUnits : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "unit_name",
            table: "Units",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "unit_code",
            table: "Units",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "unit_acronym",
            table: "Units",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "UnitUser",
            columns: table => new
            {
                ResponsibleUsersUserNii = table.Column<string>(type: "nvarchar(450)", nullable: false),
                UnitsResponsibleForUnitId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UnitUser", x => new { x.ResponsibleUsersUserNii, x.UnitsResponsibleForUnitId });
                table.ForeignKey(
                    name: "FK_UnitUser_Units_UnitsResponsibleForUnitId",
                    column: x => x.UnitsResponsibleForUnitId,
                    principalTable: "Units",
                    principalColumn: "unit_id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UnitUser_Users_ResponsibleUsersUserNii",
                    column: x => x.ResponsibleUsersUserNii,
                    principalTable: "Users",
                    principalColumn: "user_nii",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UnitUser_UnitsResponsibleForUnitId",
            table: "UnitUser",
            column: "UnitsResponsibleForUnitId");


    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UnitUser");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Roles");

        migrationBuilder.AlterColumn<string>(
            name: "unit_name",
            table: "Units",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "unit_code",
            table: "Units",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "unit_acronym",
            table: "Units",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }
}
