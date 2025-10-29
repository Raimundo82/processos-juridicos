using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class AddCanCompensateToUsers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "can_compensate",
            table: "Units",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AlterColumn<string>(
            name: "state_name",
            table: "Process_states",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "can_compensate",
            table: "Units");

        migrationBuilder.AlterColumn<string>(
            name: "state_name",
            table: "Process_states",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }
}
