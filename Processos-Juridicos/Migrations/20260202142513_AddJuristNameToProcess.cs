using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processos_Juridicos.Migrations;

/// <inheritdoc />
public partial class AddJuristNameToProcess : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "jurist_name",
            table: "Processes",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "jurist_name",
            table: "Processes");
    }
}
