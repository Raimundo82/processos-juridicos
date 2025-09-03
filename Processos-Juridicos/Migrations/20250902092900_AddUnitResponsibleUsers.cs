using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class AddUnitResponsibleUsers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UnitUser");

        migrationBuilder.CreateTable(
            name: "Unit_commanders",
            columns: table => new
            {
                UnitId = table.Column<int>(type: "int", nullable: false),
                UserNii = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Unit_commanders", x => new { x.UnitId, x.UserNii });
                table.ForeignKey(
                    name: "FK_Unit_commanders_Units_UnitId",
                    column: x => x.UnitId,
                    principalTable: "Units",
                    principalColumn: "unit_id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Unit_commanders_Users_UserNii",
                    column: x => x.UserNii,
                    principalTable: "Users",
                    principalColumn: "user_nii",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Unit_commanders_UserNii",
            table: "Unit_commanders",
            column: "UserNii");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Unit_commanders");

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
}
