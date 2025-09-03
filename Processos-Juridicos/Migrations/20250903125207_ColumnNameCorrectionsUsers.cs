using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class ColumnNameCorrectionsUsers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Unit_commanders_Units_UnitId",
            table: "Unit_commanders");

        migrationBuilder.DropForeignKey(
            name: "FK_Unit_commanders_Users_UserNii",
            table: "Unit_commanders");

        migrationBuilder.RenameColumn(
            name: "UserName",
            table: "Users",
            newName: "user_name");

        migrationBuilder.RenameColumn(
            name: "UserNii",
            table: "Unit_commanders",
            newName: "user_nii");

        migrationBuilder.RenameColumn(
            name: "UnitId",
            table: "Unit_commanders",
            newName: "unit_id");

        migrationBuilder.RenameIndex(
            name: "IX_Unit_commanders_UserNii",
            table: "Unit_commanders",
            newName: "IX_Unit_commanders_user_nii");

        migrationBuilder.AddForeignKey(
            name: "FK_Unit_commanders_Units_unit_id",
            table: "Unit_commanders",
            column: "unit_id",
            principalTable: "Units",
            principalColumn: "unit_id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Unit_commanders_Users_user_nii",
            table: "Unit_commanders",
            column: "user_nii",
            principalTable: "Users",
            principalColumn: "user_nii",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Unit_commanders_Units_unit_id",
            table: "Unit_commanders");

        migrationBuilder.DropForeignKey(
            name: "FK_Unit_commanders_Users_user_nii",
            table: "Unit_commanders");

        migrationBuilder.RenameColumn(
            name: "user_name",
            table: "Users",
            newName: "UserName");

        migrationBuilder.RenameColumn(
            name: "user_nii",
            table: "Unit_commanders",
            newName: "UserNii");

        migrationBuilder.RenameColumn(
            name: "unit_id",
            table: "Unit_commanders",
            newName: "UnitId");

        migrationBuilder.RenameIndex(
            name: "IX_Unit_commanders_user_nii",
            table: "Unit_commanders",
            newName: "IX_Unit_commanders_UserNii");

        migrationBuilder.AddForeignKey(
            name: "FK_Unit_commanders_Units_UnitId",
            table: "Unit_commanders",
            column: "UnitId",
            principalTable: "Units",
            principalColumn: "unit_id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Unit_commanders_Users_UserNii",
            table: "Unit_commanders",
            column: "UserNii",
            principalTable: "Users",
            principalColumn: "user_nii",
            onDelete: ReferentialAction.Cascade);
    }
}
